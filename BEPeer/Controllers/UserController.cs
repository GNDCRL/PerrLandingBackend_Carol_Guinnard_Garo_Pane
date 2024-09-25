using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositores.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userservices;


        public UserController(IUserServices userServices)
        {
            _userservices = userServices;
        }

        [HttpPost]
        public async Task<IActionResult> Register(ReqRegisterUserDto register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation error occurred!");
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = error
                    });
                }

                var res = await _userservices.Register(register);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User registered",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "email already used")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }



        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userservices.GetAllUsers();
                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResUserDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDto loginDto)
        {
            try
            {
                var response = await _userservices.Login(loginDto);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User login succsess",
                    Data = response
                });
            }catch (Exception ex)
            {
                if(ex.Message == "Invalid email or password")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }



        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ReqUpdateUserDto updateUserDto)
        {
            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserRole != "admin")
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResBaseDto<string>
                {
                    Success = false,
                    Message = "Only admin can add new users.",
                    Data = null
                });
            }

            try
            {
                var result = await _userservices.UpdateUser(id, updateUserDto, currentUserRole);
                return Ok(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUser(ReqRegisterUserDto registerUserDto)
        {
            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserRole != "admin")
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResBaseDto<string>
                {
                    Success = false,
                    Message = "Only admin can add new users.",
                    Data = null
                });
            }

            try
            {
                var result = await _userservices.Register(registerUserDto);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User added successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id, [FromBody] ReqDeleteUserDto deleteDto)
        {
            try
            {
                var result = await _userservices.DeleteUser(id, deleteDto);
                return Ok(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


    }
}

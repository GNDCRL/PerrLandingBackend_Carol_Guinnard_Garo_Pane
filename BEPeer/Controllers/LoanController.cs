using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositores.Services;
using DAL.Repositores.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost]
        public async Task<IActionResult> NewLoan(ReqLoanDto loan)
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

                var res = await  _loanService.CreateLoan(loan);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Success add loan data",
                    Data = res
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

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLoan(string id, ReqUpdateLoanDto updateLoanDto)
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

                var res = await _loanService.UpdateLoan(id, updateLoanDto);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Success add loan data",
                    Data = res
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





        [HttpGet]
        public async Task<IActionResult> LoanList()
        {
            try
            {
                var res = await _loanService.LoanList();
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "success",
                    Data = res
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



        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetStatus(string status)
        {
            try
            {
                var res = await _loanService.GetStatus(status);
                if (res == null || res.Count == 0)
                {
                    return NotFound(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = $"No loans found with status: {status}",
                        Data = null
                    });
                }

                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success",
                    Data = res
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
    }
}

using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Model;
using DAL.Repositores.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services
{
    public class UserServices : IUserServices
    {
        private readonly PeerlandingContext _context;
        private readonly IConfiguration _configuration;
        public UserServices(PeerlandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);
            if (isAnyEmail != null)
            {
                throw new Exception("email already used");
            }
            var newUser = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = register.Balance,
            };


            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name; 

            
        }


        

        public async Task<List<ResUserDto>> GetAllUsers()
        {
            return await _context.MstUsers
                 .Select(user => new ResUserDto
                 {
                     Id = user.Id,
                     Name = user.Name,
                     Email = user.Email,
                     Role = user.Role,
                     Balance = user.Balance ?? 0,

                 }).ToListAsync();
        }

        public async Task<List<ResUserDto>> GetAllNonAdminUsers()
        {
            var users = await _context.MstUsers
                .Where(user => user.Role != "admin") // Asumsikan bahwa ada properti Role
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance ?? 0,
                })
                .ToListAsync();

            return users;
        }




        public async Task<string> AddUser(ReqRegisterUserDto registerUserDto)
        {

            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == registerUserDto.Email);
            if (isAnyEmail != null)
            {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = registerUserDto.Name,
                Email = registerUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password),
                Role = registerUserDto.Role,
                Balance = registerUserDto.Balance
            };

            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;
        }






        public async Task<string> UpdateUser(string userId, ReqUpdateUserDto updateUserDto, string currentUserRole)
        {
                if (currentUserRole != "admin")
                {
                    throw new Exception("Only admin can update user details.");
                }

                var user = await _context.MstUsers.FindAsync(userId);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                user.Name = updateUserDto.Name;
                user.Role = updateUserDto.Role;
                user.Balance = updateUserDto.Balance;

                await _context.SaveChangesAsync();

                return "User updated successfully.";
        }




        public async Task<string> DeleteUser(string userId)
        {
            var user = await _context.MstUsers.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            _context.MstUsers.Remove(user);
            await _context.SaveChangesAsync();

            return "User deleted successfully.";
        }


        public async Task<ResByIdUser> GetUserId(string userId)
        {
            // Cari pengguna berdasarkan userId
            var user = await _context.MstUsers
                .Where(u => u.Id == userId)  
                .Select(user => new ResByIdUser
                {
                    Id = user.Id,
                    Name = user.Name,
                    Role = user.Role,
                    Balance = user.Balance ?? 0,  
                }).FirstOrDefaultAsync();  

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }





        public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if (user == null) 
            {
                throw new Exception("Invalid email or password");        
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            var loginResponse = new ResLoginDto
            {
                Token = token,
            };

            return loginResponse;
        }

        public string GenerateJwtToken(MstUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSetting");
            var secretKey = jwtSettings["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}

using DAL.DTO.Req;
using DAL.DTO.Res;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services.Interfaces
{
    public interface IUserServices
    {
        Task<string> Register(ReqRegisterUserDto register);

        Task<List<ResUserDto>> GetAllUsers();// Di IUserServices

        Task<ResLoginDto> Login(ReqLoginDto reqLogin);

        Task<string> UpdateUser(string userId, ReqUpdateUserDto updateUserDto, string currentUserRole);

        Task<string> AddUser(ReqRegisterUserDto registerUserDto);

        Task<string> DeleteUser(string userId);
        // Di implementasi UserServices

        Task<ResByIdUser> GetUserId(string userId);

    }
}

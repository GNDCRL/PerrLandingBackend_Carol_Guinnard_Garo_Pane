using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services.Interfaces
{
    public interface ILoanService
    {
        Task<string> CreateLoan(ReqLoanDto loan);

        Task<string> UpdateLoan(string id, ReqUpdateLoanDto updateLoanDto);

        Task<List<ResListLoanDto>> LoanList();

        Task<List<ResListLoanDto>> GetStatus(string status);
    }
}

using DAL.DTO.Req;
using DAL.DTO.Req.ReqLoan;
using DAL.DTO.Req.ReqLoanBorrower;
using DAL.DTO.Res;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services.Interfaces
{
    public interface ILoanService
    {

        //old aja
        Task<string> CreateLoan(ReqLoanDto loan);

        Task<string> UpdateLoan(string id, ReqUpdateLoanDto updateLoanDto);

        Task<List<ResListLoanDto>> LoanList();

        Task<List<ResListLoanDto>> GetStatus(string status);
    }
}

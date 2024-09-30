using DAL.DTO.Req.ReqLoan;
using DAL.DTO.Req.ReqLoanBorrower;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services.Interfaces
{
    public interface IBorrowerService
    {
        Task<IEnumerable<ResListLoanDto>> GetDaftarPinjaman(string borrowerId);
        Task<string> AddRequestLoan(ReqLoanBorrowerDto request);
        Task<IEnumerable<ResLoanHistoryDto>> GetHistoryLoan(string borrowerId);
        Task<string> RepayLoans(string borrowerId, string loanId, List<int> installmentNumbers);


    }
}

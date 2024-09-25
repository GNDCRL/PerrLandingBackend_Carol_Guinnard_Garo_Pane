using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Model;
using DAL.Repositores.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services
{
    public class LoanServices : ILoanService
    {
        private readonly PeerlandingContext _peerlandingContext;
        public LoanServices(PeerlandingContext peerlandingContext)
        {
            _peerlandingContext = peerlandingContext;
        }


        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
            };

            await _peerlandingContext.AddAsync(newLoan);
            await _peerlandingContext.SaveChangesAsync();

            return newLoan.BorrowerId;
        }






        public async Task<string> UpdateLoan(string id, ReqUpdateLoanDto updateLoanDto)
        {
            var existingLoan = await _peerlandingContext.MstLoans.FindAsync(id);

            if (existingLoan == null)
            {
                throw new Exception("Loan not found");
            }

            existingLoan.Status = updateLoanDto.Status;
            existingLoan.UpdateAt = DateTime.UtcNow; 

            _peerlandingContext.MstLoans.Update(existingLoan);
            await _peerlandingContext.SaveChangesAsync();

            return existingLoan.BorrowerId;
        }






        public async Task<List<ResListLoanDto>> LoanList()
        {
            var loans = await _peerlandingContext.MstLoans
                .Include(l => l.User)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowerName = loan.User.Name,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatAt = loan.CreatedAt,
                    UpdateAt = loan.UpdateAt
                }).ToListAsync();
            return loans;
        }






        public async Task<List<ResListLoanDto>> GetStatus(string status)
        {
            var loans = await _peerlandingContext.MstLoans
                .Include(l => l.User)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowerName = loan.User.Name,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatAt = loan.CreatedAt,
                    UpdateAt = loan.UpdateAt
                }).ToListAsync();
            return loans;
        }
    }
}

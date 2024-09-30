using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.ReqLoan
{
    public class ReqLoanBorrowerDto
    {
        public string BorrowerId { get; set; }

        [Required(ErrorMessage = "Amount is requaired")]
        [Range(0, double.MaxValue, ErrorMessage = "amount must be a positive value")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Interest rate is requaired")]
        public decimal InterestRate { get; set; }
    }
}

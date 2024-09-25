using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqLoanDto
    {
        [Required(ErrorMessage = "BrrowerId is requaired")]

        public string BorrowerId { get; set; }

        [Required(ErrorMessage = "Amount is requaired")]
        [Range(0, double.MaxValue, ErrorMessage = "amount must be a positive value")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Interest rate is requaired")]
        public decimal  InterestRate { get; set; }

        [Required(ErrorMessage = "Duration is requaired")]
        public int Duration { get; set; }
    }
}

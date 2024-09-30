using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqStatusLoanDto
    {
        
        [Required(ErrorMessage = "LoanId is required.")]
        public string LoanId { get; set; }

        [Required(ErrorMessage = "LoanId is required.")]
        public string LenderId { get; set; }

        [Required(ErrorMessage = "NewStatus is required.")]
        [RegularExpression("requested|funded|rejected", ErrorMessage = "Invalid status. Valid statuses are: requested, funded, and Repaid.")]
        public string NewStatus { get; set; }
    }
}

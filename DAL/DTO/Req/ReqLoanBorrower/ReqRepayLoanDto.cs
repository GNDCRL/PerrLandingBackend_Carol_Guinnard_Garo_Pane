using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.ReqLoanBorrower
{
    public class ReqRepayLoanDto
    {
        public string BorrowerId { get; set; }
        public string LoanId { get; set; }
        public List<int> InstallmentNumbers { get; set; }
    }
}

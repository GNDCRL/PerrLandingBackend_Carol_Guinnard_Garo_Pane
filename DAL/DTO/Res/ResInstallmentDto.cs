using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResInstallmentDto
    {
        public int InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } // Misal 'paid' atau 'pending'
        public DateTime? PaidAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [Table("trn_debtbills")]
    public class TrnDebtBills
    {
        
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey("Loans")]
        [Column("loans_id")]
        public string LoansId { get; set; } // FK ke pinjaman

        [Required]
        [Column("installment_number")]
        public int InstallmentNumber { get; set; } // Cicilan ke-1, ke-2, dst.

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } // "pending", "paid"

        [Required]
        [Column("paid_at")]
        public DateTime? PaidAt { get; set; } = DateTime.UtcNow;// Tanggal pembayaran, null jika belum




        public MstLoans Loans { get; set; }
    }
    
}


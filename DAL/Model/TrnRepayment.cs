using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [Table("trn_repayment")]

    public class TrnRepayment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loans")]
        [Column("loans_id")]
        public string LoansId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("repaid_amount")]
        public decimal RepaidAmount { get; set; }

        [Required]
        [Column("balance_amount")]
        public decimal BalanceAmount { get; set; }

        [Required]
        [Column("repaid_status")]
        public string RepaidStatus { get; set; } = "requested";

        [Required]
        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }


        public MstLoans Loans { get; set; }

    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [Table("mst_loans")]
    public class MstLoans
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("User")]
        [Column("borrower_id")]
        public string BorrowerId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("interest_rate")]
        public decimal InterestRate { get; set; }

        [Required]
        [Column("duration_month")]
        public int Duration { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } = "requested";

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("update_at")]
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;


        public MstUser User { get; set; }

    }
}

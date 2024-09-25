using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqUpdateLoanDto
    {

        [Required(ErrorMessage = "status is required")]
        [MaxLength(50, ErrorMessage = "status cannot exceed 50 characters")]
        public string Status { get; set; }
    }
}

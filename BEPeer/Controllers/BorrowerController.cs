using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.DTO.Req.ReqLoanBorrower;
using DAL.DTO.Res;
using DAL.Repositores.Services.Interfaces;
using DAL.DTO.Req.ReqLoan;
using DAL.Repositores.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowerController : ControllerBase
    {
        private readonly IBorrowerService _borrowerService;

        public BorrowerController(IBorrowerService borrowerService)
        {
            _borrowerService = borrowerService;
        }




        // Mendapatkan daftar pinjaman borrower
        [HttpGet("{borrowerId}/daftarpinjaman")]
        public async Task<ActionResult<IEnumerable<ResListLoanDto>>> GetDaftarPinjaman(string borrowerId)
        {
            var loans = await _borrowerService.GetDaftarPinjaman(borrowerId);
            return Ok(loans);
        }




        // Menambahkan permohonan pinjaman
        [HttpPost("addpinjaman")]
        public async Task<ActionResult<string>> AddRequestLoan([FromBody] ReqLoanBorrowerDto request)
        {
            var result = await _borrowerService.AddRequestLoan(request);
            return Ok(result);
        }




        // Mendapatkan riwayat pinjaman borrower
        [HttpGet("{borrowerId}/history")]
        public async Task<ActionResult<IEnumerable<ResLoanHistoryDto>>> GetHistoryLoan(string borrowerId)
        {
            var history = await _borrowerService.GetHistoryLoan(borrowerId);
            return Ok(history);
        }





        // Merekam pembayaran pinjaman
        [HttpPost("bayarpinjaman")]
        public async Task<IActionResult> RepayLoan([FromBody] ReqRepayLoanDto request)
        {
            var result = await _borrowerService.RepayLoans(request.BorrowerId, request.LoanId, request.InstallmentNumbers);
            if (result.StartsWith("An error occurred"))
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

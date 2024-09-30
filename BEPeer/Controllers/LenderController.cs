using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositores.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LenderController : ControllerBase
    {
        private readonly ILenderService _lenderServices;

        public LenderController(ILenderService lenderServices)
        {
            _lenderServices = lenderServices;
        }




        [HttpGet("saldo/{lenderId}")]
        public async Task<IActionResult> GetSaldo(string lenderId)
        {
            try
            {
                var saldo = await _lenderServices.GetSaldo(lenderId);
                return Ok(new { Saldo = saldo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }




        [HttpPost("UpdateSaldo")]
        public async Task<IActionResult> UpdateSaldo([FromBody] ReqUpdateSaldoLenderDto request)
        {
            try
            {
                var result = await _lenderServices.UpdateSaldo(request.LenderId, request.Amount);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/Lender/ubah-status-pinjaman
        [HttpPut("UbahStatusPinjaman")]
        public async Task<IActionResult> UbahStatusPinjaman([FromBody] ReqStatusLoanDto request)
        {
            try
            {
                // Memasukkan lenderId ke dalam pemanggilan layanan
                var result = await _lenderServices.UbahStatusPinjaman(request.LoanId, request.NewStatus, request.LenderId);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("PeminjamTerpilih/{lenderId}")]
        public async Task<IActionResult> GetDaftarPeminjamYangDipilih(string lenderId)
        {
            try
            {
                var daftarPeminjam = await _lenderServices.GetDaftarPeminjamYangDipilih(lenderId);
                return Ok(daftarPeminjam);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}

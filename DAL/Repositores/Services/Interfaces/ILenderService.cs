using DAL.DTO.Req;
using DAL.DTO.Res;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositores.Services.Interfaces
{
    public interface ILenderService
    {
        Task<decimal> GetSaldo(string lenderId); 

        Task<string> UpdateSaldo(string lenderId, decimal amount); 

        Task<List<ResListLoanDto>> GetDaftarPeminjam();

        Task<string> UbahStatusPinjaman(string loanId, string newStatus, string lenderId);

        Task<List<ResLoanHistoryDto>> GetDaftarPeminjamYangDipilih(string lenderId);
    }
}

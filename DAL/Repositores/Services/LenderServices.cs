using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Model;
using DAL.Repositores.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositores.Services
{
    public class LenderServices : ILenderService
    {
        private readonly PeerlandingContext _peerlandingContext;

        public LenderServices(PeerlandingContext peerlandingContext)
        {
            _peerlandingContext = peerlandingContext;
        }





        public async Task<decimal> GetSaldo(string lenderId)
        {
            var lender = await _peerlandingContext.MstUsers.FindAsync(lenderId);
            if (lender == null || lender.Role != "lender")
            {
                throw new Exception("Lender not found or invalid role.");
            }
            return lender.Balance ?? 0;
        }




        public async Task<string> UpdateSaldo(string lenderId, decimal amount)
        {
            var lender = await _peerlandingContext.MstUsers.FindAsync(lenderId);
            if (lender == null || lender.Role != "lender")
            {
                throw new Exception("Lender not found or invalid role.");
            }

            lender.Balance += amount;
            await _peerlandingContext.SaveChangesAsync();

            return "Saldo updated successfully.";
        }




        public async Task<List<ResListLoanDto>> GetDaftarPeminjam()
        {
            return await _peerlandingContext.MstLoans
                .Where(loan => loan.Status == "requested" || loan.Status == "funded")
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowerName = loan.User.Name, // Mengakses nama peminjam dari relasi User
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status
                }).ToListAsync();
        }







        public async Task<string> UbahStatusPinjaman(string loanId, string newStatus, string lenderId)
        {
            // Mencari pinjaman berdasarkan ID
            var loan = await _peerlandingContext.MstLoans.FindAsync(loanId);
            if (loan == null)
            {
                throw new Exception("Loan not found.");
            }

            // Cek apakah status baru yang ingin diterapkan valid
            if (loan.Status != "requested" || newStatus != "funded")
            {
                throw new Exception("Invalid status change. Loan must be requested before being funded.");
            }

            // Cek apakah lenderId valid
            if (string.IsNullOrEmpty(lenderId))
            {
                throw new Exception("Lender ID cannot be null or empty.");
            }

            // Temukan lender berdasarkan ID
            var lender = await _peerlandingContext.MstUsers.FindAsync(lenderId);
            if (lender == null)
            {
                throw new Exception("Lender not found.");
            }

            // Cek saldo lender
            if (lender.Balance < loan.Amount)
            {
                throw new Exception("Insufficient balance for lender.");
            }

            // Ubah status pinjaman
            loan.Status = newStatus;

            // Update saldo lender dan borrower
            lender.Balance -= loan.Amount; // Kurangi saldo lender
            var borrower = await _peerlandingContext.MstUsers.FindAsync(loan.BorrowerId);
            if (borrower != null)
            {
                borrower.Balance += loan.Amount; // Tambah saldo borrower
            }

            // Tambahkan record funding ke dalam database
            var funding = new TrnFunding
            {
                Id = Guid.NewGuid().ToString(), // Buat ID baru untuk funding
                LoansId = loan.Id,
                LenderId = lender.Id, // Gunakan ID lender yang sudah ditemukan
                Amount = loan.Amount,
                FundedAt = DateTime.UtcNow
            };

            await _peerlandingContext.TrnFunding.AddAsync(funding);

            // Simpan perubahan status dan saldo lender/borrower
            await _peerlandingContext.SaveChangesAsync();

            // Buat tagihan cicilan setelah status pinjaman menjadi funded
            await GenerateInstallments(loan.Id, loan.Amount, loan.InterestRate);

            return "Loan status updated successfully, installments generated, and funding record created.";
        }

        private async Task GenerateInstallments(string loanId, decimal amount, decimal interestRate)
        {
            // Hitung total pembayaran berdasarkan jumlah pinjaman dan suku bunga
            var totalPayment = CalculateTotalPayment(amount, interestRate, 12); // Asumsi 12 bulan

            // Dapatkan tanggal saat ini (waktu generate cicilan)
            var currentDate = DateTime.UtcNow;

            // Tambahkan cicilan ke dalam tabel cicilan
            for (int i = 1; i <= 12; i++)
            {
                var installment = new TrnDebtBills
                {
                    Id = Guid.NewGuid().ToString(),
                    LoansId = loanId,
                    InstallmentNumber = i,
                    // Mengatur agar cicilan dibulatkan hingga 2 angka di belakang koma
                    Amount = Math.Round(totalPayment / 12, 2), // Cicilan dibagi rata untuk 12 bulan, dibulatkan ke 2 desimal
                    Status = "pending", // Status awal cicilan
                    PaidAt = currentDate.AddMonths(i) // Set PaidAt menjadi bulan depan dan seterusnya
                };

                // Tambahkan cicilan ke konteks
                await _peerlandingContext.TrnDebtBills.AddAsync(installment);
            }

            // Simpan semua perubahan ke database
            await _peerlandingContext.SaveChangesAsync();
        }

        // Hitung total pembayaran berdasarkan jumlah pinjaman, suku bunga, dan durasi
        private decimal CalculateTotalPayment(decimal principal, decimal interestRate, int duration)
        {
            decimal monthlyRate = interestRate / 100 / 12; // Menghitung suku bunga bulanan
            var totalPayment = principal * (1 + (monthlyRate * duration)); // Menghitung total pembayaran
            return Math.Round(totalPayment, 2); // Membulatkan total payment ke 2 angka di belakang koma
        }










        public async Task<List<ResLoanHistoryDto>> GetDaftarPeminjamYangDipilih(string lenderId)
        {
            // Ambil data funding yang terkait dengan lender
            var fundings = await _peerlandingContext.TrnFunding
                .Where(f => f.LenderId == lenderId)
                .Include(f => f.Loans) // Mengambil data pinjaman
                .Select(f => new ResLoanHistoryDto
                {
                    Id = f.Loans.Id,
                    Amount = f.Loans.Amount,
                    InterestRate = f.Loans.InterestRate,
                    Duration = f.Loans.Duration,
                    Status = f.Loans.Status
                })
                .ToListAsync();

            return fundings;
        }

    }
}

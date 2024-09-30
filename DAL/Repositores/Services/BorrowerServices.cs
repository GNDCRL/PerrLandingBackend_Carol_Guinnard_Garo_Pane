using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.DTO.Req.ReqLoan;
using DAL.DTO.Req.ReqLoanBorrower;
using DAL.DTO.Res;
using DAL.Model;
using DAL.Repositores.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositores.Services
{
    public class BorrowerService : IBorrowerService
    {
        private readonly PeerlandingContext _context;

        public BorrowerService(PeerlandingContext context)
        {
            _context = context;
        }






        // Mendapatkan daftar pinjaman yang diajukan oleh borrower
        public async Task<IEnumerable<ResListLoanDto>> GetDaftarPinjaman(string borrowerId)
        {
            var loans = await _context.MstLoans
                .Where(loan => loan.BorrowerId == borrowerId)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status
                }).ToListAsync();

            return loans;
        }






        public async Task<IEnumerable<ResLoanHistoryDto>> GetHistoryLoan(string borrowerId)
        {
            var loans = await _context.MstLoans
                .Where(loan => loan.BorrowerId == borrowerId && loan.Status == "funded") // Tambahkan filter status di sini
                .Select(loan => new ResLoanHistoryDto
                {
                    Id = loan.Id,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    Installments = _context.TrnDebtBills
                        .Where(bill => bill.LoansId == loan.Id)
                        .Select(bill => new ResInstallmentDto
                        {
                            InstallmentNumber = bill.InstallmentNumber,
                            Amount = bill.Amount,
                            Status = bill.Status,
                            PaidAt = bill.PaidAt
                        }).ToList() // Mengubah List ke dalam bentuk List secara langsung
                }).ToListAsync(); // Ini akan mengeksekusi query dan mengembalikan hasilnya

            return loans;
        }







        // Menambahkan permohonan pinjaman baru 
        public async Task<string> AddRequestLoan(ReqLoanBorrowerDto request)
        {
            // Validasi BorrowerId
            if (string.IsNullOrEmpty(request.BorrowerId))
            {
                throw new ArgumentException("BorrowerId cannot be null or empty.");
            }

            // Simpan pengajuan pinjaman
            var loan = new MstLoans
            {
                Id = Guid.NewGuid().ToString(),
                BorrowerId = request.BorrowerId,
                Amount = request.Amount,
                InterestRate = request.InterestRate,
                Duration = 12,
                Status = "requested",
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            try
            {
                // Simpan data pinjaman
                await _context.MstLoans.AddAsync(loan);
                await _context.SaveChangesAsync();

                return "Loan request submitted successfully.";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }








        // Merekam pembayaran pinjaman oleh borrower
        public async Task<string> RepayLoans(string borrowerId, string loanId, List<int> installmentNumbers)
        {
            // Cari pinjaman berdasarkan borrowerId dan loanId
            var loan = await _context.MstLoans
                .Where(l => l.Id == loanId && l.BorrowerId == borrowerId)
                .FirstOrDefaultAsync();

            if (loan == null)
            {
                return "Loan not found or you are not the borrower.";
            }

            decimal totalPayment = 0;

            foreach (var installmentNumber in installmentNumbers)
            {
                // Cari cicilan yang ingin dibayar
                var installmentToPay = await _context.TrnDebtBills
                    .Where(i => i.LoansId == loanId && i.InstallmentNumber == installmentNumber)
                    .FirstOrDefaultAsync();

                // Periksa apakah cicilan tidak ditemukan atau sudah dibayar
                if (installmentToPay == null || installmentToPay.Status == "paid")
                {
                    return $"Installment {installmentNumber} already paid or not found.";
                }

                // Cek apakah cicilan bulan sebelumnya sudah dibayar
                if (installmentNumber > 1)
                {
                    var previousInstallment = await _context.TrnDebtBills
                        .Where(i => i.LoansId == loanId && i.InstallmentNumber == installmentNumber - 1)
                        .FirstOrDefaultAsync();

                    // Jika cicilan sebelumnya tidak ada atau belum dibayar, kembalikan pesan
                    if (previousInstallment == null || previousInstallment.Status != "paid")
                    {
                        return $"You must pay the previous installment {installmentNumber - 1} first.";
                    }
                }

                // Update status cicilan saat ini menjadi 'paid'
                installmentToPay.Status = "paid";

                // Set tanggal pembayaran
                installmentToPay.PaidAt = DateTime.UtcNow; // Menyimpan waktu pembayaran

                // Tambahkan jumlah cicilan ke total pembayaran
                totalPayment += installmentToPay.Amount;
            }

            // Update saldo borrower
            var borrowerAccount = await _context.MstUsers
                .Where(a => a.Id == borrowerId) // Asumsikan ada tabel Account
                .FirstOrDefaultAsync();

            if (borrowerAccount != null)
            {
                borrowerAccount.Balance -= totalPayment; // Mengurangi saldo borrower
            }

            // Update saldo lender
            var lenderId = loan.Id; // Ambil ID lender dari pinjaman
            var lenderAccount = await _context.MstUsers
                .Where(a => a.Id == lenderId)
                .FirstOrDefaultAsync();

            if (lenderAccount != null)
            {
                lenderAccount.Balance += totalPayment; // Menambahkan saldo lender
            }

            // Buat entri baru dalam tabel trn_repayment
            var repayment = new TrnRepayment
            {
                Id = Guid.NewGuid().ToString(),
                LoansId = loanId,
                Amount = totalPayment,
                RepaidAmount = await _context.TrnRepayment
                    .Where(r => r.LoansId == loanId)
                    .SumAsync(r => r.Amount) + totalPayment,
                BalanceAmount = (await _context.TrnDebtBills
                    .Where(b => b.LoansId == loanId && b.Status != "paid")
                    .SumAsync(b => b.Amount)) - (totalPayment + await _context.TrnRepayment
                    .Where(r => r.LoansId == loanId)
                    .SumAsync(r => r.Amount))
            };

            // Menyimpan entri pembayaran
            await _context.TrnRepayment.AddAsync(repayment);

            // Simpan semua perubahan ke database
            await _context.SaveChangesAsync();

            return "Installments paid successfully.";
        }
    }

}

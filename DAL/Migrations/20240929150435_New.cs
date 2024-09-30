using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class New : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trn_debtbills",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    loans_id = table.Column<string>(type: "text", nullable: false),
                    installment_number = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    paid_at = table.Column<DateTime?>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_debtbills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trn_debtbills_mst_loans_loans_id",
                        column: x => x.loans_id,
                        principalTable: "mst_loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trn_repayment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    loans_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    repaid_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    balance_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    repaid_status = table.Column<string>(type: "text", nullable: false),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_repayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trn_repayment_mst_loans_loans_id",
                        column: x => x.loans_id,
                        principalTable: "mst_loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trn_debtbills_loans_id",
                table: "trn_debtbills",
                column: "loans_id");

            migrationBuilder.CreateIndex(
                name: "IX_trn_repayment_loans_id",
                table: "trn_repayment",
                column: "loans_id");
        }
    }
}

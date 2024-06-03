using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvailabilityPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameSurname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "DateRange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    AvailabilityPeriodsId = table.Column<int>(type: "int", nullable: true),
                    AvailabilityPeriodsId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateRange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DateRange_AvailabilityPeriods_AvailabilityPeriodsId",
                        column: x => x.AvailabilityPeriodsId,
                        principalTable: "AvailabilityPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DateRange_AvailabilityPeriods_AvailabilityPeriodsId1",
                        column: x => x.AvailabilityPeriodsId1,
                        principalTable: "AvailabilityPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Deposits",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AvailabilityPeriodsId = table.Column<int>(type: "int", nullable: false),
                    ClimateControl = table.Column<bool>(type: "bit", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposits", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Deposits_AvailabilityPeriods_AvailabilityPeriodsId",
                        column: x => x.AvailabilityPeriodsId,
                        principalTable: "AvailabilityPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepositId = table.Column<int>(type: "int", nullable: false),
                    DepositName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ClientEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: true),
                    DurationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_DateRange_DurationId",
                        column: x => x.DurationId,
                        principalTable: "DateRange",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Deposits_DepositName",
                        column: x => x.DepositName,
                        principalTable: "Deposits",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Users_ClientEmail",
                        column: x => x.ClientEmail,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidityId = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    DepositName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotions_DateRange_ValidityId",
                        column: x => x.ValidityId,
                        principalTable: "DateRange",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Promotions_Deposits_DepositName",
                        column: x => x.DepositName,
                        principalTable: "Deposits",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ClientEmail",
                table: "Bookings",
                column: "ClientEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DepositName",
                table: "Bookings",
                column: "DepositName");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DurationId",
                table: "Bookings",
                column: "DurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PaymentId",
                table: "Bookings",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_DateRange_AvailabilityPeriodsId",
                table: "DateRange",
                column: "AvailabilityPeriodsId");

            migrationBuilder.CreateIndex(
                name: "IX_DateRange_AvailabilityPeriodsId1",
                table: "DateRange",
                column: "AvailabilityPeriodsId1");

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_AvailabilityPeriodsId",
                table: "Deposits",
                column: "AvailabilityPeriodsId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_DepositName",
                table: "Promotions",
                column: "DepositName");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_ValidityId",
                table: "Promotions",
                column: "ValidityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DateRange");

            migrationBuilder.DropTable(
                name: "Deposits");

            migrationBuilder.DropTable(
                name: "AvailabilityPeriods");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerInfo_FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerInfo_Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CustomerInfo_PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CustomerInfo_Nationality = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CustomerInfo_PassportNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    TripDetails_TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripDetails_TripName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TripDetails_Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TripDetails_Duration = table.Column<int>(type: "int", nullable: false),
                    PriceDetails_BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceDetails_Taxes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceDetails_Discounts = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceDetails_TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceDetails_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    SpecialRequests = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");
        }
    }
}

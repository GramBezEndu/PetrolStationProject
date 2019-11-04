using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PetrolStation.Migrations
{
    public partial class PetrolStationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    IdClient = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    NIP = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: false),
                    HouseNumber = table.Column<string>(nullable: false),
                    ApartmentNumber = table.Column<int>(nullable: true),
                    Postcode = table.Column<string>(nullable: false),
                    Locality = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.IdClient);
                });

            migrationBuilder.CreateTable(
                name: "Fuel",
                columns: table => new
                {
                    IdFuel = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    PriceForLiter = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fuel", x => x.IdFuel);
                });

            migrationBuilder.CreateTable(
                name: "GasPump",
                columns: table => new
                {
                    IdGasPump = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GasPump", x => x.IdGasPump);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    IdProduct = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    QuantityInStorage = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    PriceInPoints = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.IdProduct);
                });

            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    IdCar = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClient = table.Column<int>(nullable: false),
                    NumberPlate = table.Column<string>(nullable: false),
                    CarBrand = table.Column<string>(nullable: true),
                    CarModel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Car", x => x.IdCar);
                    table.ForeignKey(
                        name: "FK_Car_Client_IdClient",
                        column: x => x.IdClient,
                        principalTable: "Client",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoyalityCard",
                columns: table => new
                {
                    IdLoyalityCard = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClient = table.Column<int>(nullable: false),
                    ActualPoints = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyalityCard", x => x.IdLoyalityCard);
                    table.ForeignKey(
                        name: "FK_LoyalityCard_Client_IdClient",
                        column: x => x.IdClient,
                        principalTable: "Client",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuelTank",
                columns: table => new
                {
                    IdFuelTank = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFuel = table.Column<int>(nullable: false),
                    Capacity = table.Column<float>(nullable: false),
                    ActualQuantity = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTank", x => x.IdFuelTank);
                    table.ForeignKey(
                        name: "FK_FuelTank_Fuel_IdFuel",
                        column: x => x.IdFuel,
                        principalTable: "Fuel",
                        principalColumn: "IdFuel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fueling",
                columns: table => new
                {
                    IdFueling = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFuel = table.Column<int>(nullable: false),
                    IdGasPump = table.Column<int>(nullable: false),
                    Quantity = table.Column<float>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fueling", x => x.IdFueling);
                    table.ForeignKey(
                        name: "FK_Fueling_Fuel_IdFuel",
                        column: x => x.IdFuel,
                        principalTable: "Fuel",
                        principalColumn: "IdFuel",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fueling_GasPump_IdGasPump",
                        column: x => x.IdGasPump,
                        principalTable: "GasPump",
                        principalColumn: "IdGasPump",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    IdTransaction = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLoyalityCard = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    IdClient = table.Column<int>(nullable: true),
                    IdCar = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.IdTransaction);
                    table.ForeignKey(
                        name: "FK_Transaction_LoyalityCard_IdLoyalityCard",
                        column: x => x.IdLoyalityCard,
                        principalTable: "LoyalityCard",
                        principalColumn: "IdLoyalityCard",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_Car_IdCar",
                        column: x => x.IdCar,
                        principalTable: "Car",
                        principalColumn: "IdCar",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_Client_IdClient",
                        column: x => x.IdClient,
                        principalTable: "Client",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PumpTank",
                columns: table => new
                {
                    IdGasPump = table.Column<int>(nullable: false),
                    IdFuelTank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpTank", x => new { x.IdGasPump, x.IdFuelTank });
                    table.ForeignKey(
                        name: "FK_PumpTank_FuelTank_IdFuelTank",
                        column: x => x.IdFuelTank,
                        principalTable: "FuelTank",
                        principalColumn: "IdFuelTank",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpTank_GasPump_IdGasPump",
                        column: x => x.IdGasPump,
                        principalTable: "GasPump",
                        principalColumn: "IdGasPump",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuelingList",
                columns: table => new
                {
                    IdFueling = table.Column<int>(nullable: false),
                    IdTransaction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelingList", x => x.IdFueling);
                    table.ForeignKey(
                        name: "FK_FuelingList_Fueling_IdFueling",
                        column: x => x.IdFueling,
                        principalTable: "Fueling",
                        principalColumn: "IdFueling",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelingList_Transaction_IdTransaction",
                        column: x => x.IdTransaction,
                        principalTable: "Transaction",
                        principalColumn: "IdTransaction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductList",
                columns: table => new
                {
                    IdTransaction = table.Column<int>(nullable: false),
                    IdProduct = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductList", x => new { x.IdProduct, x.IdTransaction });
                    table.ForeignKey(
                        name: "FK_ProductList_Product_IdProduct",
                        column: x => x.IdProduct,
                        principalTable: "Product",
                        principalColumn: "IdProduct",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductList_Transaction_IdTransaction",
                        column: x => x.IdTransaction,
                        principalTable: "Transaction",
                        principalColumn: "IdTransaction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Car_IdClient",
                table: "Car",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Fueling_IdFuel",
                table: "Fueling",
                column: "IdFuel");

            migrationBuilder.CreateIndex(
                name: "IX_Fueling_IdGasPump",
                table: "Fueling",
                column: "IdGasPump");

            migrationBuilder.CreateIndex(
                name: "IX_FuelingList_IdTransaction",
                table: "FuelingList",
                column: "IdTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTank_IdFuel",
                table: "FuelTank",
                column: "IdFuel");

            migrationBuilder.CreateIndex(
                name: "IX_LoyalityCard_IdClient",
                table: "LoyalityCard",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_ProductList_IdTransaction",
                table: "ProductList",
                column: "IdTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_PumpTank_IdFuelTank",
                table: "PumpTank",
                column: "IdFuelTank");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_IdLoyalityCard",
                table: "Transaction",
                column: "IdLoyalityCard");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_IdCar",
                table: "Transaction",
                column: "IdCar");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_IdClient",
                table: "Transaction",
                column: "IdClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelingList");

            migrationBuilder.DropTable(
                name: "ProductList");

            migrationBuilder.DropTable(
                name: "PumpTank");

            migrationBuilder.DropTable(
                name: "Fueling");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "FuelTank");

            migrationBuilder.DropTable(
                name: "GasPump");

            migrationBuilder.DropTable(
                name: "LoyalityCard");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "Fuel");

            migrationBuilder.DropTable(
                name: "Client");
        }
    }
}

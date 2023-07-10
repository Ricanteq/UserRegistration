#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AUserRegister.Persistence.Migrations;

public partial class InitialMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Users",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FirstName = table.Column<string>("text", nullable: false),
                LastName = table.Column<string>("text", nullable: false),
                Email = table.Column<string>("text", nullable: false),
                Password = table.Column<string>("text", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Users", x => x.Id); });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Users");
    }
}
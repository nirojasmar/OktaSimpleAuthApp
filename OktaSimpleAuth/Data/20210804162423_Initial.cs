using Microsoft.EntityFrameworkCore.Migrations;

namespace OktaSimpleAuth.Data
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    NameIdentifier = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Password = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Firstname = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Lastname = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Mobile = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Roles = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.UserId);
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "UserId", "Email", "Firstname", "Lastname", "Mobile", "NameIdentifier", "Password", "Provider", "Roles", "Username" },
                values: new object[] { 1, "nirojasmar@gmail.com", "Nicolas", "Tester", "123456789", null, "123", "Cookies", "Admin", "nirojasmar@gmail.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");
        }
    }
}

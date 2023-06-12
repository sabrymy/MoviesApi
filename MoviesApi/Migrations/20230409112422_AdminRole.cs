using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesApiNew.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"insert into dbo.AspNetRoles (Id, [Name],[NormalizedName]) values ('03b64fa7-8226-486d-8f65-f041c96bef63','Admin','Admin')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete dbo.AspNetRoles where Id = '03b64fa7-8226-486d-8f65-f041c96bef63'");
        }
    }
}

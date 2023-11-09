using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseProject.DAO.Migrations
{
    public partial class ProcLimparLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"
                CREATE OR ALTER PROCEDURE LimparLogs (@N INT)
                AS
                BEGIN
                    --Limpa os logs de N dias atrás
	                DELETE FROM Log WHERE TimeStamp <= DATEADD(DAY, -@N, CAST(GETDATE() AS DATE))
                END
            ";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

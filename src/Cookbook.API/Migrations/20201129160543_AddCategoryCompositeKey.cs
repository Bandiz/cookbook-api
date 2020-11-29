using Microsoft.EntityFrameworkCore.Migrations;

namespace Cookbook.API.Migrations
{
    public partial class AddCategoryCompositeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Recipe_RecipeId",
                table: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "Category",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                columns: new[] { "Name", "RecipeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Recipe_RecipeId",
                table: "Category",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Recipe_RecipeId",
                table: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeId",
                table: "Category",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Recipe_RecipeId",
                table: "Category",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoanOrigination.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresSQLMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "User"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "loan_applications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    loan_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    term_months = table.Column<int>(type: "integer", nullable: false),
                    income_monthly = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    debt_monthly = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    credit_score = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewed_by = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loan_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_loan_applications_users_reviewed_by",
                        column: x => x.reviewed_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_loan_applications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    loan_application_id = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    extracted_data_json = table.Column<string>(type: "jsonb", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    verified_by = table.Column<int>(type: "integer", nullable: true),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_documents_loan_applications_loan_application_id",
                        column: x => x.loan_application_id,
                        principalTable: "loan_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_documents_users_verified_by",
                        column: x => x.verified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "predictions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    loan_application_id = table.Column<int>(type: "integer", nullable: false),
                    approval_probability = table.Column<float>(type: "real", nullable: false),
                    explanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    model_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "1.0"),
                    confidence_score = table.Column<float>(type: "real", nullable: false),
                    features_used = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_predictions", x => x.id);
                    table.ForeignKey(
                        name: "FK_predictions_loan_applications_loan_application_id",
                        column: x => x.loan_application_id,
                        principalTable: "loan_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_documents_loan_application_id",
                table: "documents",
                column: "loan_application_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_type",
                table: "documents",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_documents_uploaded_at",
                table: "documents",
                column: "uploaded_at");

            migrationBuilder.CreateIndex(
                name: "IX_documents_verified_by",
                table: "documents",
                column: "verified_by");

            migrationBuilder.CreateIndex(
                name: "IX_loan_applications_created_at",
                table: "loan_applications",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_loan_applications_reviewed_by",
                table: "loan_applications",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "IX_loan_applications_status",
                table: "loan_applications",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_loan_applications_user_id",
                table: "loan_applications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_predictions_created_at",
                table: "predictions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_predictions_loan_application_id",
                table: "predictions",
                column: "loan_application_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "predictions");

            migrationBuilder.DropTable(
                name: "loan_applications");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

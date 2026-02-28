using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebStudents.Migrations
{
    /// <inheritdoc />
    public partial class AttestationRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Course_CourseId",
                table: "Assignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Proffessor_Course_CourseId",
                table: "Proffessor");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Course_CourseId",
                table: "Student");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Student",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentGroupId",
                table: "Student",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Proffessor",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "DisciplineOfferingId",
                table: "Grade",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DisciplineOfferingId",
                table: "Attendance",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Assignment",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "DisciplineOfferingId",
                table: "Assignment",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcademicYear",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartYear = table.Column<int>(type: "integer", nullable: false),
                    EndYear = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYear", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discipline",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Hours = table.Column<int>(type: "integer", nullable: false),
                    ControlType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discipline", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Semester",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semester", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Semester_AcademicYear_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StudyYear = table.Column<int>(type: "integer", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentGroup_AcademicYear_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisciplineOffering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisciplineId = table.Column<Guid>(type: "uuid", nullable: false),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProffessorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplineOffering", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisciplineOffering_Discipline_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Discipline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisciplineOffering_Proffessor_ProffessorId",
                        column: x => x.ProffessorId,
                        principalTable: "Proffessor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisciplineOffering_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisciplineOffering_StudentGroup_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GradeSheet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisciplineOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeSheet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeSheet_DisciplineOffering_DisciplineOfferingId",
                        column: x => x.DisciplineOfferingId,
                        principalTable: "DisciplineOffering",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinalGrade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GradeSheetId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinalScore = table.Column<decimal>(type: "numeric", nullable: true),
                    FinalMark = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalGrade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinalGrade_GradeSheet_GradeSheetId",
                        column: x => x.GradeSheetId,
                        principalTable: "GradeSheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinalGrade_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_StudentGroupId",
                table: "Student",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Grade_DisciplineOfferingId_StudentId",
                table: "Grade",
                columns: new[] { "DisciplineOfferingId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_DisciplineOfferingId_StudentId_Date",
                table: "Attendance",
                columns: new[] { "DisciplineOfferingId", "StudentId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_DisciplineOfferingId",
                table: "Assignment",
                column: "DisciplineOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplineOffering_DisciplineId",
                table: "DisciplineOffering",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplineOffering_ProffessorId",
                table: "DisciplineOffering",
                column: "ProffessorId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplineOffering_SemesterId",
                table: "DisciplineOffering",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplineOffering_StudentGroupId_SemesterId_DisciplineId_P~",
                table: "DisciplineOffering",
                columns: new[] { "StudentGroupId", "SemesterId", "DisciplineId", "ProffessorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinalGrade_GradeSheetId_StudentId",
                table: "FinalGrade",
                columns: new[] { "GradeSheetId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinalGrade_StudentId",
                table: "FinalGrade",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeSheet_DisciplineOfferingId",
                table: "GradeSheet",
                column: "DisciplineOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_Semester_AcademicYearId",
                table: "Semester",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroup_AcademicYearId",
                table: "StudentGroup",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Course_CourseId",
                table: "Assignment",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_DisciplineOffering_DisciplineOfferingId",
                table: "Assignment",
                column: "DisciplineOfferingId",
                principalTable: "DisciplineOffering",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_DisciplineOffering_DisciplineOfferingId",
                table: "Attendance",
                column: "DisciplineOfferingId",
                principalTable: "DisciplineOffering",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Grade_DisciplineOffering_DisciplineOfferingId",
                table: "Grade",
                column: "DisciplineOfferingId",
                principalTable: "DisciplineOffering",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Proffessor_Course_CourseId",
                table: "Proffessor",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Course_CourseId",
                table: "Student",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_StudentGroup_StudentGroupId",
                table: "Student",
                column: "StudentGroupId",
                principalTable: "StudentGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Course_CourseId",
                table: "Assignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_DisciplineOffering_DisciplineOfferingId",
                table: "Assignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_DisciplineOffering_DisciplineOfferingId",
                table: "Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Grade_DisciplineOffering_DisciplineOfferingId",
                table: "Grade");

            migrationBuilder.DropForeignKey(
                name: "FK_Proffessor_Course_CourseId",
                table: "Proffessor");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Course_CourseId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_StudentGroup_StudentGroupId",
                table: "Student");

            migrationBuilder.DropTable(
                name: "FinalGrade");

            migrationBuilder.DropTable(
                name: "GradeSheet");

            migrationBuilder.DropTable(
                name: "DisciplineOffering");

            migrationBuilder.DropTable(
                name: "Discipline");

            migrationBuilder.DropTable(
                name: "Semester");

            migrationBuilder.DropTable(
                name: "StudentGroup");

            migrationBuilder.DropTable(
                name: "AcademicYear");

            migrationBuilder.DropIndex(
                name: "IX_Student_StudentGroupId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Grade_DisciplineOfferingId_StudentId",
                table: "Grade");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_DisciplineOfferingId_StudentId_Date",
                table: "Attendance");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_DisciplineOfferingId",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "DisciplineOfferingId",
                table: "Grade");

            migrationBuilder.DropColumn(
                name: "DisciplineOfferingId",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "DisciplineOfferingId",
                table: "Assignment");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Student",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Proffessor",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Assignment",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Course_CourseId",
                table: "Assignment",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proffessor_Course_CourseId",
                table: "Proffessor",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Course_CourseId",
                table: "Student",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

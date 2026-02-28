using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class FinalGradeService
{
    private readonly StudentDbContext _context;

    public FinalGradeService(StudentDbContext context)
    {
        _context = context;
    }

    public Task<List<FinalGrade>> GetBySheetAsync(Guid sheetId)
    {
        return _context.FinalGrades
            .Where(x => x.GradeSheetId == sheetId)
            .OrderBy(x => x.StudentId)
            .ToListAsync();
    }

    public async Task<List<FinalGrade>> RecalculateAsync(Guid sheetId)
    {
        var sheet = await _context.GradeSheets
            .Include(s => s.DisciplineOffering)
                .ThenInclude(o => o!.Discipline)
            .Include(s => s.DisciplineOffering)
                .ThenInclude(o => o!.StudentGroup)
                    .ThenInclude(g => g!.Students)
            .FirstOrDefaultAsync(s => s.Id == sheetId);

        if (sheet == null)
        {
            throw new ApiException("Ведомость не найдена", StatusCodes.Status404NotFound);
        }

        if (sheet.Status == SheetStatus.Closed)
        {
            throw new ApiException("Ведомость закрыта, пересчёт запрещён", StatusCodes.Status409Conflict);
        }

        var offering = sheet.DisciplineOffering;
        if (offering == null)
        {
            throw new ApiException("У ведомости отсутствует offering", StatusCodes.Status400BadRequest);
        }

        var groupStudentIds = offering.StudentGroup?.Students.Select(s => s.Id).ToHashSet() ?? new HashSet<Guid>();

        var grades = await _context.Grades
            .Where(g => g.DisciplineOfferingId == offering.Id)
            .ToListAsync();

        var existingFinals = await _context.FinalGrades
            .Where(f => f.GradeSheetId == sheetId)
            .ToListAsync();

        foreach (var studentId in groupStudentIds)
        {
            var studentScores = grades.Where(g => g.StudentId == studentId).Select(g => g.Score).ToList();
            var avg = studentScores.Count == 0 ? 0m : Convert.ToDecimal(studentScores.Average());

            var mark = MapMark(avg, offering.Discipline?.ControlType ?? ControlType.Exam);

            var final = existingFinals.FirstOrDefault(f => f.StudentId == studentId);
            if (final == null)
            {
                final = new FinalGrade
                {
                    Id = Guid.NewGuid(),
                    GradeSheetId = sheetId,
                    StudentId = studentId
                };
                _context.FinalGrades.Add(final);
                existingFinals.Add(final);
            }

            final.FinalScore = Math.Round(avg, 2);
            final.FinalMark = mark;
            final.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return existingFinals;
    }

    public async Task<FinalGrade> UpsertManualAsync(Guid sheetId, Guid studentId, decimal? finalScore, string? finalMark)
    {
        var sheet = await _context.GradeSheets.FirstOrDefaultAsync(s => s.Id == sheetId);
        if (sheet == null)
        {
            throw new ApiException("Ведомость не найдена", StatusCodes.Status404NotFound);
        }

        if (sheet.Status == SheetStatus.Closed)
        {
            throw new ApiException("Ведомость закрыта, изменения запрещены", StatusCodes.Status409Conflict);
        }

        var final = await _context.FinalGrades.FirstOrDefaultAsync(f => f.GradeSheetId == sheetId && f.StudentId == studentId);
        if (final == null)
        {
            final = new FinalGrade
            {
                Id = Guid.NewGuid(),
                GradeSheetId = sheetId,
                StudentId = studentId
            };
            _context.FinalGrades.Add(final);
        }

        final.FinalScore = finalScore;
        final.FinalMark = finalMark;
        final.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return final;
    }

    private static string MapMark(decimal score, ControlType controlType)
    {
        if (controlType == ControlType.PassFail)
        {
            return score >= 60m ? "зачет" : "незачет";
        }

        if (score >= 85m) return "5";
        if (score >= 70m) return "4";
        if (score >= 55m) return "3";
        return "2";
    }
}

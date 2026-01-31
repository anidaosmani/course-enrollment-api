using CourseEnrollment.Data;
using CourseEnrollment.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _db;
    public EnrollmentRepository(ApplicationDbContext db) => _db = db;

    public Task<Enrollment?> FindAsync(int studentId, int courseId) =>
        _db.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();
        return enrollment;
    }

    public Task<List<Enrollment>> GetByCourseAsync(int courseId) =>
        _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.CourseId == courseId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
}

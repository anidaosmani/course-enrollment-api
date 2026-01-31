using CourseEnrollment.Models;

namespace CourseEnrollment.Repositories;

public interface IEnrollmentRepository
{
    Task<Enrollment?> FindAsync(int studentId, int courseId);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<List<Enrollment>> GetByCourseAsync(int courseId);
}

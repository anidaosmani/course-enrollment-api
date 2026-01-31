using CourseEnrollment.Models;

namespace CourseEnrollment.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync(string? search);
    Task<Course?> GetByIdAsync(int id);
    Task<Course> CreateAsync(Course course);
    Task<bool> UpdateAsync(Course course);
    Task<bool> DeleteAsync(int id);
    Task<int> CountActiveEnrollmentsAsync(int courseId);
}

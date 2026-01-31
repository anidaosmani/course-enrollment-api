using CourseEnrollment.Data;
using CourseEnrollment.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _db;
    public CourseRepository(ApplicationDbContext db) => _db = db;

    public async Task<List<Course>> GetAllAsync(string? search)
    {
        var q = _db.Courses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(c => c.Title.ToLower().Contains(search.ToLower()));

        return await q.OrderBy(c => c.Title).ToListAsync();
    }

    public Task<Course?> GetByIdAsync(int id) =>
        _db.Courses.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Course> CreateAsync(Course course)
    {
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return course;
    }

    public async Task<bool> UpdateAsync(Course course)
    {
        _db.Courses.Update(course);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await GetByIdAsync(id);
        if (c is null) return false;

        _db.Courses.Remove(c);
        return await _db.SaveChangesAsync() > 0;
    }

    public Task<int> CountActiveEnrollmentsAsync(int courseId) =>
        _db.Enrollments.CountAsync(e => e.CourseId == courseId && e.Status == "Active");
}

using CourseEnrollment.Models;
using CourseEnrollment.Repositories;

namespace CourseEnrollment.Services;

public class EnrollmentService
{
    private readonly ICourseRepository _courses;
    private readonly IEnrollmentRepository _enrollments;

    public EnrollmentService(ICourseRepository courses, IEnrollmentRepository enrollments)
    {
        _courses = courses;
        _enrollments = enrollments;
    }

    public async Task<(bool ok, string error, Enrollment? enrollment)> EnrollAsync(int studentId, int courseId)
    {
        var existing = await _enrollments.FindAsync(studentId, courseId);
        if (existing is not null)
            return (false, "Student is already enrolled in this course.", null);

        var course = await _courses.GetByIdAsync(courseId);
        if (course is null)
            return (false, "Course not found.", null);

        var taken = await _courses.CountActiveEnrollmentsAsync(courseId);
        if (taken >= course.MaxSeats)
            return (false, "Course is full.", null);

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            Status = "Active",
            EnrolledAt = DateTime.UtcNow
        };

        var created = await _enrollments.CreateAsync(enrollment);
        return (true, "", created);
    }
}

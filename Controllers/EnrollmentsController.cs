using CourseEnrollment.DTOs;
using CourseEnrollment.Data;
using CourseEnrollment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly EnrollmentService _service;
    private readonly ApplicationDbContext _db;

    public EnrollmentsController(EnrollmentService service, ApplicationDbContext db)
    {
        _service = service;
        _db = db;
    }

    // Student enrolls (secured)
    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<ActionResult> Enroll(EnrollmentCreateDto dto)
    {
        var studentExists = await _db.Students.AnyAsync(s => s.Id == dto.StudentId);
        if (!studentExists) return BadRequest("Student not found. Add a student in DB first.");

        var (ok, error, enrollment) = await _service.EnrollAsync(dto.StudentId, dto.CourseId);
        if (!ok) return Conflict(error);

        return Ok(new { Message = "Enrolled successfully.", EnrollmentId = enrollment!.Id });
    }

    // Relationship demo (Admin): show all enrollments for a course
    [Authorize(Roles = "Admin")]
    [HttpGet("by-course/{courseId:int}")]
    public async Task<ActionResult<List<EnrollmentReadDto>>> GetByCourse(int courseId)
    {
        var list = await _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.CourseId == courseId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        var result = list.Select(e => new EnrollmentReadDto(
            e.Id,
            e.StudentId,
            e.Student?.FullName ?? "",
            e.CourseId,
            e.Course?.Title ?? "",
            e.EnrolledAt,
            e.Status
        )).ToList();

        return Ok(result);
    }
}

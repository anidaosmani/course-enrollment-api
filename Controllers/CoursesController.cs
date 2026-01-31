using CourseEnrollment.DTOs;
using CourseEnrollment.Models;
using CourseEnrollment.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseEnrollment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseRepository _repo;
    public CoursesController(ICourseRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<List<CourseReadDto>>> GetAll([FromQuery] string? search)
    {
        var courses = await _repo.GetAllAsync(search);
        var result = new List<CourseReadDto>();

        foreach (var c in courses)
        {
            var taken = await _repo.CountActiveEnrollmentsAsync(c.Id);
            result.Add(new CourseReadDto(
                c.Id, c.Title, c.Credits, c.MaxSeats,
                taken,
                Math.Max(0, c.MaxSeats - taken)
            ));
        }

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseReadDto>> GetById(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c is null) return NotFound();

        var taken = await _repo.CountActiveEnrollmentsAsync(id);

        return Ok(new CourseReadDto(
            c.Id, c.Title, c.Credits, c.MaxSeats,
            taken,
            Math.Max(0, c.MaxSeats - taken)
        ));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> Create(CourseCreateDto dto)
    {
        var created = await _repo.CreateAsync(new Course
        {
            Title = dto.Title,
            Credits = dto.Credits,
            MaxSeats = dto.MaxSeats
        });

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, CourseCreateDto dto)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c is null) return NotFound();

        c.Title = dto.Title;
        c.Credits = dto.Credits;
        c.MaxSeats = dto.MaxSeats;

        await _repo.UpdateAsync(c);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var ok = await _repo.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}

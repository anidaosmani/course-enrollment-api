namespace CourseEnrollment.DTOs;

public record CourseCreateDto(string Title, int Credits, int MaxSeats);

public record CourseReadDto(
    int Id,
    string Title,
    int Credits,
    int MaxSeats,
    int TakenSeats,
    int AvailableSeats
);

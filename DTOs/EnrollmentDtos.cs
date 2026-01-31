namespace CourseEnrollment.DTOs;

public record EnrollmentCreateDto(int StudentId, int CourseId);

public record EnrollmentReadDto(
    int Id,
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseTitle,
    DateTime EnrolledAt,
    string Status
);

namespace CourseEnrollment.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int MaxSeats { get; set; } = 30;

        public List<Enrollment> Enrollments { get; set; } = new();
    }
}

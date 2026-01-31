using Microsoft.AspNetCore.Identity;

namespace CourseEnrollment.Services;

public interface ITokenService
{
    (string token, DateTime expiresAt) CreateToken(IdentityUser user, string role);
}

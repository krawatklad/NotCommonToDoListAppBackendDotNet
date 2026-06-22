using Application.Authentication.Interfaces;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using IdentityPasswordHasher = Microsoft.AspNetCore.Identity.PasswordHasher<Domain.Entities.User>;

namespace Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private readonly IdentityPasswordHasher _innerHasher = new();

    public string HashPassword(string password)
    {
        return _innerHasher.HashPassword(user: null!, password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = _innerHasher.VerifyHashedPassword(user: null!, passwordHash, password);
        return result != PasswordVerificationResult.Failed;
    }
}

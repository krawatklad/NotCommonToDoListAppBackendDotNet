using Domain.Entities;

namespace Application.Authentication.Interfaces;

public interface IJwtTokenGenerator
{    
    string GenerateToken(User user);
}
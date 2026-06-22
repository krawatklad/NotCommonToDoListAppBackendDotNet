using Domain.Entities;

namespace Application.Authentication.DTOs;

public record LoginResult(User User, string Token);
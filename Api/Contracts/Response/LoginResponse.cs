namespace Api.Contracts.Response;

public record LoginResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Token
);
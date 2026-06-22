namespace Api.Contracts.Request;

public record LoginRequest(
    string Email,
    string Password
);
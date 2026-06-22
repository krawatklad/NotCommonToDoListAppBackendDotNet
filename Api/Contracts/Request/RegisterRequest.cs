namespace Api.Contracts.Request;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);
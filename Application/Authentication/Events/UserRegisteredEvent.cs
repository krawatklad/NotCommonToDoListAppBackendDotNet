namespace Application.Authentication.Events;

public record UserRegisteredEvent(string Email, string FirstName);

namespace Application.Authentication.Events.UserRegistered;

public record UserRegisteredEvent(string Email, string FirstName);

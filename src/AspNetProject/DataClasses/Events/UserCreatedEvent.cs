namespace AspNetProject.Events;

public record UserCreatedEvent(long UserId, string Email);
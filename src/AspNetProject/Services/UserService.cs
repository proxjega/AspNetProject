using AspNetProject.Models;
using AspNetProject.DTOs;
using Microsoft.AspNetCore.Identity;
using AspNetProject.Controllers;
using AspNetProject.Events;

namespace AspNetProject.Services;

public class UserService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ApplicationContext _context;

    public UserService(IEventPublisher eventPublisher, ApplicationContext context)
    {
        _eventPublisher = eventPublisher;
        _context = context;
    }

    public async Task<UserDTO> CreateUserAsync(RegisterDTO registerDTO)
    {
        var user = new User
        {
            Email = registerDTO.Email,
            Name = registerDTO.Name,
            Surname = registerDTO.Surname,
            PasswordHash = ""
        };

        var hasher = new PasswordHasher<User>();
        var hashedPassword = hasher.HashPassword(user, registerDTO.password);
        user.PasswordHash = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _eventPublisher.PublishAsync(
            new UserCreatedEvent(user.Id, user.Email),
            routingKey: "user.created");

        return UserController.UserToDTO(user);
    }
}


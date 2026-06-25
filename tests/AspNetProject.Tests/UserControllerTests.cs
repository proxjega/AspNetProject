using Xunit;
using AspNetProject.Controllers;
using Moq;
using AspNetProject.Models;
using Microsoft.AspNetCore.Mvc;
using AspNetProject.Tests.Helpers;

namespace AspNetProject.Tests;

public class UserControllerTests
{

    [Fact]
    public async Task GetUsers_ShouldReturnNoUsers()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.Create();
        var controller = new UserController(context);

        // Act
        var result = await controller.GetUsers();

        // Assert
        Assert.Equal([], result.Value);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser()
    {
        //Arrange
        using var context = InMemoryDbContextFactory.Create();

        long userId = 1;
        var name = "Nikita";
        var surname = "Jegorov";
        var email = "example@gmail.com";

        var user = new User{
            Id = userId,
            Name = name,
            Surname = surname,
            Email = email
        };

        var userDTO = new UserDTO{
            Id = userId,
            Name = name,
            Surname = surname,
            Email = email
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var controller = new UserController(context);

        // Act
        var result = await controller.GetUser(user.Id);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(userId, result.Value.Id);
    }

    [Fact]
    public async Task PostUser_WorksAsync()
    {
        // Given
        using var context = InMemoryDbContextFactory.Create();
        var user = TestData.CreateUser();
        var userDTO = TestData.CreateUserDTO();


        var controller = new UserController(context);

        // When
        var result = await controller.PostUser(userDTO);

        // Then
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(UserController.GetUser), created.ActionName);
        
        var returnedDto = Assert.IsType<UserDTO>(created.Value);
        Assert.Equal(userDTO.Name, returnedDto.Name);
        Assert.Equal(userDTO.Email, returnedDto.Email);

        var userFromDb = await context.Users.FindAsync(new object?[] { returnedDto.Id }, TestContext.Current.CancellationToken);
        Assert.NotNull(userFromDb);
        Assert.Equal(userDTO.Email, userFromDb.Email);

    }

}
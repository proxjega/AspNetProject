using AspNetProject.Models;

namespace AspNetProject.Tests.Helpers;

public static class TestData
{
    public static User CreateUser() =>
    new User
    {
        Id = 1,
        Name = "John",
        Surname = "Smith",
        Email = "john.smith@gmail.com"
    };

    public static UserDTO CreateUserDTO() =>
    new UserDTO
    {
        Id = 1,
        Name = "John",
        Surname = "Smith",
        Email = "john.smith@gmail.com"
    };

    public static Post CreatePost() =>
    new Post
    {
        Id = 1,
        Title = "Title",
        Content = "Content"
    };

    public static PostDTO CreatePostDTO() =>
    new PostDTO
    {
        Id = 1,
        Title = "Title",
        Content = "Content"
    };
}
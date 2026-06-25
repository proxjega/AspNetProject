using AspNetProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetProject.Tests.Helpers;

public static class InMemoryDbContextFactory
{
    public static ApplicationContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationContext(options);
    }
}
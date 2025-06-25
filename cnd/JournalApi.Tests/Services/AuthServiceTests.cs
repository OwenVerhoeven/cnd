using cnd;
using cnd.Models;
using cnd.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace JournalApi.Tests.Services
{
    public class AuthServiceTests
    {
        private JournalDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<JournalDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            return new JournalDbContext(options);
        }

        private IConfiguration GetFakeConfig()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "DezeSleutelMoetMinimaal32TekensLangZijn!" },
                    { "Jwt:Issuer", "JournalApi" },
                    { "Jwt:Audience", "JournalApiUser" }
                })
                .Build();
        }

        private class MockPasswordHasher : IPasswordHasher
        {
            public string HashPassword(string password) => "hashed_" + password;

            public bool VerifyPassword(string password, string passwordHash) =>
                passwordHash == "hashed_" + password;
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser()
        {
            var context = GetDbContext();
            var service = new AuthService(context, GetFakeConfig(), new MockPasswordHasher());

            var token = await service.RegisterAsync("testuser", "password123");

            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Single(context.Users);
        }

        [Fact]
        public async Task RegisterAsync_ShouldFailForDuplicateUser()
        {
            var context = GetDbContext();
            context.Users.Add(new User { Username = "duplicate", PasswordHash = "hashed_password123" });
            await context.SaveChangesAsync();

            var service = new AuthService(context, GetFakeConfig(), new MockPasswordHasher());
            var token = await service.RegisterAsync("duplicate", "password123");

            Assert.Null(token);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokenForValidUser()
        {
            var context = GetDbContext();
            var service = new AuthService(context, GetFakeConfig(), new MockPasswordHasher());

            await service.RegisterAsync("loginuser", "password123");

            var token = await service.LoginAsync("loginuser", "password123");

            Assert.NotNull(token);
            Assert.StartsWith("eyJ", token); // JWT tokens starten meestal zo
        }

        [Fact]
        public async Task LoginAsync_ShouldFailForWrongPassword()
        {
            var context = GetDbContext();
            var service = new AuthService(context, GetFakeConfig(), new MockPasswordHasher());

            await service.RegisterAsync("wrongpass", "password123");

            var token = await service.LoginAsync("wrongpass", "wrongpassword");

            Assert.Null(token);
        }
    }
}

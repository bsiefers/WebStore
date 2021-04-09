using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Application.CreateUser;
using WebStore.Database;
using WebStore.Models;
using WebStore.Models.Configs;

namespace WebStore.Tests.UnitTests.UserTest
{
    [TestClass]
    public class CreateUserTest
    {
        [TestMethod]
        public void TestCreateUser()
        {
            Random random = new Random();
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();                        
            

            User user = new User { 
                Id = 1,
                Email = "block@user.com",
                PasswordHash = "Test Hash",
                UserRole = "user",
                Salt = "Test Salt"
            };

            var userData = new List<User> { user }.AsQueryable();
            var mockUserSet = new Mock<DbSet<User>>();
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => userData.GetEnumerator());
            mockContext.Setup(m => m.Users).Returns(mockUserSet.Object);

            JWTConfig config = new JWTConfig{Key = "Test Key0123456789",  Issuer = "Test Issuer"};
            CreateUser createUser = new CreateUser(config,  mockContext.Object);
            
            var request = new CreateUser.Request { Email = "Test@User.com", Password = "testpassword" };
            
            var response = createUser.Do(request).Result;
            response.Should().NotBeNull();
            response.Status.Should().Be(201);
            response.Token.Should().NotBeNullOrEmpty();


            request.Email = "Block@User.com";
            response = createUser.Do(request).Result;
            response.Status.Should().Be(409);
            response.Token.Should().BeNullOrEmpty();

            request.Email = "block@user.com";
            response = createUser.Do(request).Result;
            response.Status.Should().Be(409);
            response.Token.Should().BeNullOrEmpty();
        }
    }
}

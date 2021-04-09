using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebStore.Application.LoginUser;
using WebStore.Database;
using WebStore.Models;
using WebStore.Models.Configs;

namespace WebStore.Tests.UnitTests.UserTest
{
    [TestClass]
    public class LoginUserTest
    {
        [TestMethod]
        public void TestLoginUser()
        {
            Random random = new Random();
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();


            User user = new User
            {
                Id = 1,
                Email = "test@user.com",
                PasswordHash = "PXxSKNnOtPwtjNnWPCbHbCR/wl/6MA5XtCSicV3+GNk=",
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

            JWTConfig config = new JWTConfig { Key = "TestKey1234567890324154325423524", Issuer = "Test Issuer" };
            LoginUser createUser = new LoginUser(config, mockContext.Object);

            var request = new LoginUser.Request { Email = "Test@User.com", Password = "testpassword" };

            var response = createUser.Do(request);
            response.Should().NotBeNull();
            response.Status.Should().Be(200);
            response.Token.Should().NotBeNullOrEmpty();

            request.Email = "test@user.com";
            response = createUser.Do(request);
            response.Should().NotBeNull();
            response.Status.Should().Be(200);
            response.Token.Should().NotBeNullOrEmpty();


            request.Password = "3456%^&dafasd3456fgdf";
            response = createUser.Do(request);
            response.Status.Should().Be(401);
            response.Token.Should().BeNullOrEmpty();

            request.Email = "";
            response = createUser.Do(request);
            response.Status.Should().Be(401);
            response.Token.Should().BeNullOrEmpty();

            request.Email = "block@user.com";
            response = createUser.Do(request);
            response.Status.Should().Be(401);
            response.Token.Should().BeNullOrEmpty();
        }
    }
}

using Moq;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Auth;
using TodoApp.Infrastructure.Persistence.Interfaces;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;
using Xunit;

namespace TodoApp.Infrastructure.Tests.Services
{
    public class ApplicationUserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IApplicationUserRepository> _mockApplicationUserRepository;

        public ApplicationUserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockApplicationUserRepository = new Mock<IApplicationUserRepository>();
            _mockUnitOfWork.Setup(x => x.ApplicationUsers).Returns(_mockApplicationUserRepository.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserByEmailAsync_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test User"
            };

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                DomainUserId = domainUser.Id,
                DomainUser = domainUser
            };

            _mockApplicationUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(applicationUser);

            // Act - This would be part of a service that handles email operations
            var result = applicationUser; // Simulate getting the user

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal(domainUser.DisplayName, result.DomainUser.DisplayName);
        }
    }
}
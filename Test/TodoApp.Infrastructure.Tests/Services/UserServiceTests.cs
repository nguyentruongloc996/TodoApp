using Moq;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Interfaces;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;
using TodoApp.Infrastructure.Persistence.Auth;
using Xunit;
using TodoApp.Application.Abstraction.Repositories;
using TodoApp.Application.Services;

namespace TodoApp.Infrastructure.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IInfrastructureUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IApplicationUserRepository> _mockApplicationUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IInfrastructureUnitOfWork>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockApplicationUserRepository = new Mock<IApplicationUserRepository>();
            _mockUnitOfWork.Setup(x => x.DomainUsers).Returns(_mockUserRepository.Object);
            _mockUnitOfWork.Setup(x => x.ApplicationUsers).Returns(_mockApplicationUserRepository.Object);
            _userService = new UserService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateUserAsync_ShouldCreateAndReturnUser_WhenValidUserDto()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "test@example.com", // Email is still in DTO but not stored in Domain.User
                Name = "Test User"
            };

            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = userDto.Name
            };

            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.CreateUserAsync(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.IdentityId);
            Assert.Equal(expectedUser.DisplayName, result.Name);
            // Note: Email is not stored in Domain.User anymore, so we don't verify it here

            _mockUserRepository.Verify(x => x.AddAsync(It.Is<User>(u =>
                u.DisplayName == userDto.Name)), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateUserAsync_ShouldUpdateAndReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDto = new UserDto
            {
                Email = "updated@example.com", // Email in DTO but not updated in Domain.User
                Name = "Updated User"
            };

            var existingUser = new User
            {
                Id = userId,
                DisplayName = "Original User"
            };

            var updatedUser = new User
            {
                Id = userId,
                DisplayName = userDto.Name
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(updatedUser);

            // Act
            var result = await _userService.UpdateUserAsync(userId, userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.IdentityId);
            Assert.Equal(userDto.Name, result.Name);
            // Note: Email is not updated in Domain.User

            _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.Is<User>(u =>
                u.Id == userId &&
                u.DisplayName == userDto.Name)), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateUserAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDto = new UserDto
            {
                Email = "test@example.com",
                Name = "Test User"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _userService.UpdateUserAsync(userId, userDto));

            Assert.Equal("User not found", exception.Message);
            _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserByIdAsync_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                DisplayName = "Test User"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.IdentityId);
            Assert.Equal(user.DisplayName, result.Name);
            // Note: Email is not returned since it's not stored in Domain.User

            _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserByIdAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _userService.GetUserByIdAsync(userId));

            Assert.Equal("User not found", exception.Message);
            _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task SearchUsersAsync_ShouldReturnUserList_WhenUsersFound()
        {
            // Arrange
            var searchTerm = "test";
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Test User 1"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Test User 2"
                }
            };

            _mockUserRepository.Setup(x => x.SearchByNameAsync(searchTerm))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.SearchUsersAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(users[0].Id, result[0].IdentityId);
            Assert.Equal(users[0].DisplayName, result[0].Name);
            Assert.Equal(users[1].Id, result[1].IdentityId);
            Assert.Equal(users[1].DisplayName, result[1].Name);

            _mockUserRepository.Verify(x => x.SearchByNameAsync(searchTerm), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task SearchUsersAsync_ShouldReturnEmptyList_WhenNoUsersFound()
        {
            // Arrange
            var searchTerm = "nonexistent";
            _mockUserRepository.Setup(x => x.SearchByNameAsync(searchTerm))
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.SearchUsersAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockUserRepository.Verify(x => x.SearchByNameAsync(searchTerm), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteUserAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                DisplayName = "Test User"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(x => x.DeleteAsync(userId), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        // Note: Email-related methods like GetUserByEmailAsync and InviteUserAsync
        // have been removed since email is now handled by ApplicationUser/UserIdentityService
        // If these operations are still needed, they should be tested in ApplicationUserRepositoryTests
        // or UserIdentityServiceTests instead.
    }
}

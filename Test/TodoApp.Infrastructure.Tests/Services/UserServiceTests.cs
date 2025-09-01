using Moq;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Persistence.Interfaces;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;
using TodoApp.Infrastructure.Services;
using Xunit;

namespace TodoApp.Infrastructure.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUnitOfWork.Setup(x => x.Users).Returns(_mockUserRepository.Object);
            _userService = new UserService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateUserAsync_ShouldCreateAndReturnUser_WhenValidUserDto()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "test@example.com",
                Name = "Test User",
                ProfilePicture = "profile.jpg"
            };

            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email(userDto.Email),
                DisplayName = userDto.Name
            };

            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.CreateUserAsync(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.Email.Value, result.Email);
            Assert.Equal(expectedUser.DisplayName, result.Name);
            Assert.Equal(userDto.ProfilePicture, result.ProfilePicture);

            _mockUserRepository.Verify(x => x.AddAsync(It.Is<User>(u =>
                u.Email.Value == userDto.Email &&
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
                Email = "updated@example.com",
                Name = "Updated User",
                ProfilePicture = "updated.jpg"
            };

            var existingUser = new User
            {
                Id = userId,
                Email = new Email("original@example.com"),
                DisplayName = "Original User"
            };

            var updatedUser = new User
            {
                Id = userId,
                Email = new Email(userDto.Email),
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
            Assert.Equal(userId, result.Id);
            Assert.Equal(userDto.Email, result.Email);
            Assert.Equal(userDto.Name, result.Name);
            Assert.Equal(userDto.ProfilePicture, result.ProfilePicture);

            _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.Is<User>(u =>
                u.Id == userId &&
                u.Email.Value == userDto.Email &&
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
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email.Value, result.Email);
            Assert.Equal(user.DisplayName, result.Name);
            Assert.Null(result.ProfilePicture);

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
        public async System.Threading.Tasks.Task GetUserByEmailAsync_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email(email),
                DisplayName = "Test User"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email.Value, result.Email);
            Assert.Equal(user.DisplayName, result.Name);
            Assert.Null(result.ProfilePicture);

            _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserByEmailAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _userService.GetUserByEmailAsync(email));

            Assert.Equal("User not found", exception.Message);
            _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
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
                    Email = new Email("test1@example.com"),
                    DisplayName = "Test User 1"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = new Email("test2@example.com"),
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
            Assert.Equal(users[0].Id, result[0].Id);
            Assert.Equal(users[0].Email.Value, result[0].Email);
            Assert.Equal(users[0].DisplayName, result[0].Name);
            Assert.Equal(users[1].Id, result[1].Id);
            Assert.Equal(users[1].Email.Value, result[1].Email);
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
                Email = new Email("test@example.com"),
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

        [Fact]
        public async System.Threading.Tasks.Task InviteUserAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var inviterId = Guid.NewGuid();
            var email = "existing@example.com";
            var message = "You're invited!";
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email(email),
                DisplayName = "Existing User"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _userService.InviteUserAsync(inviterId, email, message);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task InviteUserAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var inviterId = Guid.NewGuid();
            var email = "nonexistent@example.com";
            var message = "You're invited!";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.InviteUserAsync(inviterId, email, message);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        }
    }
}

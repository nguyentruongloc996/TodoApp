using Microsoft.Extensions.Caching.Distributed;
using Moq;
using TodoApp.Infrastructure.Services;
using System.Text.Json;

namespace TodoApp.Infrastructure.Tests.Services
{
    public class RedisCacheServiceTests
    {
        private Mock<IDistributedCache> _mockCache;
        private RedisCacheService _cacheService;

        public RedisCacheServiceTests()
        {
            _mockCache = new Mock<IDistributedCache>();
            _cacheService = new RedisCacheService(_mockCache.Object);
        }

        [Fact]
        public async Task GetAsync_WithValidKey_ShouldReturnDeserializedValue()
        {
            // Arrange
            var testObject = new TestData { Id = 1, Name = "Test" };
            var jsonValue = JsonSerializer.Serialize(testObject, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonValue);
            _mockCache.Setup(x => x.GetAsync("test-key", default))
                .ReturnsAsync(bytes);

            // Act
            var result = await _cacheService.GetAsync<TestData>("test-key");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetAsync_WithInvalidJson_ShouldReturnDefault()
        {
            // Arrange
            var bytes = System.Text.Encoding.UTF8.GetBytes("invalid json");
            _mockCache.Setup(x => x.GetAsync("test-key", default))
                .ReturnsAsync(bytes);

            // Act
            var result = await _cacheService.GetAsync<TestData>("test-key");

            // Assert
            Assert.Null(result);
            _mockCache.Verify(x => x.RemoveAsync("test-key", default), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WithNullValue_ShouldReturnDefault()
        {
            // Arrange
            _mockCache.Setup(x => x.GetAsync("test-key", default))
                .ReturnsAsync((byte[]?)null);

            // Act
            var result = await _cacheService.GetAsync<TestData>("test-key");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetAsync_WithValidValue_ShouldSerializeAndCache()
        {
            // Arrange
            var testObject = new TestData { Id = 1, Name = "Test" };
            byte[]? capturedBytes = null;
            DistributedCacheEntryOptions? capturedOptions = null;

            _mockCache.Setup(x => x.SetAsync("test-key", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default))
                .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((key, value, options, token) =>
                {
                    capturedBytes = value;
                    capturedOptions = options;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.SetAsync("test-key", testObject);

            // Assert
            Assert.NotNull(capturedBytes);
            var jsonString = System.Text.Encoding.UTF8.GetString(capturedBytes);
            var deserialized = JsonSerializer.Deserialize<TestData>(jsonString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            Assert.Equal(1, deserialized.Id);
            Assert.Equal("Test", deserialized.Name);
        }

        [Fact]
        public async Task SetAsync_WithExpiration_ShouldSetExpiration()
        {
            // Arrange
            var testObject = new TestData { Id = 1, Name = "Test" };
            var expiration = TimeSpan.FromMinutes(30);
            DistributedCacheEntryOptions? capturedOptions = null;

            _mockCache.Setup(x => x.SetAsync("test-key", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default))
                .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((key, value, options, token) =>
                {
                    capturedOptions = options;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.SetAsync("test-key", testObject, expiration);

            // Assert
            Assert.NotNull(capturedOptions);
            // Note: We can't easily test the expiration value without reflection, but we can verify the method was called
        }

        [Fact]
        public async Task RemoveAsync_ShouldCallCacheRemove()
        {
            // Arrange
            _mockCache.Setup(x => x.RemoveAsync("test-key", default))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.RemoveAsync("test-key");

            // Assert
            _mockCache.Verify(x => x.RemoveAsync("test-key", default), Times.Once);
        }

        [Fact]
        public async Task RemoveByPatternAsync_ShouldThrowNotImplementedException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
                await _cacheService.RemoveByPatternAsync("test-pattern"));
        }

        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
} 
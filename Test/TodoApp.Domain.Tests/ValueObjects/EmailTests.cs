using TodoApp.Domain.ValueObjects;
using Xunit;

namespace TodoApp.Domain.Tests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void CreateEmail_WithValidValue_ShouldSetValue()
        {
            // Arrange
            var value = "test@example.com";

            // Act
            var email = new Email(value);

            // Assert
            Assert.Equal(value, email.Value);
        }

        [Theory]
        [InlineData("user@domain.com")]
        [InlineData("test.email@example.org")]
        [InlineData("user123@test-domain.net")]
        [InlineData("a@b.co")]
        [InlineData("user+tag@example.com")]
        [InlineData("user_name@domain.info")]
        public void CreateEmail_WithValidFormats_ShouldSucceed(string validEmail)
        {
            // Act
            var email = new Email(validEmail);

            // Assert
            Assert.Equal(validEmail, email.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void CreateEmail_WithNullOrWhitespace_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            Assert.Equal("Email cannot be empty.", exception.Message);
        }

        [Fact]
        public void CreateEmail_WithNull_ShouldThrowArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(null!));
            Assert.Equal("Email cannot be empty.", exception.Message);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@domain.com")]
        [InlineData("user@")]
        [InlineData("user@domain")]
        [InlineData("user.domain.com")]
        [InlineData("user@domain.")]
        [InlineData("user @domain.com")]
        [InlineData("user@ domain.com")]
        [InlineData("user@domain .com")]
        [InlineData("user@@domain.com")]
        [InlineData("user@domain..com")]
        [InlineData(".user@domain.com")]
        [InlineData("user.@domain.com")]
        public void CreateEmail_WithInvalidFormat_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            Assert.Equal("Invalid email format.", exception.Message);
        }

        [Fact]
        public void Equals_WithSameEmail_ShouldReturnTrue()
        {
            // Arrange
            var email1 = new Email("test@example.com");
            var email2 = new Email("test@example.com");

            // Act
            var result = email1.Equals(email2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentCasing_ShouldReturnTrue()
        {
            // Arrange
            var email1 = new Email("test@example.com");
            var email2 = new Email("TEST@EXAMPLE.COM");

            // Act
            var result = email1.Equals(email2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithMixedCasing_ShouldReturnTrue()
        {
            // Arrange
            var email1 = new Email("Test@Example.Com");
            var email2 = new Email("test@example.com");

            // Act
            var result = email1.Equals(email2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentEmails_ShouldReturnFalse()
        {
            // Arrange
            var email1 = new Email("test1@example.com");
            var email2 = new Email("test2@example.com");

            // Act
            var result = email1.Equals(email2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithNullObject_ShouldReturnFalse()
        {
            // Arrange
            var email = new Email("test@example.com");

            // Act
            var result = email.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_WithSameEmail_ShouldReturnSameHashCode()
        {
            // Arrange
            var email1 = new Email("test@example.com");
            var email2 = new Email("test@example.com");

            // Act
            var hash1 = email1.GetHashCode();
            var hash2 = email2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithDifferentCasing_ShouldReturnSameHashCode()
        {
            // Arrange
            var email1 = new Email("test@example.com");
            var email2 = new Email("TEST@EXAMPLE.COM");

            // Act
            var hash1 = email1.GetHashCode();
            var hash2 = email2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithDifferentEmails_ShouldReturnDifferentHashCodes()
        {
            // Arrange
            var email1 = new Email("test1@example.com");
            var email2 = new Email("test2@example.com");

            // Act
            var hash1 = email1.GetHashCode();
            var hash2 = email2.GetHashCode();

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void EqualityOperator_WithSameEmails_ShouldWork()
        {
            // Arrange
            var email1 = new Email("test@example.com");
            var email2 = new Email("TEST@EXAMPLE.COM");

            // Act & Assert
            Assert.True(email1.Equals(email2));
            Assert.True(email2.Equals(email1));
        }

        [Fact]
        public void Email_ShouldBeImmutable()
        {
            // Arrange
            var originalValue = "test@example.com";
            var email = new Email(originalValue);

            // Act
            var retrievedValue = email.Value;

            // Assert
            Assert.Equal(originalValue, retrievedValue);
            // Value property should be read-only (get-only)
            Assert.True(typeof(Email).GetProperty(nameof(Email.Value))?.CanWrite == false);
        }

        [Theory]
        [InlineData("a@b.c")]
        [InlineData("very.long.email.address.with.many.dots@very.long.domain.name.with.many.subdomains.example.com")]
        public void CreateEmail_WithEdgeCaseValidEmails_ShouldSucceed(string edgeCaseEmail)
        {
            // Act
            var email = new Email(edgeCaseEmail);

            // Assert
            Assert.Equal(edgeCaseEmail, email.Value);
        }
    }
}

using FluentAssertions;
using UnitTestCourse;

namespace NetworkUtility.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void NetworkService_SendPing_ReturnString()
        {
            NetworkService service = new NetworkService();

            var result = service.SendPing();

            result.Should().Be("Ping!");
        }

        [Theory]
        [InlineData(1, 2, 3)]

        public void MathOperation(int a, int b, int c)
        {
            var result = a + b;

            result.Should().Be(c);  
        }
    }
}
using FluentAssertions;
using WebStudents.src.Common;

namespace WebStudents.UnitTests.Support;

public static class Assertions
{
    public static void ShouldBeApiException(this Exception exception, int statusCode)
    {
        exception.Should().BeOfType<ApiException>();
        ((ApiException)exception).StatusCode.Should().Be(statusCode);
    }
}

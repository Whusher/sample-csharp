using Xunit;
public sealed class HealthTests
{
    [Fact]
    public void Application_has_a_valid_version() => Assert.NotNull(typeof(Program).Assembly.GetName().Version);
}

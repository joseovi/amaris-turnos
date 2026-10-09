using Xunit;
using Xunit.Abstractions;

namespace AmarisTurnos.Application.Tests.Tests
{
    public class GeneradorHashTemporal
    {
        private readonly ITestOutputHelper _output;

        public GeneradorHashTemporal(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GenerarHash()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("012345");
            _output.WriteLine(hash);
        }
    }
}

using FluentAssertions;
using Xunit;

namespace Infrastructure.Tests
{
    public class DemoElevatorTests
    {
        [Theory]
        [DapperAutoData]
        public async void MoveUp_ReasonableDelay(DemoElevator elevator)
        {
            // Act
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await elevator.MoveUpAsync().ConfigureAwait(false);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            // Assert
            elapsedMs.Should().BeGreaterOrEqualTo(3000);
        }

        [Theory]
        [DapperAutoData]
        public async void MoveDown_ReasonableDelay(DemoElevator elevator)
        {
            // Act
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await elevator.MoveDownAsync().ConfigureAwait(false);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            // Assert
            elapsedMs.Should().BeGreaterOrEqualTo(3000);
        }
    }
}

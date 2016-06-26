using System;
using System.Threading;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApplicationServices.Tests
{
    public class PersonActionsTests
    {
        [Theory]
        [DapperAutoData(1)]
        [DapperAutoData(2)]
        [DapperAutoData(3)]
        [DapperAutoData(4)]
        [DapperAutoData(5)]
        public void CheckElevatorPositionAsync_AnyFloor_ReturnsCorrectFloor(
            int floor,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(floor);

            // Act
            var reportedFloor = personActions.CheckElevatorPositionAsync();

            // Assert
            reportedFloor.Should().Be(floor.ToString());

        }

        [Theory]
        [DapperAutoData(1)]
        [DapperAutoData(2)]
        [DapperAutoData(3)]
        [DapperAutoData(4)]
        [DapperAutoData(5)]
        public void CheckSurroundings_OnAnyFloor_ReportsOutsideOfElevatorAndCorrectFloor(
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange

            // Act
            var whereAmI = personActions.CheckSurroundings();

            // Assert
            whereAmI.Should().Be($"Floor {panel.Object.Floor}");
        }


        [Theory]
        [DapperAutoData()]
        public async void EnterDoorWhenItOpens_OutsideOfElevatorAndDoorOpensOnOurFloor_GoInElevator(
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange
            panel.Setup(x => x.IsDoorOpen).Returns(true);

            // Act
            await personActions.EnterDoorWhenItOpensAsync().ConfigureAwait(false);

            // Assert
            personActions.CheckSurroundings().Should().Be("In elevator"); // TODO: Shouldn't be magic string
        }

        [Theory]
        [DapperAutoData()]
        public async void EnterDoorWhenItOpens_InsideElevator_GoOutsideOfElevator(
            Mock<ICallPanel> newFloorPanel, 
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange
            newFloorPanel.Setup(x => x.IsDoorOpen).Returns(true);
            panel.Setup(x => x.IsDoorOpen).Returns(true);
            await personActions.EnterDoorWhenItOpensAsync().ConfigureAwait(false);

            // Act
            await personActions.EnterDoorWhenItOpensAsync().ConfigureAwait(false);

            // Assert
            personActions.CheckSurroundings().Should().Be($"Floor {newFloorPanel.Object.Floor}");
        }

        [Theory]
        [DapperAutoData()]
        public void EnterDoorWhenItOpens_DoorNeverOpens_ThrowsException(
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange
            panel.Setup(x => x.IsDoorOpen).Returns(false);

            // Act
            Func<Task> act = async () => await personActions.EnterDoorWhenItOpensAsync().ConfigureAwait(false);
            // Assert
            act.ShouldThrow<OperationCanceledException>();
        }

    }
}

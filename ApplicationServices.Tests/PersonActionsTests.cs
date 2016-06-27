using System;
using System.Threading;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using FluentAssertions.Execution;
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
            var reportedFloor = personActions.CheckElevatorPosition();

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
            [Frozen] int floor,
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

        [Theory, DapperAutoData()]
        public void EnterDoor_OutsideOfElevatorDoorOpen_Succeed(
            [Frozen]Mock<ICallPanel> panel,
            PersonActions actions)
        {
            // Arrange
            panel.Setup(x => x.IsDoorOpen).Returns(true);

            // Act
            actions.EnterDoor();
            
            // Assert
            actions.CheckSurroundings().Should().Be("In elevator");
        }

        [Theory, DapperAutoData()]
        public void EnterDoor_OutsideOfElevatorDoorClosed_DoesNotSucceed(
            [Frozen]Mock<ICallPanel> panel,
            PersonActions actions)
        {
            // Arrange
            panel.Setup(x => x.IsDoorOpen).Returns(false);
            panel.Setup(x => x.Floor).Returns(1);

            // Act
            actions.EnterDoor();

            // Assert
            actions.CheckSurroundings().Should().Be("Floor 1");
        }

        [Theory, DapperAutoData()]
        public void EnterDoor_InElevatorDoorOpen_Succeed(
            [Frozen]Mock<ICallPanel> panel,
            PersonActions actions)
        {
            // Arrange
            panel.Setup(x => x.IsDoorOpen).Returns(true);
            panel.Setup(x => x.Floor).Returns(1);
            actions.EnterDoor();

            // Act
            actions.EnterDoor();

            // Assert
            actions.CheckSurroundings().Should().Be("Floor 1");
        }

        [Theory, DapperAutoData()]
        public void EnterDoor_InElevatorDoorClosed_DoesNotSucceed(
            [Frozen]Mock<ICallPanel> panel,
            PersonActions actions)
        {
            // Arrange
            panel.SetupSequence(x => x.IsDoorOpen)
                .Returns(true)
                .Returns(false);
            actions.EnterDoor();

            // Act
            actions.EnterDoor();

            // Assert
            actions.CheckSurroundings().Should().Be("In elevator");
        }

    }
}

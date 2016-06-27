using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApplicationServices.Tests
{
    public class ElevatorExteriorActionsTests
    {
        private static void GetInElevator(Mock<ICallPanel> originalCallPanel, ElevatorExteriorActions elevator)
        {
            originalCallPanel.Setup(x => x.IsDoorOpen).Returns(true);
            elevator.EnterDoor();
        }

        #region PushGoingUpButtonAsync

        [Theory]
        [DapperAutoData(1)]
        [DapperAutoData(2)]
        [DapperAutoData(3)]
        [DapperAutoData(4)]
        public async void PushGoingUpButtoAsync_AnyFloor_UpCallRequestAdded(
            int floor,
            [Frozen] Mock<IPersonActions> personActions,
            [Frozen] Mock<IElevatorService> elevatorService,
            [Frozen] Mock<ICallPanel> callPanel,
            ElevatorExteriorActions actions)
        {
            // Arrange
            personActions.Setup(x => x.CheckSurroundings()).Returns("Floor 1");
            callPanel.Setup(x => x.Floor).Returns(floor);

            // Act
            await actions.PushGoingUpButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(floor), Times.Once);
        }

        [Theory, DapperAutoData()]
        public void PushGoingUpButtonAsync_TopFloor_ThrowsException(
            int totalFloors,
            [Frozen] Mock<IElevatorService> elevatorService,
            [Frozen] Mock<ICallPanel> panel,
            ElevatorExteriorActions elevator)
        {
            // arrange
            panel.Setup(x => x.Floor).Returns(totalFloors);
            elevatorService.Setup(x => x.TotalFloors).Returns(totalFloors);
            
            // act
            Func<Task> act = async () => await elevator.PushGoingUpButtonAsync().ConfigureAwait(false);

            // assert
            act.ShouldThrow<Exception>();

        }

        [Theory, DapperAutoData()]
        public void PushGoingUpButton_NotInElevator_ThrowsException(
            [Frozen] Mock<ICallPanel> originalCallPanel,
            ElevatorExteriorActions elevator)
        {
            // Arrange
            GetInElevator(originalCallPanel, elevator);

            // act
            Func<Task> act = async () => await elevator.PushGoingDownButtonAsync().ConfigureAwait(false);
            
            // assert
            act.ShouldThrow<Exception>();
        }

        #endregion

        #region PushGoingDownButton

        [Theory]
        [DapperAutoData(2)]
        [DapperAutoData(3)]
        [DapperAutoData(4)]
        [DapperAutoData(5)]
        public async void PushGoingDownButtonAsync_AnyFloor_DownCallRequestAdded(
            int floor,
            [Frozen] Mock<IPersonActions> personActions,
            [Frozen] Mock<IElevatorService> elevatorService,
            [Frozen] Mock<ICallPanel> callPanel,
            ElevatorExteriorActions actions)
        {
            // Arrange
            personActions.Setup(x => x.CheckSurroundings()).Returns("Floor 1");
            callPanel.Setup(x => x.Floor).Returns(floor);

            // Act
            await actions.PushGoingDownButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(floor), Times.Once);
        }

        [Theory, DapperAutoData()]
        public void PushGoingDownButtonAsync_TopFloor_ThrowsException(
            int totalFloors,
            [Frozen] Mock<IElevatorService> elevatorService,
            [Frozen] Mock<ICallPanel> panel,
            ElevatorExteriorActions elevator)
        {
            // arrange
            panel.Setup(x => x.Floor).Returns(1);
            elevatorService.Setup(x => x.TotalFloors).Returns(1);

            // act
            Func<Task> act = async () => await elevator.PushGoingDownButtonAsync().ConfigureAwait(false);

            // assert
            act.ShouldThrow<Exception>();

        }

        [Theory, DapperAutoData()]
        public void PushGoingDownButton_NotInElevator_ThrowsException(
            [Frozen] Mock<ICallPanel> originalCallPanel,
            ElevatorExteriorActions elevator)
        {
            // Arrange
            GetInElevator(originalCallPanel, elevator);

            // act
            Func<Task> act = async () => await elevator.PushGoingDownButtonAsync().ConfigureAwait(false);

            // assert
            act.ShouldThrow<Exception>();
        }

        #endregion

    }
}

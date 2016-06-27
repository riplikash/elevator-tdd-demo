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
        private static async Task GetInElevator(Mock<ICallPanel> originalCallPanel, ElevatorExteriorActions elevator)
        {
            originalCallPanel.Setup(x => x.IsDoorOpen).Returns(true);
            await elevator.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);
        }

        #region PushGoingUpButtonAsync

        [Theory]
        [DapperAutoData(1)]
        [DapperAutoData(2)]
        [DapperAutoData(3)]
        [DapperAutoData(4)]
        public async void PushGoingUpButtoAsync_AnyFloor_UpCallRequestAdded(
            int floor,
            [Frozen] Mock<IElevatorService> elevatorService,
            [Frozen] Mock<ICallPanel> callPanel,
            ElevatorExteriorActions actions)
        {
            // Arrange
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
        public async void PushGoingUpButton_NotInElevator_ThrowsException(
            [Frozen] Mock<ICallPanel> originalCallPanel,
            ElevatorExteriorActions elevator)
        {
            // Arrange
            await GetInElevator(originalCallPanel, elevator).ConfigureAwait(false);

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
            [Frozen] Mock<IElevatorService> elevatorService,
            [Frozen] Mock<ICallPanel> callPanel,
            ElevatorExteriorActions actions)
        {
            // Arrange
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
        public async void PushGoingDownButton_NotInElevator_ThrowsException(
            [Frozen] Mock<ICallPanel> originalCallPanel,
            ElevatorExteriorActions elevator)
        {
            // Arrange
            await GetInElevator(originalCallPanel, elevator).ConfigureAwait(false);

            // act
            Func<Task> act = async () => await elevator.PushGoingDownButtonAsync().ConfigureAwait(false);

            // assert
            act.ShouldThrow<Exception>();
        }

        #endregion

    }
}

using System.Threading;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApplicationServices.Tests
{
    public class ElevatorInteriorActionsTests
    {
        [Theory, DapperAutoData()]
        public void CheckCurrentFloorAsync_InElevator_ReportsElevatorFloor(
            int currentFloor,
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorActions elevator)
        {
            // arrange
            panel.Setup(x => x.IsDoorOpen).Returns(true);
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevator.EnterDoor();

            // act
            var reportedFloor = elevator.CheckCurrentFloorAsync();

            // assert
            reportedFloor.Should().Be(currentFloor.ToString());
        }

        [Theory]
        [DapperAutoData(1)]
        [DapperAutoData(2)]
        [DapperAutoData(3)]
        [DapperAutoData(4)]
        [DapperAutoData(5)]
        public async void PushButton1_CallFromAnyFloor_NewFloorCallPanelIsSet(
            int callingFloor,
            Mock<ICallPanel> newPanel,
            [Frozen] Mock<IPersonActions> personActions,
            [Frozen] Mock<ICallPanel> originalCallPanel,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorActions elevator)
        {
            // arrange
            personActions.Setup(x => x.CheckSurroundings()).Returns("In Elevator");
            elevatorService.Setup(x => x.GetCallPanelForFloor(It.IsAny<int>())).Returns(newPanel.Object);
            newPanel.Setup(x => x.IsDoorOpen).Returns(true);

            // act
            await elevator.PushButtonNumberAsync(1).ConfigureAwait(false);
            elevator.EnterDoor();

            // assert
            elevator.CallPanel.Should().Be(newPanel.Object);
        }

        private static void GetInElevator(Mock<ICallPanel> originalCallPanel, ElevatorInteriorActions elevator)
        {
            originalCallPanel.Setup(x => x.IsDoorOpen).Returns(true);
            elevator.EnterDoor();
        }

        [Theory]
        [DapperAutoData(5, 1)]
        [DapperAutoData(5, 4)]
        [DapperAutoData(4, 3)]
        [DapperAutoData(4, 2)]
        [DapperAutoData(3, 2)]
        [DapperAutoData(2, 1)]
        public async void PushButton_FloorAbove_AddDownCallRequest(
            int currentFloor,
            int desiredFloor,
            [Frozen] Mock<ICallPanel> originalCallPanel,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorActions elevator)
        {
            // arrange
            GetInElevator(originalCallPanel, elevator);
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);

            // act
            switch (desiredFloor)
            {
                case 1:
                    await elevator.PushButtonNumberAsync(1).ConfigureAwait(false);
                    break;
                case 2:
                    await elevator.PushButtonNumberAsync(2).ConfigureAwait(false);
                    break;
                case 3:
                    await elevator.PushButtonNumberAsync(3).ConfigureAwait(false);
                    break;
                case 4:
                    await elevator.PushButtonNumberAsync(4).ConfigureAwait(false);
                    break;
            }

            // assert
            elevatorService.Verify(x => x.DownCallRequestAsync(desiredFloor), Times.Once);
        }

        [Theory]
        [DapperAutoData(1, 1)]
        [DapperAutoData(1, 2)]
        [DapperAutoData(1, 3)]
        [DapperAutoData(1, 4)]
        [DapperAutoData(1, 5)]
        [DapperAutoData(2, 2)]
        [DapperAutoData(2, 5)]
        [DapperAutoData(3, 5)]
        [DapperAutoData(4, 5)]
        [DapperAutoData(5, 5)]
        public async void PushButton_BelowOrEqual_AddUpCalleRequest(int currentFloor,
            int desiredFloor,
            [Frozen] Mock<ICallPanel> originalCallPanel,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorActions elevator)
        {
            // arrange
            GetInElevator(originalCallPanel, elevator);
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);

            // act
            switch (desiredFloor)
            {
                case 1:
                    await elevator.PushButtonNumberAsync(1).ConfigureAwait(false);
                    break;
                case 2:
                    await elevator.PushButtonNumberAsync(2).ConfigureAwait(false);
                    break;
                case 3:
                    await elevator.PushButtonNumberAsync(3).ConfigureAwait(false);
                    break;
                case 4:
                    await elevator.PushButtonNumberAsync(4).ConfigureAwait(false);
                    break;
                case 5:
                    await elevator.PushButtonNumberAsync(5).ConfigureAwait(false);
                    break;
            }

            // assert
            elevatorService.Verify(x => x.UpCallRequestAsync(desiredFloor), Times.Once);
        }
    }
}

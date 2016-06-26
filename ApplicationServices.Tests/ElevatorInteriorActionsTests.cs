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
        public async void CheckCurrentFloorAsync_InElevator_ReportsElevatorFloor(
            int currentFloor,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorActions elevator)
        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            await elevator.EnterDoorWhenItOpensAsync().ConfigureAwait(false);

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
            [Frozen] Mock<ICallPanel> originalCallPanel,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorActions elevator)
        {
            // arrange
            elevatorService.Setup(x => x.GetExternalCallInterfaceForFloor(It.IsAny<int>())).Returns(newPanel.Object);
            newPanel.Setup(x => x.IsDoorOpen).Returns(true);
            await GetInElevator(originalCallPanel, elevator).ConfigureAwait(false);

            // act
            await elevator.PushButton1().ConfigureAwait(false);
            await elevator.EnterDoorWhenItOpensAsync().ConfigureAwait(false);

            // assert
            newPanel.Verify(x => x.IsDoorOpen, Times.Once);
        }

        private static async Task GetInElevator(Mock<ICallPanel> originalCallPanel, ElevatorInteriorActions elevator)
        {
            originalCallPanel.Setup(x => x.IsDoorOpen).Returns(true);
            await elevator.EnterDoorWhenItOpensAsync().ConfigureAwait(false);
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
            await GetInElevator(originalCallPanel, elevator).ConfigureAwait(false);
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);

            // act
            switch (desiredFloor)
            {
                case 1:
                    await elevator.PushButton1().ConfigureAwait(false);
                    break;
                case 2:
                    await elevator.PushButton2().ConfigureAwait(false);
                    break;
                case 3:
                    await elevator.PushButton3().ConfigureAwait(false);
                    break;
                case 4:
                    await elevator.PushButton4().ConfigureAwait(false);
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
            await GetInElevator(originalCallPanel, elevator).ConfigureAwait(false);
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);

            // act
            switch (desiredFloor)
            {
                case 1:
                    await elevator.PushButton1().ConfigureAwait(false);
                    break;
                case 2:
                    await elevator.PushButton2().ConfigureAwait(false);
                    break;
                case 3:
                    await elevator.PushButton3().ConfigureAwait(false);
                    break;
                case 4:
                    await elevator.PushButton4().ConfigureAwait(false);
                    break;
                case 5:
                    await elevator.PushButton5().ConfigureAwait(false);
                    break;
            }

            // assert
            elevatorService.Verify(x => x.UpCallRequestAsync(desiredFloor), Times.Once);
        }
    }
}

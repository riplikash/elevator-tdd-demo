using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace DomainTests
{
    public class CallPanelTests
    {
        [Theory]
        [DapperAutoData]
        public async void UpCallButtonPress_ButtonIsPressed_CurrentFloorAddedToUpcallQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            CallPanel callPanel)
        {
            // Arrange

            // Act
            await callPanel.PushUpCallAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(callPanel.Floor), Times.Once);
        }

        [Theory]
        [DapperAutoData]
        public async void DownCallButtonPress_ButtonIsPressed_CurrentFloorAddedToDowncallQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            CallPanel callPanel)
        {
            // Act
            await callPanel.PushDownCallAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(callPanel.Floor), Times.Once);
        }

        [Theory]
        [DapperAutoData]
        public async void FloorChangeEventHandler_EventHandlerEvoked_FloorDisplayUpdates(
            [Frozen] int floor,
            CallPanel callPanel)
        {
            // Arrange

            // Act
            await callPanel.FloorChangeEventHandlerAsync(floor).ConfigureAwait(false);

            // Assert
            callPanel.ElevatorFloorDisplay.Should().Be(floor.ToString());
        }

        [Theory]
        [DapperAutoData]
        public async void DoorOpenEventHandler_EventHandlerEvoked_DoorIsOpened(
            CallPanel callPanel)
        {
            // Arrange
            await callPanel.DoorCloseEventHandlerAsync().ConfigureAwait(false);

            // Act
            await callPanel.DoorOpenEventHandlerAsync().ConfigureAwait(false);

            // Assert
            callPanel.IsDoorOpen.Should().BeTrue();
        }

        [Theory]
        [DapperAutoData]
        public async void DoorClosedEventHandler_EventHandlerEvoked_DoorIsClosed(
            CallPanel callPanel)
        {
            // Arrange
            await callPanel.DoorOpenEventHandlerAsync().ConfigureAwait(false);

            // Act
            await callPanel.DoorCloseEventHandlerAsync().ConfigureAwait(false);

            // Assert
            callPanel.IsDoorOpen.Should().BeFalse();
        }

        [Theory]
        [DapperAutoData]
        public void IsDoorClosed_InitialConstruction_DoorIsClosed([Frozen] IElevatorService elevatorService,
            int totalFloors, int floor)
        {
            // Arrange
            var callInterface = new CallPanel(elevatorService, floor, totalFloors);

            // Act

            // Assert
            callInterface.IsDoorOpen.Should().BeFalse();
        }

        // TODO: Down button disabled on first floor
        // TODO: Up button disabled on top flor
    }
}

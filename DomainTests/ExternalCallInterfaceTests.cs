using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace DomainTests
{
    public class ExternalCallInterfaceTests
    {
        [Theory]
        [AutoMoqData]
        public async void UpCallButtonPress_ButtonIsPressed_CurrentFloorAddedToUpcallQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ExternalCallInterface callInterface)
        {
            // Arrange

            // Act
            await callInterface.PushUpCallAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(callInterface.Floor), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async void DownCallButtonPress_ButtonIsPressed_CurrentFloorAddedToDowncallQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ExternalCallInterface callInterface)
        {
            // Act
            await callInterface.PushDownCallAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(callInterface.Floor), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async void FloorChangeEventHandler_EventHandlerEvoked_FloorDisplayUpdates(
            [Frozen] int floor,
            ExternalCallInterface callInterface)
        {
            // Arrange

            // Act
            await callInterface.FloorChangeEventHandlerAsync(floor).ConfigureAwait(false);

            // Assert
            callInterface.ElevatorFloorDisplay.Should().Be(floor.ToString());
        }

        [Theory]
        [AutoMoqData]
        public async void DoorOpenEventHandler_EventHandlerEvoked_DoorIsOpened(
            ExternalCallInterface callInterface)
        {
            // Arrange
            await callInterface.DoorCloseEventHandlerAsync().ConfigureAwait(false);

            // Act
            await callInterface.DoorOpenEventHandlerAsync().ConfigureAwait(false);

            // Assert
            callInterface.IsDoorOpen.Should().BeTrue();
        }

        [Theory]
        [AutoMoqData]
        public async void DoorClosedEventHandler_EventHandlerEvoked_DoorIsClosed(
            ExternalCallInterface callInterface)
        {
            // Arrange
            await callInterface.DoorOpenEventHandlerAsync().ConfigureAwait(false);

            // Act
            await callInterface.DoorCloseEventHandlerAsync().ConfigureAwait(false);

            // Assert
            callInterface.IsDoorOpen.Should().BeFalse();
        }

        [Theory]
        [AutoMoqData]
        public void IsDoorClosed_InitialConstruction_DoorIsClosed([Frozen] IElevatorService elevatorService,
            int totalFloors, int floor)
        {
            // Arrange
            var callInterface = new ExternalCallInterface(elevatorService, floor, totalFloors);

            // Act

            // Assert
            callInterface.IsDoorOpen.Should().BeFalse();
        }

        // TODO: Down button disabled on first floor
        // TODO: Up button disabled on top flor
    }
}

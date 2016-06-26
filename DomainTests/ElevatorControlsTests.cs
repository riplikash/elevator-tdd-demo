using System;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace DomainTests
{
    public class ElevatorControlsTests
    {


        #region PushButton1Async
        // TODO: really should use inline data and test both floors 1 & 2
    
        [Theory, DapperAutoData]
        public async void PushButton1Async_CalledFromFloor5_Floor3AddedToDownQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls controls)
        {
            // Arrange
            await controls.FloorUpdateEventHandlerAsync(5).ConfigureAwait(false);

            // Act
            await controls.PushFloorButtonAsync(1).ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(1), Times.Once);
            elevatorService.Verify(x => x.UpCallRequestAsync(1), Times.Never);

        }

        [Theory, DapperAutoData]
        public async void PushButton1Async_CalledFromFloor1_NothingHappens(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls controls)
        {
            // Arrange
            await controls.FloorUpdateEventHandlerAsync(1).ConfigureAwait(false);

            // Act
            await controls.PushFloorButtonAsync(1).ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(3), Times.Never);
            elevatorService.Verify(x => x.DownCallRequestAsync(1), Times.Never);

        }

        #endregion
        #region PushButton2Async
        // TODO: really should use inline data and test both floors 1 & 2
        [Theory, DapperAutoData]

        public async void PushButton2Async_CalledFromFloor1_Floor3AddedToUpQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls controls)
        {
            // Arrange
            await controls.FloorUpdateEventHandlerAsync(1).ConfigureAwait(false);

            // Act
            await controls.PushFloorButtonAsync(2).ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(2), Times.Once);
            elevatorService.Verify(x => x.DownCallRequestAsync(2), Times.Never);

        }

        [Theory, DapperAutoData]
        public async void PushButton2Async_CalledFromFloor5_Floor3AddedToDownQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls controls)
        {
            // Arrange
            await controls.FloorUpdateEventHandlerAsync(5).ConfigureAwait(false);

            // Act
            await controls.PushFloorButtonAsync(2).ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(2), Times.Once);
            elevatorService.Verify(x => x.UpCallRequestAsync(2), Times.Never);

        }

        [Theory, DapperAutoData]
        public async void PushButton2Async_CalledFromFloor3_NothingHappens(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls controls)
        {
            // Arrange
            await controls.FloorUpdateEventHandlerAsync(2).ConfigureAwait(false);

            // Act
            await controls.PushFloorButtonAsync(2).ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(2), Times.Never);
            elevatorService.Verify(x => x.DownCallRequestAsync(2), Times.Never);

        }

        #endregion

        #region PushButton3Async
        // TODO: really should use inline data and test both floors 1 & 2
        [Theory, DapperAutoData]

        public async void PushButton3Async_CalledFromFloor1_Floor3AddedToUpQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls controls)
        {
            // Arrange
            await controls.FloorUpdateEventHandlerAsync(1).ConfigureAwait(false);

            // Act
            await controls.PushFloorButtonAsync(3).ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(3), Times.Once);
            elevatorService.Verify(x => x.DownCallRequestAsync(3), Times.Never);

        }

        [Theory, DapperAutoData]
        public async void PushButton3Async_CalledFromFloor5_Floor3AddedToDownQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls controls)
        {
            // Arrange
            await controls.FloorUpdateEventHandlerAsync(5).ConfigureAwait(false);

            // Act
            await controls.PushFloorButtonAsync(3).ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(3), Times.Once);
            elevatorService.Verify(x => x.UpCallRequestAsync(3), Times.Never);

        }

        [Theory, DapperAutoData]
        public async void PushButton3Async_CalledFromFloor3_NothingHappens(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls controls)
        {
            // Arrange
            await controls.FloorUpdateEventHandlerAsync(3).ConfigureAwait(false);

            // Act
            await controls.PushFloorButtonAsync(3).ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(3), Times.Never);
            elevatorService.Verify(x => x.DownCallRequestAsync(3), Times.Never);

        }

        #endregion


        // TODO: repeat for other four buttons

        [Theory, DapperAutoData]
        public async void FloorUpdateEventHandlerAsync_MethodCalled_CurrentFloorUpdates(
            ElevatorControls controls)
        {
            // Arrange
            int floor = new Random().Next(1, 5);

            // Act
            await controls.FloorUpdateEventHandlerAsync(floor).ConfigureAwait(false);

            // Assert
            controls.FloorDisplay.Should().Be(floor.ToString());

        }

        // TODO: Get inlineAutoData tests running
        [Theory]
        [DapperAutoData(0)]
        [DapperAutoData(int.MaxValue)]
        public void FloorUpdateEventHandlerAsync_OutOfRange_ThrowsoutOfrangeException(
            int newFloor,
            ElevatorControls controls)
        {
            // Arrange

            // Act
            Func<Task> act = async () => await controls.FloorUpdateEventHandlerAsync(newFloor).ConfigureAwait(false);

            // Assert
            act.ShouldThrow<Exception>();

        }
    }
}

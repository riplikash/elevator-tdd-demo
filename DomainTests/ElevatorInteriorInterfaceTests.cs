using System;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace DomainTests
{
    public class ElevatorInteriorInterfaceTests
    {


        #region PushButton1Async
        // TODO: really should use inline data and test both floors 1 & 2
    
        [Theory, AutoMoqData]
        public async void PushButton1Async_CalledFromFloor5_Floor3AddedToDownQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            await interiorInterface.FloorUpdateEventHandlerAsync(5).ConfigureAwait(false);

            // Act
            await interiorInterface.PushFloor1ButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(1), Times.Once);
            elevatorService.Verify(x => x.UpCallRequestAsync(1), Times.Never);

        }

        [Theory, AutoMoqData]
        public async void PushButton1Async_CalledFromFloor1_NothingHappens(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            await interiorInterface.FloorUpdateEventHandlerAsync(1).ConfigureAwait(false);

            // Act
            await interiorInterface.PushFloor1ButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(3), Times.Never);
            elevatorService.Verify(x => x.DownCallRequestAsync(1), Times.Never);

        }

        #endregion
        #region PushButton2Async
        // TODO: really should use inline data and test both floors 1 & 2
        [Theory, AutoMoqData]

        public async void PushButton2Async_CalledFromFloor1_Floor3AddedToUpQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            await interiorInterface.FloorUpdateEventHandlerAsync(1).ConfigureAwait(false);

            // Act
            await interiorInterface.PushFloor2ButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(2), Times.Once);
            elevatorService.Verify(x => x.DownCallRequestAsync(2), Times.Never);

        }

        [Theory, AutoMoqData]
        public async void PushButton2Async_CalledFromFloor5_Floor3AddedToDownQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            await interiorInterface.FloorUpdateEventHandlerAsync(5).ConfigureAwait(false);

            // Act
            await interiorInterface.PushFloor2ButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(2), Times.Once);
            elevatorService.Verify(x => x.UpCallRequestAsync(2), Times.Never);

        }

        [Theory, AutoMoqData]
        public async void PushButton2Async_CalledFromFloor3_NothingHappens(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            await interiorInterface.FloorUpdateEventHandlerAsync(2).ConfigureAwait(false);

            // Act
            await interiorInterface.PushFloor2ButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(2), Times.Never);
            elevatorService.Verify(x => x.DownCallRequestAsync(2), Times.Never);

        }

        #endregion

        #region PushButton3Async
        // TODO: really should use inline data and test both floors 1 & 2
        [Theory, AutoMoqData]

        public async void PushButton3Async_CalledFromFloor1_Floor3AddedToUpQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            await interiorInterface.FloorUpdateEventHandlerAsync(1).ConfigureAwait(false);

            // Act
            await interiorInterface.PushFloor3ButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(3), Times.Once);
            elevatorService.Verify(x => x.DownCallRequestAsync(3), Times.Never);

        }

        [Theory, AutoMoqData]
        public async void PushButton3Async_CalledFromFloor5_Floor3AddedToDownQueue(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            await interiorInterface.FloorUpdateEventHandlerAsync(5).ConfigureAwait(false);

            // Act
            await interiorInterface.PushFloor3ButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.DownCallRequestAsync(3), Times.Once);
            elevatorService.Verify(x => x.UpCallRequestAsync(3), Times.Never);

        }

        [Theory, AutoMoqData]
        public async void PushButton3Async_CalledFromFloor3_NothingHappens(
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            await interiorInterface.FloorUpdateEventHandlerAsync(3).ConfigureAwait(false);

            // Act
            await interiorInterface.PushFloor3ButtonAsync().ConfigureAwait(false);

            // Assert
            elevatorService.Verify(x => x.UpCallRequestAsync(3), Times.Never);
            elevatorService.Verify(x => x.DownCallRequestAsync(3), Times.Never);

        }

        #endregion


        // TODO: repeat for other four buttons

        [Theory, AutoMoqData]
        public async void FloorUpdateEventHandlerAsync_MethodCalled_CurrentFloorUpdates(
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange
            int floor = new Random().Next(1, 5);

            // Act
            await interiorInterface.FloorUpdateEventHandlerAsync(floor).ConfigureAwait(false);

            // Assert
            interiorInterface.FloorDisplay.Should().Be(floor.ToString());

        }

        // TODO: Get inlineAutoData tests running
        [Theory]
        [InlineAutoData(0)]
        [InlineAutoData(6)]
        public void FloorUpdateEventHandlerAsync_OutOfRange_ThrowsoutOfrangeException(
            int newFloor,
            ElevatorInteriorInterface interiorInterface)
        {
            // Arrange

            // Act
            Action act = async () => await interiorInterface.FloorUpdateEventHandlerAsync(newFloor).ConfigureAwait(false);

            // Assert
            act.ShouldThrow<ArgumentOutOfRangeException>();

        }
    }
}

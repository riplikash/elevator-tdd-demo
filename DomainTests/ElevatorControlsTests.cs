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
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls elevator)
        {
            // arrange
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
        public async void PushButton_FloorBelowOrEqual_AddUpCallRequest(
            int currentFloor,
            int desiredFloor,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorControls elevator)
        {
            // arrange
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using Moq;
using Xunit;

namespace ApplicationServices.Tests
{
    public class ElevatorExteriorActionsTests
    {

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

    }
}

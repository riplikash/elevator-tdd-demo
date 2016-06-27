using System;
using System.Threading;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Xunit;

namespace ApplicationServices.Tests
{
    public class PersonActionsTests
    {
        [Theory]
        [DapperAutoData(1)]
        [DapperAutoData(2)]
        [DapperAutoData(3)]
        [DapperAutoData(4)]
        [DapperAutoData(5)]
        public void CheckElevatorPositionAsync_AnyFloor_ReturnsCorrectFloor(
            int floor,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(floor);

            // Act
            var reportedFloor = personActions.CheckElevatorPositionAsync();

            // Assert
            reportedFloor.Should().Be(floor.ToString());

        }

        [Theory]
        [DapperAutoData(1)]
        [DapperAutoData(2)]
        [DapperAutoData(3)]
        [DapperAutoData(4)]
        [DapperAutoData(5)]
        public void CheckSurroundings_OnAnyFloor_ReportsOutsideOfElevatorAndCorrectFloor(
            [Frozen] int floor,
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange


            // Act
            var whereAmI = personActions.CheckSurroundings();

            // Assert
            whereAmI.Should().Be($"Floor {panel.Object.Floor}");
        }


        [Theory]
        [DapperAutoData()]
        public async void EnterDoorWhenItOpens_OutsideOfElevatorAndDoorOpensOnOurFloor_GoInElevator(
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange
            panel.Setup(x => x.IsDoorOpen).Returns(true);

            // Act
            await personActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);

            // Assert
            personActions.CheckSurroundings().Should().Be("In elevator"); // TODO: Shouldn't be magic string
        }

        [Theory]
        [DapperAutoData()]
        public async void EnterDoorWhenItOpens_InsideElevator_GoOutsideOfElevator(
            Mock<ICallPanel> newFloorPanel, 
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange
            newFloorPanel.Setup(x => x.IsDoorOpen).Returns(true);
            panel.Setup(x => x.IsDoorOpen).Returns(true);
            await personActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);

            // Act
            await personActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);

            // Assert
            personActions.CheckSurroundings().Should().Be($"Floor {newFloorPanel.Object.Floor}");
        }

        [Theory]
        [DapperAutoData()]
        public async void EnterDoorWhenItOpens_Cancel_ThrowsException(
            [Frozen] Mock<ICallPanel> panel,
            [Frozen] Mock<IElevatorService> elevatorService,
            PersonActions personActions)
        {
            // Arrange
            CancellationTokenSource cts = new CancellationTokenSource();
            panel.Setup(x => x.IsDoorOpen).Returns(Cancel(cts));
            elevatorService.Setup(x => x.GetCallPanelForFloor(It.IsAny<int>())).Returns(panel.Object);
            await personActions.EnterDoorWhenItOpensAsync(cts.Token).ConfigureAwait(false);
 
            // Act
            try
            {
                await personActions.EnterDoorWhenItOpensAsync(cts.Token).ConfigureAwait(true);
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }
                throw new AssertionFailedException("OperationCanceledException was not thrown");
            }
            catch (OperationCanceledException e)
            {
                e.Should().NotBeNull();
            }
            

            // Assert
            
        }
        
        private bool Cancel(CancellationTokenSource cts)
        {
           
            cts.CancelAfter(1000);
           
            return false;

        }
    }
}

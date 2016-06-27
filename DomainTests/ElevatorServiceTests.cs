using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace DomainTests
{
    public class ElevatorServiceTests
    {
        [Theory, DapperAutoData]
        public void Constructor_Instantiate_StartsOnFloorOne(List<ICallPanel> floorInterface,
            IElevator elevator, IElevatorControls controls)
        {
            // Act
            var service = new ElevatorService(elevator);

            // Assert
            service.CurrentFloor.Should().Be(1);
        }

        [Theory, DapperAutoData]
        public async void ArrivesAtFloor_DoorOpenIsCalled(Mock<ICallPanel> panel1, Mock<ICallPanel> panel2, IElevator elevator)
        {
            // Arrange
            ElevatorService service = new ElevatorService(elevator);
            panel1.Setup(x => x.Floor).Returns(1);
            panel2.Setup(x => x.Floor).Returns(2);
            service.RegisterCallPanel(panel1.Object);
            service.RegisterCallPanel(panel2.Object);
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(2).ConfigureAwait(false);

            // Assert
            await Task.Delay(2000).ConfigureAwait(false);
            panel2.Verify(x => x.DoorOpenEventHandlerAsync(), Times.Once);
        }

        [Theory, DapperAutoData]
        private bool AssertDoorOpensWithinTimePeriod(ICallPanel floorOnePanel, int milliseconds)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < milliseconds) // TODO: A bit of a hack. Think of a more elegant solution
            {
                if (floorOnePanel.IsDoorOpen)
                {
                    return true;
                }
            }
            watch.Stop();
            return false;
        }

        [Theory, DapperAutoData]
        public async void FloorChange_FloorChangeEventHandlerCalledOnEachFloor(
            [Frozen] Mock<IElevator> elevator,
            List<Mock<ICallPanel>> panels)
        {
            // Arrange
            var service = GetFunctionalElevatorService(panels, elevator.Object);
            int moveUpCount = 0;
            elevator.Setup(x => x.MoveUpAsync()).Returns(() =>
            {
                moveUpCount++;
                return Task.CompletedTask;
            });
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(service.TotalFloors).ConfigureAwait(false);

            // Assert
            // TODO: still a bit hacky. Need to create a proper way to wait on floor movement
            while (moveUpCount != service.TotalFloors - 1)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }
            elevator.Verify(x => x.MoveUpAsync(), Times.Exactly(service.TotalFloors - 1));
        }


        [Theory, DapperAutoData]
        public async void RegisterCallPanel_PassACallPanel_CallPanelIsRegistered(

            List<Mock<ICallPanel>> panels,
            IElevator elevator)
        {
            // Arrange
            var service = GetFunctionalElevatorService(panels, elevator);
            await service.StartAsync().ConfigureAwait(false);

            // act
            service.RegisterCallPanel(panels[0].Object);
            service.RegisterCallPanel(panels[1].Object);

            // assert
            service.GetCallPanelForFloor(1).Should().Be(panels[0].Object);
            service.GetCallPanelForFloor(2).Should().Be(panels[1].Object);

        }

        private static ElevatorService GetFunctionalElevatorService(List<Mock<ICallPanel>> panels, IElevator elevator)
        {
            ElevatorService service = new ElevatorService(elevator);
            foreach (var panel in panels)
            {
                service.RegisterCallPanel(panel.Object);
            }
            return service;
        }

        //  GetInterfaceForFloor_OutOfRangeRequest_ThrowsException

        #region UpCall

        [Theory, DapperAutoData]
        public async void UpCall_elevatorIsBelowWithNoOtherCalls_ElevatorComesOnlyToThisFloor(
            List<Mock<ICallPanel>> panels,
            IElevator elevator)
        {
            // Arrange
            ElevatorService service = new ElevatorService(elevator);
            foreach (var panel in panels)
            {
                service.RegisterCallPanel(panel.Object);
            }
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(5).ConfigureAwait(false);
            

            // Assert
            while (service.CurrentFloor != service.TotalFloors)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }
            await Task.Delay(3000).ConfigureAwait(false);
            panels[1].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            panels[2].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            panels[3].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            panels[4].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Once);
        }

        [Theory, DapperAutoData]
        public async void UpCall_ElevatorIsBelowWithNoOtherCalls_ElevatorIsMovedUpCorrectAmountOfTimes(
            [Frozen] Mock<IElevator> elevator,
            ElevatorService service
            )
        {
            // Arrange
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(service.TotalFloors).ConfigureAwait(false);

            // Assert
            while (service.CurrentFloor != service.TotalFloors)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }
            await Task.Delay(3000).ConfigureAwait(false);
            elevator.Verify(x => x.MoveUpAsync(), Times.Exactly(4));
        }

        //        UpCall_elevatorIsBelowWithUpCallsAbove_ElevatorStopsHereFirst
        //        UpCall_ElevatorIsBelowWithDowncallsBelow_ElevatorStopsHereFirst
        //        UpCall_ElevatorArrivesAtFloor_DoorOpenEventHandlerCalled
        //        UpCall_ElevatorIsOnItsWayupWhenCallIsMadeInBetween_ElevatorStopsAtThatFloorFirst
        //        UpCall_ElevatorIsOnCurrentFloorWithDoorClosed_DoorOpensThenClosesInReasonableAmountOfTime

        #endregion

        #region DownCall

        // See above due to time

        #endregion

        #region RegisterCallPanel

        public void RegisterCallPanel_PassACallPanel_CallPanelIsRegistered()
        {
            
        }
        #endregion

    }
}

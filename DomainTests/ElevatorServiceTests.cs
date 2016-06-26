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
            var service = new ElevatorService(floorInterface, elevator, controls);

            // Assert
            service.CurrentFloor.Should().Be(1);
        }

        [Theory, DapperAutoData]
        public async void ArrivesAtFloor_DoorOpenIsCalled(
            Mock<ICallPanel> panel1,
            Mock<ICallPanel> panel2,
            IElevator elevator,
            IElevatorControls controls)
        {
            // Arrange
            ElevatorService service = new ElevatorService(new List<ICallPanel> {panel1.Object, panel2.Object}, elevator, controls);
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(2).ConfigureAwait(false);

            // Assert
            Thread.Sleep(2000);
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
            ElevatorService service)
        {
            // Arrange
            int moveUpCount = 0;
            elevator.Setup(x => x.MoveUpAsync()).Returns(() =>
            {
                moveUpCount++;
                return Task.FromResult(0);
            });
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(service.TotalFloors).ConfigureAwait(false);

            // Assert
            // TODO: still a bit hacky. Need to create a proper way to wait on floor movement
            while (moveUpCount != service.TotalFloors - 1)
            {
                Thread.Sleep(100);
            }
            elevator.Verify(x => x.MoveUpAsync(), Times.Exactly(service.TotalFloors - 1));
        }


        [Theory, DapperAutoData]
        public void GetInterfaceForFloor_FloorCallInterfacesHaveBeenInjected_CorrectInterfaceIsReturned(
            List<ICallPanel> seedExternalCallInterfaces,
            IElevator elevator,
            IElevatorControls controls)
        {
            var service = new ElevatorService(seedExternalCallInterfaces, elevator, controls);
            for (int i = 0; i < seedExternalCallInterfaces.Count; i++)
            {
                service.GetCallPanelForFloor(i + 1).Should().Be(seedExternalCallInterfaces[i]);
            }
        }

        //  GetInterfaceForFloor_OutOfRangeRequest_ThrowsException

        #region UpCall

        [Theory, DapperAutoData]
        public async void UpCall_elevatorIsBelowWithNoOtherCalls_ElevatorComesOnlyToThisFloor(
            List<Mock<ICallPanel>> callInterfaces,
            IElevator elevator,
            IElevatorControls controls)
        {
            // Arrange
            List<ICallPanel> callPanels = callInterfaces.Select(x => x.Object).ToList(); 
            var service = new ElevatorService(callPanels, elevator, controls);
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(5).ConfigureAwait(false);
            

            // Assert
            while (service.CurrentFloor != service.TotalFloors)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(3000);
            callInterfaces[1].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            callInterfaces[2].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            callInterfaces[3].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            callInterfaces[4].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Once);
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
                Thread.Sleep(100);
            }
            Thread.Sleep(3000);
            elevator.Verify(x => x.MoveUpAsync(), Times.Exactly(4));
        }

        [Theory, DapperAutoData]
        public async void UpCall_ElevatorIsBelowWithUpCallsBelow_ElevatorStopsAtFloorsInOrder(
            [Frozen] Mock<IElevator> elevator,
            ElevatorService service)
        {
            // Arrange
            await service.StopAsync().ConfigureAwait(false);
            await service.UpCallRequestAsync(2).ConfigureAwait(false);
            await service.UpCallRequestAsync(3).ConfigureAwait(false);
            var floor2 = service.GetCallPanelForFloor(2);
            var floor3 = service.GetCallPanelForFloor(3);
            var floor5 = service.GetCallPanelForFloor(5);
            elevator.Setup(x => x.MoveUpAsync()).Callback(() => Thread.Sleep(1000));
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(5).ConfigureAwait(false);

            // Assert
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


    }
}

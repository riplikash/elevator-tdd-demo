using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        // TODO: if there are upcalls above me and i hit the up button before it leaves the door should open
        // inverse for down
        // if I hit the down button while the direction is up on the current floor the door should NOT open
        // inverse forf u

        [Theory, DapperAutoData]
        public void Constructor_Instantiate_StartsOnFloorOne(List<IExternalCallInterface> floorInterface,
            IElevator elevator, IElevatorInteriorInterface interiorInterface)
        {
            // Act
            var service = new ElevatorService(5, floorInterface, elevator, interiorInterface);

            // Assert
            service.CurrentFloor.Should().Be(1);
        }

//        public void DepartsFloor_DoorCloseCalled(ElevatorService service)
//        {
//            // Arrange
//            var floorOneInterface = service.GetExternalCallInterfaceForFloor(1);
//            // Act
//
//
//            // Assert
//        }

        [Theory, DapperAutoData]
        public async void ArrivesAtFloor_DoorOpenIsCalled(
            IElevatorService service)
        {
            // Arrange
            var floorOneInterface = service.GetExternalCallInterfaceForFloor(1);

            // Act
            await service.UpCallRequestAsync(2).ConfigureAwait(false);

            // Assert
            AssertDoorOpensWithinTimePeriod(floorOneInterface, 3000).Should().BeTrue();
        }

        [Theory, DapperAutoData]
        private bool AssertDoorOpensWithinTimePeriod(IExternalCallInterface floorOneInterface, int milliseconds)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < milliseconds) // TODO: A bit of a hack. Think of a more elegant solution
            {
                if (floorOneInterface.IsDoorOpen)
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
            IElevatorService service)
        {
            // Arrange
            var floorFiveInterface = service.GetExternalCallInterfaceForFloor(5);

            // Act
            await service.UpCallRequestAsync(5).ConfigureAwait(false);
            await WaitUntilDoorOpensAsync(floorFiveInterface).ConfigureAwait(false);

            // Assert
            elevator.Verify(x => x.MoveUpAsync(), Times.Exactly(service.TotalFloors - 1));
        }

        [Theory, DapperAutoData]
        private Task WaitUntilDoorOpensAsync(IExternalCallInterface externalCallInterface)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < 5000) // TODO: A bit of a hack. Think of a more elegant solution
            {
                if (externalCallInterface.IsDoorOpen)
                {
                    return Task.FromResult(0);
                }
            }
            watch.Stop();
            throw new TimeoutException("Elevator never arrived");
        }

        [Theory, DapperAutoData]
        public void GetInterfaceForFloor_FloorCallInterfacesHaveBeenInjected_CorrectInterfaceIsReturned(
            [Frozen] List<IExternalCallInterface> seedExternalCallInterfaces,
            IElevatorService service)
        {
            for (int i = 0; i < seedExternalCallInterfaces.Count; i++)
            {
                service.GetExternalCallInterfaceForFloor(i).Should().Be(seedExternalCallInterfaces[i]);
            }
        }

        //  GetInterfaceForFloor_OutOfRangeRequest_ThrowsException

        #region UpCall

        [Theory, DapperAutoData]
        public async void UpCall_elevatorIsBelowWithNoOtherCalls_ElevatorComesOnlyToThisFloor(
            [Frozen] List<Mock<IExternalCallInterface>> callInterfaces,
            [Frozen] Mock<IElevator> elevator,
            IElevatorService service)
        {
            // Arrange

            // Act
            await service.UpCallRequestAsync(5).ConfigureAwait(false);
            await WaitUntilDoorOpensAsync(service.GetExternalCallInterfaceForFloor(5)).ConfigureAwait(false);

            // Assert
            callInterfaces[1].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            callInterfaces[2].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            callInterfaces[3].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Never);
            callInterfaces[4].Verify(x => x.DoorOpenEventHandlerAsync(), Times.Once);
        }

        [Theory, DapperAutoData]
        public async void UpCall_ElevatorIsBelowWithNoOtherCalls_ElevatorIsMovedUpCorrectAmountOfTimes(
            [Frozen] Mock<IElevator> elevator,
            IElevatorService service
            )
        {
            // Arrange

            // Act
            await service.UpCallRequestAsync(5).ConfigureAwait(false);
            await WaitUntilDoorOpensAsync(service.GetExternalCallInterfaceForFloor(5)).ConfigureAwait(false);

            // Assert
            elevator.Verify(x => x.MoveUpAsync(), Times.Exactly(4));
        }

        [Theory, DapperAutoData]
        public async void UpCall_ElevatorIsBelowWithUpCallsBelow_ElevatorStopsAtFloorsInOrder(
            [Frozen] Mock<IElevator> elevator,
            IElevatorService service)
        {
            // Arrange
            await service.StopAsync().ConfigureAwait(false);
            await service.UpCallRequestAsync(2).ConfigureAwait(false);
            await service.UpCallRequestAsync(3).ConfigureAwait(false);
            var floor2 = service.GetExternalCallInterfaceForFloor(2);
            var floor3 = service.GetExternalCallInterfaceForFloor(3);
            var floor5 = service.GetExternalCallInterfaceForFloor(5);
            elevator.Setup(x => x.MoveUpAsync()).Callback(() => Thread.Sleep(1000));
            await service.StartAsync().ConfigureAwait(false);

            // Act
            await service.UpCallRequestAsync(5).ConfigureAwait(false);

            // Assert
            await WaitUntilDoorOpensAsync(floor2).ConfigureAwait(false);
            await WaitUntilDoorOpensAsync(floor3).ConfigureAwait(false);
            await WaitUntilDoorOpensAsync(floor5).ConfigureAwait(false);
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

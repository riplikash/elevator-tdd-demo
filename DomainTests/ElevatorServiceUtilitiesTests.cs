using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexprof.AutoMoq;
using Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace DomainTests
{
    public class ElevatorServiceUtilitiesTests
    {
        #region IsAtApex

        [Theory]
        [DapperAutoData(5, 1, 1)]
        [DapperAutoData(2, 1, 1)]
        [DapperAutoData(3, 2, 1)]
        [DapperAutoData(3, 1, 2)]
        public void IsAtApex_AllCallsBelowCurrentFloor_True(
            int currentFloor,
            int upCall,
            int downCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int> {upCall});
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> {downCall});

            // act
            var atApex = utilities.IsAtApex();

            // assert
            atApex.Should().BeTrue();
        }

        [Theory]
        [DapperAutoData(1, 2, 3)]
        [DapperAutoData(1, 3, 2)]
        [DapperAutoData(3, 4, 2)]
        [DapperAutoData(3, 2, 4)]
        [DapperAutoData(4, 5, 1)]
        public void IsAtApex_CallsAboveCurrentFloor_False(
            int currentFloor,
            int upCall,
            int downCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int> {upCall});
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> {downCall});

            // act
            var atApex = utilities.IsAtApex();

            // assert
            atApex.Should().BeFalse();
        }

        #endregion
        #region IsAtNadir

        [Theory]
        [DapperAutoData(5, 1, 1)]
        [DapperAutoData(2, 1, 1)]
        [DapperAutoData(3, 2, 4)]
        [DapperAutoData(3, 4, 2)]
        public void IsAtNadir_CallsBelowCurrentFloor_False(
            int currentFloor,
            int upCall,
            int downCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int> { upCall });
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> { downCall });

            // act
            var atNadir = utilities.IsAtNadir();

            // assert
            atNadir.Should().BeFalse();
        }

        [Theory]
        [DapperAutoData(1, 2, 3)]
        [DapperAutoData(1, 3, 2)]
        [DapperAutoData(3, 4, 5)]
        [DapperAutoData(3, 5, 4)]
        [DapperAutoData(2, 5, 3)]
        public void IsAtNadir_AllCallsAboveCurrentFloor_True(
            int currentFloor,
            int upCall,
            int downCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int> { upCall });
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> { downCall });

            // act
            var atNadir = utilities.IsAtNadir();

            // assert
            atNadir.Should().BeTrue();
        }

        #endregion


        #region IsThereADownCallAboveCurrentFloor

        [Theory]
        [DapperAutoData(1, 2)]
        [DapperAutoData(1, 3)]
        [DapperAutoData(1, 5)]
        [DapperAutoData(2, 3)]
        [DapperAutoData(4, 5)]
        public void IsThereADownCallAboveCurrentFloor_Yes_ReturnsTrue(
            int currentFloor,
            int downCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int>());
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> {downCall});

            // act
            var isThere = utilities.IsThereADowncallAboveCurrentFloor();

            // assert
            isThere.Should().BeTrue();
        }

        [Theory]
        [DapperAutoData(1, 1)]
        [DapperAutoData(2, 1)]
        [DapperAutoData(5, 4)]
        [DapperAutoData(5, 1)]
        public void IsThereADownCallAboveCurrentFloor_No_ReturnsFalse(
            int currentFloor,
            int DownCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int>());
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> {DownCall});

            // act
            var isThere = utilities.IsThereADowncallAboveCurrentFloor();

            // assert
            isThere.Should().BeFalse();
        }

        #endregion

        #region IsThereAnUpCallAboveCurrentFloor

        [Theory]
        [DapperAutoData(1, 2)]
        [DapperAutoData(1, 3)]
        [DapperAutoData(1, 5)]
        [DapperAutoData(2, 3)]
        [DapperAutoData(4, 5)]
        public void IsThereAnUpCallAboveCurrentFloor_Yes_ReturnsTrue(
            int currentFloor,
            int upCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int> {upCall});
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int>());

            // act
            var isThere = utilities.IsThereAnUpcallAboveCurrentFloor();

            // assert
            isThere.Should().BeTrue();
        }

        [Theory]
        [DapperAutoData(1, 1)]
        [DapperAutoData(2, 1)]
        [DapperAutoData(5, 4)]
        public void IsThereAnUpCallAboveCurrentFloor_No_ReturnsFalse(
            int currentFloor,
            int upCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int>());
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> {upCall});

            // act
            var isThere = utilities.IsThereAnUpcallAboveCurrentFloor();

            // assert
            isThere.Should().BeFalse();
        }

        #endregion
        
        #region IsThereADownCallBelowCurrentFloor

        [Theory]
        [DapperAutoData(5, 4)]
        [DapperAutoData(5, 1)]
        [DapperAutoData(3, 2)]
        [DapperAutoData(2, 1)]
        public void IsThereADownCallBelowCurrentFloor_Yes_ReturnsTrue(
            int currentFloor,
            int downCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int>());
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> {downCall});

            // act
            var isThere = utilities.IsThereADowncallBelowCurrentFloor();

            // assert
            isThere.Should().BeTrue();
        }

        [Theory]
        [DapperAutoData(5, 5)]
        [DapperAutoData(1, 1)]
        [DapperAutoData(4, 5)]
        [DapperAutoData(2, 3)]
        public void IsThereADownCallBelowCurrentFloor_No_ReturnsFalse(
            int currentFloor,
            int DownCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int>());
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> {DownCall});

            // act
            var isThere = utilities.IsThereADowncallBelowCurrentFloor();

            // assert
            isThere.Should().BeFalse();
        }

        #endregion

        #region IsThereAnUpCallBelowCurrentFloor

        [Theory]
        [DapperAutoData(5, 4)]
        [DapperAutoData(5, 1)]
        [DapperAutoData(3, 2)]
        [DapperAutoData(2, 1)]
        public void IsThereAnUpCallBelowCurrentFloor_Yes_ReturnsTrue(
            int currentFloor,
            int upCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int> {upCall});
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int>());

            // act
            var isThere = utilities.IsThereAnUpcallBelowCurrentFloor();

            // assert
            isThere.Should().BeTrue();
        }

        [Theory]
        [DapperAutoData(5, 5)]
        [DapperAutoData(1, 1)]
        [DapperAutoData(4, 5)]
        [DapperAutoData(2, 3)]
        public void IsThereAnUpCallBelowCurrentFloor_No_ReturnsFalse(
            int currentFloor,
            int upCall,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(new HashSet<int>());
            elevatorService.Setup(x => x.DownQueue).Returns(new HashSet<int> {upCall});

            // act
            var isThere = utilities.IsThereAnUpcallBelowCurrentFloor();

            // assert
            isThere.Should().BeFalse();
        }

        #endregion

        #region GetClosestDirectionWhereCallAndDirectionAreSame

        [Theory]
        [DapperAutoData(3, 4, 1, DirectionEnum.Up)]
        [DapperAutoData(3, 2, 5, DirectionEnum.Stationary)]
        [DapperAutoData(3, 4, 2, DirectionEnum.Up)]
        [DapperAutoData(1, 5,  0, DirectionEnum.Up)]
        [DapperAutoData(5, 0, 1, DirectionEnum.Down)]
        [DapperAutoData(3, 2, 1, DirectionEnum.Down)]
        [DapperAutoData(3, 4, 5, DirectionEnum.Up)]
        [DapperAutoData(4, 4, 4, DirectionEnum.Stationary)]
        [DapperAutoData(2, 0, 0, DirectionEnum.Stationary)]
        [DapperAutoData(3, 2, 4, DirectionEnum.Stationary)]
        public void GetClosestDirectionWhereCallAndDirectionAreSame_TwoOptions_ChoosesCloser(
            int currentFloor,
            int upCall,
            int downCall,
            DirectionEnum  correctDirection,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)

        {
            // arrange
            var upQueue = new HashSet<int>();
            var downQueue = new HashSet<int>();

            if (upCall > 0) upQueue.Add(upCall);
            if (downCall > 0) downQueue.Add(downCall);

            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(upQueue);
            elevatorService.Setup(x => x.DownQueue).Returns(downQueue);

            // act
            var direction = utilities.GetClosestDirectionWhereCallAndDirectionAreSame();
            

            // assert
            direction.Should().Be(correctDirection);
        }
        #endregion

        #region GetClosestDowncallBelowCurrentFloor
        [Theory] 
        [DapperAutoData(5, 2, 1, 2)]
        [DapperAutoData(5, 3, 4, 4)]
        [DapperAutoData(2, 1, 5, 1)]
        [DapperAutoData(4, 5, 1, 1)]
        [DapperAutoData(5, 1, 0, 1)]
        [DapperAutoData(5, 0, 1, 1)]
        [DapperAutoData(3, 0, 0, 0)]
        [DapperAutoData(3, 4, 5, 0)]
        public void GetClosestDowncallBelowCurrentFloor_InlineOptions_ComparesCorrectlyAgainstExpectedOutput(
            int currentFloor,
            int downCall1,
            int downCall2,
            int expected,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // Arrange
            var downQueue = new HashSet<int>();

            if (downCall1 > 0) downQueue.Add(downCall1);
            if (downCall2 > 0) downQueue.Add(downCall2);

            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.DownQueue).Returns(downQueue);

            // Act
            var choice = utilities.GetClosestDowncallBelowCurrentFloor();

            // Assert
            if (expected != 0)
                choice.Should().Be(expected);
            else
                choice.Should().Be(null);
        }

        #endregion

        #region GetClosestUpcallAboveCurrentFloor

        [Theory]
        [DapperAutoData(1, 2, 3, 2)]
        [DapperAutoData(1, 5, 3, 3)]
        [DapperAutoData(4, 5, 3, 5)]
        [DapperAutoData(5, 4, 4, 0)]
        [DapperAutoData(5, 5, 5, 0)]
        [DapperAutoData(2, 1, 5, 5)]
        public void GetClosestUpcallAboveCurrentFloor(int currentFloor,
            int upCall1,
            int upCall2,
            int expected,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // Arrange
            var upQueue = new HashSet<int>();

            if (upCall1 > 0) upQueue.Add(upCall1);
            if (upCall2 > 0) upQueue.Add(upCall2);

            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(upQueue);

            // Act
            var choice = utilities.GetClosestUpcallAboveCurrentFloor();

            // Assert
            if (expected != 0)
                choice.Should().Be(expected);
            else
                choice.Should().Be(null);
        }

        #endregion
        #region GetFurthestDirectionWhereCallAndDirectionAreDifferent
        [Theory]
        [DapperAutoData(3, 2, 5, DirectionEnum.Up)]
        [DapperAutoData(3, 1, 4, DirectionEnum.Down)]
        [DapperAutoData(3, 3, 3, DirectionEnum.Stationary)]
        [DapperAutoData(3, 0, 0, DirectionEnum.Stationary)]
        [DapperAutoData(1, 5, 2, DirectionEnum.Up)]
        [DapperAutoData(5, 4, 1, DirectionEnum.Down)]
        public void GetFurthestDirectionWhereCallAndDirectionAreDifferent(
            int currentFloor,
            int upCall,
            int downCall,
            DirectionEnum expected,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // Arrange
            var upQueue = new HashSet<int>();
            var downQueue = new HashSet<int>();
            if (upCall > 0) upQueue.Add(upCall);
            if (downCall > 0) downQueue.Add(downCall);

            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(upQueue);
            elevatorService.Setup(x => x.DownQueue).Returns(downQueue);

            // Act
            var choice = utilities.GetFurthestDirectionWhereCallAndDirectionAreDifferent();

            // Assert
            choice.Should().Be(expected);

        }
        #endregion
        #region GetFurthestUpcallBelowCurrentFloor
        [Theory]
        [DapperAutoData(5, 4, 1, 1)]
        [DapperAutoData(5, 1, 4, 1)]
        [DapperAutoData(2, 1, 5, 1)]
        [DapperAutoData(2, 3, 5, null)]
        [DapperAutoData(3, 4, 2, 2)]
        [DapperAutoData(3, 4, 5, null)]
        [DapperAutoData(1, 2, 3, null)]
        [DapperAutoData(3, 2, 0, 2)]
        [DapperAutoData(3, 0, 1, 1)]
        [DapperAutoData(3, 0, 0, null)]
        public void GetFurthestUpcallBelowCurrentFloor(
            int currentFloor,
            int upCall1,
            int upCall2,
            int? expected,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // Arrange
            var upQueue = new HashSet<int>();
            var downQueue = new HashSet<int>();
            if (upCall1 > 0) upQueue.Add(upCall1);
            if (upCall2 > 0) upQueue.Add(upCall2);

            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(upQueue);
            elevatorService.Setup(x => x.DownQueue).Returns(downQueue);

            // Act
            var choice = utilities.GetFurthestUpcallBelowCurrentFloor();

            // Assert
            choice.Should().Be(expected);
        }

        #endregion

        #region GetFurthestDowncallAboveCurrentFloor
        [Theory]
        [DapperAutoData(1, 2, 5, 5)]
        [DapperAutoData(1, 5, 2, 5)]
        [DapperAutoData(4, 5, 1, 5)]
        [DapperAutoData(4, 3, 1, null)]
        [DapperAutoData(3, 2, 4, 4)]
        [DapperAutoData(3, 2, 1, null)]
        [DapperAutoData(5, 4, 3, null)]
        [DapperAutoData(3, 4, 0, 4)]
        [DapperAutoData(3, 0, 5, 5)]
        [DapperAutoData(3, 0, 0, null)]
        public void GetFurthestDowncallBelowCurrentFloor(
            int currentFloor,
            int downCall1,
            int downCall2,
            int? expected,
            [Frozen] Mock<IElevatorService> elevatorService,
            ElevatorServiceUtilities utilities)
        {
            // Arrange
            var upQueue = new HashSet<int>();
            var downQueue = new HashSet<int>();
            if (downCall1 > 0) downQueue.Add(downCall1);
            if (downCall2 > 0) downQueue.Add(downCall2);

            elevatorService.Setup(x => x.CurrentFloor).Returns(currentFloor);
            elevatorService.Setup(x => x.TotalFloors).Returns(5);
            elevatorService.Setup(x => x.UpQueue).Returns(upQueue);
            elevatorService.Setup(x => x.DownQueue).Returns(downQueue);

            // Act
            var choice = utilities.GetFurthestDowncallAboveCurrentFloor();

            // Assert
            choice.Should().Be(expected);
        }
    #endregion
    }
}

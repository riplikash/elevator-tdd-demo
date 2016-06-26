using System;

namespace Domain
{
    public class ElevatorServiceUtilities
    {
        public IElevatorService elevatorService;

        public ElevatorServiceUtilities(IElevatorService elevatorService)
        {
            this.elevatorService = elevatorService;
        }

        public DirectionEnum SelectMostAppropriateDirectionBasedOnHeuristic()
        {
            var rtValue = GetClosestDirectionWhereCallAndDirectionAreSame();
            if (rtValue == DirectionEnum.Stationary)
                rtValue = GetFurthestDirectionWhereCallAndDirectionAreDifferent();
            return rtValue;
        }

        public DirectionEnum GetFurthestDirectionWhereCallAndDirectionAreDifferent()
        {
            var furthestDowncall = GetFurthestDowncallAboveCurrentFloor();
            var furthestUpcall = GetFurthestUpcallBelowCurrentFloor();
            if (furthestUpcall == null && furthestDowncall == null) return DirectionEnum.Stationary;

            if (furthestUpcall == null) return DirectionEnum.Up;
            if (furthestDowncall == null) return DirectionEnum.Down;

            var distanceToUpcall = elevatorService.CurrentFloor - furthestUpcall.Value;
            var distanceToDowncall = furthestDowncall - elevatorService.CurrentFloor ;

            if (distanceToUpcall >= distanceToDowncall) return DirectionEnum.Down;
            return DirectionEnum.Up;
        }

        public int? GetFurthestUpcallBelowCurrentFloor()
        {
            for (var i = 1; i < elevatorService.CurrentFloor; i++)
            {
                if (elevatorService.UpCalls.Contains(i)) return i;
            }
            return null;
        }

        public int? GetFurthestDowncallAboveCurrentFloor()
        {
            for (var i = elevatorService.TotalFloors; i > elevatorService.CurrentFloor; i--)
            {
                if (elevatorService.DownCalls.Contains(i)) return i;
            }
            return null;
        }

        public DirectionEnum GetClosestDirectionWhereCallAndDirectionAreSame()
        {
            var closestUpcallAboveCurrentFloor = GetClosestUpcallAboveCurrentFloor();
            var closestDowncallBelowCurrentFloor = GetClosestDowncallBelowCurrentFloor();

            // If there are no upcalls above or downcalls below we select nothing
            if (closestDowncallBelowCurrentFloor == null && closestUpcallAboveCurrentFloor == null) return DirectionEnum.Stationary;

            // If there is only one choice, choose it
            if (closestUpcallAboveCurrentFloor == null) return DirectionEnum.Down;
            if (closestDowncallBelowCurrentFloor == null) return DirectionEnum.Up;

            // attempt to choose the closest option
            var distanceUp = closestUpcallAboveCurrentFloor - elevatorService.CurrentFloor;
            var distanceDown = elevatorService.CurrentFloor - closestDowncallBelowCurrentFloor;

            if (distanceUp <= distanceDown)
            {
                return DirectionEnum.Up;
            } else if (distanceDown < distanceUp)
            {
                return DirectionEnum.Down;
            }

            // if all options are equally close then choose the route that will provide the most options in the future
            if (elevatorService.CurrentFloor >= 3)
            {
                return DirectionEnum.Down;
            }

            return DirectionEnum.Up;
        }

        public int? GetClosestDowncallBelowCurrentFloor()
        {
            for (var i = elevatorService.CurrentFloor - 1; i >= 1; i--)
            {
                if (elevatorService.DownCalls.Contains(i)) return i;
            }
            return null;
        }

        public int? GetClosestUpcallAboveCurrentFloor()
        {
            for (var i = elevatorService.CurrentFloor + 1; i <= elevatorService.TotalFloors; i++)
            {
                if (elevatorService.UpCalls.Contains(i)) return i;
            }
            return null;
        }

        public bool IsAtNadir()
        {
            if (elevatorService.CurrentFloor == 1) return true;
            return !IsThereADowncallBelowCurrentFloor() && !IsThereAnUpcallBelowCurrentFloor();
        }

        public bool IsAtApex()
        {
            if (elevatorService.CurrentFloor == elevatorService.TotalFloors) return true;
            return !IsThereADowncallAboveCurrentFloor() && !IsThereAnUpcallAboveCurrentFloor();
        }

        public bool IsThereAnUpcallBelowCurrentFloor()
        {
            if (elevatorService.CurrentFloor == 1) return false;
            for (var i = elevatorService.CurrentFloor - 1; i >= 1; i--)
            {
                if (elevatorService.UpCalls.Contains(i)) return true;
            }
            return false;
        }

        public bool IsThereADowncallBelowCurrentFloor()
        {
            if (elevatorService.CurrentFloor == 1) return false;
            for (var i = elevatorService.CurrentFloor -1; i >= 1; i--)
            {
                if (elevatorService.DownCalls.Contains(i)) return true;
            }
            return false;
        }

        public bool IsThereAnUpcallAboveCurrentFloor()
        {
            if (elevatorService.CurrentFloor == elevatorService.TotalFloors) return false;
            for (var i = elevatorService.CurrentFloor + 1; i <= elevatorService.TotalFloors; i++)
            {
                if (elevatorService.UpCalls.Contains(i)) return true;
            }
            return false;
        }

        public bool IsThereADowncallAboveCurrentFloor()
        {
            if (elevatorService.CurrentFloor == elevatorService.TotalFloors) return false;
            for (var i = elevatorService.CurrentFloor + 1; i <= elevatorService.TotalFloors; i++)
            {
                if (elevatorService.DownCalls.Contains(i)) return true;
            }
            return false;
        }


    }
}
using System;

namespace Domain
{
    public class ElevatorServiceUtilities
    {
        public ElevatorService _elevatorService;

        public ElevatorServiceUtilities(ElevatorService elevatorService)
        {
            _elevatorService = elevatorService;
        }

        public string SelectMostAppropriateDirectionBasedOnHeuristic()
        {
            string rtValue = (GetClosestDirectionWhereCallAndDirectionAreSame() ??
                              GetFurthestDirectionWhereCallAndDirectionAreDifferent()) ?? "";
            return rtValue;
        }

        public string GetFurthestDirectionWhereCallAndDirectionAreDifferent()
        {
            string direction;
            int? furthestDowncall = GetFurthestDowncallAboveCurrentFloor();
            int? furthestUpcall = GetFurthestUpcallBelowCurrentFloor();
            if (furthestUpcall == null && furthestDowncall == null) return "";

            if (furthestUpcall == null && furthestDowncall != null) return "down";
            if (furthestDowncall == null && furthestUpcall != null) return "up";

            var distanceToUpcall = furthestUpcall - _elevatorService.CurrentFloor;
            var distanceToDowncall = _elevatorService.CurrentFloor - furthestDowncall;

            if (distanceToUpcall >= distanceToDowncall) return "down";
            return "up";
        }

        public int? GetFurthestUpcallBelowCurrentFloor()
        {
            for (int i = 1; i > _elevatorService.CurrentFloor; i++)
            {
                if (_elevatorService.upQueue.Contains(i)) return i;
            }
            return null;
        }

        public int? GetFurthestDowncallAboveCurrentFloor()
        {
            for (int i = _elevatorService.TotalFloors; i < _elevatorService.CurrentFloor; i--)
            {
                if (_elevatorService.downQueue.Contains(i)) return i;
            }
            return null;
        }

        public string GetClosestDirectionWhereCallAndDirectionAreSame()
        {
            int? closestUpcallAboveCurrentFloor = GetClosestUpcallAboveCurrentFloor();
            int? closestDowncallBelowCurrentFloor = GetClosestDowncallBelowCurrentFloor();

            // If there are no upcalls above or downcalls below we select nothing
            if (closestDowncallBelowCurrentFloor == null && closestUpcallAboveCurrentFloor == null) return null;

            // If there is only one choice, choose it
            if (closestUpcallAboveCurrentFloor == null) return "down";
            if (closestDowncallBelowCurrentFloor == null) return "up";

            // attempt to choose the closest option
            var distanceUp = closestUpcallAboveCurrentFloor - _elevatorService.CurrentFloor;
            var distanceDown = _elevatorService.CurrentFloor - closestDowncallBelowCurrentFloor;

            if (distanceUp < distanceDown)
            {
                return "up";
            } else if (distanceDown < distanceUp)
            {
                return "down";
            }

            // if all options are equally close then choose the route that will provide the most options in the future
            if (_elevatorService.CurrentFloor >= 3)
            {
                return "down";
            }

            return "up";
        }

        public int? GetClosestDowncallBelowCurrentFloor()
        {
            for (int i = _elevatorService.CurrentFloor; i >= 1; i--)
            {
                if (_elevatorService.downQueue.Contains(i)) return i;
            }
            return null;
        }

        public int? GetClosestUpcallAboveCurrentFloor()
        {
            for (int i = _elevatorService.CurrentFloor; i <= _elevatorService.TotalFloors; i++)
            {
                if (_elevatorService.upQueue.Contains(i)) return i;
            }
            return null;
        }

        public bool IsAtNadir()
        {
            if (_elevatorService.CurrentFloor == 1) return true;
            return !IsThereADowncallBelowCurrentFloor() && !IsThereAnUpcallBelowCurrentFloor();
        }

        public bool IsAtApex()
        {
            if (_elevatorService.CurrentFloor == _elevatorService.TotalFloors) return true;
            return !IsThereADowncallAboveCurrentFloor() && !IsThereAnUpcallAboveCurrentFloor();
        }

        public bool IsThereAnUpcallBelowCurrentFloor()
        {
            if (_elevatorService.CurrentFloor == 1) return false;
            for (int i = _elevatorService.CurrentFloor; i >= 1; i--)
            {
                if (_elevatorService.upQueue.Contains(i)) return true;
            }
            return false;
        }

        public bool IsThereADowncallBelowCurrentFloor()
        {
            if (_elevatorService.CurrentFloor == 1) return false;
            for (int i = _elevatorService.CurrentFloor; i >= 1; i--)
            {
                if (_elevatorService.downQueue.Contains(i)) return true;
            }
            return false;
        }

        public bool IsThereAnUpcallAboveCurrentFloor()
        {
            if (_elevatorService.CurrentFloor == _elevatorService.TotalFloors) return false;
            for (int i = _elevatorService.CurrentFloor; i <= _elevatorService.TotalFloors; i++)
            {
                if (_elevatorService.upQueue.Contains(i)) return true;
            }
            return false;
        }

        public bool IsThereADowncallAboveCurrentFloor()
        {
            if (_elevatorService.CurrentFloor == _elevatorService.TotalFloors) return false;
            for (int i = _elevatorService.CurrentFloor; i <= _elevatorService.TotalFloors; i++)
            {
                if (_elevatorService.downQueue.Contains(i)) return true;
            }
            return false;
        }
    }
}
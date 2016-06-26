using System.Threading.Tasks;

namespace ApplicationServices
{
    public interface IElevatorInteriorActions : IPersonActions
    {
        Task<string> CheckCurrentFloorAsync();
        
        Task LeaveWhenDoorOpensAsync();
        Task PushButton1();
        Task PushButton2();
        Task PushButton3();
        Task PushButton4();
        Task PushButton5();
    }

    class ElevatorInteriorActions : IElevatorInteriorActions
    {
        public string CheckElevatorPositionAsync()
        {
            throw new System.NotImplementedException();
        }

        public string CheckSurroundings()
        {
            throw new System.NotImplementedException();
        }

        public Task EnterDoorWhenItOpensAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> CheckCurrentFloorAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task LeaveWhenDoorOpensAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task PushButton1()
        {
            throw new System.NotImplementedException();
        }

        public Task PushButton2()
        {
            throw new System.NotImplementedException();
        }

        public Task PushButton3()
        {
            throw new System.NotImplementedException();
        }

        public Task PushButton4()
        {
            throw new System.NotImplementedException();
        }

        public Task PushButton5()
        {
            throw new System.NotImplementedException();
        }
    }
}
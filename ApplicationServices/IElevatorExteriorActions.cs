using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices
{
    public interface IElevatorExteriorActions : IPersonActions
    {
        Task PushGoingUpButtonAsync();
        Task PushGoingDownButtonAsync();
    }
}

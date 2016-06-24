using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IElevatorService
    {
        Task UpCallRequestAsync(int floor);
        Task DownCallRequestAsync(int floor);
    }
}

using System.Threading.Tasks;

namespace Domain
{
    public interface IElevator
    {
        Task MoveUpAsync();
        Task MoveDownAsync();
    }
}

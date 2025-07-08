using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Interfaces
{
    public interface ISubmitService
    {

        bool RemoveUserFromEvent(int eventId, string userId);
    }
}

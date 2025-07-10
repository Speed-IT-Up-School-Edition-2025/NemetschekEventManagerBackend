
using Microsoft.AspNetCore.Identity.UI.Services;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;
namespace NemetschekEventManagerBackend;


public interface IEventService
{
    bool Create(Event newEvent);
    Event? GetEventById(int eventId);
    List<EventSummaryDto> GetEvents();
    List<Event> GetEvents(string searchName, DateTime? date, bool? activeOnly);
    Task<bool> RemoveById(int eventId, IEmailSender _emailSender);
    bool Update(int eventId, UpdateEventDto dto);

}

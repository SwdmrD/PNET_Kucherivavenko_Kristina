using System.Threading.Tasks;
using BlazorApp.Models;

namespace BlazorApp
{
    public class ActivityLogServiceFacade
    {
        private readonly ActivityLogService _activityLogService;

        public ActivityLogServiceFacade(ActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        public async Task LogActivity(string action, string entityType, string entityId, string details)
        {
            await _activityLogService.LogActivity(action, entityType, entityId, details);
        }

        public async Task<List<ActivityLog>> GetAllLogs()
        {
            return await _activityLogService.GetAllLogs();
        }
    }
}

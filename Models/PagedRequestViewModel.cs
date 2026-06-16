using YokohamaMaintenanceSystem.Enums;

namespace YokohamaMaintenanceSystem.Models
{
    public class PagedRequestViewModel
    {
        public List<MaintenanceRequest> Requests { get; set; } = new();
        public int CurrentPage { get; set; }
        public bool HasNextPage { get; set; }
        public string? Keyword { get; set; }
        public RequestStatus? SelectStatus { get; set; }
    }
}

using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Models;
using YokohamaMaintenanceSystem.Services;

namespace YokohamaMaintenanceSystem.Tests;

public class MaintenanceNotifierTests
{
    [Fact]
    public void ChangeStatus_ShouldRaiseStatusChangedEvent()
    {
        // Arrange
        var notifier = new MaintenanceNotifier();
        bool eventRaised = false;
        RequestStatus? receivedStatus = null;

        notifier.StatusChanged += (sender, args) =>
        {
            eventRaised = true;
            receivedStatus = args.NewStatus;
        };

        notifier.ChangeStatus(requestId: 1, newStatus: RequestStatus.Completed);

        // Assert
        Assert.True(eventRaised);
        Assert.Equal(RequestStatus.Completed, receivedStatus);
    }
}

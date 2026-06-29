using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using YokohamaMaintenanceSystem.Controllers;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Hubs;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;

namespace YokohamaMaintenanceSystem.Tests;

public class MaintenanceRequestControllerTests
{
    private readonly Mock<IMaintenanceRequestRepository> _mockRepo;
    private readonly Mock<ILogger<MaintenanceRequestsController>> _mockLogger;
    private readonly Mock<IHubContext<MaintenanceHub>> _mockHub;
    private readonly MaintenanceRequestsController _controller;

    public MaintenanceRequestControllerTests()
    {
        _mockRepo = new Mock<IMaintenanceRequestRepository>();
        _mockLogger = new Mock<ILogger<MaintenanceRequestsController>>();
        _mockHub = new Mock<IHubContext<MaintenanceHub>>();

        // setup Clients.All เพื่อไม่ให้ NullReferenceException
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        _mockHub.Setup(h => h.Clients).Returns(mockClients.Object);
        mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb").Options;
        var context = new AppDbContext(options);

        _controller = new MaintenanceRequestsController(
            _mockRepo.Object, context, _mockLogger.Object, _mockHub.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithList()
    {
        // Arrange
        var fakeList = new List<MaintenanceRequest>
        {
            new MaintenanceRequest { Id = 1, Title = "Fix Machine A", Description = "Test", Priority = "High" },
            new MaintenanceRequest { Id = 2, Title = "Check Conveyor", Description = "Test", Priority = "Low" }
        };

        _mockRepo.Setup(r => r.GetFilteredAsync(
            It.IsAny<string?>(),
            It.IsAny<RequestStatus?>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
        .ReturnsAsync(fakeList);

        // Act
        var result = await _controller.Index(null, null) as ViewResult;

        // Assert
        var model = Assert.IsType<PagedRequestViewModel>(result!.Model);
        Assert.Equal(2, model.Requests.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    public async Task Details_InvalidId_ReturnsNotFound(int invalidId)
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(invalidId))
                 .ReturnsAsync((MaintenanceRequest)null);

        // Act
        var result = await _controller.Details(invalidId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ValidRequest_RedirectsToIndex()
    {
        // Arrange
        var request = new MaintenanceRequest
        {
            Title = "Test",
            Description = "Test",
            Priority = "High"
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Index_ReturnsEmptyList_WhenNoRequests()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetFilteredAsync(
            It.IsAny<string?>(),
            It.IsAny<RequestStatus?>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
        .ReturnsAsync(new List<MaintenanceRequest>());

        // Act
        var result = await _controller.Index(null, null) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<PagedRequestViewModel>(result!.Model);
        Assert.Equal(0, model.Requests.Count);
    }

    [Fact]
    public async Task Index_FilterByStatus_ReturnsMatchingRecords()
    {
        // Arrange
        var pendingList = new List<MaintenanceRequest>
        {
            new MaintenanceRequest { Id = 1, Title = "Fix pump",
                Description = "Test", Priority = "High",
                Status = RequestStatus.Pending }
        };

        _mockRepo.Setup(r => r.GetFilteredAsync(
            It.IsAny<string?>(),
            It.IsAny<RequestStatus?>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
        .ReturnsAsync(pendingList);

        // Act
        var result = await _controller.Index(null, RequestStatus.Pending) as ViewResult;

        // Assert
        var model = Assert.IsType<PagedRequestViewModel>(result!.Model);
        Assert.Single(model.Requests);
    }

    [Fact]
    public async Task Index_FilterByKeyword_ReturnsMatchingRecords()
    {
        // Arrange
        var filtered = new List<MaintenanceRequest>
        {
            new MaintenanceRequest
            {
                Id = 2,
                Title = "Pump broken",
                Description = "Test",
                Priority = "High",
                Status = RequestStatus.Pending
            }
        };

        _mockRepo.Setup(r => r.GetFilteredAsync(
            "Pump",
            null,
            It.IsAny<int>(),
            It.IsAny<int>()))
        .ReturnsAsync(filtered);

        // Act
        var result = await _controller.Index("Pump", null) as ViewResult;

        // Assert
        var model = Assert.IsType<PagedRequestViewModel>(result!.Model);
        Assert.Single(model.Requests);
        Assert.Contains("Pump", model.Requests.First().Title);
    }
}

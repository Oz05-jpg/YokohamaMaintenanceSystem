using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using YokohamaMaintenanceSystem.Controllers;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.Enums;
using YokohamaMaintenanceSystem.Interfaces;
using YokohamaMaintenanceSystem.Models;
namespace YokohamaMaintenanceSystem.Tests;

public class MaintenanceRequestControllerTests
{
    private readonly Mock<IMaintenanceRequestRepository> _mockRepo;
    private readonly MaintenanceRequestsController _controller;

    public MaintenanceRequestControllerTests()
    {
        _mockRepo = new Mock<IMaintenanceRequestRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb").Options;
        var context = new AppDbContext(options);

        _controller = new MaintenanceRequestsController(_mockRepo.Object, context);
    }

    [Fact]
    public async Task Index_ReturnsViewWithList()
    {
        // Arrange คือ การเตรียมข้อมูล หรือ Mock up
        var fakeList = new List<MaintenanceRequest>
        {
            new MaintenanceRequest { Id = 1, Title = "Fix Machine A", Description = "Test", Priority = "High" },
            new MaintenanceRequest { Id = 2, Title = "Check Conveyor", Description = "Test", Priority = "Low" }
        };

        _mockRepo.Setup(r => r.GetFilteredAsync(
            It.IsAny<string?>(),
            It.IsAny<RequestStatus?>()))
        .ReturnsAsync(fakeList);

        // Act คือ ดึง method จริงมา มาใช้ Test
        var result = await _controller.Index(null, null) as ViewResult;

        // Assert คือ ตรวจสอบผลลัพธ์
        var model = Assert.IsAssignableFrom<IEnumerable<MaintenanceRequest>>(result!.Model);
        Assert.Equal(2, model.Count());
    }

    //[Fact]
    //public async Task Details_InvalidId_ReturnsNotFound()
    //{
    //    // Arrange
    //    _mockRepo.Setup(r => r.GetByIdAsync(99))
    //             .ReturnsAsync((MaintenanceRequest)null);

    //    // Act
    //    var result = await _controller.Details(99);

    //    // Assert
    //    Assert.IsType<NotFoundResult>(result);
    //}

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
    //Create
    public async Task Create_ValidRequest_RedirectsToIndex()
    {
        //Arrange
        var request = new MaintenanceRequest
        {
            Title = "Test",
            Description = "Test",
            Priority = "High"
        };
        // Act
        var result = await _controller.Create(request);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);//
        Assert.Equal("Index", redirect.ActionName);//
    }

    [Fact]
    public async Task Index_ReturnsEmptyList_WhenNoRequests()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetFilteredAsync(
        It.IsAny<string?>(), It.IsAny<RequestStatus?>()))
        .ReturnsAsync(new List<MaintenanceRequest>());

        // Act
        var result = await _controller.Index(null, null) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsAssignableFrom<IEnumerable<MaintenanceRequest>>(result!.Model);
        Assert.Equal(0, model.Count());
    }

    [Fact]
    public async Task Index_FilterByStatus_ReturnsMatchingRecords()
    {
        //arrange
        var pendingList = new List<MaintenanceRequest>
        {
            new MaintenanceRequest { Id = 1, Title = "Fix pump",
                Description = "Test", Priority = "High",
                Status = RequestStatus.Pending }
        };

        _mockRepo.Setup(r => r.GetFilteredAsync(
        It.IsAny<string?>(),
        It.IsAny<RequestStatus?>()))
        .ReturnsAsync(pendingList);

        // Act
        var result = await _controller.Index(null, "Pending") as ViewResult;


        // Assert
        var model = Assert.IsAssignableFrom<IEnumerable<MaintenanceRequest>>(result!.Model);
        Assert.Single(model);
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
                Status = YokohamaMaintenanceSystem.Enums.RequestStatus.Pending }
        };
        _mockRepo.Setup(r => r.GetFilteredAsync("Pump", null))
                 .ReturnsAsync(filtered);
        // Act
        var result = await _controller.Index("Pump", null) as ViewResult;

        // Assert
        var model = Assert.IsAssignableFrom<IEnumerable<MaintenanceRequest>>(result!.Model);
        Assert.Single(model);
        Assert.Contains("Pump", model.First().Title);
    }
}

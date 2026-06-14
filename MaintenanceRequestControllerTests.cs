using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using YokohamaMaintenanceSystem.Controllers;
using YokohamaMaintenanceSystem.Data;
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
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeList);

        // Act คือ ดึง method จริงมา มาใช้ Test
        var result = await _controller.Index() as ViewResult;

        // Assert คือ ตรวจสอบผลลัพธ์
        var model = Assert.IsAssignableFrom<IEnumerable<MaintenanceRequest>>(result!.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(99))
                 .ReturnsAsync((MaintenanceRequest)null);

        // Act
        var result = await _controller.Details(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

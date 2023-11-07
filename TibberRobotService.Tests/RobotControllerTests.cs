using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TibberRobotService.Controllers;
using TibberRobotService.Data;
using TibberRobotService.Models;
using System;
using Microsoft.EntityFrameworkCore;
using TibberRobotService.Services;
using Newtonsoft.Json;
using IntervalTree;

public class RobotControllerTests
{
    private readonly RobotController _controller;
    private readonly ApplicationDbContext _context;
    private readonly IRobotService _robotService = new RobotService();
    private readonly Mock<IRobotService> _robotServiceMock = new Mock<IRobotService>();
    
    public RobotControllerTests()
    {
        // Setup in-memory database context
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TibberRobotDbTest")
            .Options;
        
        _context = new ApplicationDbContext(options);
        
        // Ensure the database is created
        _context.Database.EnsureCreated();

        // Initialize controller with in-memory context
        _controller = new RobotController(_robotService, _context);
    }
    [Fact]
    public async Task Post_ValidPath_ReturnsUniquePlacesCleaned()
    {
        // Arrange
        var path = new PathInputModel
        {
            Start = new Coordinate { X = 10, Y = 22 },
            Commands = new List<Command> {
                new Command { Direction = "east", Steps = 2 },
                new Command { Direction = "north", Steps = 1 }
            }
        };

        // Act
        var result = await _controller.EnterPath(path);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var execution = Assert.IsType<Execution>(okResult.Value);
        Assert.Equal(4, execution.Result); 
    }

    [Fact]
    public async Task Post_ValidPath_SavesExecutionInDatabase()
    {
        // Arrange
        var path = new PathInputModel
        {
            Start = new Coordinate { X = 0, Y = 0 },
            Commands = new List<Command> {
                new Command { Direction = "east", Steps = 1 }
            }
        };

        // Act
        await _controller.EnterPath(path);

        // Assert
        var execution = await _context.Executions.FirstOrDefaultAsync(e => e.Result == 2);
        Assert.NotNull(execution);
    }

    [Fact]
    public async Task Post_IntersectingPath()
    {
        // Arrange
        var path = new PathInputModel
        {
            Start = new Coordinate { X = 0, Y = 0 },
            Commands = new List<Command> {
                new Command { Direction = "east", Steps = 10 },
                new Command { Direction = "south", Steps = 5 },
                new Command { Direction = "west", Steps = 5 },
                new Command { Direction = "north", Steps = 6 },
            }
        };

        // Act
        await _controller.EnterPath(path);

        // Assert
        var execution = await _context.Executions.FirstOrDefaultAsync(x => x.Result == 26);
    }

    [Fact]
    public async Task Post_HeavyInput()
    {
        var str = File.ReadAllText("../../../robotcleanerpathheavy.json");
        var path = JsonConvert.DeserializeObject<PathInputModel>(str);

        // Act
        var result = await _controller.EnterPath(path);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var execution = Assert.IsType<Execution>(okResult.Value);
        Assert.Equal(993737501, execution.Result); 
    }

    [Fact]
    public void CalculatePathAndClean_WithValidCommands_ShouldReturnCorrectResult()
    {
        // Arrange
        var startCommand = new PathInputModel
        {
            Start = new Coordinate { X = 0, Y = 0 },
            Commands = new List<Command>
            {
                new Command { Direction = "east", Steps = 2 },
                new Command { Direction = "north", Steps = 2 },
                new Command { Direction = "west", Steps = 2 },
                new Command { Direction = "south", Steps = 2 }
            }
        };

        // Act
        var result = _robotService.CalculatePathAndClean(startCommand);

        // Assert
        Assert.Equal(4, result.Commands);
        Assert.Equal(8, result.Result);
    }

    [Fact]
    public void CalculatePathAndClean_WithOverlappingCommands_ShouldReturnCorrectResult()
    {
        // Arrange
        var startCommand = new PathInputModel
        {
            Start = new Coordinate { X = 0, Y = 0 },
            Commands = new List<Command>
            {
                new Command { Direction = "east", Steps = 2 },
                new Command { Direction = "west", Steps = 2 }
            }
        };

        // Act
        var result = _robotService.CalculatePathAndClean(startCommand);

        // Assert
        Assert.Equal(2, result.Commands);
        Assert.Equal(3, result.Result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

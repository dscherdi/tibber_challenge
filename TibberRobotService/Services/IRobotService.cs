using TibberRobotService.Models;

namespace TibberRobotService.Services
{
    public interface IRobotService
    {
        Execution CalculatePathAndClean(PathInputModel startCommand);
    }
}

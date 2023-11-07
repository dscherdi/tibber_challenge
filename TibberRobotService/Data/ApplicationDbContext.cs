using Microsoft.EntityFrameworkCore;
using TibberRobotService.Models;

namespace TibberRobotService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Execution> Executions { get; set; }
    }
}

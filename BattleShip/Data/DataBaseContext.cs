using BattleShip.Data.Entities;
using BattleShip.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BattleShip.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

        public DbSet<GameStatistics> Statistics { get; set; }
    }
}

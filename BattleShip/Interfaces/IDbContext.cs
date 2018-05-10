using System;
using System.Data.Entity;

namespace BattleShip.Interfaces
{
    interface IDbContext : IDisposable
    {
        //NEKI INVERSION OF CONTROL MOREM NARET DA LAHKO KONTROLER TESTIRAM
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        int SaveChanges();
    }
}

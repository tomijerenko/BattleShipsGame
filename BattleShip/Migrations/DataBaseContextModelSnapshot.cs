using BattleShip.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace BattleShip.Migrations
{
    [DbContext(typeof(DataBaseContext))]
    partial class DataBaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BattleShip.Data.Entities.GameStatistics", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<TimeSpan>("LongestActiveGame");

                    b.Property<int>("TotalGamesPlayed");

                    b.Property<int>("TotalMissileHits");

                    b.Property<int>("TotalMissileMisses");

                    b.Property<int>("TotalMissileShoots");

                    b.Property<TimeSpan>("TotalTimePlayed");

                    b.HasKey("id");

                    b.ToTable("Statistics");
                });
#pragma warning restore 612, 618
        }
    }
}
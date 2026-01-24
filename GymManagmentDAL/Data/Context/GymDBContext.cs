using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Data.Context
{
    public class GymDBContext : IdentityDbContext<ApplicationUser>
    {
        public GymDBContext(DbContextOptions<GymDBContext> option): base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<ApplicationUser>(E =>
            {
                E.Property(x => x.FirstName).HasColumnType("nvarchar").HasMaxLength(50);
                E.Property(x => x.LastName).HasColumnType("nvarchar").HasMaxLength(50);

            }
                
            );
        }

        #region DBSet
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Plane> Planes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<MemberShip> MemberShips { get; set; }
        public DbSet<MemberSession> MemberSessions { get; set; }
        #endregion
    }
}

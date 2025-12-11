using GymManagmentDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Data.Configrations
{
    internal class PlaneConfigration : IEntityTypeConfiguration<Plane>
    {
        public void Configure(EntityTypeBuilder<Plane> builder)
        {
            builder.Property(x=> x.Name).HasColumnType("VarChar").HasMaxLength(50);
            builder.Property(x=> x.Description).HasColumnType("VarChar").HasMaxLength(100);
            builder.Property(x=> x.Price).HasPrecision(10,2);

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("PlaneDurationCheck", "DurationDays Between 1 and 365");
            });
        }
    }
}

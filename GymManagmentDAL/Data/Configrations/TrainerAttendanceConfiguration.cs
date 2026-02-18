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
    internal class TrainerAttendanceConfiguration : IEntityTypeConfiguration<TrainerAttendance>
    {
        public void Configure(EntityTypeBuilder<TrainerAttendance> builder)
        {
            builder.Property(x => x.TotalHours).HasDefaultValue(0);
            builder.Property(x => x.DelayMinutes).HasDefaultValue(0);
            builder.Property(x => x.OvertimeMinutes).HasDefaultValue(0);
            
            builder.HasOne(x => x.Trainer)
                   .WithMany(t => t.Attendances)
                   .HasForeignKey(x => x.TrainerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

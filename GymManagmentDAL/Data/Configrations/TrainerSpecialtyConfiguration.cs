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
    internal class TrainerSpecialtyConfiguration : IEntityTypeConfiguration<TrainerSpecialty>
    {
        public void Configure(EntityTypeBuilder<TrainerSpecialty> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(500);
            
            var initialDate = new DateTime(2024, 1, 1);
            builder.HasData(
                new TrainerSpecialty { Id = 1, Name = "General Fitness", CreatedAt = initialDate, UpdatedAt = initialDate },
                new TrainerSpecialty { Id = 2, Name = "Yoga", CreatedAt = initialDate, UpdatedAt = initialDate },
                new TrainerSpecialty { Id = 3, Name = "Boxing", CreatedAt = initialDate, UpdatedAt = initialDate },
                new TrainerSpecialty { Id = 4, Name = "CrossFit", CreatedAt = initialDate, UpdatedAt = initialDate },
                new TrainerSpecialty { Id = 5, Name = "تأهيل إصابات", CreatedAt = initialDate, UpdatedAt = initialDate },
                new TrainerSpecialty { Id = 6, Name = "تدليك", CreatedAt = initialDate, UpdatedAt = initialDate }
            );
        }
    }
}

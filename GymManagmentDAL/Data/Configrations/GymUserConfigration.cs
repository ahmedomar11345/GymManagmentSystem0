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
    internal class GymUserConfigration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name).HasColumnType("VarChar").HasMaxLength(50);
            builder.Property(x => x.Email).HasColumnType("VarChar").HasMaxLength(100);
            builder.Property(x=> x.Phone).HasColumnType("VarChar").HasMaxLength(11);

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("GymUserValidEmailCheck", "Email LIKE '_%@_%._%'");
                tb.HasCheckConstraint("GymUserValidPhoneCheck", "LEN(Phone) = 11 AND Phone Like '01%' AND Phone NOT LIKE '%[^0-9]%'");
            });

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();
            builder.OwnsOne(x=> x.Address , addressbuilder =>
            {
               addressbuilder.Property(a => a.Street).HasColumnType("VarChar").HasMaxLength(30).HasColumnName("Street");
                addressbuilder.Property(a => a.City).HasColumnType("VarChar").HasMaxLength(50).HasColumnName("City");
                addressbuilder.Property(a=> a.BuildingNumber).HasColumnName("BuildingNumber");
            });


        }
    }
}

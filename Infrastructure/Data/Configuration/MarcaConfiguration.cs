using CORE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
{
    public void Configure(EntityTypeBuilder<Marca> builder)
    {
        builder.ToTable("Marca");

        builder.Property(m => m.Id)
            .IsRequired();

        builder.Property(p => p.Nombre)
            .IsRequired().HasMaxLength(100);
    }
}

using CORE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Producto");

        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.Nombre)
            .IsRequired();

        builder.Property(p => p.Precio)
            .HasColumnType("decimal(18,2)");

        //Una marca tiene muchos productos
        builder.HasOne(p => p.Marca)
           .WithMany(m => m.Productos)
           .HasForeignKey(p => p.MarcaId);

        builder.HasOne(p => p.Categoria)
           .WithMany(c => c.Productos)
           .HasForeignKey(p => p.CategoriaId);
    }
}

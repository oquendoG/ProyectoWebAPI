using CORE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;
public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario");
        builder.Property(user => user.Id).IsRequired();
        builder.Property(user => user.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(user => user.ApellidoPaterno).IsRequired().HasMaxLength(200);
        builder.Property(user => user.UserName).IsRequired().HasMaxLength(200);
        builder.Property(user => user.Email).IsRequired().HasMaxLength(200);

        builder
            .HasMany(user => user.Roles)
            .WithMany(rol => rol.Usuarios)
            .UsingEntity<UsuariosRoles>(
                Ebuilder => Ebuilder
                            .HasOne(ur => ur.Rol)
                            .WithMany(rol => rol.UsuariosRoles)
                            .HasForeignKey(ur => ur.RolId),
                Ebuilder => Ebuilder
                            .HasOne(ur => ur.Usuario)
                            .WithMany(user => user.UsuariosRoles)
                            .HasForeignKey(ur => ur.UsuarioId),
                EBuilder =>
                {
                    EBuilder.HasKey(ur => new { ur.UsuarioId, ur.RolId });
                }
            );

        builder.HasMany(user => user.RefreshTokens)
            .WithOne(rtk => rtk.Usuario)
            .HasForeignKey(rtk => rtk.UsuarioId);
    }
}

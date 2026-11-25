using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTaskManager.Domain.Entities;

namespace ProjectTaskManager.Infrastructure.Persistence.Configurations;

public class TareaConfiguration : IEntityTypeConfiguration<Tarea>
{
    public void Configure(EntityTypeBuilder<Tarea> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Descripcion).HasMaxLength(1000);
        builder.Property(t => t.Estado).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Prioridad).HasConversion<string>().HasMaxLength(20);
    }
}
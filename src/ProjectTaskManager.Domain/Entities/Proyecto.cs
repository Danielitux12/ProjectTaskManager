namespace ProjectTaskManager.Domain.Entities;

public class Proyecto
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    // Relación 1:N
    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}

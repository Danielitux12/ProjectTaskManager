using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.Application.DTOs;
using ProjectTaskManager.Application.Interfaces;

namespace ProjectTaskManager.API.Controllers;

/// <summary>
/// Gestión de proyectos.
/// </summary>
[ApiController]
[Route("api/v1/proyectos")]
public class ProyectosController(IProyectoService proyectoService, ITareaService tareaService) : ControllerBase
{
    /// <summary>Obtiene todos los proyectos.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProyectoDto>>> ObtenerTodos()
        => Ok(await proyectoService.ObtenerTodosAsync());

    /// <summary>Obtiene un proyecto por su id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProyectoDto>> ObtenerPorId(Guid id)
        => Ok(await proyectoService.ObtenerPorIdAsync(id));

    /// <summary>Crea un nuevo proyecto.</summary>
    [HttpPost]
    public async Task<ActionResult<ProyectoDto>> Crear([FromBody] CrearProyectoDto dto)
    {
        var creado = await proyectoService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    /// <summary>Actualiza un proyecto existente.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] CrearProyectoDto dto)
    {
        await proyectoService.ActualizarAsync(id, dto);
        return NoContent();
    }

    /// <summary>Elimina un proyecto. Falla si tiene tareas sin completar.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await proyectoService.EliminarAsync(id);
        return NoContent();
    }

    /// <summary>Lista las tareas de un proyecto.</summary>
    [HttpGet("{id:guid}/tareas")]
    public async Task<ActionResult<IEnumerable<TareaDto>>> ObtenerTareas(Guid id)
        => Ok(await tareaService.ObtenerPorProyectoAsync(id));

    /// <summary>Calcula el porcentaje de avance del proyecto según sus tareas completadas.</summary>
    [HttpGet("{id:guid}/avance")]
    public async Task<ActionResult<decimal>> ObtenerAvance(Guid id)
        => Ok(await proyectoService.CalcularAvanceAsync(id));
}
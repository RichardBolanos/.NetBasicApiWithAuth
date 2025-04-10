using GestionTareasApi.Data;
using GestionTareasApi.Dtos;
using GestionTareasApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionTareasApi.Controllers
{
    [Authorize]
    [Route("api/v1/tareas")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> Get()
        {
            return await _context.Tareas.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Post(TareaDto tareaDto)
        {
            var tarea = new Tarea { Titulo = tareaDto.Titulo, Descripcion = tareaDto.Descripcion, Completada = false };
            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = tarea.Id }, tarea);
        }
    }

}

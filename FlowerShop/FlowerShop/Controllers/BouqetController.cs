using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcFlowers.Data;
using MvcFlowers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcFlowers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BouqetController : ControllerBase
    {
        private readonly MvcFlowersContext _context;

        public BouqetController(MvcFlowersContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Bouqet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bouqet>>> GetBouquets()
        {
            var bouquets = await _context.Bouqet.Include(b => b.Flowers).ToListAsync();
            return Ok(bouquets);
        }

        // GET: api/Bouqet/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBouquet(int id)
        {
            var bouquet = await _context.Bouqet
                .Include(b => b.Flowers) // Загрузите связанные данные
                    .ThenInclude(fb => fb.Flower) // Загрузите цветы в букете
                .FirstOrDefaultAsync(b => b.BouqetId == id);

            if (bouquet == null)
            {
                return NotFound();
            }

            // Возвращаем данные букета, включая цветы
            return Ok(bouquet);
        }


        // POST: api/Bouqet
        [HttpPost]
        public async Task<ActionResult<Bouqet>> CreateBouquet([FromBody] List<FlowerSelection> selectedFlowers)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Создаем букет с помощью метода менеджера
                    var newBouquet = await Manager.CreateBouquet(_context, selectedFlowers);

                    // Устанавливаем значение SelectedFlowerIds
                    newBouquet.SelectedFlowerIds = string.Join(",", selectedFlowers.Select(sf => sf.FlowerId));

                    // Добавляем букет в контекст и сохраняем изменения
                    _context.Bouqet.Add(newBouquet);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetBouquet), new { id = newBouquet.BouqetId }, newBouquet);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return BadRequest(ModelState);
        }



        // PUT: api/Bouqet/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBouquet(int id, [FromBody] Bouqet bouqet)
        {
            if (id != bouqet.BouqetId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Обновляем букет с помощью метода менеджера
                    var updatedBouquet = await Manager.EditBouquet(_context, bouqet, bouqet.Flowers.Select(f => new FlowerSelection { FlowerId = f.FlowerId, Count = f.Count }).ToList());

                    // Обновление букета в контексте
                    _context.Update(updatedBouquet);
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BouqetExists(bouqet.BouqetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return BadRequest(ModelState);
        }

        // DELETE: api/Bouqet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBouquet(int id)
        {
            try
            {
                await Manager.DeleteBouquet(_context, id);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        private bool BouqetExists(int id)
        {
            return _context.Bouqet.Any(e => e.BouqetId == id);
        }
    }
}

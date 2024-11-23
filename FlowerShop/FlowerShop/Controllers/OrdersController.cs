using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcFlowers.Data;
using MvcFlowers.Models;

namespace MvcFlowers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MvcFlowersContext _context;

        public OrdersController(MvcFlowersContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    FullName = o.FullName,
                    Phone = o.Phone,
                    Address = o.Address,
                    BouqetId = o.BouqetId,
                    TotalPrice = _context.Bouqet
                        .Where(b => b.BouqetId == o.BouqetId)
                        .Select(b => b.CalculateTotalPrice())
                        .FirstOrDefault() // Вычисляем стоимость букета по BouqetId
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null || order.BouqetId <= 0)
            {
                return BadRequest("Order or BouqetId is invalid.");
            }

            if (ModelState.IsValid)
            {
                // Проверьте, существует ли букет
                var bouqet = await _context.Bouqet.FindAsync(order.BouqetId);
                if (bouqet == null)
                {
                    return NotFound("Bouqet not found.");
                }

                // Добавьте заказ в контекст
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
            }

            return BadRequest(ModelState);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Entry(order).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return NoContent();
            }

            return BadRequest(ModelState);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}

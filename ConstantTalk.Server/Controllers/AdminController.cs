using ConstantTalk.Server.Data;
using ConstantTalk.Server.Models;
using ConstantTalk.Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ConstantTalk.Server.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Subscribers
                .Select(s => new
                {
                    s.Id,
                    //s.Auth0Id,
                    s.Name,
                    s.Email,
                    s.PhoneNumber,
                    s.IsBlocked
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("unpaid-bills")]
        public async Task<IActionResult> GetUnpaidBills()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var unpaidBills = await _context.Bills
                .Where(b => !b.IsPaid)
                .Include(b => b.Subscriber)
                .Select(static b => new
                {
                    b.Id,
                    b.Amount,
                    b.Description,
                    Subscriber = new
                    {
                        b.Subscriber.Id,
                        b.Subscriber.Name,
                        b.Subscriber.Email
                    }
                })
                .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return Ok(unpaidBills);
        }

        [HttpPost("block/{id}")]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            var user = await _context.Subscribers.FindAsync(id);
            if (user == null) return NotFound();

            user.IsBlocked = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("unblock/{id}")]
        public async Task<IActionResult> UnblockUser(Guid id)
        {
            var user = await _context.Subscribers.FindAsync(id);
            if (user == null) return NotFound();

            user.IsBlocked = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
            var user = new Subscriber
            {
                Auth0Id = request.Auth0Id,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            _context.Subscribers.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id });
        }

        [HttpPost("add-service")]
        public async Task<IActionResult> AddService([FromBody] AddServiceRequest request)
        {
            var service = new Service
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return Ok(new { service.Id });
        }

        [HttpDelete("delete-service/{serviceId:guid}")]
        public async Task<IActionResult> DeleteService(Guid serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                return NotFound("Service not found");

            var isUsed = await _context.SubscriberServices.AnyAsync(ss => ss.ServiceId == serviceId);
            if (isUsed)
                return BadRequest("Cannot delete service that is currently in use by subscribers");

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

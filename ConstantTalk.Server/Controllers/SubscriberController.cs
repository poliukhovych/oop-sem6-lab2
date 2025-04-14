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
    [Route("api/subscriber")]
    [Authorize(Policy = "SubscriberOnly")]
    public class SubscriberController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscriberController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetAuth0Id() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task<Subscriber?> GetCurrentSubscriberAsync()
        {
            var auth0Id = GetAuth0Id();
            //Console.WriteLine("Auth0Id: {0}", auth0Id);
            if (auth0Id == null) return null;
            return await _context.Subscribers
                .Include(s => s.SubscriberServices)
                .Include(s => s.Bills)
                .FirstOrDefaultAsync(s => s.Auth0Id == auth0Id);
        }

        [HttpGet("services/available")]
        public async Task<IActionResult> GetAvailableServices()
        {
            var subscriber = await GetCurrentSubscriberAsync();
            if (subscriber == null) return Unauthorized();

            var usedServiceIds = subscriber.SubscriberServices?.Select(ss => ss.ServiceId).ToHashSet() ?? new();

            var availableServices = await _context.Services
                .Where(s => !usedServiceIds.Contains(s.Id))
                .ToListAsync();

            return Ok(availableServices);
        }

        [HttpGet("services/my")]
        public async Task<IActionResult> GetMyServices()
        {
            var subscriber = await GetCurrentSubscriberAsync();
            if (subscriber == null) return Unauthorized();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8601 // Possible null reference assignment.
            var services = await _context.SubscriberServices
                .Where(ss => ss.SubscriberId == subscriber.Id)
                .Include(ss => ss.Service)
                .Select(ss => new ServiceDto
                {
                    Id = ss.Service.Id,
                    Name = ss.Service.Name,
                    Description = ss.Service.Description,
                    Price = ss.Service.Price
                })
                .ToListAsync();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return Ok(services);
        }

        [HttpPost("services/{serviceId:guid}/add")]
        public async Task<IActionResult> AddService(Guid serviceId)
        {
            var subscriber = await GetCurrentSubscriberAsync();
            if (subscriber == null) return Unauthorized();

            if (subscriber.IsBlocked)
            {
                return Problem("You are blocked and cannot add new services.", statusCode: 403);
            }

            if (await _context.SubscriberServices.AnyAsync(ss => ss.SubscriberId == subscriber.Id && ss.ServiceId == serviceId))
            {
                return BadRequest("Service already added");
            }

            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                return NotFound("Service not found");

            var newSubscriberService = new SubscriberService
            {
                SubscriberId = subscriber.Id,
                ServiceId = serviceId
            };

            _context.SubscriberServices.Add(newSubscriberService);

            var bill = new Bill
            {
                SubscriberId = subscriber.Id,
                Amount = service.Price,
                IsPaid = false,
                Description = $"Service activation: {service.Name}",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("bills")]
        public async Task<IActionResult> GetMyBills()
        {
            var subscriber = await GetCurrentSubscriberAsync();
            if (subscriber == null) return Unauthorized();

#pragma warning disable CS8601 // Possible null reference assignment.
            var bills = await _context.Bills
                .Where(b => b.SubscriberId == subscriber.Id)
                .Select(b => new BillDto
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    IsPaid = b.IsPaid,
                    DueDate = b.DueDate,
                    Description = b.Description
                })
                .ToListAsync();
#pragma warning restore CS8601 // Possible null reference assignment.

            return Ok(bills);
        }

        [HttpPost("bills/{billId:guid}/pay")]
        public async Task<IActionResult> PayBill(Guid billId)
        {
            var subscriber = await GetCurrentSubscriberAsync();
            if (subscriber == null) return Unauthorized();

            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == billId && b.SubscriberId == subscriber.Id);
            if (bill == null) return NotFound();
            if (bill.IsPaid) return BadRequest("Bill already paid");

            bill.IsPaid = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

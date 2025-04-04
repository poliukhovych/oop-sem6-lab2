using ConstantTalk.Server.Data;
using ConstantTalk.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ConstantTalk.Server.Controllers
{
    [ApiController]
    [Route("api/subscriber")]
    //[Authorize(Roles = "subscriber")]
    public class SubscriberController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscriberController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class SubscriberDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public bool IsBlocked { get; set; }
        }

        private string? GetAuth0Id() => User.FindFirstValue("sub");

        private async Task<Subscriber?> GetCurrentSubscriberAsync()
        {
            var auth0Id = GetAuth0Id();
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

            var services = await _context.SubscriberServices
                .Where(ss => ss.SubscriberId == subscriber.Id)
                .Include(ss => ss.Service)
                .Select(ss => ss.Service)
                .ToListAsync();

            return Ok(services);
        }

        [HttpPost("services/{serviceId:guid}/add")]
        public async Task<IActionResult> AddService(Guid serviceId)
        {
            var subscriber = await GetCurrentSubscriberAsync();
            if (subscriber == null) return Unauthorized();

            if (await _context.SubscriberServices.AnyAsync(ss => ss.SubscriberId == subscriber.Id && ss.ServiceId == serviceId))
            {
                return BadRequest("Service already added");
            }

            var newSubscriberService = new SubscriberService
            {
                SubscriberId = subscriber.Id,
                ServiceId = serviceId
            };

            _context.SubscriberServices.Add(newSubscriberService);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("bills")]
        public async Task<IActionResult> GetMyBills()
        {
            var subscriber = await GetCurrentSubscriberAsync();
            if (subscriber == null) return Unauthorized();

            var bills = await _context.Bills
                .Where(b => b.SubscriberId == subscriber.Id)
                .ToListAsync();

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

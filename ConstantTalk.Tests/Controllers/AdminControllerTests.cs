using ConstantTalk.Server.Controllers;
using ConstantTalk.Server.Data;
using ConstantTalk.Server.Models;
using ConstantTalk.Server.Models.DTOs;
using ConstantTalk.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace ConstantTalk.Tests.Controllers
{
    public class AdminControllerTests
    {
        private AdminController GetController(ApplicationDbContext context)
        {
            return new AdminController(context);
        }

        [Fact]
        public async Task GetUnpaidBills_ReturnsOnlyUnpaid()
        {
            var context = TestHelpers.GetInMemoryDbContext();

            var subscriber = new Subscriber { Id = Guid.NewGuid(), Name = "User1", Auth0Id = "auth0|test" };
            var paidBill = new Bill { Id = Guid.NewGuid(), SubscriberId = subscriber.Id, Amount = 100, IsPaid = true };
            var unpaidBill = new Bill { Id = Guid.NewGuid(), SubscriberId = subscriber.Id, Amount = 200, IsPaid = false };

            await context.Subscribers.AddAsync(subscriber);
            await context.Bills.AddRangeAsync(paidBill, unpaidBill);
            await context.SaveChangesAsync();

            var controller = GetController(context);
            var result = await controller.GetUnpaidBills() as OkObjectResult;

            Assert.NotNull(result);

            var json = JsonSerializer.Serialize(result.Value);
            var list = JsonSerializer.Deserialize<List<JsonElement>>(json);

            Assert.Single(list);

            var bill = list[0];
            var amount = bill.GetProperty("Amount").GetDecimal();
            var subscriberJson = bill.GetProperty("Subscriber");

            Assert.Equal(200, amount);
            Assert.Equal(subscriber.Id, subscriberJson.GetProperty("Id").GetGuid());
        }

        [Fact]
        public async Task BlockSubscriber_SetsIsBlockedTrue()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Name = "John", IsBlocked = false };

            await context.Subscribers.AddAsync(subscriber);
            await context.SaveChangesAsync();

            var controller = GetController(context);
            var result = await controller.BlockUser(subscriber.Id);

            Assert.IsType<NoContentResult>(result);
            var updated = await context.Subscribers.FindAsync(subscriber.Id);
            Assert.True(updated!.IsBlocked);
        }

        [Fact]
        public async Task UnblockSubscriber_SetsIsBlockedFalse()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Name = "Jane", IsBlocked = true };

            await context.Subscribers.AddAsync(subscriber);
            await context.SaveChangesAsync();

            var controller = GetController(context);
            var result = await controller.UnblockUser(subscriber.Id);

            Assert.IsType<NoContentResult>(result);
            var updated = await context.Subscribers.FindAsync(subscriber.Id);
            Assert.False(updated!.IsBlocked);
        }

        [Fact]
        public async Task AddSubscriber_CreatesNewSubscriber()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var controller = GetController(context);

            var request = new AddUserRequest
            {
                Name = "Alice",
                Email = "alice@example.com",
                PhoneNumber = "+380501234567"
            };

            var result = await controller.AddUser(request) as OkObjectResult;

            Assert.NotNull(result);

            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(result.Value));
            
            var id = dict["Id"].GetGuid();

            Assert.NotEqual(Guid.Empty, id);

            var user = await context.Subscribers.FindAsync(id);
            Assert.NotNull(user);
            Assert.Equal("Alice", user.Name);
        }


        [Fact]
        public async Task BlockSubscriber_ReturnsNotFound_IfInvalidId()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var controller = GetController(context);

            var result = await controller.BlockUser(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UnblockSubscriber_ReturnsNotFound_IfInvalidId()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var controller = GetController(context);

            var result = await controller.UnblockUser(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}

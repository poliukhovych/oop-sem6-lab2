using ConstantTalk.Server.Controllers;
using ConstantTalk.Server.Data;
using ConstantTalk.Server.Models;
using ConstantTalk.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ConstantTalk.Tests.Controllers
{
    public class SubscriberControllerTests
    {
        private SubscriberController GetControllerWithUser(ApplicationDbContext context, string auth0Id)
        {
            var controller = new SubscriberController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = TestHelpers.GetHttpContextWithUser(auth0Id)
            };
            return controller;
        }

        [Fact]
        public async Task GetAvailableServices_ReturnsAvailableServices()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var auth0Id = "auth0|123";
            var subscriber = new Subscriber
            {
                Id = Guid.NewGuid(),
                Auth0Id = auth0Id
            };
            var service1 = new Service { Id = Guid.NewGuid(), Name = "S1", Price = 10, Description = "" };
            var service2 = new Service { Id = Guid.NewGuid(), Name = "S2", Price = 20, Description = "" };

            await context.Subscribers.AddAsync(subscriber);
            await context.Services.AddRangeAsync(service1, service2);
            await context.SubscriberServices.AddAsync(new SubscriberService { SubscriberId = subscriber.Id, ServiceId = service1.Id });
            await context.SaveChangesAsync();

            var controller = GetControllerWithUser(context, auth0Id);
            var result = await controller.GetAvailableServices() as OkObjectResult;

            var returnedServices = Assert.IsType<List<Service>>(result!.Value);
            Assert.Single(returnedServices);
            Assert.Equal(service2.Id, returnedServices[0].Id);
        }

        [Fact]
        public async Task GetMyServices_ReturnsOnlySubscribed()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var auth0Id = "auth0|xyz";
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Auth0Id = auth0Id };
            var service1 = new Service { Id = Guid.NewGuid(), Name = "A" };
            var service2 = new Service { Id = Guid.NewGuid(), Name = "B" };

            await context.Subscribers.AddAsync(subscriber);
            await context.Services.AddRangeAsync(service1, service2);
            await context.SubscriberServices.AddAsync(new SubscriberService { SubscriberId = subscriber.Id, ServiceId = service1.Id });
            await context.SaveChangesAsync();

            var controller = GetControllerWithUser(context, auth0Id);
            var result = await controller.GetMyServices() as OkObjectResult;

            var services = Assert.IsType<List<Service>>(result!.Value);
            Assert.Single(services);
            Assert.Equal("A", services[0].Name);
        }

        [Fact]
        public async Task AddService_WorksCorrectly_IfNotAlreadyAdded()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var auth0Id = "auth0|abc";
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Auth0Id = auth0Id };
            var service = new Service { Id = Guid.NewGuid(), Name = "Premium" };

            await context.Subscribers.AddAsync(subscriber);
            await context.Services.AddAsync(service);
            await context.SaveChangesAsync();

            var controller = GetControllerWithUser(context, auth0Id);
            var result = await controller.AddService(service.Id);

            Assert.IsType<OkResult>(result);
            Assert.True(await context.SubscriberServices.AnyAsync(ss => ss.SubscriberId == subscriber.Id && ss.ServiceId == service.Id));
        }

        [Fact]
        public async Task AddService_ReturnsBadRequest_IfAlreadyAdded()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var auth0Id = "auth0|def";
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Auth0Id = auth0Id };
            var service = new Service { Id = Guid.NewGuid(), Name = "Basic" };

            await context.Subscribers.AddAsync(subscriber);
            await context.Services.AddAsync(service);
            await context.SubscriberServices.AddAsync(new SubscriberService { SubscriberId = subscriber.Id, ServiceId = service.Id });
            await context.SaveChangesAsync();

            var controller = GetControllerWithUser(context, auth0Id);
            var result = await controller.AddService(service.Id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Service already added", badRequest.Value);
        }

        [Fact]
        public async Task GetMyBills_ReturnsSubscriberBills()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var auth0Id = "auth0|bills";
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Auth0Id = auth0Id };
            var bill1 = new Bill { Id = Guid.NewGuid(), SubscriberId = subscriber.Id, Amount = 100, DueDate = DateTime.Now };
            var bill2 = new Bill { Id = Guid.NewGuid(), SubscriberId = subscriber.Id, Amount = 200, DueDate = DateTime.Now };

            await context.Subscribers.AddAsync(subscriber);
            await context.Bills.AddRangeAsync(bill1, bill2);
            await context.SaveChangesAsync();

            var controller = GetControllerWithUser(context, auth0Id);
            var result = await controller.GetMyBills() as OkObjectResult;

            var bills = Assert.IsType<List<Bill>>(result!.Value);
            Assert.Equal(2, bills.Count);
        }

        [Fact]
        public async Task PayBill_Succeeds_IfValid()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var auth0Id = "auth0|pay";
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Auth0Id = auth0Id };
            var bill = new Bill { Id = Guid.NewGuid(), SubscriberId = subscriber.Id, Amount = 50, DueDate = DateTime.Now, IsPaid = false };

            await context.Subscribers.AddAsync(subscriber);
            await context.Bills.AddAsync(bill);
            await context.SaveChangesAsync();

            var controller = GetControllerWithUser(context, auth0Id);
            var result = await controller.PayBill(bill.Id);

            Assert.IsType<OkResult>(result);
            Assert.True((await context.Bills.FindAsync(bill.Id))!.IsPaid);
        }

        [Fact]
        public async Task PayBill_ReturnsNotFound_IfInvalidId()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var auth0Id = "auth0|notfound";
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Auth0Id = auth0Id };
            await context.Subscribers.AddAsync(subscriber);
            await context.SaveChangesAsync();

            var controller = GetControllerWithUser(context, auth0Id);
            var result = await controller.PayBill(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PayBill_ReturnsBadRequest_IfAlreadyPaid()
        {
            var context = TestHelpers.GetInMemoryDbContext();
            var auth0Id = "auth0|paid";
            var subscriber = new Subscriber { Id = Guid.NewGuid(), Auth0Id = auth0Id };
            var bill = new Bill { Id = Guid.NewGuid(), SubscriberId = subscriber.Id, Amount = 30, DueDate = DateTime.Now, IsPaid = true };

            await context.Subscribers.AddAsync(subscriber);
            await context.Bills.AddAsync(bill);
            await context.SaveChangesAsync();

            var controller = GetControllerWithUser(context, auth0Id);
            var result = await controller.PayBill(bill.Id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bill already paid", badRequest.Value);
        }
    }
}

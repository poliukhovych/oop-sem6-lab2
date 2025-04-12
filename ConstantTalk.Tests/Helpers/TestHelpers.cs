using ConstantTalk.Server.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ConstantTalk.Tests.Helpers
{
    public static class TestHelpers
    {
        public static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        public static ClaimsPrincipal GetFakeUser(string auth0Id)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", auth0Id)
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));
        }

        public static DefaultHttpContext GetHttpContextWithUser(string auth0Id)
        {
            return new DefaultHttpContext
            {
                User = GetFakeUser(auth0Id)
            };
        }
    }
}

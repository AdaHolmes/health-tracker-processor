﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthTrackerProcessor.Models;

namespace HealthTrackerProcessor.Controllers
{
    public abstract class ConfigDbContextController : Controller
    {
        protected readonly ConfigContext _configDbContext = new ConfigContext(new DbContextOptions<ConfigContext>());

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_configDbContext != null)
            {
                _configDbContext.Dispose();
            }
        }
    }
}

using CacheingDemo.Models;
using CacheingDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheingDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ICacheService cacheService;
        private readonly DataContext dataContext;

        public DriversController(ICacheService cacheService, DataContext dataContext)
        {
            this.cacheService = cacheService;
            this.dataContext = dataContext;
        }

        [HttpGet("drivers")]
        public async Task<IActionResult> Get()
        {
            var cachedDrivers = cacheService.GetData<IEnumerable<Driver>>("drivers");

            if(cachedDrivers == null || cachedDrivers.Count() < 1) 
            {
                var drivers = await dataContext.Drivers.ToListAsync();

                var expTime = DateTime.Now.AddMinutes(2);
                cacheService.SetData<IEnumerable<Driver>>("drivers", drivers, expTime);

                return Ok(drivers);
            }
            return Ok(cachedDrivers);
        }

        [HttpGet("drivers/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cachedDrivers = cacheService.GetData<IEnumerable<Driver>>("drivers");
            var driver = new Driver();
            if (cachedDrivers == null || cachedDrivers.Count() < 1)
            {
                driver = await dataContext.Drivers.FindAsync(id);
                return Ok(driver);
            }

            driver = cachedDrivers.FirstOrDefault(x=> x.Id == id);
            return Ok(driver);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DriverDto driver)
        {
            var newDriver = new Driver
            {
                Name = driver.Name,
                Team =driver.Team,
                RacingNumber = driver.RacingNumber
            };
            await dataContext.Drivers.AddAsync(newDriver);
            await dataContext.SaveChangesAsync();

            return Ok(driver);
        }
    }
}

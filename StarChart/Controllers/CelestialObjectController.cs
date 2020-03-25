using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name ="GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);
            if (!celestialObjects.Any())
                return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute(
                "GetById",
                new { id = celestialObject.Id },
                celestialObject);
        }

        [HttpPut("{id}"]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var toUpdate = _context.CelestialObjects.Find(id);

            if (toUpdate == null)
                return NotFound();

            toUpdate.Name = celestialObject.Name;
            toUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            toUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(toUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult Rename(int id, string name)
        {
            var toRename = _context.CelestialObjects.Find(id);

            if (toRename == null)
                return NotFound();

            toRename.Name = name;

            _context.CelestialObjects.Update(toRename);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var toDelete = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id);

            if (!toDelete.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(toDelete);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

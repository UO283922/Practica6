using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Amigos.DataAccessLayer;
using Amigos.Models;

namespace Amigos.Controllers
{
    public class AmigoController : Controller
    {
        private readonly AmigoDBContext _context;

        public AmigoController(AmigoDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? latitud, string? longitud, double? distanciaMaxKm)
        {
            var amigos = from a in _context.Amigos select a;

            if (!string.IsNullOrEmpty(latitud) && !string.IsNullOrEmpty(longitud) && distanciaMaxKm.HasValue)
            {
                double latRef = double.Parse(latitud, System.Globalization.CultureInfo.InvariantCulture);
                double lonRef = double.Parse(longitud, System.Globalization.CultureInfo.InvariantCulture);

                var todos = await amigos.ToListAsync();
                var filtrados = todos.Where(a =>
                {
                    if (double.TryParse(a.lati, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out double aLat) &&
                        double.TryParse(a.longi, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out double aLon))
                    {
                        double distancia = Distancia(latRef, lonRef, aLat, aLon);
                        return distancia <= distanciaMaxKm.Value;
                    }
                    return false;
                }).ToList();

                return View(new AmigoDistanciaViewModel
                {
                    Amigos = filtrados,
                    Latitud = latitud,
                    Longitud = longitud,
                    DistanciaMaxKm = distanciaMaxKm
                });
            }

            return View(new AmigoDistanciaViewModel
            {
                Amigos = await amigos.ToListAsync()
            });
        }

        [HttpGet("amigo/json/{id}")]
        public string GetJson(int id)
        {
            var amigo = _context.Amigos!.Find(id);

            if (amigo == null)
                return "{}";

            return System.Text.Json.JsonSerializer.Serialize(amigo);
        }

        // Fórmula de Haversine para calcular distancia entre dos coordenadas en km
        private static double Distancia(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radio de la Tierra en km
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        /*
        // GET: Amigo
        public async Task<IActionResult> Index()
        {
              return _context.Amigos != null ? 
                          View(await _context.Amigos.ToListAsync()) :
                          Problem("Entity set 'AmigoDBContext.Amigos'  is null.");
        }
        */

        // GET: Amigo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Amigos == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos
                .FirstOrDefaultAsync(m => m.ID == id);
            if (amigo == null)
            {
                return NotFound();
            }

            return View(amigo);
        }

        // GET: Amigo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Amigo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,name,longi,lati")] Amigo amigo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(amigo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(amigo);
        }

        // GET: Amigo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Amigos == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos.FindAsync(id);
            if (amigo == null)
            {
                return NotFound();
            }
            return View(amigo);
        }

        // POST: Amigo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,name,longi,lati")] Amigo amigo)
        {
            if (id != amigo.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(amigo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmigoExists(amigo.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(amigo);
        }

        // GET: Amigo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Amigos == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos
                .FirstOrDefaultAsync(m => m.ID == id);
            if (amigo == null)
            {
                return NotFound();
            }

            return View(amigo);
        }

        // POST: Amigo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Amigos == null)
            {
                return Problem("Entity set 'AmigoDBContext.Amigos'  is null.");
            }
            var amigo = await _context.Amigos.FindAsync(id);
            if (amigo != null)
            {
                _context.Amigos.Remove(amigo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AmigoExists(int id)
        {
          return (_context.Amigos?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}

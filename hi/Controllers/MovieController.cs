using Jquery_Ajax.DbContent;
using Jquery_Ajax.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jquery_Ajax.Controllers
{
    public class MovieController : Controller
    {
        private readonly MovieDbContext _context;

        public MovieController(MovieDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {

            return View(await _context.Movies.ToListAsync());
        }


        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Movie());
            else
            {
                var MovieModel = await _context.Movies.FindAsync(id);
                if (MovieModel == null)
                {
                    return NotFound();
                }
                return View(MovieModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (id == 0)
                {
                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                }
                //Update
                else
                {
                    try
                    {
                        _context.Update(movie);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MovieModelExists(movie.Id))
                        { return NotFound(); }
                        else
                        { throw; }
                    }
                }
                return Json(new { isValid = true, html = await Helper.RenderRazorViewToStringAsync(this, "_ViewAll", _context.Movies.ToList()) });
            }
            return Json(new { isValid = false, html = await Helper.RenderRazorViewToStringAsync(this, "AddOrEdit", movie) });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return Json(new { html = await Helper.RenderRazorViewToStringAsync(this, "_ViewAll", _context.Movies.ToList()) });
        }


        private bool MovieModelExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        public IActionResult GetSearchData(string SearchBy, string SearchValue)
        {
            var movies = from m in _context.Movies select m;
            IEnumerable<Movie> movie = new List<Movie>();
            if (SearchBy == "ID")
            {
                try
                {
                    int Id = Convert.ToInt32(SearchValue);
                    movie = movies.Where(x => x.Id == Id || SearchValue == null).ToList();
                }
                catch (FormatException)
                {
                    Console.WriteLine("{0} Is Not A ID ", SearchValue);
                }
                //return View(movie);
                return Json(movie);
            }
            else
            {
                movie = movies.Where(x => x.Genre.StartsWith(SearchValue) || SearchValue == null).ToList();
            }
            return Json(movies);
        }
    }
}


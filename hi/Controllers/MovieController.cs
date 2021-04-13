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
        public async Task<IActionResult> Index(string SearchBy, string SearchValue)
        {

            return View(await _context.Movies.ToListAsync());
        }

        private async Task<MovieViewModel> PrepareDropDownViewModelAsync(string movieGenre)
        {

            IQueryable<string> genreQuery = from m in _context.Movies orderby m.Genre select m.Genre;

            var movies = from m in _context.Movies
                         select m;

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }
            var model = new MovieViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Movies = await movies.ToListAsync()
            };
            return model;
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Movie());
            else
            {
                var transactionModel = await _context.Movies.FindAsync(id);
                if (transactionModel == null)
                {
                    return NotFound();
                }
                return View(transactionModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
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
                        if (!TransactionModelExists(movie.Id))
                        { return NotFound(); }
                        else
                        { throw; }
                    }
                }
                return Json(new { isValid = true, html = await Helper.RenderRazorViewToStringAsync(this, "_ViewAll", _context.Movies.ToList()) });
            }
            return Json(new { isValid = false, html = await Helper.RenderRazorViewToStringAsync(this, "AddOrEdit", movie) });
        }

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionModel = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transactionModel == null)
            {
                return NotFound();
            }

            return View(transactionModel);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactionModel = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(transactionModel);
            await _context.SaveChangesAsync();
            return Json(new { html = await Helper.RenderRazorViewToStringAsync(this, "_ViewAll", _context.Movies.ToList()) });
        }

        private bool TransactionModelExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
        public IActionResult GetSelect()
        {
            var productsList = (from product in _context.Movies
                                select new SelectListItem()
                                {
                                    Text = product.Genre,
                                    Value = product.Id.ToString(),
                                }).ToList();

            productsList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = string.Empty
            });

            return Json(productsList);
        }
        //form vì trả vê action index 

        public async Task<IActionResult> GetSearchDataAsync(string SearchBy, string SearchValue)
        {
            var movies = await _context.Movies.ToListAsync();

            List<Movie> movie = new List<Movie>();
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
                return View(movie);
                //return Json(movie);
            }
            else
            {
                movie = movies.Where(x => x.Genre.StartsWith(SearchValue) || SearchValue == null).ToList();
            }
            //return View(movies);
            return Json(movies);
        }
    }
}


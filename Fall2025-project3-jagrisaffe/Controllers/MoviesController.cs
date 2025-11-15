using Fall2025_project3_jagrisaffe.Data;
using Fall2025_project3_jagrisaffe.Models;
using Fall2025_project3_jagrisaffe.Models.ViewModels;
using Fall2025_project3_jagrisaffe.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourProjectName.Services;

namespace Fall2025_project3_jagrisaffe.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIService _aiService;
        private readonly ISentimentService _sentimentService;

        public MoviesController(ApplicationDbContext context, IAIService aiService, ISentimentService sentimentService)

        {
            _context = context;
            _aiService = aiService;
            _sentimentService = sentimentService;
        }

        public async Task<IActionResult> Poster(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var movie = await _context.Movie.FirstOrDefaultAsync(s => s.Id == id);

            if (movie == null || movie.Poster == null)
            {
                return NotFound();
            }

            return File(movie.Poster, "image/jpg");
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.ActorMovies)
                    .ThenInclude(am => am.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            // Get the list of actors
            var actors = movie.ActorMovies.Select(am => am.Actor).ToList();

            // Generate reviews using AI
            var reviewTexts = await _aiService.GenerateMovieReviews(movie.Title, 10);

            // Analyze sentiment for each review
            var reviews = reviewTexts.Select(text => new ReviewWithSentiment
            {
                Text = text,
                SentimentScore = _sentimentService.AnalyzeSentiment(text)
            }).ToList();

            // Calculate average sentiment
            var averageSentiment = reviews.Any() ? reviews.Average(r => r.SentimentScore) : 0;

            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                Actors = actors,
                Reviews = reviews,
                AverageSentiment = averageSentiment
            };

            return View(viewModel);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,IMDBLink,Year,Genre")] Movie movie, IFormFile? Poster)
        {

            if (ModelState.IsValid)
            {
                if(Poster != null && Poster.Length > 0)
                {
                    using var stream = new MemoryStream();
                    await Poster.CopyToAsync(stream);
                    movie.Poster = stream.ToArray();
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IMDBLink,Year,Genre")] Movie movie, IFormFile? Poster)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (Poster != null && Poster.Length > 0)
                {
                    using var stream = new MemoryStream();
                    await Poster.CopyToAsync(stream);
                    movie.Poster = stream.ToArray();
                }
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
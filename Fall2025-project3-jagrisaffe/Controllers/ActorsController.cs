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
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIService _aiService;
        private readonly ISentimentService _sentimentService;

        public ActorsController(ApplicationDbContext context, IAIService aiService, ISentimentService sentimentService)
        {
            _context = context;
            _aiService = aiService;
            _sentimentService = sentimentService;
        }

        public async Task<IActionResult> Photo(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var actor = await _context.Actor.FirstOrDefaultAsync(s => s.Id == id);

            if (actor == null || actor.Photo == null)
            {
                return NotFound();
            }

            return File(actor.Photo, "image/jpg");
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .Include(m => m.ActorMovies)
                    .ThenInclude(am => am.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            var movies = await _context.ActorMovie
                .Include(m => m.Movie)
                .Where(m => m.ActorId == id)
                .Select(m => m.Movie!)
                .ToListAsync();

            // Generate tweets using AI
            var tweetTexts = await _aiService.GenerateActorTweets(actor.Name, 20);

            // Analyze sentiment for tweets
            var tweets = tweetTexts.Select(text => new TweetsWithSentiment
            {
                Text = text,
                SentimentScore = _sentimentService.AnalyzeSentiment(text)
            }).ToList();

            // Calculate average sentiment
            var averageSentiment = tweets.Any() ? tweets.Average(r => r.SentimentScore) : 0;

            var vm = new ActorDetailsViewModel()
            {
                Actor = actor,
                Movies = movies,
                Tweets = tweets,
                AverageSentiment = averageSentiment

            };

            return View(vm);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,IMDBLink")] Actor actor, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    using var stream = new MemoryStream();
                    await Photo.CopyToAsync(stream);
                    actor.Photo = stream.ToArray();
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,IMDBLink")] Actor actor, IFormFile? Photo)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    using var stream = new MemoryStream();
                    await Photo.CopyToAsync(stream);
                    actor.Photo = stream.ToArray();
                }
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}
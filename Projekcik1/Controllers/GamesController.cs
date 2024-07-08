using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekcik1.Areas.Identity.Data;
using Projekcik1.Data;
using Projekcik1.Models;
using Projekcik1.Services;
using System.Security.Claims;

namespace Projekcik1.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGDBService _igdbService;
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GamesController(IGDBService igdbService, AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _igdbService = igdbService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = page;

            const int pageSize = 20;
            var games = await _igdbService.SearchGamesAsync(searchString, page, pageSize);

            return View(games);
        }

        public async Task<IActionResult> GetGameDetails(long id)
        {
            var game = await _igdbService.GetGameDetailsAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userGameExists = await _context.UserGames
                .AnyAsync(ug => ug.GameId == id.ToString() && ug.UserId == userId);

            ViewBag.GameInUserList = userGameExists;
            ViewBag.CurrentFilter = Request.Query["currentFilter"].ToString();
            ViewBag.CurrentPage = Request.Query["page"].ToString();

            return PartialView("_GameDetails", game);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToUserGames(long id, string currentFilter, int page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var userGameExists = await _context.UserGames
                .AnyAsync(ug => ug.GameId == id.ToString() && ug.UserId == userId);

            if (userGameExists)
            {
                TempData["ErrorMessage"] = "The game is already in your list.";
                return RedirectToAction("Index", new { searchString = currentFilter, page = page });
            }

            var gameDetails = await _igdbService.GetGameDetailsAsync(id);
            if (gameDetails == null)
            {
                return NotFound();
            }

            var userGame = new UserGame
            {
                GameId = id.ToString(),
                Title = gameDetails.Name,
                CoverUrl = gameDetails.Cover?.Url,
                UserId = userId
            };

            _context.UserGames.Add(userGame);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { searchString = currentFilter, page = page });
        }


        [Authorize]
        public async Task<IActionResult> UserGames(string searchString, int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            const int pageSize = 20;

            var userGamesQuery = _context.UserGames
                .Where(ug => ug.UserId == userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                userGamesQuery = userGamesQuery.Where(ug => ug.Title.Contains(searchString));
            }

            var userGames = await PaginatedList<UserGame>.CreateAsync(userGamesQuery.AsNoTracking(), page, pageSize);

            ViewBag.CurrentFilter = searchString;
            return View(userGames);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteUserGame(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(ug => ug.GameId == id && ug.UserId == userId);

            if (userGame == null)
            {
                return NotFound();
            }

            _context.UserGames.Remove(userGame);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserGames));
        }

    }
}
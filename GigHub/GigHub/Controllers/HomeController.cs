using GigHub.Models;
using GigHub.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using GigHub.Repositories;
using Microsoft.AspNet.Identity;

namespace GigHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AttendanceRepository _attendanceRepository;

        public HomeController()
        {
            _dbContext = new ApplicationDbContext();
            _attendanceRepository = new AttendanceRepository(_dbContext);
        }

        public ActionResult Index(string query = null)
        {
            var upcomingGigs = _dbContext.Gigs
                .Include(g => g.Artist)
                .Include(g => g.Genre)
                .Where(g => g.DateTime > DateTime.Now && g.IsCanceled == false);

            if (!String.IsNullOrEmpty(query))
            {
                upcomingGigs = upcomingGigs.Where(g => g.Artist.Name.Contains(query) ||
                                                       g.Genre.Name.Contains(query) ||
                                                       g.Venue.Contains(query));
            }

            var userId = User.Identity.GetUserId();

            var viewModel = new GigsViewModel
            {
                UpcomingGigs = upcomingGigs,
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Gigs",
                SearchTerm = query,
                Attendances = _attendanceRepository.GetFutureAttendances(userId)
                    .ToLookup(a => a.GigId)
            };

            return View("Gigs", viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
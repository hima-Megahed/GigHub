using GigHub.Models;
using GigHub.ViewModels;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using GigHub.Persistence;

namespace GigHub.Controllers
{
    public class GigsController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public GigsController()
        {
            _unitOfWork = new UnitOfWork(new ApplicationDbContext());
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Title = "Create";

            var viewModel = new GigFormViewModel
            {
                Id = 0,
                Genres = _unitOfWork.GenreRepository.GetGenres(),
                Heading = "Add a Gig"
            };

            return View("GigForm", viewModel);
        }

        [Authorize]
        public ActionResult Edit(int gigId)
        {
            ViewBag.Title = "Edit";

            var gig = _unitOfWork.GigRepository.GetGig(gigId);

            if (gig.ArtistId != User.Identity.GetUserId())
                return new HttpUnauthorizedResult();

            var viewModel = new GigFormViewModel
            {
                Genres = _unitOfWork.GenreRepository.GetGenres(),
                Id = gigId,
                Date = gig.DateTime.ToString("d MMM yyyy"),
                Genre = gig.GenreId,
                Time = gig.DateTime.ToString("HH:mm"),
                Venue = gig.Venue,
                Heading = "Edit a Gig"
            };

            return View("GigForm", viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GigFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres = _unitOfWork.GenreRepository.GetGenres();
                return View("GigForm", viewModel);
            }
            var gig = new Gig
            {
                ArtistId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                GenreId = viewModel.Genre,
                Venue = viewModel.Venue
            };

            _unitOfWork.GigRepository.AddGig(gig);
            _unitOfWork.Complete();

            Notification.GigCreated(gig);

            return RedirectToAction("Mine", "Gigs");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GigFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres = _unitOfWork.GenreRepository.GetGenres();
                return View("GigForm", viewModel);
            }

            var gig = _unitOfWork.GigRepository.GetGigWithAttendees(viewModel.Id);

            if (gig == null)
                return HttpNotFound();
            if (gig.ArtistId != User.Identity.GetUserId())
                return new HttpUnauthorizedResult();

            gig.Modify(viewModel.GetDateTime(), viewModel.Venue, viewModel.Genre);


            _unitOfWork.Complete();

            return RedirectToAction("Mine", "Gigs");
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();

            var viewModel = new GigsViewModel
            {
                UpcomingGigs = _unitOfWork.GigRepository.GetGigsUserAttending(userId),
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Gigs I'm Attending",
                Attendances = _unitOfWork.AttendanceRepository.GetFutureAttendances(userId)
                    .ToLookup(a => a.GigId)
            };

            return View("Gigs", viewModel);
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _unitOfWork.GigRepository.GetUpcomingGigsByArtist(userId);

            return View(gigs);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Cancel(int gigId)
        {
            var userId = User.Identity.GetUserId();
            var gig = _unitOfWork.GigRepository.GetGigWithAttendees(gigId);

            if (gig.IsCanceled)
            {
                return HttpNotFound();
            }

            gig.Cancel();

            _unitOfWork.Complete();

            return Content("");
        }

        public ActionResult Search(GigsViewModel viewModel)
        {
            return RedirectToAction("Index", "Home", new { query = viewModel.SearchTerm });
        }

        public ActionResult Details(int gigId)
        {
            var gig = _unitOfWork.GigRepository.GetGig(gigId);

            if (gig == null)
            {
                return HttpNotFound();
            }

            var viewModel = new GigDetailsViewModel { Gig = gig };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();

                viewModel.IsAttending = (_unitOfWork.AttendanceRepository.GetAttendance(gigId, userId) != null);

                viewModel.IsFollowing = (_unitOfWork.FollowingRepository.GetFollowing(userId, gig.ArtistId) != null);
            }

            return View("Details", viewModel);
        }

        [HttpPost]
        public ActionResult Miss(int gigId)
        {
            var userId = User.Identity.GetUserId();

            var attendance = _unitOfWork.AttendanceRepository.GetAttendance(gigId, userId);

            if (attendance == null)
                return HttpNotFound();

            _unitOfWork.AttendanceRepository.RemoveAttendance(attendance);
            _unitOfWork.Complete();

            return Content("Success");
        }
    }
}
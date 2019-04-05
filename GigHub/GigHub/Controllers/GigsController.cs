using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using GigHub.Core;
using GigHub.Core.Models;
using GigHub.Core.ViewModels;
using GigHub.Persistence;

namespace GigHub.Controllers
{
    public class GigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GigsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Title = "Create";

            var viewModel = new GigFormViewModel
            {
                Id = 0,
                Genres = _unitOfWork.Genres.GetGenres(),
                Heading = "Add a Gig"
            };

            return View("GigForm", viewModel);
        }

        [Authorize]
        public ActionResult Edit(int gigId)
        {
            ViewBag.Title = "Edit";

            var gig = _unitOfWork.Gigs.GetGig(gigId);

            if (gig.ArtistId != User.Identity.GetUserId())
                return new HttpUnauthorizedResult();

            var viewModel = new GigFormViewModel
            {
                Genres = _unitOfWork.Genres.GetGenres(),
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
                viewModel.Genres = _unitOfWork.Genres.GetGenres();
                return View("GigForm", viewModel);
            }
            var gig = new Gig
            {
                ArtistId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                GenreId = viewModel.Genre,
                Venue = viewModel.Venue
            };

            _unitOfWork.Gigs.AddGig(gig);
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
                viewModel.Genres = _unitOfWork.Genres.GetGenres();
                return View("GigForm", viewModel);
            }

            var gig = _unitOfWork.Gigs.GetGigWithAttendees(viewModel.Id);

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
                UpcomingGigs = _unitOfWork.Gigs.GetGigsUserAttending(userId),
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Gigs I'm Attending",
                Attendances = _unitOfWork.Attendances.GetFutureAttendances(userId)
                    .ToLookup(a => a.GigId)
            };

            return View("Gigs", viewModel);
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _unitOfWork.Gigs.GetUpcomingGigsByArtist(userId);

            return View(gigs);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Cancel(int gigId)
        {
            var gig = _unitOfWork.Gigs.GetGigWithAttendees(gigId);
            var userId = User.Identity.GetUserId();

            if (gig == null || gig.IsCanceled )
                return HttpNotFound();

            if(gig.ArtistId != userId)
                return new HttpUnauthorizedResult();

            gig.Cancel();

            _unitOfWork.Complete();

            return Content("Cancelled");
        }

        public ActionResult Search(GigsViewModel viewModel)
        {
            return RedirectToAction("Index", "Home", new { query = viewModel.SearchTerm });
        }

        public ActionResult Details(int gigId)
        {
            var gig = _unitOfWork.Gigs.GetGig(gigId);

            if (gig == null)
            {
                return HttpNotFound();
            }

            var viewModel = new GigDetailsViewModel { Gig = gig };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();

                viewModel.IsAttending = (_unitOfWork.Attendances.GetAttendance(gigId, userId) != null);

                viewModel.IsFollowing = (_unitOfWork.Followings.GetFollowing(userId, gig.ArtistId) != null);
            }

            return View("Details", viewModel);
        }

        [HttpPost]
        public ActionResult Miss(int gigId)
        {
            var userId = User.Identity.GetUserId();

            var attendance = _unitOfWork.Attendances.GetAttendance(gigId, userId);

            if (attendance == null)
                return HttpNotFound();

            _unitOfWork.Attendances.RemoveAttendance(attendance);
            _unitOfWork.Complete();

            return Content("Success");
        }
    }
}
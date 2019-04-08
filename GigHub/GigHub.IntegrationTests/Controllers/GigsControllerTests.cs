using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GigHub.Controllers;
using GigHub.Core.Models;
using GigHub.Core.ViewModels;
using GigHub.IntegrationTests.Extensions;
using GigHub.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace GigHub.IntegrationTests.Controllers
{
    [TestFixture]
    public class GigsControllerTests
    {
        private GigsController _controller;
        private ApplicationDbContext _applicationDbContext;

        [SetUp]
        public void SetUp()
        {
            _applicationDbContext = new ApplicationDbContext();
            _controller = new GigsController(new UnitOfWork(_applicationDbContext));
        }

        [TearDown]
        public void TearDown()
        {
            _applicationDbContext.Dispose();
        }

        [Test, Isolated]
        public void Mine_WhenCalled_ShouldReturnUpcomingGigs()
        {
            //Arrange
            var user = _applicationDbContext.Users.First();
            _controller.MockUser(user.Name, user.Id);

            var genre = _applicationDbContext.Genres.First();
            var gig = new Gig {Artist = user, DateTime = DateTime.Now.AddDays(1), Genre = genre, Venue = "-"};
            _applicationDbContext.Gigs.Add(gig);
            _applicationDbContext.SaveChanges();

            //Act
            var result = _controller.Mine();
            
            // Assert
            (result.ViewData.Model as IEnumerable<Gig>).Should().HaveCount(1);

        }

        [Test, Isolated]
        public void Update_WhenCalled_ShouldReturnUpcomingGigs()
        {
            //Arrange
            var user = _applicationDbContext.Users.First();
            _controller.MockUser(user.Name, user.Id);

            var genre = _applicationDbContext.Genres.Single(g => g.Id == 1);
            var gig = new Gig { Artist = user, DateTime = DateTime.Now.AddDays(1), Genre = genre, Venue = "-" };
            _applicationDbContext.Gigs.Add(gig);
            _applicationDbContext.SaveChanges();

            //Act
            var result = _controller.Edit(new GigFormViewModel
            {
                Id = gig.Id,
                Date = DateTime.Today.AddMonths(1).ToString("d MMM yyyy"),
                Time = "20:00",
                Venue = "Venue",
                Genre = 2
                
            });

            // Assert
            _applicationDbContext.Entry(gig).Reload();
            gig.DateTime.Should().Be(DateTime.Today.AddMonths(1).AddHours(20));
            gig.Venue.Should().Be("Venue");
            gig.GenreId.Should().Be(2);

        }
    }
}

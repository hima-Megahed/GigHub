using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using GigHub.Core.Dtos;
using GigHub.Core.Models;
using GigHub.Migrations;
using GigHub.Persistence;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace GigHub.Controllers.APIs
{
    [Authorize]
    public class NotificationsController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public NotificationsController()
        {
            _dbContext = new ApplicationDbContext();
        }
        
        [HttpGet]
        public IEnumerable<NotificationDto> GetNewNotifications()
        {
            var userId = User.Identity.GetUserId();
            var notification = _dbContext.UserNotification.Where(un => un.UserId == userId && un.IsRead == false)
                 .Select(un => un.Notification)
                 .Include(n => n.Gig.Artist)
                 .ToList();

            return notification.Select(Mapper.Map<Notification, NotificationDto>);

            #region Manual mapping
            //return notification.Select(n => new NotificationDto
            //{
            //    DateTime = n.DateTime,
            //    Gig = new GigDto
            //    {
            //        Artist = new UserDto
            //        {
            //            Id = n.Gig.Artist.Id,
            //            Name = n.Gig.Artist.Name
            //        },
            //        DateTime = n.Gig.DateTime,
            //        Id = n.Gig.Id,
            //        IsCanceled = n.Gig.IsCanceled,
            //        Venue = n.Gig.Venue
            //    },
            //    OriginalDateTime = n.OriginalDateTime,
            //    OriginalVenue = n.OriginalVenue,
            //    Type = n.Type

            //});
            #endregion

        }

        [HttpPost]
        public IHttpActionResult MarkAsRead()
        {
            var userId = User.Identity.GetUserId();
            var notifications = _dbContext.UserNotification
                .Where(un => un.UserId == userId && un.IsRead == false)
                .ToList();

            notifications.ForEach(n => n.Read());

            _dbContext.SaveChanges();

            return Ok();
        }
    }
}

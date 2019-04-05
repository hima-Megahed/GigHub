using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Moq;

namespace GigHub.Tests.Extensions
{
    public static class ControllerExtensions
    {
        public static void MockUser(this Controller controller, string name = "user1", string id = "1")
        {
            //var userMock = new Mock<IPrincipal>();
            //userMock.SetupGet(p => p.Identity.Name).Returns("user1");
            //userMock.SetupGet(p => p.Identity.IsAuthenticated).Returns(false);


            var identity = new GenericIdentity(name);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));

            var principal = new GenericPrincipal(identity, new[] { "user" });

            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(ctx => ctx.User)
                .Returns(principal);

            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(con => con.HttpContext)
                .Returns(contextMock.Object);

            controller.ControllerContext = controllerContextMock.Object;

            
        }
    }
}

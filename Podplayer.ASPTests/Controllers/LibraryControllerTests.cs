using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Podplayer.ASP.Controllers;
using Podplayer.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Podplayer.ASP.Controllers.Tests
{
    [TestClass()]
    public class LibraryControllerTests
    {

        private LibraryController Controller;
        private Mock<UserManager<AppUser>> MockedUserManager;

        [TestInitialize()]
        public void SetUp()
        {
            var mockedStore = new Mock<IUserStore<AppUser>>();
            MockedUserManager = new Mock<UserManager<AppUser>>(mockedStore.Object, null, null,
                null, null, null, null, null, null); // UserManager constructor args
            Controller = new LibraryController(null, MockedUserManager.Object, null, null, null);
        }

        [TestMethod()]
        public void Subscribe_With_Id()
        {
            Assert.Fail();
        }
    }
}
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SignalR.Services;
using SignalR.Models;
using SignalR.Data;

namespace SignalR.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var session = new Session()
            {
                SessionCode = "QWERT"                
            };
            var mockRepo = new Mock<ApplicationDbContext>();
            mockRepo.Setup(x => x);

            var obj = new DBService(mockRepo.Object);
            var data = obj.SearchSession("EJELN");
            Assert.IsNotNull(data);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Can_Return_Products_Index()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { Name = "prod1", Price = 10m },
                new Product { Name = "prod2", Price = 20m },
                new Product { Name = "prod3", Price = 30m },
                new Product { Name = "prod4", Price = 40m }
            }
            );

            AdminController target = new AdminController(mock.Object);

            // Act
            Product[] result = ((IEnumerable<Product>)target.Index().Model).ToArray();

            // Assert
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("prod1", result[0].Name);
            Assert.AreEqual("prod4", result[3].Name);
            Assert.AreEqual(40m, result[3].Price);
        }

        [TestMethod]
        public void Can_Edit_Products()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "prod1", Price = 10m },
                new Product { ProductID = 2, Name = "prod2", Price = 20m },
                new Product { ProductID = 3, Name = "prod3", Price = 30m },
                new Product { ProductID = 4, Name = "prod4", Price = 40m }
            }
            );

            AdminController target = new AdminController(mock.Object);

            // Act
            Product result = (Product)target.Edit(2).Model;
            Product result2 = (Product)target.Edit(4).Model;

            // Assert
            Assert.AreEqual("prod2", result.Name);
            Assert.AreEqual(4, result2.ProductID);
        }

        [TestMethod]
        public void Cant_Edit_Bad_ID_Products()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "prod1", Price = 10m },
                new Product { ProductID = 2, Name = "prod2", Price = 20m },
                new Product { ProductID = 3, Name = "prod3", Price = 30m },
                new Product { ProductID = 4, Name = "prod4", Price = 40m }
            }
            );

            AdminController target = new AdminController(mock.Object);

            // Act
            Product result = (Product)target.Edit(5).Model;
            Product result2 = (Product)target.Edit(6).Model;

            // Assert
            Assert.IsNull(result);
            Assert.IsNull(result2);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };

            // Act
            ActionResult result = target.Edit(product);

            // Assert
            mock.Verify(m => m.SaveProduct(product));
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cant_Save_Invalid_Changes()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };

            target.ModelState.AddModelError("error", "error");

            // Act
            ActionResult result = target.Edit(product);

            // Assert
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Products()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "prod1", Price = 10m },
                new Product { ProductID = 2, Name = "prod2", Price = 20m },
                new Product { ProductID = 3, Name = "prod3", Price = 30m },
                new Product { ProductID = 4, Name = "prod4", Price = 40m }
            }
            );

            AdminController target = new AdminController(mock.Object);

            // Act
            target.Delete(1);

            // Assert
            mock.Verify(m => m.DeleteProduct(1));
        }
    }
}

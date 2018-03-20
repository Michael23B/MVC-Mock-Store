using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Get_Image()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns( new Product[] {
                new Product { ProductID = 1, Name = "product1", ImageData = new byte[] { }, ImageMimeType = "imageType1" },
                new Product { ProductID = 2, Name = "product2", ImageData = new byte[] { }, ImageMimeType = "imageType2" },
                new Product { ProductID = 3, Name = "product3", ImageData = new byte[] { }, ImageMimeType = "imageType3" } }
            .AsQueryable());

            ProductController target = new ProductController(mock.Object);

            // Act
            ActionResult result = target.GetImage(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual("imageType1", ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cant_Get_Invalid_Product_Image()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "product1", ImageData = new byte[] { }, ImageMimeType = "imageType1" },
                new Product { ProductID = 2, Name = "product2", ImageData = new byte[] { }, ImageMimeType = "imageType2" },
                new Product { ProductID = 3, Name = "product3", ImageData = new byte[] { }, ImageMimeType = "imageType3" } }
            .AsQueryable());

            ProductController target = new ProductController(mock.Object);

            // Act
            ActionResult result = target.GetImage(4);

            // Assert
            Assert.IsNull(result);
        }

    }
}

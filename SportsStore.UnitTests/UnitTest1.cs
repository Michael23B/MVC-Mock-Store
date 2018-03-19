using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[] {
                    new Product { ProductID = 1, Name = "P1" },
                    new Product { ProductID = 2, Name = "P2" },
                    new Product { ProductID = 3, Name = "P3" },
                    new Product { ProductID = 4, Name = "P4" },
                    new Product { ProductID = 5, Name = "P5" }
                }
                );

            ProductController controller = new ProductController(mock.Object) { pageSize = 3 };

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // Arrange - define an HTML helper - we need to do this in order to apply the extension method
            HtmlHelper myHelper = null;
            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            // Arrange - set up the delegate using a lambda expression
            string pageUrlDelegate(int i) => "Page" + i;
            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);
            // Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>" 
            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
            + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
            result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[] {
                    new Product { ProductID = 1, Name = "P1" },
                    new Product { ProductID = 2, Name = "P2" },
                    new Product { ProductID = 3, Name = "P3" },
                    new Product { ProductID = 4, Name = "P4" },
                    new Product { ProductID = 5, Name = "P5" }
                }
                );

            ProductController controller = new ProductController(mock.Object)
            {
                pageSize = 3
            };

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(2, pagingInfo.CurrentPage);
            Assert.AreEqual(3, pagingInfo.ItemsPerPage);
            Assert.AreEqual(5, pagingInfo.TotalItems);
            Assert.AreEqual(2, pagingInfo.TotalPages);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
                    new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
                    new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
                    new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
                    new Product { ProductID = 5, Name = "P5", Category = "Cat3" }
                }
                );

            ProductController controller = new ProductController(mock.Object)
            {
                pageSize = 3
            };

            // Act
            Product[] result = ((ProductsListViewModel)controller.List("Cat2",1).Model).Products.ToArray();

            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Apples" },
                    new Product { ProductID = 2, Name = "P2", Category = "Apples" },
                    new Product { ProductID = 3, Name = "P3", Category = "Plums" },
                    new Product { ProductID = 4, Name = "P4", Category = "Oranges" },
                    new Product { ProductID = 5, Name = "P5", Category = "Oranges" }
                }
                );

            NavController controller = new NavController(mock.Object);

            // Act
            string[] results = ((IEnumerable<string>)controller.Menu().Model).ToArray();

            // Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicate_Selected_Category()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Apples" },
                    new Product { ProductID = 2, Name = "P2", Category = "Pears" },
                }
                );

            NavController controller = new NavController(mock.Object);

            // Act
            string result = controller.Menu("Apples").ViewBag.SelectedCategory;

            // Assert
            Assert.AreEqual("Apples", result);
        }

        [TestMethod]
        public void Can_Generate_Product_Category_Count()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
                    new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
                    new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
                    new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
                    new Product { ProductID = 5, Name = "P5", Category = "Cat3" }
                }
                );

            ProductController controller = new ProductController(mock.Object)
            {
                pageSize = 3
            };

            // Act
            int result1 = ((ProductsListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int result2 = ((ProductsListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int result3 = ((ProductsListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resultAll = ((ProductsListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            // Assert
            Assert.AreEqual(2, result1);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(1, result3);
            Assert.AreEqual(5, resultAll);
        }
    }
}
//Install-Package Ninject -version 3.0.1.10 -projectname SportsStore.WebUI

//Install-Package Ninject.Web.Common -version 3.0.0.7 -projectnameSportsStore.WebUI

//Install-Package Ninject.MVC3 -Version 3.0.0.6 -projectnameSportsStore.WebUI

//Install-Package Ninject -version 3.0.1.10 -projectnameSportsStore.UnitTests

//Install-Package Ninject.Web.Common -version 3.0.0.7 -projectnameSportsStore.UnitTests

//Install-Package Ninject.MVC3 -Version 3.0.0.6 -projectnameSportsStore.UnitTests

//Install-Package Moq -version 4.1.1309.1617 -projectname SportsStore.WebUI

//Install-Package Moq -version 4.1.1309.1617 -projectnameSportsStore.UnitTests

//Install-Package Microsoft.Aspnet.Mvc -version 5.0.0 -projectnameSportsStore.Domain

//Install-Package Microsoft.Aspnet.Mvc -version 5.0.0 -projectnameSportsStore.UnitTests

/*
<connectionStrings>
<add name="EFDbContext"
     connectionString="Data Source=(localdb)\MSSQLLocalDB;InitialCatalog=SportsStore;Integrated Security=True"
     providerName="System.Data.SqlClient"/>
</connectionStrings>
*/
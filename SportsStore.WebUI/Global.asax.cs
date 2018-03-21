using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common.WebHost;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Infrastructure.Binders;

namespace SportsStore.WebUI
{
    public class MvcApplication : NinjectHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Home", "", new { controller = "Product", action = "List", category = (string)null, page = 1 });

            routes.MapRoute("All/Page", "Page{page}", new { controller = "Product", action = "List", category = (string)null }, new { page = @"\d+" });

            routes.MapRoute("Category", "{category}", new { controller = "Product", action = "List", page = 1 });

            routes.MapRoute("Category/Page", "{category}/Page{page}", new { controller = "Product", action = "List" }, new { page = @"\d+" });

            routes.MapRoute("Controller/Action", "{controller}/{action}");
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private void RegisterServices(IKernel kernel)
        {
            DependencyResolver.SetResolver(new Infrastructure.NinjectDependencyResolver(kernel));
        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());
        }
    }
}

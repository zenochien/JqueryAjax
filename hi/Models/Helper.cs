using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Jquery_Ajax.Models
{
    public class Helper
    {
        public static async Task<string> RenderRazorViewToStringAsync(Controller controllers, string viewName, object model = null)
        {
            controllers.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine = controllers.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controllers.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controllers.ControllerContext,
                    viewResult.View,
                    controllers.ViewData,
                    controllers.TempData,
                    sw,
                    new HtmlHelperOptions()
                    );
                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.GetTypedHeaders().Referer == null ||
          filterContext.HttpContext.Request.GetTypedHeaders().Host.Host.ToString() != filterContext.HttpContext.Request.GetTypedHeaders().Referer.Host.ToString())
            {
                filterContext.HttpContext.Response.Redirect("/");
            }
        }
    }
}

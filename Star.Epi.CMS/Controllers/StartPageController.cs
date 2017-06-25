using System.Web.Mvc;
using EPiServer.Web.Mvc;
using Star.Epi.CMS.Models.Pages;

namespace Star.Epi.CMS.Controllers
{
    public class StartPageController : PageController<StartPage>
    {
        public ActionResult Index(StartPage currentPage)
        {
            return Content(currentPage.Name);
        }
    }
}
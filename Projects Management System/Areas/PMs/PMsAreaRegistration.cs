using System.Web.Mvc;

namespace Projects_Management_System.Areas.PMs
{
    public class PMsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PMs";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PMs_default",
                "PMs/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
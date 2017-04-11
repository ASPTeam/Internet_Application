using System.Web.Mvc;

namespace Projects_Management_System.Areas.JEs
{
    public class JEsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "JEs";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "JEs_default",
                "JEs/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
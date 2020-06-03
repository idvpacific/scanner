using System.Web.Mvc;

namespace IDV_ScannerWS.Areas.DrivingLicence
{
    public class DrivingLicenceAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DrivingLicence";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DrivingLicence_default",
                "DrivingLicence/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
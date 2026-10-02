using Microsoft.AspNetCore.Mvc;

namespace VisionHub.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CamerasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IEnumerable<MockIPCamera> GetCameras()
        {
            // Return a list of cameras (this is just a placeholder, you would typically fetch this from a database or service)
            return new List<MockIPCamera>
            {
                new MockIPCamera(id: 1, name: "Maindoor Camera"),
                new MockIPCamera(id: 2, name: "Backyard Camera")
            };
        }
    }
}

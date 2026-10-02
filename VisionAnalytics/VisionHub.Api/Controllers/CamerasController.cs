using Microsoft.AspNetCore.Mvc;
using VisionHub.Api.Models;
using VisionHub.Api.Models.Camera;
using VisionHub.Api.Services;

namespace VisionHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CamerasController : Controller
    {

        private readonly CameraService _cameraService;
        public CamerasController(CameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cameras = await _cameraService.GetCamerasAsync();
            return Ok(cameras);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var camera = await _cameraService.GetCameraByIdAsync(id);
            if (camera == null)
                return NotFound();

            return Ok(camera);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCameraRequest request)
        {
            var camera = await _cameraService.AddCameraAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = camera.Id },
                camera);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateCameraRequest request)
        {
            var existingCamera = await _cameraService.GetCameraByIdAsync(request.Id);
            if (existingCamera == null)
                return NotFound();
            await _cameraService.UpdateCameraAsync(request);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetByStatus(bool enabled)
        {
            var cameras = await _cameraService.GetCamerasByStatusAsync(enabled);
            return Ok(cameras);
        }

        [HttpGet]
        public async Task<IActionResult> GetByAnalyticsStatus(bool analyticsEnabled)
        {
            var cameras = await _cameraService.GetCamerasByAnalyticsStatusAsync(analyticsEnabled);
            return Ok(cameras);
        }
    }
}

using VisionHub.Api.Models.Camera;
using VisionHub.Api.Repositories.Interfaces;

namespace VisionHub.Api.Services
{
    public class CameraService
    {
        private readonly ICameraRepository _cameraRepository;

        public CameraService(ICameraRepository cameraRepository)
        {
            _cameraRepository = cameraRepository;
        }

        public async Task<IEnumerable<Camera>> GetCamerasAsync()
        {
            return await _cameraRepository.GetCamerasAsync();
        }

        public async Task<Camera> GetCameraByIdAsync(int id)
        {
            return await _cameraRepository.GetCameraByIdAsync(id);
        }

        public async Task<Camera> AddCameraAsync(CreateCameraRequest request)
        {
            var camera = new Camera(0, request.Name);
            return await _cameraRepository.AddCameraAsync(camera);
        }

        public async Task UpdateCameraAsync(UpdateCameraRequest request)
        {
            var existingCamera = await _cameraRepository.GetCameraByIdAsync(request.Id);
            if (existingCamera != null)
            {
                existingCamera.Name = request.Name;
                existingCamera.Enabled = request.Enabled;
                existingCamera.AnalyticsEnabled = request.AnalyticsEnabled;
                await _cameraRepository.UpdateCameraAsync(existingCamera);
            }
        }

        public async Task DeleteCameraAsync(int id)
        {
            await _cameraRepository.DeleteCameraAsync(id);
        }

        public async Task<IEnumerable<Camera>> GetCamerasByNameAsync(string name)
        {
            var cameras = await _cameraRepository.GetCamerasAsync();
            return cameras.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Camera>> GetCamerasByIdsAsync(IEnumerable<int> ids)
        {
            var cameras = await _cameraRepository.GetCamerasAsync();
            return cameras.Where(c => ids.Contains(c.Id));
        }
        public async Task<IEnumerable<Camera>> GetCamerasByNameAndIdsAsync(string name, IEnumerable<int> ids)
        {
            var cameras = await _cameraRepository.GetCamerasAsync();
            return cameras.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase) && ids.Contains(c.Id));
        }

        public async Task<IEnumerable<Camera>> GetCamerasByNameOrIdsAsync(string name, IEnumerable<int> ids)
        {
            var cameras = await _cameraRepository.GetCamerasAsync();
            return cameras.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase) || ids.Contains(c.Id));
        }

        public async Task<IEnumerable<Camera>> GetCamerasByNameAndIdsOrAsync(string name, IEnumerable<int> ids)
        {
            var cameras = await _cameraRepository.GetCamerasAsync();
            return cameras.Where(c => (c.Name.Contains(name, StringComparison.OrdinalIgnoreCase) && ids.Contains(c.Id)) || c.Name.Contains(name, StringComparison.OrdinalIgnoreCase) || ids.Contains(c.Id));

        }

    }
}

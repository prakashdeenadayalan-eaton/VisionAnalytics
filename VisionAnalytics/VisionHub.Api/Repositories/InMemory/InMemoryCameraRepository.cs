using VisionHub.Api.Models.Camera;
using VisionHub.Api.Repositories.Interfaces;

namespace VisionHub.Api.Repositories.InMemory
{
    public class InMemoryCameraRepository : ICameraRepository
    {

        private readonly List<Camera> _cameras = new List<Camera>();

        public InMemoryCameraRepository()
        {
            // Initialize with some sample cameras
            _cameras.Add(new Camera(1, "Maindoor Camera"));
            _cameras.Add(new Camera(2, "Backyard Camera"));
        }

        public async Task<IEnumerable<Camera>> GetCamerasAsync()
        {
            return await Task.FromResult(_cameras);
        }

        public async Task<Camera> GetCameraByIdAsync(int id)
        {
            var camera = _cameras.FirstOrDefault(c => c.Id == id);
            return await Task.FromResult(camera);
        }

        public async Task<IEnumerable<Camera>> GetCamerasByNameAsync(string name)
        {
            var cameras = _cameras.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            return await Task.FromResult(cameras);
        }
        

        public async Task UpdateCameraAsync(Camera camera)
        {
            var existingCamera = _cameras.FirstOrDefault(c => c.Id == camera.Id);
            if (existingCamera != null)
            {
                existingCamera.Name = camera.Name;
            }
            await Task.CompletedTask;
        }

        public async Task<Camera> AddCameraAsync(Camera camera)
        {
            if (camera is null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            // Assign an Id if none provided or invalid
            if (camera.Id <= 0)
            {
                var nextId = _cameras.Any() ? _cameras.Max(c => c.Id) + 1 : 1;
                camera.Id = nextId;
            }
            else
            {
                // Ensure uniqueness of provided Id
                if (_cameras.Any(c => c.Id == camera.Id))
                {
                    throw new InvalidOperationException($"A camera with Id {camera.Id} already exists.");
                }
            }

            _cameras.Add(camera);
           
            return await Task.FromResult(camera);
        }

        public async Task DeleteCameraAsync(int id)
        {
            var existingCamera = _cameras.FirstOrDefault(c => c.Id == id);
            if (existingCamera != null)
            {
                _cameras.Remove(existingCamera);
            }

            await Task.CompletedTask;
        }
    }
}

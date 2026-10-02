using VisionHub.Api.Models.Camera;

namespace VisionHub.Api.Repositories.Interfaces
{
    public interface ICameraRepository
    {
        Task<IEnumerable<Camera>> GetCamerasAsync();
        Task<Camera> GetCameraByIdAsync(int id);
        Task<Camera> AddCameraAsync(Camera camera);
        Task UpdateCameraAsync(Camera camera);
        Task DeleteCameraAsync(int id);
        Task<IEnumerable<Camera>> GetCamerasByNameAsync(string name);
        Task<IEnumerable<Camera>> GetCamerasByStatusAsync(bool enabled);
        Task<IEnumerable<Camera>> GetCamerasByAnalyticsStatusAsync(bool analyticsEnabled);
    }
}

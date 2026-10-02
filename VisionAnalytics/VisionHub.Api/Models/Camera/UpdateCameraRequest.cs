namespace VisionHub.Api.Models.Camera
{
    public class UpdateCameraRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Enabled { get; set; } = false;
        public bool AnalyticsEnabled { get; set; } = false;
    }
}

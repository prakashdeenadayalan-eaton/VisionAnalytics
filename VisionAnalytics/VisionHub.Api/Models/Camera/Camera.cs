namespace VisionHub.Api.Models.Camera
{
    public class Camera
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; } = false;
        public bool AnalyticsEnabled { get; set; } = false;
        public Camera(int id, string name, bool enabled = false, bool analyticsEnabled = false)
        {
            Id = id;
            Name = name;
            Enabled = enabled;
            AnalyticsEnabled = analyticsEnabled;
        }
    }
}

namespace VisionHub.Api.Models.Camera
{
    public class Camera
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Camera(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

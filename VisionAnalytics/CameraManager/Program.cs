class MockIPCamera
{
    public int Id { get; set; }
    public string Name { get; set; }

    public MockIPCamera(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<MockIPCamera> cameras = new List<MockIPCamera>();

        // Sample camera data
        cameras.Add(new MockIPCamera(id: 1, name: "Maindoor Camera"));
        cameras.Add(new MockIPCamera(id: 2, name: "Backyard Camera"));


        while (true)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine("----- CAMERA MANAGER --------");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("1. Add Camera");
            Console.WriteLine("2. List Cameras");
            Console.WriteLine("3. Remove Camera");
            Console.WriteLine("0. Exit");

            Console.WriteLine("Enter option to proceed.");
            int option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {
                case 1:
                    Console.WriteLine("Enter Camera id ");
                    int id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter Camera Name ");
                    string name = Console.ReadLine();
                    cameras.Add(new MockIPCamera(id: id, name: name));
                    Console.WriteLine($"Camera {name} added successfully.");
                    break;
                case 2:
                    Console.WriteLine("Camera list");
                    foreach(var camera in cameras)
                    {
                        Console.WriteLine($"Camera Id {camera.Id}, Camera Name {camera.Name}");
                    }
                    break;
                case 0:
                    Console.WriteLine("Application is closing.");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

    }
}
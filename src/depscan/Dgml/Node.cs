namespace Dgml
{
    public class Node(string id, string name, string colour)
    {
        public string Id { get; set; } = id;
        public string Name { get; set; } = name;
        public string Colour { get; set; } = colour;
    }
}
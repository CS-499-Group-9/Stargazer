
namespace DataLayer.EquatorialObjects
{
    // Represents a constellation in the sky. 
    // Use the Constellation Name to print the name
    // The Constellation Lines is a list that can be iterated through 
    public class Constellation
    {
        public string? ConstellationId { get; internal set; }
        public IEnumerable<Tuple<int, int>> ConstellationLines { get; internal set; }

        public string? ConstellationName { get; internal set; }
        public Constellation(string Id, string name, string? nativeName)
        {
            ConstellationId = Id;
            ConstellationName = string.Equals(name,nativeName) ? name : $"{name} ({nativeName})";
            ConstellationLines = new List<Tuple<int, int>>();
        }

    }
}

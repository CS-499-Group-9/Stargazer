using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace DataLayer.Implementations
{
    /// <summary>
    /// A custom type converter to handle blank space in the repository
    /// </summary>
    internal class NullableIntConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            // Attempt to parse to an integer. If the parsing fails (usually because of white space or null string), return a null.
            return int.TryParse(text, out int value) ? value : null;
        }
    }
}

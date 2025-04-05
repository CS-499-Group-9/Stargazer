using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace DataLayer.Implementations
{
    /// <summary>
    /// A custom type converter to handle blank space in the csv file
    /// </summary>
    internal class NullableDoubleConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            // Attempt to parse to a double. If the parsing fails (usually because of white space or no value in the cell), return null.
            return double.TryParse(text, out double value) ? value : null;
        }
    }
}

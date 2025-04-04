using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DataLayer.EquatorialObjects;
using DataLayer.Interfaces;
using System.Globalization;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Handles retrieving data from the csv listing of Messier Deep Space Objects retrieved from [Starlust.org](https://starlust.org/messier-catalog/)
    /// If a new repository is used, a new converter that implements the <see cref="IMessierRepository"/> needs to be created for it. 
    /// </summary>
    internal class StarLustMessierCsvRepository : IMessierRepository
    {
        /// <summary>
        /// The path to the repository file.
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="repositoryPath">The path to the directory containing the file.</param>
        public StarLustMessierCsvRepository(string repositoryPath)
        {
            filePath = Path.Combine(repositoryPath, "messier-catalog.csv");
            if (!File.Exists(filePath)) throw new FileNotFoundException($"{filePath} does not exist");

        }

        /// <inheritdoc/>
        public IEnumerable<EquatorialMessierObject> GetMessierObjects()
        {
            var reader = new StreamReader(filePath);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<StarMap>();

            try
            {
                foreach (var record in csv.GetRecords<EquatorialMessierObject>())
                {
                    yield return record;
                }
            }
            finally
            {
                csv.Dispose();
                reader.Dispose();
            }
        }

        /// <summary>
        /// A class internal to the <see cref="StarLustMessierCsvRepository"/> to map the csv columns to <see cref="EquatorialMessierObject"/> objects.
        /// </summary>
        private class StarMap : ClassMap<EquatorialMessierObject>
        {
            /// <summary>
            /// Creates the object and maps all the columns
            /// </summary>
            public StarMap()
            {
                Map(m => m.MessierId).Name("M");
                Map(m => m.NewGeneralCatalog).Name("NGC");
                Map(m => m.Type).Name("TYPE");
                Map(m => m.Constellation).Name("CONS");
                Map(m => m.RightAscension).TypeConverter<HmsToDecimalDegreesConverter>().Name("RA");
                Map(m => m.Declination).TypeConverter<DdmToDecimalDegreesConverter>().Name("DEC");
                Map(m => m.Magnitude).Name("MAG");
                Map(m => m.Size).Name("SIZE");
                Map(m => m.Distance).Name("DIST (ly)");
                Map(m => m.ViewingSeason).Name("VIEWING SEASON");
                Map(m => m.ViewingDifficulty).Name("VIEWING DIFFICULTY");
            }

            /// <summary>
            /// A type converter internal to the <see cref="StarLustMessierCsvRepository"/> class used to convert the <c>Right Ascension</c> from hours and decimal minutes to decimal hours. 
            /// </summary>
            private class HmsToDecimalDegreesConverter : DefaultTypeConverter
            {
                public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
                {
                    if (text == null) return null;
                    // String should come in in ##H ##.##m format, split on the space.
                    var degDecMin = text.Split(' ');
                    if 
                    (
                        // Attempt to parse the values
                        !(
                            // Remove the letter from the end and attempt to parse the rest of the string to a double.
                            double.TryParse(degDecMin[0].Remove(degDecMin[0].Length - 1), out double hours) &&
                            double.TryParse(degDecMin[1].Remove(degDecMin[1].Length - 1), out double minutes)
                          )
                    )
                    {
                        // The Right Ascention is crucial data. Any value that can't be parsed should break the program so the converter can be modified to handle it. 
                        throw new InvalidDataException($"{text} could not be converted to decimal degrees.");
                    }
                    minutes /= 60;
                    return hours + minutes;
                }
            }

            /// <summary>
            /// A type converter internal to the <see cref="StarLustMessierCsvRepository"/> class used to convert the <c>Declination</c> from degrees and minutes to decimal degrees.
            /// </summary>
            private class DdmToDecimalDegreesConverter : DefaultTypeConverter
            {
                public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
                {
                    if (text == null) return null;
                    // The current repository uses the ° character to denote degrees with no space between the ° character and the decimal degrees. Split on the ° character to get the degrees and minutes 
                    var degreesMinutes = text.Split('°');
                    if 
                    (
                        // Attempt to parse the values
                        !(
                            double.TryParse(degreesMinutes[0], out double degrees) &&
                            double.TryParse(degreesMinutes[1], out double minutes)
                         )
                    ) 
                    {
                        // The Declincation is crucial data. Any value that can't be parsed shoudl break the program so the converter can be modified to handle it.
                        throw new InvalidDataException($"{text} could not be converted into decimal degrees"); 
                    }
                    // Convert the decimal degrees to decimal hours and add it to the hours to get a total decimal hours.
                    minutes /= 60;
                    return degrees + minutes;
                }
            }
        }
    }
}

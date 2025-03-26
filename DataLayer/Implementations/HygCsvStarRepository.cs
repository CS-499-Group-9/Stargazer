using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DataLayer.Interfaces;
using DataLayer.EquatorialObjects;
using System.Globalization;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Retrieves raw star data from the HYG.csv repository
    /// </summary>
    internal class HygCsvStarRepository : IStarRepository
    {
        /// <summary>
        /// The file path to the repository csv file
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance and sets the file path
        /// </summary>
        /// <param name="repositoryPath">The path to the directory containing the file</param>
        public HygCsvStarRepository(string repositoryPath) 
        {
            this.filePath = Path.Combine(repositoryPath, "hyg.csv");
            if (!File.Exists(filePath)) throw new FileNotFoundException($"{filePath} does not exist");
        }

        /// <summary>
        /// Retrieves a single star from the repository
        /// </summary>
        /// <param name="hipparcosId">The Hipparcos ID of the star to find</param>
        /// <returns>Null if the star is not found, otherwise returns the star in the equatorial coordinate form.</returns>
        public async Task<EquatorialStar?> GetStarByHipAsync(int hipparcosId)
        {
            // Start a new task to retrieve the data and return the running task. 
            return await Task.Run(() =>
            {
                // Create a new list
                var targetList = new List<EquatorialStar>();
                // Using statements handle disposal of the items
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                // Register the class mapper
                csv.Context.RegisterClassMap<StarMap>();

                // CsvHelper.GetRecords returns a yieldable IEnumerable that will not be used until it is iterated.
                var records = csv.GetRecords<EquatorialStar>();

                // Provide a filter to select only the stars needed from the IEnumerable
                // Create a new thread for each row that will read that row into the list
                return records.FirstOrDefault(s => s.HipparcosId == hipparcosId);
            });
        }

        /// <summary>
        /// Gets all stars that are brighter than a defined brightness.
        /// </summary>
        /// <returns>A list of stars in the equatorial coordinate form.</returns>
        Task<IList<EquatorialStar>> IStarRepository.GetAllStarsAsync()
        {
            // Start a new task to retrieve the data and return the running task
            return Task<IList<EquatorialStar>>.Factory.StartNew(() =>
            {
                // Build the list instance and a stream reader to read the rows
                var targetList = new List<EquatorialStar>();
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<StarMap>();

                // CsvHelper.GetRecords returns a yieldable IEnumerable that will not be used until it is iterated.
                var records = csv.GetRecords<EquatorialStar>();

                // Provide a filter to select only the stars needed from the IEnumerable
                // Create a new thread for each row that will read that row into the list
                return records.ToList();

            });
        }

        /// <summary>
        /// A custom class (internal to the HygStarRepository) to map the columns in the repository to the POCO
        /// </summary>
        private class StarMap : ClassMap<EquatorialStar>
        {
            /// <summary>
            /// Creates an instance of the mapper
            /// </summary>
            public StarMap()
            {
                Map(m => m.StarId).Name("StarID");
                Map(m => m.HipparcosId).TypeConverter<NullableIntConverter>().Name("Hip");
                Map(m => m.HenryDraperId).TypeConverter<NullableIntConverter>().Name("HD");
                Map(m => m.HarvardRevisedId).TypeConverter<NullableIntConverter>().Name("HR");
                Map(m => m.GlieseId).Name("Gliese");
                Map(m => m.BayerFlamsteedDesignation).Name("BayerFlamsteed");
                Map(m => m.ProperName);
                Map(m => m.RightAscension).Name("RA");
                Map(m => m.Declination).TypeConverter<NullableDoubleConverter>().Name("Dec");
                Map(m => m.Distance).TypeConverter(new ParsecToLightyearConverter());
                Map(m => m.Magnitude).Name("Mag");
                Map(m => m.AbsoluteMagnitude).Name("AbsMag");
                Map(m => m.Spectrum);
                Map(m => m.ColorIndex).TypeConverter(new NullableDoubleConverter());
            }

            /// <summary>
            /// A custom type converter to convert parsecs to light years (the distance data in the repository is in Parsecs... yes... that's actually a real thing).
            /// </summary>
            private class ParsecToLightyearConverter : DefaultTypeConverter
            {
                private const double conversionFactor = 3.262;
                public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
                {
                    return double.TryParse(text, out var value) && value > 0? value * conversionFactor : 1;
                }
            }
        }
    }
}

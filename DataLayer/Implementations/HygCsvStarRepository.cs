using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DataLayer.EquatorialObjects;
using DataLayer.Interfaces;
using System.Globalization;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Retrieves raw star data from the HYG.csv repository
    /// Author: Josh Johner
    /// Created: SPR 2025
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

        /// <inheritdoc/>
        public IEnumerable<EquatorialStar> GetAllStars()
        {
            var reader = new StreamReader(filePath);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<StarMap>();

            try
            {
                foreach (var record in csv.GetRecords<EquatorialStar>())
                {
                    if (record.StarId > 0)
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
                    return double.TryParse(text, out var value) && value > 0 ? value * conversionFactor : 1;
                }
            }
        }
    }
}

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DataLayer.HorizonalObjects;
using DataLayer.Interfaces;
using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Implementations
{
    internal class HygCsvStarRepository : IStarRepository
    {
        private readonly string baseDirectoryPath;
        public HygCsvStarRepository(string baseDirectoryPath) 
        {
            this.baseDirectoryPath = baseDirectoryPath;
        }
        Task<IList<EquitorialStar>> IStarRepository.GetAllStarsAsync(double maximumMagnitude)
        {
            DirectoryInfo dir = new DirectoryInfo(baseDirectoryPath);
            
            string dataLayer = Path.Combine(dir.Parent.FullName, "DataLayer");
            string repositories = Path.Combine(dataLayer, "Repositories");
            string filePath = Path.Combine(repositories, "hyg.csv");

            if (File.Exists(filePath))
            {
                // Return the Task that will return the list
                return new Task<IList<EquitorialStar>>(() =>
                {
                    // Build the list instance and a stream reader to read the rows
                    var targetList = new List<EquitorialStar>();
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<StarMap>();

                        // CsvHelper.GetRecords returns a yieldable IEnumerable that will not be used until it is iterated.
                        var records = csv.GetRecords<EquitorialStar>();

                        // Provide a filter to select only the stars needed from the IEnumberable
                        // Create a new thread for each row that will read that row into the list
                        return records.Where(s => s.Magnitude <=  maximumMagnitude).ToList();

                    }

                });
            }
            throw new FileNotFoundException();
        }

        private class StarMap : ClassMap<EquitorialStar>
        {
            public StarMap()
            {
                Map(m => m.StarId).Name("StarID");
                Map(m => m.HipparcosId).TypeConverter<NullableIntConverter>().Name("Hip");
                Map(m => m.HenryDraperId).TypeConverter<NullableIntConverter>().Name("HD");
                Map(m => m.HarvardRevisedId).TypeConverter<NullableIntConverter>().Name("HR");
                Map(m => m.GlieseId).Name("Gliese");
                Map(m => m.BayerFlamsteedDesignation).Name("BayerFlamsteed");
                Map(m => m.ProperName);
                Map(m => m.RightAscention).Name("RA");
                Map(m => m.Declination).TypeConverter<NullableDoubleConverter>().Name("Dec");
                Map(m => m.Distance).TypeConverter(new ParsecToLightyearConverter());
                Map(m => m.Magnitude).Name("Mag");
                Map(m => m.AbsoluteMagnitude).Name("AbsMag");
                Map(m => m.Spectrum);
                Map(m => m.ColorIndex).TypeConverter(new NullableDoubleConverter());
            }

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

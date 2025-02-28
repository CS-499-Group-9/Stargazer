using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DataLayer.Interfaces;
using DataLayer.EquatorialObjects;
using System.Globalization;

namespace DataLayer.Implementations
{
    internal class StarLustMessierCsvRepository : IMessierRepository
    {
        private readonly string filePath;
        public StarLustMessierCsvRepository(string repositoryPath)
        {
            filePath = Path.Combine(repositoryPath, "messier-catalog.csv"); 
        }

        public Task<IEnumerable<EquatorialMessierObject>> GetRawMessierObjectsAsync()
        {
            if (File.Exists(filePath))
            {
                // Build a task to retrieve the raw messier objects from the repository and return the task to the calling code
                return Task<IEnumerable<EquatorialMessierObject>>.Factory.StartNew(() =>
                {
                    using var reader = new StreamReader(filePath);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csv.Context.RegisterClassMap<StarMap>();

                    // the CsvHelper.GetRecords retrieve a yieldable list. Objects are not read until they are requested
                    var records = csv.GetRecords<EquatorialMessierObject>();

                    // Create a new thread for each row in the csv and map that row to a new object
                    return records.ToList();
                });
            }
            throw new FileNotFoundException();
        }

        private class StarMap : ClassMap<EquatorialMessierObject>
        {
            public StarMap()
            {
                Map(m => m.MessierId).Name("M");
                Map(m => m.NewGeneralCatalog).Name("NGC");
                Map(m=> m.Type).Name("TYPE");
                Map(m => m.Constellation).Name("CONS");
                Map(m => m.RightAscension).TypeConverter<HmsToDecimalDegreesConverter>().Name("RA");
                Map(m => m.Declination).TypeConverter<DdmToDecimalDegreesConverter>().Name("DEC");
                Map(m => m.Magnitude).Name("MAG");
                Map(m => m.Size).Name("SIZE");
                Map(m => m.Distance).Name("DIST (ly)");
                Map(m => m.ViewingSeason).Name("VIEWING SEASON");
                Map(m => m.ViewingDifficulty).Name("VIEWING DIFFICULTY");
            }

            private class HmsToDecimalDegreesConverter : DefaultTypeConverter
            {
                public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
                {
                    if (text == null) return null;
                    var degDecMin = text.Split(' ');
                    if (
                        !(
                            double.TryParse(degDecMin[0].Remove(degDecMin[0].Length -1), out double hours) &&
                            double.TryParse(degDecMin[1].Remove(degDecMin[1].Length - 1), out double minutes)
                          )
                        )
                    {
                        throw new InvalidDataException($"{text} could not be converted to decimal degrees.");
                    }
                    minutes /= 60;
                    return hours + minutes;
                }
            }
            
            private class DdmToDecimalDegreesConverter : DefaultTypeConverter
            {
                public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
                {
                    if(text == null) return null;
                    var degreesMinutes = text.Split('°');
                    if (
                        !(
                            double.TryParse(degreesMinutes[0], out double degrees) &&
                            double.TryParse(degreesMinutes[1], out double minutes)
                        )
                        )
                    { throw new InvalidDataException($"{text} could not be converted into decimal degrees"); }
                    minutes /= 60;
                    return degrees + minutes;
                }
            }
        }
    }
}

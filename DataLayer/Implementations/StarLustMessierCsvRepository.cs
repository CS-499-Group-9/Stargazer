using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DataLayer.Interfaces;
using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Implementations
{
    internal class StarLustMessierCsvRepository : IMessierRepository
    {
        private readonly string baseDirectoryPath;
        public StarLustMessierCsvRepository(string baseDirectoryPath)
        {
            this.baseDirectoryPath = baseDirectoryPath;
        }

        public Task<IEnumerable<EquitorialMessierObject>> GetRawMessierObjectsAsync()
        {
            DirectoryInfo dir = new DirectoryInfo(baseDirectoryPath);
            string dataLayer = Path.Combine(dir.Parent.FullName, "DataLayer");
            string repositories = Path.Combine(dataLayer, "Repositories");
            string filePath = Path.Combine(repositories, "messier-catalog.csv");

            if (File.Exists(filePath))
            {
                // Build a task to retrieve the raw messier objects from the repository and return the task to the calling code
                return new Task<IEnumerable<EquitorialMessierObject>>(() =>
                {
                    
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        Console.WriteLine($"CsvMessierRepository ({Task.CurrentId}): Parallel Retrieve Request");

                        // the CsvHelper.GetRecords retrieve a yeildable list. Objects are not read until they are requested
                        var records = csv.GetRecords<EquitorialMessierObject>();

                        // Create a new thread for each row in the csv and map that row to a new object
                        return records.ToList();
                    }
                });
            }
            throw new FileNotFoundException();
        }

        private class StarMap : ClassMap<EquitorialMessierObject>
        {
            public StarMap()
            {
                Map(m => m.MessierId).Name("M");
                Map(m => m.NewGeneralCatalog).Name("NGC");
                Map(m=> m.Type).Name("TYPE");
                Map(m => m.Constellation).Name("CONS");
                Map(m => m.RightAscention).TypeConverter<HmsToDecimalDegreesConverter>().Name("RA");
                Map(m => m.Declination).TypeConverter<DdmToDecimalDegreesConverter>().Name("DEC");
                Map(m => m.Magnitude).Name("MAG");
                Map(m => m.Size).Name("SIZE");
                Map(m => m.Distance).Name("DIST (ly)");
                Map(m => m.ViewingSeason).Name("VIEWING SEASON");
                Map(m => m.ViewingDifficulty).Name("VIEWING DIFFICULTY");
            }

            private class DdmToDecimalDegreesConverter : DefaultTypeConverter
            {
                public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
                {
                    var degDecMin = text.Split(' ');
                    if (
                        !(
                            double.TryParse(degDecMin[0].Remove(degDecMin[0].Length -1), out double hours) &&
                            double.TryParse(degDecMin[1].Remove(degDecMin[1].Length - 1), out double minutes)
                          )
                        )
                    {
                        throw new InvalidDataException($"{text} could not be converted to decmial degrees.");
                    }
                    hours = (hours / 24) * 360;
                    minutes /= 60;
                    return hours + minutes;
                }
            }

            private class HmsToDecimalDegreesConverter : DefaultTypeConverter
            {
                public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
                {
                    var degreesMinutes = text.Split("°");
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

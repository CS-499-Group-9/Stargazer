using DataLayer.EquitorialObjects;
using DataLayer.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Implementations
{
    internal class StellariumJsonConstellationRepository : IConstellationRepository
    {
        private readonly string filePath;

        public StellariumJsonConstellationRepository(string repositoryPath)
        {
            this.filePath = Path.Combine(repositoryPath, "constellations.json"); ;
        }

        Task<IList<Constellation>> IConstellationRepository.GetAllConstellationsAsync()
        {

            if (File.Exists(filePath))
            {
                return Task<IList<Constellation>>.Factory.StartNew(() =>
                {
                    // Null values are not handled because the program SHOULD break if this does not work. 
                    // TODO: May need to consider a more elegant failure for production version.
                    string jsonContent = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<Constellation>>(jsonContent, new JsonConstellationListConverter());
                });
            }
            throw new FileNotFoundException();
 

        }



        private class JsonConstellationListConverter : JsonConverter<IList<Constellation>>
        {

            public override IList<Constellation>? ReadJson(JsonReader reader, Type objectType, IList<Constellation>? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject obj = JObject.Load(reader);
                var jsonConstellations = (JArray)obj["constellations"];
                List<Tuple<int, int>> constellationlines = new List<Tuple<int, int>>();
                List<Constellation> constellations = new ();
                foreach (var constellation in jsonConstellations)
                {
                    ConstellationName name = constellation["common_name"].ToObject<ConstellationName>();


                    var lines = BuildLines((JArray)constellation["lines"], new List<Tuple<int,int>>());
                    Constellation eqConst = new(constellation["id"].Value<string>(), name.English, name.Native);
                    eqConst.ConstellationLines = lines;
                    constellations.Add(eqConst);
                }
                return constellations;
            }

        private IEnumerable<Tuple<int,int>> BuildLines(JArray jArray, IList<Tuple<int, int>> lines)
        {
            int previousStar = 0;
            foreach (var item in jArray)
            {

                if (item is JArray list) { BuildLines(list, lines); }
                    else 
                    { 
                        var star = item.Value<int>();
                        if (previousStar == 0) {previousStar = star;}
                        else 
                        {
                            lines.Add(Tuple.Create(previousStar, star)); 
                            previousStar = star;
                        }
                    }
            }
            return lines;
        }


            public override void WriteJson(JsonWriter writer, IList<Constellation>? value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        private class ConstellationName
        {
            [JsonProperty("english")]
            public string? English { get; set; }

            [JsonProperty("native")]
            public string? Native { get; set; }
        }

    }
}

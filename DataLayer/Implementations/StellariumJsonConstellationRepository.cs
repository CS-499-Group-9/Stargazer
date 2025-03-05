using DataLayer.EquatorialObjects;
using DataLayer.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Used to retrieve constellation data from the Stellarium Json file.
    /// </summary>
    internal class StellariumJsonConstellationRepository : IConstellationRepository
    {
        /// <summary>
        /// The path to the Stellarium Json file to read from
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of the repository
        /// </summary>
        /// <param name="repositoryPath">The path to the directory containing the Json file.</param>
        public StellariumJsonConstellationRepository(string repositoryPath)
        {
            this.filePath = Path.Combine(repositoryPath, "constellations.json"); ;
        }

        /// <summary>
        /// Asynchronously retrieves a <see cref="IList{Constellation}"/> from the Json file.
        /// </summary>
        /// <returns>An <see cref="IList{Constellation}"/></returns>
        /// <exception cref="FileNotFoundException"></exception>
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


        /// <summary>
        /// Custom <see cref="JsonConverter{Constellation}"/> used to build out the <see cref="Constellation"/> graph during retrieval from the repository
        /// </summary>
        private class JsonConstellationListConverter : JsonConverter<IList<Constellation>>
        {
            public override IList<Constellation>? ReadJson(JsonReader reader, Type objectType, IList<Constellation>? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                // Get all constellations
                JObject obj = JObject.Load(reader);
                // Extract the nested list of int
                var jsonConstellations = (JArray)obj["constellations"];
                // Temporary constellation collection
                List<Constellation> constellations = new ();

                // Loop through the retrieved constellations
                foreach (var constellation in jsonConstellations)
                {
                    // Get the constellation name (and native name) 
                    ConstellationName name = constellation["common_name"].ToObject<ConstellationName>();

                    // Build out the graph of lines
                    var lines = BuildLines((JArray)constellation["lines"], new List<Tuple<int,int>>());

                    // Instantiate the new constellation
                    Constellation eqConst = new(constellation["id"].Value<string>(), name.English, name.Native)
                    {
                        ConstellationLines = lines
                    };
                    // Add the constellation graph to the constellation
                    constellations.Add(eqConst);
                }
                return constellations;
            }

            /// <summary>
            /// Inserts the edges from the <see cref="Constellation"/> graph into memory 
            /// </summary>
            /// <param name="jArray">The array of objects retrieved from the repository (nested)</param>
            /// <param name="lines">The table to insert the graph edges</param>
            /// <returns>A list of all graph edges</returns>
            private static IEnumerable<Tuple<int,int>> BuildLines(JArray jArray, IList<Tuple<int, int>> lines)
            {
                // Star a new branch
                int previousStar = 0;
                // Iterate through the array of retrieved stars
                foreach (var item in jArray)
                {

                    // If the nested item is an array call recursevely to skip to another iteration
                    if (item is JArray list) { BuildLines(list, lines); }
                        else 
                        { 
                            // The current item is a single item
                            // Get the value from the wrapper
                            var star = item.Value<int>();
                            // Check if this is a new branch and skip if true
                            if (previousStar == 0) {previousStar = star;}
                            else 
                            {
                                // This branch is continuing
                                // Add the Tuple &lt; int, int &lt; to the constellation lines
                                lines.Add(Tuple.Create(previousStar, star)); 
                                // reference the previous star
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

        /// <summary>
        /// <see cref="StellariumJsonConstellationRepository"/> internal class to instantiate the constellation name from the Json repository
        /// </summary>
        private class ConstellationName
        {
            [JsonProperty("english")]
            public string? English { get; set; }

            [JsonProperty("native")]
            public string? Native { get; set; }
        }

    }
}

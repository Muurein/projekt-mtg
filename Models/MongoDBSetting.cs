//stores MongoDB settings so they can be used across the application

namespace Mtg.Models
{
    public class MongoDBSetting
    {
        public string AtlasURI { get; set; }

        //database name
        public string MtgIndex { get; set; }
    }
}
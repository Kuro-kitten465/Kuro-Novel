using KuroNovel.DataNode;
using Newtonsoft.Json;

namespace KuroNovel.Asset.Json
{
    public class JsonManager
    {
        private static readonly JsonSerializerSettings m_jsonSettings = new JsonSerializerSettings()
        { 
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        public static VNCharacter CharacterLoader(string key)
        {
            var json = JsonConvert.DeserializeObject<VNCharacter>(key, m_jsonSettings);
            return json ?? null;
        }  

        public static VNSequence SequenceLoader(string key)
        {
            var json = JsonConvert.DeserializeObject<VNSequence>(key, m_jsonSettings);
            return json ?? null;
        }
    }
}
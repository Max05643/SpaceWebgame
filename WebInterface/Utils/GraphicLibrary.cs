using GameDesign.Models;
using GameDesign.Utils;
using System.Text.Json;

namespace WebInterface.Utils
{
    /// <summary>
    /// Stores graphic library entries in memory in a simple way
    /// </summary>
    public class GraphicLibrary : IGraphicLibraryProvider
    {
        Dictionary<string, GraphicLibraryEntry> library = new Dictionary<string, GraphicLibraryEntry>();
        Dictionary<string, uint> namesToIds = new Dictionary<string, uint>();
        uint nextFreeId = 0;

        /// <summary>
        /// Loads graphic library from json file
        /// </summary>
        public static GraphicLibrary LoadFromFile(string path)
        {
            var result = new GraphicLibrary();

            result.library = JsonSerializer.Deserialize<Dictionary<string, GraphicLibraryEntry>>(File.ReadAllText(path))!;
            result.namesToIds = result.library.Select((x, i) => new { id = (uint)i, name = x.Key }).ToDictionary(x => x.name, x => x.id);
            result.nextFreeId = result.namesToIds.Values.Max() + 1;
            return result;
        }

        private GraphicLibrary()
        {

        }

        GraphicLibraryEntry IGraphicLibraryProvider.GetEntry(string name) => library[name];

        IReadOnlyDictionary<uint, GraphicLibraryEntry> IGraphicLibraryProvider.GetLibrary() => library.ToDictionary(pair => namesToIds[pair.Key], pair => pair.Value);

        uint IGraphicLibraryProvider.NameToId(string name) => namesToIds[name];
    }
}

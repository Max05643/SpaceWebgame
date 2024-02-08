using GameDesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Utils
{


    /// <summary>
    /// Represents a storage of GraphicLibraryEntry that are indexed by both names (string) and ids (uint).
    /// Reads can be done thread-safely
    /// </summary>
    public interface IGraphicLibraryProvider
    {
        /// <summary>
        /// Converts string name of the GraphicLibraryEntry to its id
        /// </summary>
        uint NameToId(string name);

        /// <summary>
        /// Returns whole library indexed by ids
        /// </summary>
        IReadOnlyDictionary<uint, GraphicLibraryEntry> GetLibrary();


        /// <summary>
        /// Returns an entry by its name
        /// </summary>
        GraphicLibraryEntry GetEntry(string name);

    }
}

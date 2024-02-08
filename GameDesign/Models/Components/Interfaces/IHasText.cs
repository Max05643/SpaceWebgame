using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components.Interfaces
{
    /// <summary>
    /// Interface for objects that should have displayed text
    /// </summary>
    public interface IHasText
    {
        /// <summary>
        /// Object's displayed text
        /// </summary>
        public string Name { get;}
    }
}

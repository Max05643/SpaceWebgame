using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components.Interfaces
{

    /// <summary>
    /// Interface for objects that should have displayed health value
    /// </summary>
    public interface IHasHealth
    {
        /// <summary>
        /// Object's health
        /// </summary>
        public int Health { get; }

        /// <summary>
        /// Object's max health
        /// </summary>
        public int MaxHealth { get; }
    }
}

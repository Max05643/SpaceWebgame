using Genbox.VelcroPhysics.Collision.ContactSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Represents a collision between two objects
    /// </summary>
    public class Collision
    {
        public readonly GameObject gameObjectA;
        public readonly GameObject gameObjectB;
        public readonly Contact contact;

        public Collision(GameObject gameObjectA, GameObject gameObjectB, Contact contact)
        {
            this.contact = contact;
            this.gameObjectA = gameObjectA;
            this.gameObjectB = gameObjectB;
        }
    }
}



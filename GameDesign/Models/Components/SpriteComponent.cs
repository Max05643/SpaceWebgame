using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameDesign.Models.GraphicInfo;

namespace GameDesign.Models.Components
{
    /// <summary>
    /// Stores object sprite's name
    /// </summary>
    public class SpriteComponent : Component
    {
        
        /// <summary>
        /// Info about object's graphics
        /// </summary>
        public GraphicInfo CurrentGraphicInfo { get; private set; }


        public SpriteComponent(GameObject parentObject) : base(parentObject)
        {
            CurrentGraphicInfo = new GraphicInfo();
        }


        /// <summary>
        /// Setups new target size for graphic
        /// </summary>
        public void SetupTargetSize(Vector2 size)
        {
            CurrentGraphicInfo.TargetSize = size;
        }
        
        /// <summary>
        /// Changes graphic library entry associated with this component.
        /// Will change object's animation info to null
        /// </summary>
        public void SetupGraphicLibraryEntryName(string name)
        {
            CurrentGraphicInfo.GraphicLibraryEntryName = name;
            CurrentGraphicInfo.ObjectAnimationInfo = null;
        }

        /// <summary>
        /// Makes this object completely invisible.
        /// Will change object's animation info to null
        /// </summary>
        public void SetupNothing()
        {
            CurrentGraphicInfo.GraphicLibraryEntryName = "None";
            CurrentGraphicInfo.ObjectAnimationInfo = null;
        }


        public void SetAnimationInfo(AnimationInfo? animationInfo)
        {
            CurrentGraphicInfo.ObjectAnimationInfo = animationInfo; 
        }
    }
}

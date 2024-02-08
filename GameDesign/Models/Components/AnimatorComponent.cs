using GameDesign.Utils;
using GameServerDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components
{
    /// <summary>
    /// Basic class for managing animations
    /// </summary>
    public class AnimatorComponent : Component
    {

        /// <summary>
        /// Represents a state of animation
        /// </summary>
        public class AnimationNode
        {
            public string GraphicLibraryEntryName { get; private set; }

            public AnimationNode(string graphicLibraryEntryName, string? nextAnimationNode = null)
            {
                GraphicLibraryEntryName = graphicLibraryEntryName;
                NextAnimationNode = nextAnimationNode;
            }

            /// <summary>
            /// The animator will automatically switch the animation if this is specified and GraphicInfo is animated sprite without loop
            /// </summary>
            public string? NextAnimationNode { get; set; } = null;
        }

        /// <summary>
        /// All the possible states of animation
        /// </summary>
        protected Dictionary<string, AnimationNode> AnimationNodes { get; set; } = new Dictionary<string, AnimationNode>();

        /// <summary>
        /// Current animation's time
        /// </summary>
        protected float completedTimeInSeconds = 0;

        /// <summary>
        /// Current animation's name. Can be null if no animation is playing
        /// </summary>
        public string? CurrentAnimation => currentAnimationNode;

        /// <summary>
        /// Current animation node. Can be null if no animation is playing
        /// </summary>
        protected string? currentAnimationNode = null;

        protected readonly SpriteComponent spriteComponent;


        /// <summary>
        /// Is called when current animation is ended. The argument is AnimationNode's name
        /// </summary>
        public event Action<string>? OnAnimationEnded = null;
        /// <summary>
        /// Is called when current animation is started. The argument is AnimationNode's name
        /// </summary>
        public event Action<string>? OnAnimationStarted = null;


        /// <summary>
        /// Graphic library is used to retrieve information about animation's length, etc
        /// </summary>
        readonly IGraphicLibraryProvider graphicLibrary;




        public AnimatorComponent(GameObject parentObject, IGraphicLibraryProvider graphicLibrary) : base(parentObject)
        {

            this.graphicLibrary = graphicLibrary;

            if (!parentObject.HasComponent<SpriteComponent>())
            {
                throw new InvalidOperationException("AnimatorComponent can only work when SpriteComponent is attached");
            }

            spriteComponent = parentObject.GetComponent<SpriteComponent>();
        }

        /// <summary>
        /// Adds possible animation node
        /// </summary>
        public void AddAnimationNode(AnimationNode animationNode, string name)
        {
            AnimationNodes.Add(name, animationNode);
        }


        /// <summary>
        /// Immediately starts playing the specified animation node from the beginning if it not yet playing
        /// </summary>
        public void StartAnimationIfNotStartedYet(string nodeName)
        {
            if (currentAnimationNode != nodeName)
            {
                StartAnimation(nodeName);
            }
        }

        /// <summary>
        /// Immediately starts playing the specified animation node from the beginning
        /// </summary>
        public virtual void StartAnimation(string nodeName)
        {
            currentAnimationNode = nodeName;

            var currentNode = AnimationNodes[currentAnimationNode];
            var currentGraphicLibraryEntry = graphicLibrary.GetEntry(currentNode.GraphicLibraryEntryName);


            if (currentGraphicLibraryEntry.Type == GraphicLibraryEntry.GraphicType.Animated)
            {
                completedTimeInSeconds = 0;
            }


            SendCurrentGraphicInfoToSpriteComponent();

            OnAnimationStarted?.Invoke(currentAnimationNode);
        }


        protected virtual void SendCurrentGraphicInfoToSpriteComponent()
        {
            if (currentAnimationNode == null)
            {
                spriteComponent.SetupNothing();
            }
            else
            {
                var currentNode = AnimationNodes[currentAnimationNode];
                var currentGraphicLibraryEntry = graphicLibrary.GetEntry(currentNode.GraphicLibraryEntryName);

                spriteComponent.SetupGraphicLibraryEntryName(currentNode.GraphicLibraryEntryName);


                // Send information about time that passed since the beginning of the animation clip if the clip is not looped 
                if (currentGraphicLibraryEntry.Type == GraphicLibraryEntry.GraphicType.Animated && !currentGraphicLibraryEntry.AnimatedSpriteInfo!.IsLoop)
                {
                    spriteComponent.SetAnimationInfo(new GraphicInfo.AnimationInfo() { SecondsCompleted = completedTimeInSeconds });
                }
            }
        }

        protected virtual void HandleAnimationEnd()
        {
            if (currentAnimationNode != null)
            {
                OnAnimationEnded?.Invoke(currentAnimationNode);

                var currentNode = AnimationNodes[currentAnimationNode];
                var currentGraphicLibraryEntry = graphicLibrary.GetEntry(currentNode.GraphicLibraryEntryName);

                if (currentGraphicLibraryEntry.Type == GraphicLibraryEntry.GraphicType.Animated && !currentGraphicLibraryEntry.AnimatedSpriteInfo!.IsLoop && currentNode.NextAnimationNode != null)
                {
                    StartAnimation(currentNode.NextAnimationNode);
                }
            }
        }

        public override void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            if (currentAnimationNode == null)
                return;

            var currentNode = AnimationNodes[currentAnimationNode];
            var currentGraphicLibraryEntry = graphicLibrary.GetEntry(currentNode.GraphicLibraryEntryName);

            if (currentGraphicLibraryEntry.Type == GraphicLibraryEntry.GraphicType.Animated && !currentGraphicLibraryEntry.AnimatedSpriteInfo!.IsLoop)
            {
                completedTimeInSeconds += deltaTime;
                if (completedTimeInSeconds >= currentGraphicLibraryEntry.AnimatedSpriteInfo!.TargetTimeInSeconds)
                {
                    HandleAnimationEnd();
                }
                else
                {
                    SendCurrentGraphicInfoToSpriteComponent();
                }
            }
        }
    }
}

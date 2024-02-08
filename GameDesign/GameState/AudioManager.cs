using Genbox.VelcroPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDesign.Models;

namespace GameDesign.GameState
{

    /// <summary>
    /// Controls the audio played in game
    /// </summary>
    public class AudioManager
    {

        private class ClipsQueueElement
        {
            public SoundEffect effect;
            public float timeToRemove;

            public ClipsQueueElement(SoundEffect effect, float timeToRemove)
            {
                this.effect = effect;
                this.timeToRemove = timeToRemove;
            }
        }

        /// <summary>
        /// Time after the clip will be removed from queue
        /// </summary>
        const float timeToLive = 1f;

        ulong lastFreeId = 0;

        readonly List<ClipsQueueElement> clipsQueue = new List<ClipsQueueElement>();

        readonly GameStateManager gameStateManager;

        public AudioManager(GameStateManager gameStateManager)
        {
            this.gameStateManager = gameStateManager;
        }

        /// <summary>
        /// Clear the list of current audio clips. Should be called at the beginning of every frame
        /// </summary>
        public void ClearCurrentFrameClips(float deltaTime)
        {
            if (clipsQueue.Count > 0)
            {
                clipsQueue[0].timeToRemove -= deltaTime;
            }

            while (clipsQueue.Count > 0)
            {
                var lastClip = clipsQueue.First();
                if (lastClip.timeToRemove <= float.Epsilon)
                {
                    clipsQueue.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
        }

        public void AddAudioClipForTheCurrentFrame(SoundEffect newClip)
        {
            newClip.Id = ++lastFreeId;
            clipsQueue.Add(new ClipsQueueElement(newClip, timeToLive - clipsQueue.Sum(clip => clip.timeToRemove)));
        }

        public IEnumerable<SoundEffect> GetCurrentFrameClips() => clipsQueue.Select(clip => clip.effect);


    }
}

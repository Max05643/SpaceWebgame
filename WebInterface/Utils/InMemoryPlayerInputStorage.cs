using GameDesign.Utils;
using GameServerDefinitions;
using GameServerImplementation;
using System.Collections.Concurrent;

namespace WebInterface.Utils
{
    /// <summary>
    /// Stores players' input in memory thread-safely
    /// </summary>
    public class InMemoryPlayerInputStorage<PlayerInput> : PlayerInputStorage<PlayerInput>
    {

        class InputEntry
        {
            public PlayerInput LastInput { get; set; }

            public DateTime LastInputTime { get; set; }

            public InputEntry(PlayerInput lastInput, DateTime lastInputTime)
            {
                LastInput = lastInput;
                LastInputTime = lastInputTime;
            }
        }

        readonly ConcurrentDictionary<PlayerId, InputEntry> inputStorage = new ConcurrentDictionary<PlayerId, InputEntry>();

        public InMemoryPlayerInputStorage(IPlayerInputProcessor<PlayerInput> playerInputProcessor) : base(playerInputProcessor)
        {
        }

        public override void DisposePlayer(PlayerId playerId)
        {
            inputStorage.TryRemove(playerId, out _);
        }

        public override PlayerInput PopPlayerInput(PlayerId playerId)
        {
            if (inputStorage.TryGetValue(playerId, out InputEntry? value))
            {
                lock (value)
                {
                    var result = playerInputProcessor.PopInput(value.LastInput, out PlayerInput newStoredInput);
                    value.LastInput = newStoredInput;
                    return result;
                }
            }
            else
            {
                return playerInputProcessor.GetDefaultInput();
            }
        }

        public override void StoreNewInput(PlayerInput newInput, PlayerId playerId)
        {
            if (inputStorage.TryGetValue(playerId, out InputEntry? value))
            {
                lock (value)
                {
                    value.LastInput = playerInputProcessor.StoreNewInput(value.LastInput, newInput);
                    value.LastInputTime = DateTime.Now;
                }
            }
            else
            {
                inputStorage.TryAdd(playerId, new InputEntry(newInput, DateTime.Now));
            }
        }

        public override DateTime? GetLastInputTime(PlayerId playerId)
        {
            if (inputStorage.TryGetValue(playerId, out InputEntry? value))
            {
                lock (value)
                {
                    return value.LastInputTime;
                }
            }
            else
            {
                return null;
            }
        }
    }

    public class InMemoryPlayerInputStorageFactory<PlayerInput> : IPlayerInputStorageFactory<PlayerInput>
    {
        PlayerInputStorage<PlayerInput> IPlayerInputStorageFactory<PlayerInput>.CreateNewStorage(IPlayerInputProcessor<PlayerInput> playerInputProcessor)
        {
            return new InMemoryPlayerInputStorage<PlayerInput>(playerInputProcessor);
        }
    }
}

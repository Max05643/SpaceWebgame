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

            public InputEntry(PlayerInput lastInput)
            {
                LastInput = lastInput;
            }
        }

        readonly ConcurrentDictionary<PlayerId, InputEntry> inputStorage = new ConcurrentDictionary<PlayerId, InputEntry>();

        public InMemoryPlayerInputStorage(IPlayerInputProcessor<PlayerInput> playerInputProcessor) : base(playerInputProcessor)
        {
        }

        public override void DisposePlayer(PlayerId playerId)
        {
            throw new NotImplementedException();
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
                }
            }
            else
            {
                inputStorage.TryAdd(playerId, new InputEntry(newInput));
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

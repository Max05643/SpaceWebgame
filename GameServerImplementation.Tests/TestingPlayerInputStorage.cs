using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.Tests
{


    public class TestingPlayerInputStorageFactory<PlayerInput> : IPlayerInputStorageFactory<PlayerInput>
    {
        PlayerInputStorage<PlayerInput> IPlayerInputStorageFactory<PlayerInput>.CreateNewStorage(IPlayerInputProcessor<PlayerInput> playerInputProcessor)
        {
            return new TestingPlayerInputStorage<PlayerInput>(playerInputProcessor);
        }
    }

    /// <summary>
    /// In-memory storage of players' input for testing purposes. Does not provide thread-safety
    /// </summary>
    public class TestingPlayerInputStorage<PlayerInput> : PlayerInputStorage<PlayerInput>
    {

        public Dictionary<PlayerId, PlayerInput> storage = new Dictionary<PlayerId, PlayerInput>();

        public TestingPlayerInputStorage(IPlayerInputProcessor<PlayerInput> playerInputProcessor) : base(playerInputProcessor)
        {
        }

        public override void DisposePlayer(PlayerId playerId)
        {
            storage.Remove(playerId);
        }

        public override DateTime? GetLastInputTime(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        public override PlayerInput PopPlayerInput(PlayerId playerId)
        {
            if (!storage.ContainsKey(playerId))
                storage.Add(playerId, playerInputProcessor.GetDefaultInput());

            var oldStoredValue = storage[playerId];
            var valueToReturn = playerInputProcessor.PopInput(oldStoredValue, out PlayerInput newStoredValue);
            storage[playerId] = newStoredValue;

            return valueToReturn;
        }

        public override void StoreNewInput(PlayerInput newInput, PlayerId playerId)
        {
            if (!storage.ContainsKey(playerId))
                storage.Add(playerId, playerInputProcessor.GetDefaultInput());

            var oldStoredValue = storage[playerId];
            storage[playerId] = playerInputProcessor.StoreNewInput(oldStoredValue, newInput);
        }
    }
}

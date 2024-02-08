using Boxed.Mapping;
using GameDesign.Models;
using WebInterface.ClientModels;

namespace WebInterface.Mappers
{
    public class GameStateMapper : IMapper<PlayerUpdate, ClientGameState>
    {

        readonly IMapper<PlayerUpdate.PlayerGameObject, ClientGameObject> gameObjMapper;
        readonly IMapper<PlayerUpdate.PlayerSoundEffect, ClientGameState.ClientSoundEffect> soundEffectMapper;
        public GameStateMapper(IMapper<PlayerUpdate.PlayerGameObject, ClientGameObject> gameObjMapper, IMapper<PlayerUpdate.PlayerSoundEffect, ClientGameState.ClientSoundEffect> soundEffectMapper)
        {
            this.gameObjMapper = gameObjMapper;
            this.soundEffectMapper = soundEffectMapper;
        }

        void IMapper<PlayerUpdate, ClientGameState>.Map(PlayerUpdate source, ClientGameState destination)
        {
            destination.State = source.State;
            destination.Points = source.Points;
            destination.MaxHealth = source.MaxHealth;
            destination.AlreadyInvested = source.AlreadyInvested;
            destination.GameObjectsId = source.GameObjectsId;
            destination.Health = source.Health;
            destination.IsSafeZone = source.IsSafeZone;
            destination.PlayersCount = source.PlayersCount;
            destination.SoundEffectsQueue = soundEffectMapper.MapList(source.SoundEffectsQueue);
            destination.Objects = source.Objects.ToDictionary(pair => pair.Key, pair => gameObjMapper.Map(pair.Value));
        }
    }
}

using Boxed.Mapping;
using GameDesign.Models;
using WebInterface.ClientModels;

namespace WebInterface.Mappers
{
    public class SoundEffectMapper : IMapper<PlayerUpdate.PlayerSoundEffect, ClientGameState.ClientSoundEffect>
    {
        void IMapper<PlayerUpdate.PlayerSoundEffect, ClientGameState.ClientSoundEffect>.Map(PlayerUpdate.PlayerSoundEffect source, ClientGameState.ClientSoundEffect destination)
        {
            destination.Position = source.Position;
            destination.Radius = source.Radius;
            destination.AudioClipName = source.AudioClipName;
            destination.Id = source.Id;
        }
    }
}

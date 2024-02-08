using Boxed.Mapping;
using GameDesign.Models;
using WebInterface.ClientModels;

namespace WebInterface.Mappers
{
    public class GameObjectMapper : IMapper<PlayerUpdate.PlayerGameObject, ClientGameObject>
    {
        /// <summary>
        /// Used to map full features' names to short ones for optimization during data transfer to client
        /// </summary>
        static readonly Dictionary<string, string> featuresNamesMap = new()
        {
            ["health"] = "_h",
            ["text"] = "_tx"
        };

        public void Map(PlayerUpdate.PlayerGameObject source, ClientGameObject destination)
        {
            destination.GraphicInfo = new ClientGraphicInfo
            {
                GraphicLibraryEntryId = source.GraphicInfo.GraphicLibraryEntryId,
                TargetSize = source.GraphicInfo.TargetSize,
                ObjectAnimationInfo = source.GraphicInfo.ObjectAnimationInfo == null ? null : new ClientGraphicInfo.ClientAnimationInfo() { SecondsCompleted = source.GraphicInfo.ObjectAnimationInfo.SecondsCompleted }
            };


            destination.Position = source.Position;
            destination.Angle = source.Angle;
            destination.Velocity = source.Velocity;
            destination.Features = source.Features.ToDictionary(pair => featuresNamesMap[pair.Key], pair => pair.Value);
            destination.Children = source.Children == null || source.Children.Count == 0 ? null : source.Children.ToDictionary(pair => pair.Key, pair => this.Map(pair.Value));
        }
    }
}

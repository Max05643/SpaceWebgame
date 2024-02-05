using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.Tests
{
    public class GameServerTests
    {

        [Fact]
        public void CanAddPlayerToTheGame()
        {
            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.SetupGet(g => g.CurrentPlayers).Returns(playersStorageMock);
            gameStateMock.Setup(g => g.IsPlayerInGame(It.IsAny<PlayerId>())).Returns((PlayerId id) => playersStorageMock.Contains(id));
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });

            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, settings));


            gameServer.AddPlayer(playerId);


            gameServer.CurrentPlayers.ShouldContain(playerId);
            gameServer.IsPlayerInGame(playerId).ShouldBeTrue();
        }


    }
}

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
        public void CanStartTheGame()
        {
            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.SetupGet(g => g.CurrentPlayers).Returns(playersStorageMock);
            gameStateMock.Setup(g => g.IsPlayerInGame(It.IsAny<PlayerId>())).Returns((PlayerId id) => playersStorageMock.Contains(id));
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });
            gameStateMock.Setup(g => g.RemovePlayer(It.IsAny<PlayerId>())).Callback((PlayerId id) => playersStorageMock.Remove(id));

            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();
            playerInputProcessorMock.Setup(p => p.StoreNewInput(It.IsAny<string>(), It.IsAny<string>())).Returns((string oldI, string newI) => { return newI; });
            playerInputProcessorMock.Setup(p => p.GetDefaultInput()).Returns("default");
            string s = "";
            playerInputProcessorMock.Setup(p => p.PopInput(It.IsAny<string>(), out s)).Returns((string oldI, out string newI) => { newI = oldI; return oldI; });

            var testingInputStorage = new TestingPlayerInputStorage<string>(playerInputProcessorMock.Object);

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage));


            gameServer.IsRunning.ShouldBeTrue();
        }

        [Fact]
        public async Task CanAddPlayerToTheGame()
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

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();
            playerInputProcessorMock.Setup(p => p.StoreNewInput(It.IsAny<string>(), It.IsAny<string>())).Returns((string oldI, string newI) => { return newI; });
            playerInputProcessorMock.Setup(p => p.GetDefaultInput()).Returns("default");
            string s = "";
            playerInputProcessorMock.Setup(p => p.PopInput(It.IsAny<string>(), out s)).Returns((string oldI, out string newI) => { newI = oldI; return oldI; });

            var testingInputStorage = new TestingPlayerInputStorage<string>(playerInputProcessorMock.Object);

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage));

            gameServer.AddPlayer(playerId);


            await Task.Delay(100);

            gameServer.CurrentPlayers.ShouldContain(playerId);
            gameServer.IsPlayerInGame(playerId).ShouldBeTrue();
        }

        [Fact]
        public async Task CanKickPlayerFromTheGame()
        {
            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.SetupGet(g => g.CurrentPlayers).Returns(playersStorageMock);
            gameStateMock.Setup(g => g.IsPlayerInGame(It.IsAny<PlayerId>())).Returns((PlayerId id) => playersStorageMock.Contains(id));
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });
            gameStateMock.Setup(g => g.RemovePlayer(It.IsAny<PlayerId>())).Callback((PlayerId id) => playersStorageMock.Remove(id));

            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();
            playerInputProcessorMock.Setup(p => p.StoreNewInput(It.IsAny<string>(), It.IsAny<string>())).Returns((string oldI, string newI) => { return newI; });
            playerInputProcessorMock.Setup(p => p.GetDefaultInput()).Returns("default");
            string s = "";
            playerInputProcessorMock.Setup(p => p.PopInput(It.IsAny<string>(), out s)).Returns((string oldI, out string newI) => { newI = oldI; return oldI; });

            var testingInputStorage = new TestingPlayerInputStorage<string>(playerInputProcessorMock.Object);

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage));

            gameServer.AddPlayer(playerId);
            gameServer.KickPlayer(playerId);

            await Task.Delay(100);

            gameServer.CurrentPlayers.ShouldNotContain(playerId);
            gameServer.IsPlayerInGame(playerId).ShouldBeFalse();
            playersCommunicationMock.Verify(p => p.NotifyPlayerThatHeIsKicked(It.IsAny<PlayerId>()), Times.Once);
        }

        [Fact]
        public async Task CanLeaveTheGame()
        {
            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.SetupGet(g => g.CurrentPlayers).Returns(playersStorageMock);
            gameStateMock.Setup(g => g.IsPlayerInGame(It.IsAny<PlayerId>())).Returns((PlayerId id) => playersStorageMock.Contains(id));
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });
            gameStateMock.Setup(g => g.RemovePlayer(It.IsAny<PlayerId>())).Callback((PlayerId id) => playersStorageMock.Remove(id));

            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();
            playerInputProcessorMock.Setup(p => p.StoreNewInput(It.IsAny<string>(), It.IsAny<string>())).Returns((string oldI, string newI) => { return newI; });
            playerInputProcessorMock.Setup(p => p.GetDefaultInput()).Returns("default");
            string s = "";
            playerInputProcessorMock.Setup(p => p.PopInput(It.IsAny<string>(), out s)).Returns((string oldI, out string newI) => { newI = oldI; return oldI; });

            var testingInputStorage = new TestingPlayerInputStorage<string>(playerInputProcessorMock.Object);

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage));

            gameServer.AddPlayer(playerId);
            gameServer.LeaveGame(playerId);

            await Task.Delay(100);

            gameServer.CurrentPlayers.ShouldNotContain(playerId);
            gameServer.IsPlayerInGame(playerId).ShouldBeFalse();
            playersCommunicationMock.Verify(p => p.NotifyPlayerThatHeIsKicked(It.IsAny<PlayerId>()), Times.Never);
        }

        [Fact]
        public async Task CanSendPlayerInput()
        {
            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.SetupGet(g => g.CurrentPlayers).Returns(playersStorageMock);
            gameStateMock.Setup(g => g.IsPlayerInGame(It.IsAny<PlayerId>())).Returns((PlayerId id) => playersStorageMock.Contains(id));
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });
            gameStateMock.Setup(g => g.RemovePlayer(It.IsAny<PlayerId>())).Callback((PlayerId id) => playersStorageMock.Remove(id));

            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();
            playerInputProcessorMock.Setup(p => p.StoreNewInput(It.IsAny<string>(), It.IsAny<string>())).Returns((string oldI, string newI) => { return newI; });
            playerInputProcessorMock.Setup(p => p.GetDefaultInput()).Returns("default");
            string s = "";
            playerInputProcessorMock.Setup(p => p.PopInput(It.IsAny<string>(), out s)).Returns((string oldI, out string newI) => { newI = oldI; return oldI; });

            var testingInputStorage = new TestingPlayerInputStorage<string>(playerInputProcessorMock.Object);

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage));

            ((IGameServer<IGameState<string, string>, string, string>)gameServer).AddPlayer(playerId);
            ((IGameServer<IGameState<string, string>, string, string>)gameServer).AcceptPlayerInput("input1", playerId);

            await Task.Delay(100);

            ((IGameController<string, string>)gameServer).PopPlayerInput(playerId).ShouldBe("input1");
            playerInputProcessorMock.Verify(p => p.GetDefaultInput(), Times.Once);
            playerInputProcessorMock.Verify(p => p.StoreNewInput(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            playerInputProcessorMock.Verify(p => p.PopInput(It.IsAny<string>(), out s), Times.Once);
        }

        [Fact]
        public async Task CanTick()
        {
            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });
            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();

            var testingInputStorage = new TestingPlayerInputStorage<string>(playerInputProcessorMock.Object);

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage));

            await Task.Delay(150);

            gameStateMock.Verify(c => c.Tick(It.IsAny<TimeSpan>(), It.IsAny<IGameController<string, string>>()), Times.AtLeastOnce);
        }
        [Fact]
        public async Task CanSendUpdates()
        {
            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });
            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();

            var testingInputStorage = new TestingPlayerInputStorage<string>(playerInputProcessorMock.Object);

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage));

            ((IGameController<string, string>)gameServer).SendUpdate("update", playerId);

            await Task.Delay(100);

            playersCommunicationMock.Verify(c => c.SendUpdate(It.Is<string>(s => s == "update"), It.IsAny<PlayerId>()), Times.AtLeastOnce);
        }
    }
}

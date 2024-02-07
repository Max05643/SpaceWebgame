using Microsoft.Extensions.Logging;
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

            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

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

            var testingInputStorage = new TestingPlayerInputStorageFactory<string>();

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage, loggerMock.Object));


            gameServer.IsRunning.ShouldBeTrue();
        }

        [Fact]
        public async void CanStopTheGame()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

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

            var testingInputStorage = new TestingPlayerInputStorageFactory<string>();

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage, loggerMock.Object));


            gameServer.StopGame();

            await Task.Delay(100);

            gameServer.IsRunning.ShouldBeFalse();
        }

        [Fact]
        public async Task CanAddPlayerToTheGame()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

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

            var testingInputStorage = new TestingPlayerInputStorageFactory<string>();

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage, loggerMock.Object));

            await gameServer.AddPlayer(playerId);


            await Task.Delay(100);

            (await gameServer.GetCurrentPlayers()).ShouldContain(playerId);
            (await gameServer.IsPlayerInGame(playerId)).ShouldBeTrue();
        }

        [Fact]
        public async Task CanKickPlayerFromTheGame()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

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

            var testingInputStorage = new TestingPlayerInputStorageFactory<string>();

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage, loggerMock.Object));

            await gameServer.AddPlayer(playerId);
            await gameServer.KickPlayer(playerId);

            await Task.Delay(100);

            (await gameServer.GetCurrentPlayers()).ShouldNotContain(playerId);
            (await gameServer.IsPlayerInGame(playerId)).ShouldBeFalse();
            playersCommunicationMock.Verify(p => p.NotifyPlayerThatHeIsKicked(It.IsAny<PlayerId>()), Times.Once);
            playersCommunicationMock.Verify(p => p.DisposePlayerConnection(It.IsAny<PlayerId>()), Times.Once);
        }

        [Fact]
        public async Task CanLeaveTheGame()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

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

            var testingInputStorage = new TestingPlayerInputStorageFactory<string>();

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage, loggerMock.Object));

            await gameServer.AddPlayer(playerId);
            await gameServer.LeaveGame(playerId);

            await Task.Delay(100);

            (await gameServer.GetCurrentPlayers()).ShouldNotContain(playerId);
            (await gameServer.IsPlayerInGame(playerId)).ShouldBeFalse();
            playersCommunicationMock.Verify(p => p.NotifyPlayerThatHeIsKicked(It.IsAny<PlayerId>()), Times.Never);
            playersCommunicationMock.Verify(p => p.DisposePlayerConnection(It.IsAny<PlayerId>()), Times.Once);
        }

        [Fact]
        public async Task CanSendPlayerInput()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

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

            var testingInputStorage = new TestingPlayerInputStorageFactory<string>();

            var gameServer = (new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage, loggerMock.Object));

            await ((IGameServer<IGameState<string, string>, string, string>)gameServer).AddPlayer(playerId);
            await ((IGameServer<IGameState<string, string>, string, string>)gameServer).AcceptPlayerInput("input1", playerId);

            await Task.Delay(100);

            ((IGameController<string, string>)gameServer).PopPlayerInput(playerId).ShouldBe("input1");
            playerInputProcessorMock.Verify(p => p.GetDefaultInput(), Times.Once);
            playerInputProcessorMock.Verify(p => p.StoreNewInput(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            playerInputProcessorMock.Verify(p => p.PopInput(It.IsAny<string>(), out s), Times.Once);
        }


        [Fact]
        public async Task CanStorePlayerInput()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

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

            var playerInputFactoryMock = new Mock<IPlayerInputStorageFactory<string>>();
            var playerInputStorageMock = new Mock<PlayerInputStorage<string>>();
            playerInputStorageMock.Setup(s => s.PopPlayerInput(It.IsAny<PlayerId>())).Returns("input1");
            playerInputFactoryMock.Setup(f => f.CreateNewStorage(It.IsAny<IPlayerInputProcessor<string>>())).Returns(playerInputStorageMock.Object);

            var gameServer = (new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, playerInputFactoryMock.Object, loggerMock.Object));

            await ((IGameServer<IGameState<string, string>, string, string>)gameServer).AddPlayer(playerId);
            await ((IGameServer<IGameState<string, string>, string, string>)gameServer).AcceptPlayerInput("input1", playerId);

            await Task.Delay(100);

            playerInputStorageMock.Verify(s => s.StoreNewInput(It.IsAny<string>(), It.Is<PlayerId>(id => id == playerId)), Times.Once);
            playerInputStorageMock.Verify(s => s.DisposePlayer(It.IsAny<PlayerId>()), Times.Never);
        }
       
        [Fact]
        public async Task CanDisposeStoredPlayerInput()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

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

            var playerInputFactoryMock = new Mock<IPlayerInputStorageFactory<string>>();
            var playerInputStorageMock = new Mock<PlayerInputStorage<string>>();
            playerInputStorageMock.Setup(s => s.PopPlayerInput(It.IsAny<PlayerId>())).Returns("input1");
            playerInputFactoryMock.Setup(f => f.CreateNewStorage(It.IsAny<IPlayerInputProcessor<string>>())).Returns(playerInputStorageMock.Object);

            var gameServer = (new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, playerInputFactoryMock.Object, loggerMock.Object));

            await ((IGameServer<IGameState<string, string>, string, string>)gameServer).AddPlayer(playerId);
            await ((IGameServer<IGameState<string, string>, string, string>)gameServer).LeaveGame(playerId);

            await Task.Delay(100);

            playerInputStorageMock.Verify(s => s.DisposePlayer(It.Is<PlayerId>(id => id == playerId)), Times.Once);
        }

        [Fact]
        public async Task CanTick()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });
            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();

            var testingInputStorage = new TestingPlayerInputStorageFactory<string>();

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage, loggerMock.Object));

            await Task.Delay(150);

            gameStateMock.Verify(c => c.Tick(It.IsAny<TimeSpan>(), It.IsAny<IGameController<string, string>>()), Times.AtLeastOnce);
        }
        [Fact]
        public async Task CanSendUpdates()
        {
            var loggerMock = new Mock<ILogger<GameServer<IGameState<string, string>, string, string>>>();

            PlayerId playerId = PlayerId.NewGuid();

            var settings = new GameServerSettings();
            var playersCommunicationMock = new Mock<IPlayersCommunication<string>>();
            var gameStateMock = new Mock<IGameState<string, string>>();

            var playersStorageMock = new List<PlayerId>();
            gameStateMock.Setup(g => g.AddPlayer(It.IsAny<PlayerId>())).Returns((PlayerId id) => { playersStorageMock.Add(id); return true; });
            var gameStateFactoryMock = new Mock<IGameStateFactory<IGameState<string, string>, string, string>>();
            gameStateFactoryMock.Setup(f => f.StartNewGame()).Returns(gameStateMock.Object);

            var playerInputProcessorMock = new Mock<IPlayerInputProcessor<string>>();

            var testingInputStorage = new TestingPlayerInputStorageFactory<string>();

            var gameServer = (IGameServer<IGameState<string, string>, string, string>)(new GameServer<IGameState<string, string>, string, string>(gameStateFactoryMock.Object, playersCommunicationMock.Object, playerInputProcessorMock.Object, settings, testingInputStorage, loggerMock.Object));

            ((IGameController<string, string>)gameServer).SendUpdate("update", playerId);

            await Task.Delay(100);

            playersCommunicationMock.Verify(c => c.SendUpdate(It.Is<string>(s => s == "update"), It.IsAny<PlayerId>()), Times.AtLeastOnce);
        }
    }
}

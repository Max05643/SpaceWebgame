﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.ServerEvents
{
    internal class KickPlayerEvent<GameState, PlayerInput, PlayerUpdate> : ServerEvent<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        readonly PlayerId playerId;
        readonly TaskCompletionSource taskCompletionSource;
        public KickPlayerEvent(PlayerId playerId, TaskCompletionSource taskCompletionSource)
        {
            this.playerId = playerId;
            this.taskCompletionSource = taskCompletionSource;
        }


        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            gameState.RemovePlayer(playerId);
            playersCommunication.NotifyPlayerThatHeIsKicked(playerId);
            playerInputStorage.DisposePlayer(playerId);
            playersCommunication.DisposePlayerConnection(playerId);
            taskCompletionSource.SetResult();
        }
    }
}

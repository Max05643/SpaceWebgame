import * as Game from './game';

namespace GamePage {

    declare let playerId: string;
    declare let serverId: string;

    const GameInstance: Game.Game.GameInstance = new Game.Game.GameInstance(playerId, serverId, document);
}
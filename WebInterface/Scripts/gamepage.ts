import * as Game from './game';

namespace GamePage {

    declare let playerId: string;

    const GameInstance: Game.Game.GameInstance = new Game.Game.GameInstance(playerId, document);
}
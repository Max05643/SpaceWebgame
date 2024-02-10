import { Network } from './network';
import { Technical } from './technical';
import { InputUtils } from './inpututils';
import { UI } from './ui';
import { Graphic } from './graphic';
import { Audio } from './audio';
import * as GameDesign from './gamedesign';

export namespace Game {


    /** Represents an instance of the game */
    export class GameInstance {

        playerId: string;

        serverConnection: Network.ServerConnection;
        technicalInfo: Technical.TechnicalInfo = new Technical.TechnicalInfo();
        keyboardInputHandler: InputUtils.KeyboardInputHandler;
        gameUI: UI.GameUI;
        scene: Graphic.GameScene;
        audioController: Audio.AudioController;
        musicController: Audio.MusicController;
        chatController: UI.ChatController;


        constructor(playerId: string, document: Document) {
            this.playerId = playerId;
            this.gameUI = new UI.GameUI(document.getElementById("revivebutton") as HTMLButtonElement,
                document.getElementById("revivemessage") as HTMLDivElement,
                document.getElementById("debugmessage") as HTMLParagraphElement,
                document.getElementById("players-table") as HTMLTableElement,
                document.getElementById("safezonemessage") as HTMLDivElement,
                document.getElementById("investments-table") as HTMLTableElement,
                document.getElementById("damage-effect") as HTMLTableElement
            );

            this.serverConnection = new Network.ServerConnection("/gamehub");
            this.keyboardInputHandler = new InputUtils.KeyboardInputHandler(document);
            this.scene = new Graphic.GameScene(document, () => this.MainLoop(), 30,
                document.getElementById("progress-bar-sample") as HTMLDivElement);
            this.audioController = new Audio.AudioController();
            this.musicController = new Audio.MusicController();

            try {
                this.serverConnection.InitializeServerConnection((message: GameDesign.ClientGameState) => this.ReceiveGameState(message));
            }
            catch (error) {
                console.error("Error initializing:", error);
            }

            this.chatController = new UI.ChatController(document.getElementById("sendchatbutton") as HTMLButtonElement,
                document.getElementById("chattextcontainer") as HTMLDivElement,
                document.getElementById("inputchatmessage") as HTMLInputElement,
                async (message: string) => { return await this.serverConnection.SendChatMessage(message); }
            );

            //Start to update chat every second
            setInterval(() => { this.serverConnection.GetChatMessages(this.chatController.GetLastReceivedMaxId()).then((messages) => { this.chatController.StoreNewMessages(messages); }); }, 1000);
        }


        lastReceivedGameState: GameDesign.ClientGameState | null = null;
        lastProcessedGameState: GameDesign.ClientGameState | null = null;



        /** Stores an update as a newest client personal info */
        ReceiveGameState(update: GameDesign.ClientGameState) {

            const currentUpdateTime: number = new Date().getTime();
            this.technicalInfo.lastFrameTimeMs = currentUpdateTime - this.technicalInfo.lastUpdateTime;
            this.technicalInfo.lastUpdateTime = currentUpdateTime;


            if (this.lastReceivedGameState != null)
                this.technicalInfo.extraGoodFrames++;

            this.lastReceivedGameState = update;
        }


        /** A main game loop that gets called every frames. Repaint all the sprites based on last info from the server or extrapolates the last known state of the game */
        MainLoop(): void {



            let isGood: boolean = false;
            if (this.lastReceivedGameState == null && this.lastProcessedGameState == null)
                return;
            else if (this.lastReceivedGameState == null) {
                this.technicalInfo.droppedFrames++;

                if (this.lastProcessedGameState != null)
                    GameDesign.ApplyExtrapolation(this.lastProcessedGameState, this.scene.app.ticker.elapsedMS / 1000);
            }
            else {
                this.technicalInfo.goodFrames++;
                isGood = true;
                this.lastProcessedGameState = this.lastReceivedGameState;
                this.lastReceivedGameState = null;
            }


            if (this.lastProcessedGameState == null) {
                this.scene.app.view.hidden = true;
                this.scene.RemoveAllGraphics();
                return;
            }

            this.gameUI.RepaintReviveInterface(this.lastProcessedGameState);
            this.gameUI.RepaintDamageEffects(this.lastProcessedGameState);
            this.gameUI.RepaintSafeZoneMessage(this.lastProcessedGameState);
            this.gameUI.RepaintInvestmentsTable(this.lastProcessedGameState);


            if (isGood) {
                var input = this.keyboardInputHandler.GetUsersInput();
                this.gameUI.AddInputFromUI(input);
                this.serverConnection.SendMyInput(input);
            }


            if (!GameDesign.IsPlayerInGame(this.lastProcessedGameState)) {
                this.scene.app.view.hidden = true;
                this.scene.RemoveAllGraphics();
                return;
            }
            else {
                this.scene.app.view.hidden = false;
            }


            this.scene.DisplayGameObjects(this.lastProcessedGameState);




            this.audioController.ProcessQueue(this.lastProcessedGameState.soundEffectsQueue, this.lastProcessedGameState.objects[this.lastProcessedGameState.gameObjectsId].position);

            this.gameUI.DebugMyPositionAndVelocity(this.lastProcessedGameState, this.technicalInfo);
        }

    }

}
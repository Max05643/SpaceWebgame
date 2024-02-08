import * as GameDesign from './gamedesign';
import { Technical } from './technical';

export namespace UI {

    //** Handles the UI part of the game's chat and caches messages locally */
    export class ChatController {
        private sendButton: HTMLButtonElement;
        private textContainer: HTMLElement;
        private messageContainer: HTMLInputElement;

        private lastReceivedMaxMessageId: number = -1;
        private sendMessageNetworkHandler: (message: string) => Promise<boolean>;
        private storedMessages: GameDesign.GameMessageContainer[] = [];


        private async SendMessage(): Promise<void> {
            if (this.sendMessageNetworkHandler == null)
                return;

            const message = this.messageContainer.value;
            if (message == null || message.length > 100 || message.length == 0) {
                return;
            }

            const result = await this.sendMessageNetworkHandler(message);

            if (result) {
                this.messageContainer.value = "";
            }
        }

        GetLastReceivedMaxId(): number {
            return this.lastReceivedMaxMessageId;
        }


        StoreNewMessages(messages: GameDesign.GameMessageContainer[] | null): void {

            if (messages == null || messages.length == 0)
                return;

            const newMax = Math.max(...messages.map(mess => mess.id));

            if (newMax <= this.lastReceivedMaxMessageId) {
                return;
            }



            for (const message of messages) {
                if (message.id > this.lastReceivedMaxMessageId)
                    this.storedMessages.push(message);
            }

            if (this.storedMessages.length > 10) {
                this.storedMessages = this.storedMessages.slice(this.storedMessages.length - 10);
            }

            this.lastReceivedMaxMessageId = newMax;


            this.Repaint();
        }

        Repaint(): void {
            this.textContainer.innerHTML = "";
            for (const message of this.storedMessages.slice().reverse()) {
                this.textContainer.innerHTML += '<br>';
                this.textContainer.innerHTML += `${message.message?.senderNick}:${message.message?.message}`;
            }
        }

        constructor(sendButton: HTMLButtonElement, textContainer: HTMLElement, messageContainer: HTMLInputElement, sendMessageNetworkHandler: (message: string) => Promise<boolean>) {
            this.sendButton = sendButton;
            this.textContainer = textContainer;
            this.messageContainer = messageContainer;
            this.sendMessageNetworkHandler = sendMessageNetworkHandler;

            this.sendButton.onclick = () => { this.SendMessage(); };
        }


    }


    /** Stores references to UI elements and handles interaction with them*/
    export class GameUI {
        /** Button used to revive player */
        private reviveButton: HTMLButtonElement;
        /** Window that is displayed when player is dead or not entered the game */
        private reviveMessage: HTMLDivElement;
        /** Text that shows debug info */
        private debugMessage: HTMLParagraphElement;
        /** Table for displaying players info */
        private playersTable: HTMLTableElement;
        /** Message stating that player is in safe zone */
        private safeZoneMessage: HTMLDivElement;
        /** Table for displaying players info */
        private investmentsTable: HTMLTableElement;
        /** An element for damage effects */
        private damageEffects: HTMLElement;

        private lastInvestmentRequest: number | null = null;
        private lastRepairRequest: boolean = false;

        AddInputFromUI(input: GameDesign.ClientInput): void {
            input.investmentRequest = this.lastInvestmentRequest;
            input.repairRequest = this.lastRepairRequest;
            this.lastInvestmentRequest = null;
            this.lastRepairRequest = false;
        }

        constructor(reviveButton: HTMLButtonElement, reviveMessage: HTMLDivElement, debugMessage: HTMLParagraphElement, playersTable: HTMLTableElement, safeZoneMessage: HTMLDivElement, investmentsTable: HTMLTableElement, damageEffects: HTMLElement) {
            this.debugMessage = debugMessage;
            this.reviveButton = reviveButton;
            this.reviveMessage = reviveMessage;
            this.playersTable = playersTable;
            this.safeZoneMessage = safeZoneMessage;
            this.investmentsTable = investmentsTable;
            this.damageEffects = damageEffects;
        }


        RepaintDamageEffects(gameState: GameDesign.ClientGameState | null): void {
            if (gameState == null || !GameDesign.IsPlayerInGame(gameState))
                this.damageEffects.hidden = true;
            else {
                const healthNormalized = gameState.health / gameState.maxHealth;
                this.damageEffects.hidden = false;
                this.damageEffects.style.opacity = (0.3 * (1 - healthNormalized)).toString();
            }
        }

        RepaintSafeZoneMessage(gameState: GameDesign.ClientGameState | null): void {
            if (gameState == null) {
                this.safeZoneMessage.hidden = true;
            }
            else {
                this.safeZoneMessage.hidden = !gameState.isSafeZone;
            }
        }


        RepaintInvestmentsTable(gameState: GameDesign.ClientGameState | null): void {
            if (gameState == null || gameState.alreadyInvested == null || !gameState.isSafeZone) {
                this.investmentsTable.hidden = true;
            }
            else {

                this.investmentsTable.hidden = false;

                const repairButton = this.investmentsTable.querySelector("#repairButton") as HTMLDivElement;


                if (gameState.health < gameState.maxHealth && gameState.points >= (gameState.maxHealth - gameState.health)) {
                    repairButton.hidden = false;
                    repairButton.onclick = () => { this.lastRepairRequest = true; };
                    repairButton.innerHTML = `Repair for ${(gameState.maxHealth - gameState.health)} points`;
                }
                else {
                    repairButton.hidden = true;
                }


                const innerContent = this.investmentsTable.querySelector('tbody') as HTMLTableSectionElement;


                for (let i = 0; i <= 6; i++) {

                    const row = innerContent.rows[i] == undefined ? innerContent.insertRow() : innerContent.rows[i];

                    const currentVal = gameState.alreadyInvested[i];

                    const cell1 = row.cells[0] == undefined ? row.insertCell(0) : row.cells[0];
                    cell1.innerHTML = GameDesign.playerAttributes[i];

                    const cell2 = row.cells[1] == undefined ? row.insertCell(1) : row.cells[1];
                    cell2.innerHTML = `${currentVal}/100`;

                    const cell3 = row.cells[2] == undefined ? row.insertCell(2) : row.cells[2];

                    if (cell3.children[0] == undefined) {
                        const newButton = document.createElement("div");
                        newButton.innerHTML = "Update (10 points)";
                        newButton.classList.add('btn', 'btn-info', 'btn-sm');
                        cell3.appendChild(newButton);
                    }
                    const updateButton = cell3.children[0] as HTMLDivElement;




                    if (currentVal < 100 && gameState.points >= 10) {
                        updateButton.hidden = false;
                        row.classList.add('table-success');
                        updateButton.onclick = () => { this.lastInvestmentRequest = i; };
                    }
                    else {
                        updateButton.hidden = true;
                        row.classList.remove('table-success');
                    }
                }

            }
        }


        DebugMyPositionAndVelocity(gameState: GameDesign.ClientGameState, info: Technical.TechnicalInfo): void {

            const myObj = gameState.objects[gameState.gameObjectsId];

            this.debugMessage.innerHTML = `Position: ${(myObj.position.x).toFixed(2)},${(myObj.position.y).toFixed(2)};<br>Velocity: ${(myObj.velocity.x).toFixed(2)},${(myObj.velocity.y).toFixed(2)}<br>Drops: ${info.droppedFrames}<br>Goods: ${info.goodFrames}<br>Extra goods: ${info.extraGoodFrames}`;

            this.debugMessage.innerHTML += `<br> Last frame time: ${info.lastFrameTimeMs} ms`;

            this.debugMessage.innerHTML += `<br>Players on server: ${gameState.playersCount}`;

            if (info.goodFrames != 0) {
                this.debugMessage.innerHTML += `<br>DropsToGoods: ${Math.round((info.droppedFrames / info.goodFrames) * 100)}%`;
                this.debugMessage.innerHTML += `<br>ExtraGoodsToGoods: ${Math.round((info.extraGoodFrames / info.goodFrames) * 100)}%`;
            }
            if ('connection' in navigator) {
                const connection = (navigator as any).connection;
                this.debugMessage.innerHTML += `<br>Network: ${connection.effectiveType}`;
                this.debugMessage.innerHTML += `<br>RRT: ${connection.rtt} ms`;
            }



        }
        RepaintReviveInterface(gameState: GameDesign.ClientGameState | null, onReviveButtonPress: () => void): void {

            if (gameState != null && gameState.state != GameDesign.PlayerState.Alive) {
                this.reviveMessage.hidden = false;
                this.reviveButton.onclick = onReviveButtonPress;
            }
            else {
                this.reviveMessage.hidden = true;
            }
        }
    }

}

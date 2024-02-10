import * as signalR from '@microsoft/signalr';
import * as MP from '@microsoft/signalr-protocol-msgpack';
import { Mapper } from './conversion';
import * as GameDesign from './gamedesign';

export namespace Network {
    /** Manages the server connection via SignalR */
    export class ServerConnection {

        mapper: Mapper;
        connection: signalR.HubConnection;

        constructor(endpoint: string) {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(endpoint)
                .withHubProtocol(new MP.MessagePackHubProtocol())
                .build();
            this.mapper = new Mapper();
        }

        async SendChatMessage(message: string): Promise<boolean> {
            return await this.connection.invoke<boolean>("AddChatMessage", message);
        }

        async GetChatMessages(id: number): Promise<GameDesign.GameMessageContainer[] | null> {
            return await this.connection.invoke<GameDesign.GameMessageContainer[] | null>("GetChatMessages", id);
        }


        /** Initializes the connection */
        async InitializeServerConnection(personalInfoHandler: (message: GameDesign.ClientGameState) => void): Promise<void> {
            this.connection.on("ReceiveGameState", (message) => {
                if (this.mapper.isReady) {
                    const result = this.mapper.mapClientPersonalInfoFromServer(message) as GameDesign.ClientGameState;
                    personalInfoHandler(result);
                }
            });
            this.connection.on("ReceiveRemovalFromGameNotification", () => {
                document.location = "/Game/Kicked";
            });
            await this.connection.start();
            await this.connection.invoke<void>("SubscribeToUpdates");
        }

        /** Send an input to the server */
        async SendMyInput(input: GameDesign.ClientInput): Promise<void> {
            if (this.mapper.isReady) {
                const result = this.mapper.mapClientInputToServer(input);
                await this.connection.invoke<void>("SendInput", result);
            }
        }
    }
}

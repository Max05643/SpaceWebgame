export class GameMessageContainer {
    public id: number = -1;
    public message: GameMessage | null = null;
}
export class GameMessage {
    public senderNick: string = "";
    public message: string = "";
}

export const playerViewportMaxSize: ClientVector2 =
{
    x: 1500,
    y: 1500,
    Length(): number {
        return Math.sqrt(this.LengthSquared());
    },
    LengthSquared(): number {
        return this.x * this.x + this.y * this.y;
    }
};

export const playerAttributes: { [key: number]: string } = {
    0: "MaxHealth",
    1: "PhysicalDamage",
    2: "ProjectileDamage",
    3: "Power",
    4: "Acceleration",
    5: "AngularSpeed",
    6: "Reload",
};

export const possiblePlayersStatus = {
    0: "Alive",
    1: "Dead",
    2: "Not entered the game"
}

/** Represents an audio effect. The effect can just be played or played in particular part of global world*/
export class SoundEffect {
    audioClipName: string = "";
    position: ClientVector2 | null = null;
    radius: number | null = null;
    id: number = 0;
}

export const graphicTypeNone = 0;
export const graphicTypeStatic = 1;
export const graphicTypeAnimated = 2;


export function GetEstimatedSize(graphicInfo: GraphicInfo): ClientVector2 {
    const result = new ClientVector2();

    if (graphicInfo.targetSize == null) {
        result.x = 100;
        result.y = 100;
    }
    else {
        result.x = graphicInfo.targetSize.x;
        result.y = graphicInfo.targetSize.y;
    }

    return result;
}

export class GraphicLibraryEntry {
    public type: number = graphicTypeNone;
    public spriteInfo: SpriteInfo | null = null;
    public animatedSpriteInfo: AnimatedSpriteInfo | null = null;
}

export class AnimationInfo {
    public secondsCompleted: number = 0;
}

export class GraphicInfo {
    public objectAnimationInfo: AnimationInfo | null = null;
    public graphicLibraryEntryId: number = 0;
    public targetSize: ClientVector2 = new ClientVector2();
}

export class SpriteInfo {
    public sprite: string | null = null;
}
export class AnimatedSpriteInfo {
    public animationName: string | null = null;
    public isLoop: boolean = false;
    public targetTimeInSeconds: number = 1;
    public completedTimeInSeconds: number = 0;
}

export function NeedToChangeGraphic(currentInfo: GraphicInfo, newInfo: GraphicInfo): boolean {
    return currentInfo.graphicLibraryEntryId != newInfo.graphicLibraryEntryId;
}


export class ClientGameObject {
    public position: ClientVector2 = new ClientVector2();
    public velocity: ClientVector2 = new ClientVector2();
    public graphicInfo: GraphicInfo = new GraphicInfo();
    public angle: number = 0;

    public children: { [key: string]: ClientGameObject } = {};

    public features: { [key: string]: string } = {};

}

export class ClientGameState {
    public playersCount: number = 0;
    public objects: { [key: string]: ClientGameObject } = {};
    public soundEffectsQueue: SoundEffect[] = [];
    isSafeZone: boolean = false;
    points: number = 0;
    alreadyInvested: { [investmentType: number]: number; } | null = null;
    health: number = 0;
    maxHealth: number = 0;
    gameObjectsId: string = "";
    state: PlayerState = PlayerState.NotEntered;
}

/** Checks if specified player is currently in the game and is alive */
export function IsPlayerInGame(gameState: ClientGameState | null): boolean {
    return gameState != null && gameState.state == PlayerState.Alive;
}



export function Difference(a: ClientVector2, b: ClientVector2): ClientVector2 {
    const result = new ClientVector2();

    result.x = a.x - b.x;
    result.y = a.y - b.y;

    return result;
}
export function Distance(a: ClientVector2, b: ClientVector2): number {
    return Math.sqrt(DistanceSquared(a, b));
}
export function DistanceSquared(a: ClientVector2, b: ClientVector2): number {
    const dx = a.x - b.x;
    const dy = a.y - b.y;
    return dx * dx + dy * dy;
}

export function Normalized(a: ClientVector2): ClientVector2 {
    const result = new ClientVector2();
    const length = Math.sqrt(a.x * a.x + a.y * a.y);

    result.x = a.x / length;
    result.y = a.y / length;

    return result;
}

export class ClientVector2 {
    public x: number = 0;
    public y: number = 0;

    Length(): number {
        return Math.sqrt(this.LengthSquared());
    }

    LengthSquared(): number {
        return this.x * this.x + this.y * this.y;
    }
}

export enum PlayerState {
    Alive = 0,
    Dead = 1,
    NotEntered = 2
}

export class ClientInput {
    public angle: number = 0;
    public movementPower: number = 0;
    public isFire: boolean = false;
    public investmentRequest: number | null = null;
    public repairRequest: boolean = false;
    public reviveRequest: boolean = false;
}


/** Applies extrapolation to the gameState, i.e. moves objects to their estimated positions */
export function ApplyExtrapolation(gameState: ClientGameState, delta: number): void {
    for (const objectName in gameState.objects) {
        const objectInfo = gameState.objects[objectName];
        objectInfo.position.x = objectInfo.position.x + objectInfo.velocity.x * delta;
        objectInfo.position.y = objectInfo.position.y + objectInfo.velocity.y * delta;
    }

    gameState.soundEffectsQueue = [];
}
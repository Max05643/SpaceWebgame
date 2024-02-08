import * as PIXI from 'pixi.js';
import * as GameDesign from './gamedesign';
import axios from 'axios';

export namespace Graphic {

    const universalTextStyle = new PIXI.TextStyle({
        fontFamily: 'Arial',
        fontSize: 12,
        fill: 0xFFFFFF,
        align: 'center'
    });

    export class GraphicLibrary {

        entries: { [key: number]: GameDesign.GraphicLibraryEntry } = {};
        isReady: boolean = false;

        async Init(): Promise<void> {
            const result = await axios.get("/Game/GetGraphicLibrary");
            this.entries = result.data;
            this.isReady = true;
        }

        constructor() {
            this.Init();
        }

    }

    export class BackGroundController {

        backgroundObj: PIXI.TilingSprite;

        constructor(background: string, app: PIXI.Application) {
            const backgroundTexture = PIXI.Texture.from('/assets/backgrounds/' + background);
            this.backgroundObj = new PIXI.TilingSprite(backgroundTexture, app.screen.width, app.screen.height);

            app.stage.addChild(this.backgroundObj);
        }

        HandleWindowResize(): void {
            this.backgroundObj.width = window.innerWidth;
            this.backgroundObj.height = window.innerHeight;
        }

        Destroy(): void {
            this.backgroundObj.parent.removeChild(this.backgroundObj);
        }
    }

    /** Represents an object in GameScene that have a sprite assigned to it*/
    export class SceneObject {

        pixiObj: PIXI.Container | null = null;

        parent: PIXI.Container;

        app: PIXI.Application;

        public graphicInfo: GameDesign.GraphicInfo;

        public textForObject: PIXI.Text | null = null;

        public healthBar: HTMLDivElement | null = null;

        public objName: string;

        public children: { [key: string]: SceneObject } = {};

        Destroy(): void {
            for (const child in this.children) {
                this.children[child].Destroy();
                delete this.children[child];
            }

            if (this.pixiObj != null) {
                this.pixiObj.parent.removeChild(this.pixiObj);
                this.pixiObj = null;
            }
            if (this.textForObject != null) {
                this.parent.removeChild(this.textForObject);
                this.textForObject = null;
            }
            if (this.healthBar != null) {
                this.healthBar.parentNode!.removeChild(this.healthBar);
                this.healthBar = null;
            }
        }

        private ApplyTransformations(targetSize: GameDesign.ClientVector2 | null, position: GameDesign.ClientVector2, angle: number): void {

            this.pixiObj!.rotation = angle;
            this.pixiObj!.position.set(position.x, -position.y);

            if (targetSize == null || targetSize.x == 0 && targetSize.y == 0) {
                return;
            }
            this.pixiObj!.width = targetSize.x
            this.pixiObj!.height = targetSize.y;

        }

        private SetHealthBar(value: number | null, progressBarSample: HTMLDivElement, gameScene: GameScene, graphicLibrary: GraphicLibrary): void {
            const graphicLibEntry = graphicLibrary.entries[this.graphicInfo.graphicLibraryEntryId];

            if (value == null || graphicLibEntry.type == GameDesign.graphicTypeNone) {
                if (this.healthBar != null)
                    progressBarSample.parentNode!.removeChild(this.healthBar);
                this.healthBar = null;
            }
            else {
                if (this.healthBar == null) {
                    const originalDiv = progressBarSample;
                    const clonedDiv = originalDiv.cloneNode(true) as HTMLDivElement;
                    clonedDiv.id = `healthbar${this.objName}`;
                    progressBarSample.parentNode!.appendChild(clonedDiv);
                    this.healthBar = clonedDiv;
                }

                const positionOfObject = new GameDesign.ClientVector2();
                positionOfObject.x = (this.pixiObj!.position.x + gameScene.mainContainer.position.x) * gameScene.currentStageScale;
                positionOfObject.y = (this.pixiObj!.position.y + gameScene.mainContainer.position.y + this.pixiObj!.height * 0.5) * gameScene.currentStageScale;

                this.healthBar!.hidden = false;
                this.healthBar!.style.display = 'block';
                this.healthBar!.style.visibility = (value > 0) ? 'visible' : "hidden";
                this.healthBar!.style.position = 'absolute';
                this.healthBar!.style.left = `${positionOfObject.x - 50}px`;
                this.healthBar!.style.top = `${positionOfObject.y + 30}px`;

                (this.healthBar!.querySelector('.progress-bar') as HTMLDivElement).style.width = `${value}%`;
                (this.healthBar!.querySelector('.progress-bar') as HTMLDivElement).setAttribute("aria-valuenow", value.toString());


            }
        }

        private SetText(text: string | null, graphicLibrary: GraphicLibrary): void {
            const graphicLibEntry = graphicLibrary.entries[this.graphicInfo.graphicLibraryEntryId];

            if (text == null || graphicLibEntry.type == GameDesign.graphicTypeNone) {
                if (this.textForObject != null) {
                    this.parent.removeChild(this.textForObject);
                    this.textForObject = null;
                }
            }
            else {

                if (this.textForObject == null) {
                    this.textForObject = new PIXI.Text('', universalTextStyle);
                    this.parent.addChild(this.textForObject);
                }

                this.textForObject.text = text;
                this.textForObject.anchor.set(0.5);
                this.textForObject.x = this.pixiObj!.x;
                this.textForObject.y = this.pixiObj!.y - this.pixiObj!.height * 0.5 - 30;
            }
        }

        Repaint(gameObject: GameDesign.ClientGameObject, gameScene: GameScene, graphicLibrary: GraphicLibrary): void {

            this.EnsureCorrectPixiObj(gameObject.graphicInfo, graphicLibrary);

            this.graphicInfo = gameObject.graphicInfo;

            this.ApplyTransformations(gameScene.ConvertVectorFromGlobalSpaceToViewSpace(gameObject.graphicInfo.targetSize), gameScene.ConvertVectorFromGlobalSpaceToViewSpace(gameObject.position), -gameObject.angle);



            this.SetHealthBar(gameObject.features["health"] == undefined ? null : parseInt(gameObject.features["health"]), gameScene.progressBarSample, gameScene, graphicLibrary);
            this.SetText(gameObject.features["text"] == undefined ? null : gameObject.features["text"], graphicLibrary);


            this.RepaintChildren(gameObject, gameScene, graphicLibrary);
            this.RemoveUnusedChildren(gameObject, gameScene);
        }

        constructor(objName: string, gameObject: GameDesign.ClientGameObject, parent: PIXI.Container, app: PIXI.Application, graphicLibrary: GraphicLibrary) {
            this.objName = objName;
            this.parent = parent;
            this.graphicInfo = gameObject.graphicInfo;
            this.app = app;
            this.EnsureCorrectPixiObj(this.graphicInfo, graphicLibrary);
        }


        private RepaintChildren(gameObject: GameDesign.ClientGameObject, gameScene: GameScene, graphicLibrary: GraphicLibrary): void {
            for (const child in gameObject.children) {
                if (!(child in this.children)) {
                    const newChild = new SceneObject(child, gameObject.children[child], this.pixiObj!, this.app, graphicLibrary);
                    this.children[child] = newChild;
                }
                this.children[child].Repaint(gameObject.children[child], gameScene, graphicLibrary);
            }
        }
        private RemoveUnusedChildren(gameObject: GameDesign.ClientGameObject, gameScene: GameScene): void {
            for (const child in this.children) {
                if (!(child in gameObject.children)) {
                    this.children[child].Destroy();
                    delete this.children[child];
                }
            }
        }



        private EnsureCorrectPixiObj(newGraphicInfo: GameDesign.GraphicInfo, graphicLibrary: GraphicLibrary): void {
            const graphicLibEntry = graphicLibrary.entries[newGraphicInfo.graphicLibraryEntryId];

            if (GameDesign.NeedToChangeGraphic(this.graphicInfo, newGraphicInfo)) {
                this.Destroy();
            }

            if (this.pixiObj == null) {
                if (graphicLibEntry.type == GameDesign.graphicTypeNone) {
                    this.CreateFromContainer(newGraphicInfo);
                }
                else if (graphicLibEntry.type == GameDesign.graphicTypeStatic) {
                    this.CreateFromStaticSprite(newGraphicInfo, graphicLibrary);
                }
                else if (graphicLibEntry.type == GameDesign.graphicTypeAnimated) {
                    this.CreateFromAnimatedSprite(newGraphicInfo, graphicLibrary);
                }
            }
        }

        private CreateFromContainer(graphicInfo: GameDesign.GraphicInfo): void {
            const container = new PIXI.Container();
            container.pivot.set(0.5);
            this.pixiObj = container;
            this.parent.addChild(container);
        }
        private CreateFromStaticSprite(graphicInfo: GameDesign.GraphicInfo, graphicLibrary: GraphicLibrary): void {
            const graphicLibEntry = graphicLibrary.entries[graphicInfo.graphicLibraryEntryId];

            const newSprite = PIXI.Sprite.from(graphicLibEntry.spriteInfo!.sprite!);
            this.pixiObj = newSprite;
            this.parent.addChild(newSprite);

        }
        private CreateFromAnimatedSprite(graphicInfo: GameDesign.GraphicInfo, graphicLibrary: GraphicLibrary): void {
            const graphicLibEntry = graphicLibrary.entries[graphicInfo.graphicLibraryEntryId];

            const newSprite = PIXI.AnimatedSprite.fromFrames(PIXI.Assets.cache.get("/assets/sprites/sheet.json").data.animations[graphicLibEntry.animatedSpriteInfo!.animationName!]);

            this.pixiObj = newSprite;
            newSprite.animationSpeed = (newSprite.totalFrames / this.app.ticker.maxFPS) / graphicLibEntry.animatedSpriteInfo!.targetTimeInSeconds;
            newSprite.loop = graphicLibEntry.animatedSpriteInfo!.isLoop;


            if (graphicLibEntry.animatedSpriteInfo!.isLoop) {
                newSprite.play();
            }
            else {
                if (graphicInfo.objectAnimationInfo == null) {
                    newSprite.gotoAndPlay(0);
                }
                else {
                    const currentFrame = Math.floor(newSprite.totalFrames * (graphicInfo.objectAnimationInfo.secondsCompleted / graphicLibEntry.animatedSpriteInfo!.targetTimeInSeconds));
                    newSprite.gotoAndPlay(currentFrame);
                }
            }


            this.parent.addChild(newSprite);

        }





    }

    /** Represents a high-level abstraction over pixi js */
    export class GameScene {

        graphicLibrary: GraphicLibrary;

        presentObjects: { [key: string]: SceneObject } = {};

        backgroundController: BackGroundController;

        currentStageScale: number = 1;

        /** Stores all the game objects and keeps the player centered */
        mainContainer: PIXI.Container;

        private CheckIfObjectInViewport(stageCoordinates: GameDesign.ClientVector2, spriteStageSize: GameDesign.ClientVector2): boolean {
            stageCoordinates.x *= this.currentStageScale;
            stageCoordinates.y *= this.currentStageScale;

            spriteStageSize.x *= this.currentStageScale;
            spriteStageSize.y *= this.currentStageScale;

            const topRight: GameDesign.ClientVector2 = new GameDesign.ClientVector2();
            topRight.x = stageCoordinates.x + spriteStageSize.x * 0.5;
            topRight.y = stageCoordinates.y + spriteStageSize.y * 0.5;

            const bottomLeft: GameDesign.ClientVector2 = new GameDesign.ClientVector2();
            bottomLeft.x = stageCoordinates.x - spriteStageSize.x * 0.5;
            bottomLeft.y = stageCoordinates.y - spriteStageSize.y * 0.5;

            if (topRight.x < 0 && topRight.y < 0) {
                return false;
            }
            else if (bottomLeft.x > GameDesign.playerViewportMaxSize.x && bottomLeft.y > GameDesign.playerViewportMaxSize.y) {
                return false;
            }
            else {
                return true;
            }
        }

        /** Converts point from global space to pixi's pixel space */
        ConvertPointFromGlobalSpaceToViewSpace(input: GameDesign.ClientVector2, startOfCoordinates: GameDesign.ClientVector2): GameDesign.ClientVector2 {
            const result = new GameDesign.ClientVector2();
            result.x = (input.x - startOfCoordinates.x) * 10 + this.app.screen.width * 0.5 / this.currentStageScale;
            result.y = -(input.y - startOfCoordinates.y) * 10 + this.app.screen.height * 0.5 / this.currentStageScale;

            return result;
        }

        /** Converts vector from global space to pixi's pixel space */
        ConvertVectorFromGlobalSpaceToViewSpace(input: GameDesign.ClientVector2): GameDesign.ClientVector2 {
            const result = new GameDesign.ClientVector2();
            result.x = input.x * 10;
            result.y = input.y * 10;

            return result;
        }


        app: PIXI.Application<HTMLCanvasElement>;

        /** Are all assets loaded? */
        isReady: boolean = false;



        async PrepareSpriteSheet(): Promise<void> {

            await PIXI.Assets.load(["/assets/sprites/sheet.json"]);
            this.isReady = true;
        }

        /** Sample object for health indicators */
        progressBarSample: HTMLDivElement;


        HandleWindowResize(): void {
            this.app.renderer.resize(window.innerWidth, window.innerHeight);
            this.currentStageScale = Math.max(1, window.innerWidth / GameDesign.playerViewportMaxSize.x, window.innerHeight / GameDesign.playerViewportMaxSize.y);
            this.app.stage.scale.set(this.currentStageScale);
            this.backgroundController.HandleWindowResize();
        }

        constructor(document: Document, mainLoop: () => void, targetFrameRate: number = 30, progressBarSample: HTMLDivElement) {

            this.progressBarSample = progressBarSample;

            this.PrepareSpriteSheet();
            this.graphicLibrary = new GraphicLibrary();

            this.app = new PIXI.Application<HTMLCanvasElement>({
                width: window.innerWidth,
                height: window.innerHeight,
                backgroundColor: "#000000",
                autoDensity: true,
                resolution: window.devicePixelRatio
            });


            this.backgroundController = new BackGroundController("back0.png", this.app);

            this.app.ticker.maxFPS = targetFrameRate;
            document.body.appendChild(this.app.view);
            this.app.ticker.add(mainLoop);
            this.app.ticker.start();
            this.app.view.hidden = true;

            this.HandleWindowResize();
            window.addEventListener('resize', () => { this.HandleWindowResize(); });

            this.mainContainer = new PIXI.Container();
            this.mainContainer.width = 0;
            this.mainContainer.height = 0;
            this.mainContainer.pivot.set(0.5);
            this.app.stage.addChild(this.mainContainer);
        }


        private DisplayGameObject(objectName: string, objectInfo: GameDesign.ClientGameObject, startOfCoordinates: GameDesign.ClientVector2): void {

            const positionInViewSpace = this.ConvertPointFromGlobalSpaceToViewSpace(objectInfo.position, startOfCoordinates);
            if (!this.CheckIfObjectInViewport(positionInViewSpace, GameDesign.GetEstimatedSize(objectInfo.graphicInfo))) {
                this.EnsureGraphicObjectRemoved(objectName);
            }
            else {
                let currentHandle: SceneObject | null = null;

                if (objectName in this.presentObjects) {
                    currentHandle = this.presentObjects[objectName];
                }
                else {
                    currentHandle = new SceneObject(objectName, objectInfo, this.mainContainer, this.app, this.graphicLibrary);
                    this.presentObjects[objectName] = currentHandle;
                }

                currentHandle.Repaint(objectInfo, this, this.graphicLibrary);
            }
        }

        DisplayGameObjects(gameState: GameDesign.ClientGameState): void {


            if (!this.isReady || !this.graphicLibrary.isReady)
                return;

            const startOfCoordinates = gameState.objects[gameState.gameObjectsId].position;


            const mainContainerPos = this.ConvertVectorFromGlobalSpaceToViewSpace(startOfCoordinates);
            mainContainerPos.x = -mainContainerPos.x + this.app.screen.width * 0.5 / this.currentStageScale;
            mainContainerPos.y = mainContainerPos.y + this.app.screen.height * 0.5 / this.currentStageScale;

            this.mainContainer.position.set(mainContainerPos.x, mainContainerPos.y);

            for (const objectName in gameState.objects) {
                const objectInfo = gameState.objects[objectName];
                this.DisplayGameObject(objectName, objectInfo, startOfCoordinates);
            }

            this.RemoveUnusedGraphics(gameState);

        }

        RemoveAllGraphics(): void {
            for (const existingObjectName in this.presentObjects) {
                this.EnsureGraphicObjectRemoved(existingObjectName);
            }
        }

        private RemoveUnusedGraphics(gameState: GameDesign.ClientGameState): void {
            for (const existingObjectName in this.presentObjects) {
                if (!(existingObjectName in gameState.objects)) {
                    this.EnsureGraphicObjectRemoved(existingObjectName);
                }
            }
        }

        private EnsureGraphicObjectRemoved(objectName: string): void {
            if (this.presentObjects[objectName]) {
                const spriteToRemove = this.presentObjects[objectName];

                spriteToRemove.Destroy();

                delete this.presentObjects[objectName];
            }
        }


    }


}

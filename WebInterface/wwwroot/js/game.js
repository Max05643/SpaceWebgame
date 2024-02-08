!function(e,t){"object"==typeof exports&&"object"==typeof module?module.exports=t():"function"==typeof define&&define.amd?define([],t):"object"==typeof exports?exports.sample=t():e.sample=t()}(self,(()=>(()=>{var e,t={91:(e,t,s)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.Audio=void 0;const i=s(4250),n=s(9571);var a;!function(e){e.MusicController=class{constructor(){if(this.targetVol=.5,null!=localStorage.getItem("music_vol")){const e=parseFloat(localStorage.getItem("music_vol"));this.targetVol=isNaN(e)?.5:e}this.musicInstance=i.Sound.from("/assets/music/soundtrack.wav"),this.musicInstance.loop=!0,this.musicInstance.volume=this.targetVol,this.musicInstance.play()}},e.AudioController=class{constructor(){if(this.lastEffectPlayedId=-1,this.targetVol=.5,i.sound.add("ex0","/assets/sound/explosion0.wav"),i.sound.add("ex1","/assets/sound/explosion1.wav"),i.sound.add("lr","/assets/sound/laser.wav"),i.sound.add("pk","/assets/sound/pick.wav"),i.sound.add("mvt","/assets/sound/movement.wav"),null!=localStorage.getItem("sound_vol")){const e=parseFloat(localStorage.getItem("sound_vol"));this.targetVol=isNaN(e)?.5:e}}PlayOneShot(e,t){if(null==e.position){const t={volume:this.targetVol,loop:!1};i.sound.play(e.audioClipName,t)}else{const s=n.DistanceSquared(t,e.position),a=1-s/(e.radius*e.radius);if(a>0){const r=new i.filters.StereoFilter;r.pan=s>1?n.Normalized(n.Difference(e.position,t)).x:0;const o={volume:a*this.targetVol,loop:!1,filters:[r]};i.sound.play(e.audioClipName,o)}}}ProcessQueue(e,t){for(const s in e){const i=e[s];i.id>this.lastEffectPlayedId&&(this.lastEffectPlayedId=i.id,this.PlayOneShot(i,t))}}}}(a||(t.Audio=a={}))},5117:(e,t,s)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.Mapper=void 0;const i=s(7218);t.Mapper=class{async Initialize(){const e=await i.default.get("/assets/clientInputMap.json");this.clientInputToServerMapping=e.data;const t=await i.default.get("/assets/clientPersonalInfoMap.json");this.clientPersonalInfoFromServerMapping=t.data,this.isReady=!0}constructor(){this.clientPersonalInfoFromServerMapping={},this.clientInputToServerMapping={},this.isReady=!1,this.Initialize()}mapClientInputToServer(e){return this.mapObj(e,this.clientInputToServerMapping)}mapClientPersonalInfoFromServer(e){return this.mapObj(e,this.clientPersonalInfoFromServerMapping)}mapObj(e,t){const s={};for(const i in e){const n=null==t[i]?i:t[i];null==e[i]?s[n]=null:s[n]="object"==typeof e[i]?this.mapObj(e[i],t):e[i]}return s}}},7119:(e,t,s)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.Game=void 0;const i=s(1169),n=s(6305),a=s(7483),r=s(7361),o=s(1534),l=s(91),h=s(9571);var c;!function(e){e.GameInstance=class{constructor(e,t,s){this.technicalInfo=new n.Technical.TechnicalInfo,this.lastReceivedGameState=null,this.lastProcessedGameState=null,this.playerId=e,this.serverId=t,this.gameUI=new r.UI.GameUI(s.getElementById("revivebutton"),s.getElementById("revivemessage"),s.getElementById("debugmessage"),s.getElementById("players-table"),s.getElementById("safezonemessage"),s.getElementById("investments-table"),s.getElementById("damage-effect")),this.serverConnection=new i.Network.ServerConnection("/gamehub",t),this.keyboardInputHandler=new a.InputUtils.KeyboardInputHandler(s),this.scene=new o.Graphic.GameScene(s,(()=>this.MainLoop()),30,s.getElementById("progress-bar-sample")),this.audioController=new l.Audio.AudioController,this.musicController=new l.Audio.MusicController;try{this.serverConnection.InitializeServerConnection((e=>this.ReceiveGameState(e)))}catch(e){console.error("Error initializing:",e)}this.chatController=new r.UI.ChatController(s.getElementById("sendchatbutton"),s.getElementById("chattextcontainer"),s.getElementById("inputchatmessage"),(async e=>await this.serverConnection.SendChatMessage(e))),setInterval((()=>{this.serverConnection.GetChatMessages(this.chatController.GetLastReceivedMaxId()).then((e=>{this.chatController.StoreNewMessages(e)}))}),1e3)}ReceiveGameState(e){const t=(new Date).getTime();this.technicalInfo.lastFrameTimeMs=t-this.technicalInfo.lastUpdateTime,this.technicalInfo.lastUpdateTime=t,null!=this.lastReceivedGameState&&this.technicalInfo.extraGoodFrames++,this.lastReceivedGameState=e}MainLoop(){let e=!1;if(null!=this.lastReceivedGameState||null!=this.lastProcessedGameState){if(null==this.lastReceivedGameState?(this.technicalInfo.droppedFrames++,null!=this.lastProcessedGameState&&h.ApplyExtrapolation(this.lastProcessedGameState,this.scene.app.ticker.elapsedMS/1e3)):(this.technicalInfo.goodFrames++,e=!0,this.lastProcessedGameState=this.lastReceivedGameState,this.lastReceivedGameState=null),null==this.lastProcessedGameState)return this.scene.app.view.hidden=!0,void this.scene.RemoveAllGraphics();if(this.gameUI.RepaintReviveInterface(this.lastProcessedGameState,(()=>{this.serverConnection.Revive()})),this.gameUI.RepaintDamageEffects(this.lastProcessedGameState),this.gameUI.RepaintSafeZoneMessage(this.lastProcessedGameState),this.gameUI.RepaintInvestmentsTable(this.lastProcessedGameState),!h.IsPlayerInGame(this.lastProcessedGameState))return this.scene.app.view.hidden=!0,void this.scene.RemoveAllGraphics();if(this.scene.app.view.hidden=!1,this.scene.DisplayGameObjects(this.lastProcessedGameState),e){var t=this.keyboardInputHandler.GetUsersInput();this.gameUI.AddInputFromUI(t),this.serverConnection.SendMyInput(t)}this.audioController.ProcessQueue(this.lastProcessedGameState.soundEffectsQueue,this.lastProcessedGameState.objects[this.lastProcessedGameState.gameObjectsId].position),this.gameUI.DebugMyPositionAndVelocity(this.lastProcessedGameState,this.technicalInfo)}}}}(c||(t.Game=c={}))},9571:(e,t)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.ApplyExtrapolation=t.ClientInput=t.PlayerState=t.ClientVector2=t.Normalized=t.DistanceSquared=t.Distance=t.Difference=t.IsPlayerInGame=t.ClientGameState=t.ClientGameObject=t.NeedToChangeGraphic=t.AnimatedSpriteInfo=t.SpriteInfo=t.GraphicInfo=t.AnimationInfo=t.GraphicLibraryEntry=t.GetEstimatedSize=t.graphicTypeAnimated=t.graphicTypeStatic=t.graphicTypeNone=t.SoundEffect=t.possiblePlayersStatus=t.playerAttributes=t.playerViewportMaxSize=t.GameMessage=t.GameMessageContainer=void 0,t.GameMessageContainer=class{constructor(){this.id=-1,this.message=null}},t.GameMessage=class{constructor(){this.senderNick="",this.message=""}},t.playerViewportMaxSize={x:1500,y:1500,Length(){return Math.sqrt(this.LengthSquared())},LengthSquared(){return this.x*this.x+this.y*this.y}},t.playerAttributes={0:"MaxHealth",1:"PhysicalDamage",2:"ProjectileDamage",3:"Power",4:"Acceleration",5:"AngularSpeed",6:"Reload"},t.possiblePlayersStatus={0:"Alive",1:"Dead",2:"Not entered the game"},t.SoundEffect=class{constructor(){this.audioClipName="",this.position=null,this.radius=null,this.id=0}},t.graphicTypeNone=0,t.graphicTypeStatic=1,t.graphicTypeAnimated=2,t.GetEstimatedSize=function(e){const t=new n;return null==e.targetSize?(t.x=100,t.y=100):(t.x=e.targetSize.x,t.y=e.targetSize.y),t},t.GraphicLibraryEntry=class{constructor(){this.type=t.graphicTypeNone,this.spriteInfo=null,this.animatedSpriteInfo=null}},t.AnimationInfo=class{constructor(){this.secondsCompleted=0}};class s{constructor(){this.objectAnimationInfo=null,this.graphicLibraryEntryId=0,this.targetSize=new n}}function i(e,t){const s=e.x-t.x,i=e.y-t.y;return s*s+i*i}t.GraphicInfo=s,t.SpriteInfo=class{constructor(){this.sprite=null}},t.AnimatedSpriteInfo=class{constructor(){this.animationName=null,this.isLoop=!1,this.targetTimeInSeconds=1,this.completedTimeInSeconds=0}},t.NeedToChangeGraphic=function(e,t){return e.graphicLibraryEntryId!=t.graphicLibraryEntryId},t.ClientGameObject=class{constructor(){this.position=new n,this.velocity=new n,this.graphicInfo=new s,this.angle=0,this.children={},this.features={}}},t.ClientGameState=class{constructor(){this.playersCount=0,this.objects={},this.soundEffectsQueue=[],this.isSafeZone=!1,this.points=0,this.alreadyInvested=null,this.health=0,this.maxHealth=0,this.gameObjectsId="",this.state=a.NotEntered}},t.IsPlayerInGame=function(e){return null!=e&&e.state==a.Alive},t.Difference=function(e,t){const s=new n;return s.x=e.x-t.x,s.y=e.y-t.y,s},t.Distance=function(e,t){return Math.sqrt(i(e,t))},t.DistanceSquared=i,t.Normalized=function(e){const t=new n,s=Math.sqrt(e.x*e.x+e.y*e.y);return t.x=e.x/s,t.y=e.y/s,t};class n{constructor(){this.x=0,this.y=0}Length(){return Math.sqrt(this.LengthSquared())}LengthSquared(){return this.x*this.x+this.y*this.y}}var a;t.ClientVector2=n,function(e){e[e.Alive=0]="Alive",e[e.Dead=1]="Dead",e[e.NotEntered=2]="NotEntered"}(a||(t.PlayerState=a={})),t.ClientInput=class{constructor(){this.angle=0,this.movementPower=0,this.isFire=!1,this.investmentRequest=null,this.repairRequest=!1}},t.ApplyExtrapolation=function(e,t){for(const s in e.objects){const i=e.objects[s];i.position.x=i.position.x+i.velocity.x*t,i.position.y=i.position.y+i.velocity.y*t}e.soundEffectsQueue=[]}},8780:(e,t,s)=>{"use strict";var i;Object.defineProperty(t,"__esModule",{value:!0}),i||(i={}),new(s(7119).Game.GameInstance)(playerId,serverId,document)},1534:(e,t,s)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.Graphic=void 0;const i=s(8687),n=s(9571),a=s(7218);var r;!function(e){const t=new i.TextStyle({fontFamily:"Arial",fontSize:12,fill:16777215,align:"center"});class s{async Init(){const e=await a.default.get("/Game/GetGraphicLibrary");this.entries=e.data,this.isReady=!0}constructor(){this.entries={},this.isReady=!1,this.Init()}}e.GraphicLibrary=s;class r{constructor(e,t){const s=i.Texture.from("/assets/backgrounds/"+e);this.backgroundObj=new i.TilingSprite(s,t.screen.width,t.screen.height),t.stage.addChild(this.backgroundObj)}HandleWindowResize(){this.backgroundObj.width=window.innerWidth,this.backgroundObj.height=window.innerHeight}Destroy(){this.backgroundObj.parent.removeChild(this.backgroundObj)}}e.BackGroundController=r;class o{Destroy(){for(const e in this.children)this.children[e].Destroy(),delete this.children[e];null!=this.pixiObj&&(this.pixiObj.parent.removeChild(this.pixiObj),this.pixiObj=null),null!=this.textForObject&&(this.parent.removeChild(this.textForObject),this.textForObject=null),null!=this.healthBar&&(this.healthBar.parentNode.removeChild(this.healthBar),this.healthBar=null)}ApplyTransformations(e,t,s){this.pixiObj.rotation=s,this.pixiObj.position.set(t.x,-t.y),null==e||0==e.x&&0==e.y||(this.pixiObj.width=e.x,this.pixiObj.height=e.y)}SetHealthBar(e,t,s,i){const a=i.entries[this.graphicInfo.graphicLibraryEntryId];if(null==e||a.type==n.graphicTypeNone)null!=this.healthBar&&t.parentNode.removeChild(this.healthBar),this.healthBar=null;else{if(null==this.healthBar){const e=t.cloneNode(!0);e.id=`healthbar${this.objName}`,t.parentNode.appendChild(e),this.healthBar=e}const i=new n.ClientVector2;i.x=(this.pixiObj.position.x+s.mainContainer.position.x)*s.currentStageScale,i.y=(this.pixiObj.position.y+s.mainContainer.position.y+.5*this.pixiObj.height)*s.currentStageScale,this.healthBar.hidden=!1,this.healthBar.style.display="block",this.healthBar.style.visibility=e>0?"visible":"hidden",this.healthBar.style.position="absolute",this.healthBar.style.left=i.x-50+"px",this.healthBar.style.top=`${i.y+30}px`,this.healthBar.querySelector(".progress-bar").style.width=`${e}%`,this.healthBar.querySelector(".progress-bar").setAttribute("aria-valuenow",e.toString())}}SetText(e,s){const a=s.entries[this.graphicInfo.graphicLibraryEntryId];null==e||a.type==n.graphicTypeNone?null!=this.textForObject&&(this.parent.removeChild(this.textForObject),this.textForObject=null):(null==this.textForObject&&(this.textForObject=new i.Text("",t),this.parent.addChild(this.textForObject)),this.textForObject.text=e,this.textForObject.anchor.set(.5),this.textForObject.x=this.pixiObj.x,this.textForObject.y=this.pixiObj.y-.5*this.pixiObj.height-30)}Repaint(e,t,s){this.EnsureCorrectPixiObj(e.graphicInfo,s),this.graphicInfo=e.graphicInfo,this.ApplyTransformations(t.ConvertVectorFromGlobalSpaceToViewSpace(e.graphicInfo.targetSize),t.ConvertVectorFromGlobalSpaceToViewSpace(e.position),-e.angle),this.SetHealthBar(null==e.features.health?null:parseInt(e.features.health),t.progressBarSample,t,s),this.SetText(null==e.features.text?null:e.features.text,s),this.RepaintChildren(e,t,s),this.RemoveUnusedChildren(e,t)}constructor(e,t,s,i,n){this.pixiObj=null,this.textForObject=null,this.healthBar=null,this.children={},this.objName=e,this.parent=s,this.graphicInfo=t.graphicInfo,this.app=i,this.EnsureCorrectPixiObj(this.graphicInfo,n)}RepaintChildren(e,t,s){for(const i in e.children){if(!(i in this.children)){const t=new o(i,e.children[i],this.pixiObj,this.app,s);this.children[i]=t}this.children[i].Repaint(e.children[i],t,s)}}RemoveUnusedChildren(e,t){for(const t in this.children)t in e.children||(this.children[t].Destroy(),delete this.children[t])}EnsureCorrectPixiObj(e,t){const s=t.entries[e.graphicLibraryEntryId];n.NeedToChangeGraphic(this.graphicInfo,e)&&this.Destroy(),null==this.pixiObj&&(s.type==n.graphicTypeNone?this.CreateFromContainer(e):s.type==n.graphicTypeStatic?this.CreateFromStaticSprite(e,t):s.type==n.graphicTypeAnimated&&this.CreateFromAnimatedSprite(e,t))}CreateFromContainer(e){const t=new i.Container;t.pivot.set(.5),this.pixiObj=t,this.parent.addChild(t)}CreateFromStaticSprite(e,t){const s=t.entries[e.graphicLibraryEntryId],n=i.Sprite.from(s.spriteInfo.sprite);this.pixiObj=n,this.parent.addChild(n)}CreateFromAnimatedSprite(e,t){const s=t.entries[e.graphicLibraryEntryId],n=i.AnimatedSprite.fromFrames(i.Assets.cache.get("/assets/sprites/sheet.json").data.animations[s.animatedSpriteInfo.animationName]);if(this.pixiObj=n,n.animationSpeed=n.totalFrames/this.app.ticker.maxFPS/s.animatedSpriteInfo.targetTimeInSeconds,n.loop=s.animatedSpriteInfo.isLoop,s.animatedSpriteInfo.isLoop)n.play();else if(null==e.objectAnimationInfo)n.gotoAndPlay(0);else{const t=Math.floor(n.totalFrames*(e.objectAnimationInfo.secondsCompleted/s.animatedSpriteInfo.targetTimeInSeconds));n.gotoAndPlay(t)}this.parent.addChild(n)}}e.SceneObject=o,e.GameScene=class{CheckIfObjectInViewport(e,t){e.x*=this.currentStageScale,e.y*=this.currentStageScale,t.x*=this.currentStageScale,t.y*=this.currentStageScale;const s=new n.ClientVector2;s.x=e.x+.5*t.x,s.y=e.y+.5*t.y;const i=new n.ClientVector2;return i.x=e.x-.5*t.x,i.y=e.y-.5*t.y,!(s.x<0&&s.y<0||i.x>n.playerViewportMaxSize.x&&i.y>n.playerViewportMaxSize.y)}ConvertPointFromGlobalSpaceToViewSpace(e,t){const s=new n.ClientVector2;return s.x=10*(e.x-t.x)+.5*this.app.screen.width/this.currentStageScale,s.y=10*-(e.y-t.y)+.5*this.app.screen.height/this.currentStageScale,s}ConvertVectorFromGlobalSpaceToViewSpace(e){const t=new n.ClientVector2;return t.x=10*e.x,t.y=10*e.y,t}async PrepareSpriteSheet(){await i.Assets.load(["/assets/sprites/sheet.json"]),this.isReady=!0}HandleWindowResize(){this.app.renderer.resize(window.innerWidth,window.innerHeight),this.currentStageScale=Math.max(1,window.innerWidth/n.playerViewportMaxSize.x,window.innerHeight/n.playerViewportMaxSize.y),this.app.stage.scale.set(this.currentStageScale),this.backgroundController.HandleWindowResize()}constructor(e,t,n=30,a){this.presentObjects={},this.currentStageScale=1,this.isReady=!1,this.progressBarSample=a,this.PrepareSpriteSheet(),this.graphicLibrary=new s,this.app=new i.Application({width:window.innerWidth,height:window.innerHeight,backgroundColor:"#000000",autoDensity:!0,resolution:window.devicePixelRatio}),this.backgroundController=new r("back0.png",this.app),this.app.ticker.maxFPS=n,e.body.appendChild(this.app.view),this.app.ticker.add(t),this.app.ticker.start(),this.app.view.hidden=!0,this.HandleWindowResize(),window.addEventListener("resize",(()=>{this.HandleWindowResize()})),this.mainContainer=new i.Container,this.mainContainer.width=0,this.mainContainer.height=0,this.mainContainer.pivot.set(.5),this.app.stage.addChild(this.mainContainer)}DisplayGameObject(e,t,s){const i=this.ConvertPointFromGlobalSpaceToViewSpace(t.position,s);if(this.CheckIfObjectInViewport(i,n.GetEstimatedSize(t.graphicInfo))){let s=null;e in this.presentObjects?s=this.presentObjects[e]:(s=new o(e,t,this.mainContainer,this.app,this.graphicLibrary),this.presentObjects[e]=s),s.Repaint(t,this,this.graphicLibrary)}else this.EnsureGraphicObjectRemoved(e)}DisplayGameObjects(e){if(!this.isReady||!this.graphicLibrary.isReady)return;const t=e.objects[e.gameObjectsId].position,s=this.ConvertVectorFromGlobalSpaceToViewSpace(t);s.x=-s.x+.5*this.app.screen.width/this.currentStageScale,s.y=s.y+.5*this.app.screen.height/this.currentStageScale,this.mainContainer.position.set(s.x,s.y);for(const s in e.objects){const i=e.objects[s];this.DisplayGameObject(s,i,t)}this.RemoveUnusedGraphics(e)}RemoveAllGraphics(){for(const e in this.presentObjects)this.EnsureGraphicObjectRemoved(e)}RemoveUnusedGraphics(e){for(const t in this.presentObjects)t in e.objects||this.EnsureGraphicObjectRemoved(t)}EnsureGraphicObjectRemoved(e){this.presentObjects[e]&&(this.presentObjects[e].Destroy(),delete this.presentObjects[e])}}}(r||(t.Graphic=r={}))},7483:(e,t,s)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.InputUtils=void 0;const i=s(9571);var n;!function(e){e.KeyboardInputHandler=class{constructor(e){this.keyStates={},this.leftMouseButtonPressed=!1,this.lastAngle=0,this.lastMovementPower=0,e.addEventListener("keydown",(e=>{null!=e.target&&e.target.matches("input, button")||(this.keyStates[e.code.toUpperCase()]=!0)})),e.addEventListener("keyup",(e=>{null!=e.target&&e.target.matches("input, button")||(this.keyStates[e.code.toUpperCase()]=!1)})),e.addEventListener("mousemove",(e=>{let t=e.clientX-.5*window.innerWidth,s=e.clientY-.5*window.innerHeight;const n=new i.ClientVector2;n.x=t,n.y=s,this.lastMovementPower=Math.max(Math.min((n.Length()-100)/200,1),0),0==s&&0==t&&(s=1),this.lastAngle=Math.atan2(-s,t)})),e.addEventListener("mousedown",(e=>{null!=e.target&&e.target.matches("input, button")||0===e.button&&(this.leftMouseButtonPressed=!0)})),e.addEventListener("mouseup",(e=>{null!=e.target&&e.target.matches("input, button")||0===e.button&&(this.leftMouseButtonPressed=!1)}))}IsKeyDown(e){return this.keyStates[e.toUpperCase()]}GetUsersInput(){const e=new i.ClientInput;return e.angle=this.lastAngle,e.movementPower=this.lastMovementPower,e.isFire=this.leftMouseButtonPressed,e}}}(n||(t.InputUtils=n={}))},1169:(e,t,s)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.Network=void 0;const i=s(728),n=s(9638),a=s(5117);var r;!function(e){e.ServerConnection=class{constructor(e,t){this.connection=(new i.HubConnectionBuilder).withUrl(e).withHubProtocol(new n.MessagePackHubProtocol).build(),this.serverId=t,this.mapper=new a.Mapper}async SendChatMessage(e){return await this.connection.invoke("AddChatMessage",this.serverId,e)}async GetChatMessages(e){return await this.connection.invoke("GetChatMessages",this.serverId,e)}async InitializeServerConnection(e){this.connection.on("ReceivePerosnalInfo",(t=>{if(this.mapper.isReady){const s=this.mapper.mapClientPersonalInfoFromServer(t);e(s)}})),this.connection.on("ReceiveRemovalFromGameNotification",(()=>{document.location="/Game/Kicked"})),await this.connection.start(),await this.connection.invoke("SubscribeToUpdates",this.serverId)}async Revive(){await this.connection.invoke("Revive",this.serverId)}async SendMyInput(e){if(this.mapper.isReady){const t=this.mapper.mapClientInputToServer(e);await this.connection.invoke("SendInput",this.serverId,t)}}}}(r||(t.Network=r={}))},6305:(e,t)=>{"use strict";var s;Object.defineProperty(t,"__esModule",{value:!0}),t.Technical=void 0,function(e){e.TechnicalInfo=class{constructor(){this.goodFrames=0,this.extraGoodFrames=0,this.droppedFrames=0,this.lastUpdateTime=(new Date).getTime(),this.lastFrameTimeMs=0}}}(s||(t.Technical=s={}))},7361:(e,t,s)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.UI=void 0;const i=s(9571);var n;!function(e){e.ChatController=class{async SendMessage(){if(null==this.sendMessageNetworkHandler)return;const e=this.messageContainer.value;null==e||e.length>100||0==e.length||await this.sendMessageNetworkHandler(e)&&(this.messageContainer.value="")}GetLastReceivedMaxId(){return this.lastReceivedMaxMessageId}StoreNewMessages(e){if(null==e||0==e.length)return;const t=Math.max(...e.map((e=>e.id)));if(!(t<=this.lastReceivedMaxMessageId)){for(const t of e)t.id>this.lastReceivedMaxMessageId&&this.storedMessages.push(t);this.storedMessages.length>10&&(this.storedMessages=this.storedMessages.slice(this.storedMessages.length-10)),this.lastReceivedMaxMessageId=t,this.Repaint()}}Repaint(){var e,t;this.textContainer.innerHTML="";for(const s of this.storedMessages.slice().reverse())this.textContainer.innerHTML+="<br>",this.textContainer.innerHTML+=`${null===(e=s.message)||void 0===e?void 0:e.senderNick}:${null===(t=s.message)||void 0===t?void 0:t.message}`}constructor(e,t,s,i){this.lastReceivedMaxMessageId=-1,this.storedMessages=[],this.sendButton=e,this.textContainer=t,this.messageContainer=s,this.sendMessageNetworkHandler=i,this.sendButton.onclick=()=>{this.SendMessage()}}},e.GameUI=class{AddInputFromUI(e){e.investmentRequest=this.lastInvestmentRequest,e.repairRequest=this.lastRepairRequest,this.lastInvestmentRequest=null,this.lastRepairRequest=!1}constructor(e,t,s,i,n,a,r){this.lastInvestmentRequest=null,this.lastRepairRequest=!1,this.debugMessage=s,this.reviveButton=e,this.reviveMessage=t,this.playersTable=i,this.safeZoneMessage=n,this.investmentsTable=a,this.damageEffects=r}RepaintDamageEffects(e){if(null!=e&&i.IsPlayerInGame(e)){const t=e.health/e.maxHealth;this.damageEffects.hidden=!1,this.damageEffects.style.opacity=(.3*(1-t)).toString()}else this.damageEffects.hidden=!0}RepaintSafeZoneMessage(e){this.safeZoneMessage.hidden=null==e||!e.isSafeZone}RepaintInvestmentsTable(e){if(null!=e&&null!=e.alreadyInvested&&e.isSafeZone){this.investmentsTable.hidden=!1;const t=this.investmentsTable.querySelector("#repairButton");e.health<e.maxHealth&&e.points>=e.maxHealth-e.health?(t.hidden=!1,t.onclick=()=>{this.lastRepairRequest=!0},t.innerHTML=`Repair for ${e.maxHealth-e.health} points`):t.hidden=!0;const s=this.investmentsTable.querySelector("tbody");for(let t=0;t<=6;t++){const n=null==s.rows[t]?s.insertRow():s.rows[t],a=e.alreadyInvested[t];(null==n.cells[0]?n.insertCell(0):n.cells[0]).innerHTML=i.playerAttributes[t],(null==n.cells[1]?n.insertCell(1):n.cells[1]).innerHTML=`${a}/100`;const r=null==n.cells[2]?n.insertCell(2):n.cells[2];if(null==r.children[0]){const e=document.createElement("div");e.innerHTML="Update (10 points)",e.classList.add("btn","btn-info","btn-sm"),r.appendChild(e)}const o=r.children[0];a<100&&e.points>=10?(o.hidden=!1,n.classList.add("table-success"),o.onclick=()=>{this.lastInvestmentRequest=t}):(o.hidden=!0,n.classList.remove("table-success"))}}else this.investmentsTable.hidden=!0}DebugMyPositionAndVelocity(e,t){const s=e.objects[e.gameObjectsId];if(this.debugMessage.innerHTML=`Position: ${s.position.x.toFixed(2)},${s.position.y.toFixed(2)};<br>Velocity: ${s.velocity.x.toFixed(2)},${s.velocity.y.toFixed(2)}<br>Drops: ${t.droppedFrames}<br>Goods: ${t.goodFrames}<br>Extra goods: ${t.extraGoodFrames}`,this.debugMessage.innerHTML+=`<br> Last frame time: ${t.lastFrameTimeMs} ms`,this.debugMessage.innerHTML+=`<br>Players on server: ${e.playersCount}`,0!=t.goodFrames&&(this.debugMessage.innerHTML+=`<br>DropsToGoods: ${Math.round(t.droppedFrames/t.goodFrames*100)}%`,this.debugMessage.innerHTML+=`<br>ExtraGoodsToGoods: ${Math.round(t.extraGoodFrames/t.goodFrames*100)}%`),"connection"in navigator){const e=navigator.connection;this.debugMessage.innerHTML+=`<br>Network: ${e.effectiveType}`,this.debugMessage.innerHTML+=`<br>RRT: ${e.rtt} ms`}}RepaintReviveInterface(e,t){null!=e&&e.state!=i.PlayerState.Alive?(this.reviveMessage.hidden=!1,this.reviveButton.onclick=t):this.reviveMessage.hidden=!0}}}(n||(t.UI=n={}))},6771:()=>{},8022:()=>{},6045:()=>{},8190:()=>{},7333:()=>{},6139:()=>{},4654:()=>{}},s={};function i(e){var n=s[e];if(void 0!==n)return n.exports;var a=s[e]={id:e,loaded:!1,exports:{}};return t[e].call(a.exports,a,a.exports,i),a.loaded=!0,a.exports}i.m=t,e=[],i.O=(t,s,n,a)=>{if(!s){var r=1/0;for(c=0;c<e.length;c++){for(var[s,n,a]=e[c],o=!0,l=0;l<s.length;l++)(!1&a||r>=a)&&Object.keys(i.O).every((e=>i.O[e](s[l])))?s.splice(l--,1):(o=!1,a<r&&(r=a));if(o){e.splice(c--,1);var h=n();void 0!==h&&(t=h)}}return t}a=a||0;for(var c=e.length;c>0&&e[c-1][2]>a;c--)e[c]=e[c-1];e[c]=[s,n,a]},i.d=(e,t)=>{for(var s in t)i.o(t,s)&&!i.o(e,s)&&Object.defineProperty(e,s,{enumerable:!0,get:t[s]})},i.g=function(){if("object"==typeof globalThis)return globalThis;try{return this||new Function("return this")()}catch(e){if("object"==typeof window)return window}}(),i.o=(e,t)=>Object.prototype.hasOwnProperty.call(e,t),i.r=e=>{"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},i.nmd=e=>(e.paths=[],e.children||(e.children=[]),e),i.j=757,(()=>{var e={757:0};i.O.j=t=>0===e[t];var t=(t,s)=>{var n,a,[r,o,l]=s,h=0;if(r.some((t=>0!==e[t]))){for(n in o)i.o(o,n)&&(i.m[n]=o[n]);if(l)var c=l(i)}for(t&&t(s);h<r.length;h++)a=r[h],i.o(e,a)&&e[a]&&e[a][0](),e[a]=0;return i.O(c)},s=self.webpackChunksample=self.webpackChunksample||[];s.forEach(t.bind(null,0)),s.push=t.bind(null,s.push.bind(s))})();var n=i.O(void 0,[712],(()=>i(8780)));return i.O(n)})()));
//# sourceMappingURL=game.js.map
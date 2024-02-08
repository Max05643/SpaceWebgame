import * as Sound from '@pixi/sound';
import * as GameDesign from './gamedesign';

export namespace Audio {



    export class MusicController {

        private musicInstance: Sound.Sound;
        private targetVol: number = 0.5;
        constructor() {
            if (localStorage.getItem("music_vol") != null) {
                const currentVol = parseFloat(localStorage.getItem("music_vol")!);
                this.targetVol = !isNaN(currentVol) ? currentVol : 0.5;
            }


            this.musicInstance = Sound.Sound.from('/assets/music/soundtrack.wav');
            this.musicInstance.loop = true;
            this.musicInstance.volume = this.targetVol;
            this.musicInstance.play();
        }
    }
    export class AudioController {

        private lastEffectPlayedId: number = -1;
        private targetVol: number = 0.5;

        constructor() {
            Sound.sound.add("ex0", "/assets/sound/explosion0.wav");
            Sound.sound.add("ex1", "/assets/sound/explosion1.wav");
            Sound.sound.add("lr", "/assets/sound/laser.wav");
            Sound.sound.add("pk", "/assets/sound/pick.wav");
            Sound.sound.add("mvt", "/assets/sound/movement.wav");

            if (localStorage.getItem("sound_vol") != null) {
                const currentVol = parseFloat(localStorage.getItem("sound_vol")!);
                this.targetVol = !isNaN(currentVol) ? currentVol : 0.5;
            }
        }



        private PlayOneShot(effect: GameDesign.SoundEffect, currentPlayerPosition: GameDesign.ClientVector2): void {

            if (effect.position == null) {
                const options: Sound.PlayOptions = {
                    volume: this.targetVol,
                    loop: false,
                };
                Sound.sound.play(effect.audioClipName, options);
            }
            else {


                const distanceSuqared = GameDesign.DistanceSquared(currentPlayerPosition, effect.position);
                const k = 1 - distanceSuqared / (effect.radius! * effect.radius!);



                if (k > 0) {

                    const filter = new Sound.filters.StereoFilter();
                    if (distanceSuqared > 1)
                        filter.pan = GameDesign.Normalized(GameDesign.Difference(effect.position, currentPlayerPosition)).x;
                    else
                        filter.pan = 0;

                    const options: Sound.PlayOptions = {
                        volume: k * this.targetVol,
                        loop: false,
                        filters: [filter]
                    };
                    Sound.sound.play(effect.audioClipName, options);
                }
            }
        }

        ProcessQueue(queue: GameDesign.SoundEffect[], currentPlayerPosition: GameDesign.ClientVector2): void {

            for (const effect in queue) {
                const currentEffect = queue[effect];

                if (currentEffect.id > this.lastEffectPlayedId) {
                    this.lastEffectPlayedId = currentEffect.id;
                    this.PlayOneShot(currentEffect, currentPlayerPosition);
                }
            }
        }
    }

}
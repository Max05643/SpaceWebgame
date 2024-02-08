const musicVolumeRange: HTMLInputElement = document.getElementById("music_vol") as HTMLInputElement;
const soundVolumeRange: HTMLInputElement = document.getElementById("sound_vol") as HTMLInputElement;

musicVolumeRange.oninput = () => HandleMusicVolumeChange();
soundVolumeRange.oninput = () => HandleSoundVolumeChange();


if (localStorage.getItem("music_vol") != null) {
    const currentVol = parseFloat(localStorage.getItem("music_vol")!);
    if (!isNaN(currentVol)) {
        musicVolumeRange.value = currentVol.toString();
    }
}
if (localStorage.getItem("sound_vol") != null) {
    const currentVol = parseFloat(localStorage.getItem("sound_vol")!);
    if (!isNaN(currentVol)) {
        soundVolumeRange.value = currentVol.toString();
    }
}



function HandleMusicVolumeChange(): void {
    localStorage.setItem("music_vol", musicVolumeRange.value);
    console.log(musicVolumeRange.value);
}

function HandleSoundVolumeChange(): void {
    localStorage.setItem("sound_vol", soundVolumeRange.value);
    console.log(soundVolumeRange.value);
}
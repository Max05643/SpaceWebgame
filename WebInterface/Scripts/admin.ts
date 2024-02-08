import axios from 'axios';

const serversTable: HTMLTableElement = document.getElementById("servers-table") as HTMLTableElement;
const startGameButton: HTMLButtonElement = document.getElementById("startgamebutton") as HTMLButtonElement;
const stopGameButton: HTMLButtonElement = document.getElementById("stopgamebutton") as HTMLButtonElement;
const applyMapForm: HTMLFormElement = document.getElementById("applymapform") as HTMLFormElement;
const gameIdApplyMapForm: HTMLInputElement = document.getElementById("gameidapplymapform") as HTMLInputElement;


let selectedGameId: string | null = null;


let serversList: string[] = [];


async function UpdateServersList(): Promise<void> {
    serversList = (await axios.get<string[]>("/Admin/GetRunningServers")).data;

    if (selectedGameId != null && serversList.indexOf(selectedGameId) == -1) {
        selectedGameId = null;
    }
}
async function ApplyMapToGameFormOpen(gameId: string): Promise<void> {


    await Repaint();
}
async function StopGame(gameId: string): Promise<void> {

    const data =
    {
        gameId: gameId
    }

    await axios.post("/Admin/StopGame", data, {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    });

    await Repaint();
}
async function StartGame(): Promise<void> {
    await axios.post("/Admin/StartNewGame");
    await Repaint();
}

async function SelectGameFromTable(gameId: string): Promise<void> {
    if (selectedGameId == gameId)
        selectedGameId = null;
    else
        selectedGameId = gameId;
    await Repaint();
}


function RepaintButtons(): void {
    startGameButton.onclick = StartGame;

    if (selectedGameId != null) {
        stopGameButton.hidden = false;

        stopGameButton.onclick = () => StopGame(selectedGameId!);
    }
    else {
        stopGameButton.hidden = true;
    }

}

async function RepaintServersTable(): Promise<void> {
    const innerContent = serversTable.querySelector('tbody') as HTMLTableSectionElement;
    innerContent.innerHTML = "";


    for (const server in serversList) {
        const row = innerContent.insertRow();


        row.onclick = () => SelectGameFromTable(serversList[server]);

        if (selectedGameId != null && selectedGameId == serversList[server])
            row.classList.add('table-success');

        const cell1 = row.insertCell(0);
        cell1.innerHTML = serversList[server];

        const cell2 = row.insertCell(1);
        cell2.innerHTML = (await axios.get<number | null>(`/Admin/GetPlayersCount/?serverId=${serversList[server]}`)).data?.toString() ?? "Error";
    }
}


function RepaintApplyMapForm(): void {
    if (selectedGameId == null) {
        applyMapForm.hidden = true;
    }
    else {
        applyMapForm.hidden = false;
        gameIdApplyMapForm.value = selectedGameId;
    }
}

async function Repaint(): Promise<void> {
    await UpdateServersList();
    await RepaintServersTable();
    RepaintButtons();
    RepaintApplyMapForm();
}

async function Init(): Promise<void> {
    await Repaint();
}



Init();
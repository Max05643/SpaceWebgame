import axios from 'axios';

/** Used to map minimized objects to their full versions */
export class Mapper {
    private clientPersonalInfoFromServerMapping: { [key: string]: string } = {};
    private clientInputToServerMapping: { [key: string]: string } = {};
    isReady: boolean = false;


    private async Initialize(): Promise<void> {

        const result1 = await axios.get("/assets/clientInputMap.json");
        this.clientInputToServerMapping = result1.data;

        const result2 = await axios.get("/assets/clientPersonalInfoMap.json");
        this.clientPersonalInfoFromServerMapping = result2.data;

        this.isReady = true;
    }

    constructor() {
        this.Initialize();
    }

    mapClientInputToServer(obj: any) {
        return this.mapObj(obj, this.clientInputToServerMapping);
    }

    mapClientPersonalInfoFromServer(obj: any) {
        return this.mapObj(obj, this.clientPersonalInfoFromServerMapping);
    }

    private mapObj(obj: any, mapping: { [key: string]: string }) {
        const result: any = {};

        for (const key in obj) {
            const newKey = (mapping[key] == undefined) ? key : mapping[key];

            if (obj[key] == null) {
                result[newKey] = null;
            }
            else {
                result[newKey] = typeof obj[key] === 'object' ? this.mapObj(obj[key], mapping) : obj[key];
            }

        }
        return result;
    }
}

export namespace Technical {

    /** Stores technical information about current game */
    export class TechnicalInfo {

        /** Number of frames that were correctly retrieved from the server before being rendered */
        goodFrames: number = 0;

        /** Number of frames that were correctly retrieved from the server before being rendered, but were unnecessary */
        extraGoodFrames: number = 0;

        /** Number of frames that were extrapolated */
        droppedFrames: number = 0;

        lastUpdateTime: number = new Date().getTime();

        lastFrameTimeMs: number = 0;
    }

}

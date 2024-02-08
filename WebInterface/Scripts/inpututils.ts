import * as GameDesign from './gamedesign';

export namespace InputUtils {
    /**Handles user input, i.e. allows to retrieve keys that a currently pressed*/
    export class KeyboardInputHandler {
        keyStates: { [key: string]: boolean } = {};
        leftMouseButtonPressed: boolean = false;
        lastAngle: number = 0;
        lastMovementPower: number = 0;

        constructor(document: Document) {
            document.addEventListener('keydown', (event) => {
                if (event.target == null || !(event.target as Element).matches('input, button')) {
                    this.keyStates[event.code.toUpperCase()] = true;
                }
            });

            document.addEventListener('keyup', (event) => {
                if (event.target == null || !(event.target as Element).matches('input, button')) {
                    this.keyStates[event.code.toUpperCase()] = false;
                }
            });


            document.addEventListener('mousemove', (event) => {
                let mouseX = event.clientX - window.innerWidth * 0.5;
                let mouseY = event.clientY - window.innerHeight * 0.5;


                const mouseVector = new GameDesign.ClientVector2();
                mouseVector.x = mouseX;
                mouseVector.y = mouseY;
                this.lastMovementPower = Math.max(Math.min((mouseVector.Length() - 100) / 200, 1), 0);

                if (mouseY == 0 && mouseX == 0)
                    mouseY = 1;



                this.lastAngle = Math.atan2(-mouseY, mouseX);
            });


            document.addEventListener('mousedown', (event) => {
                if (event.target == null || !(event.target as Element).matches('input, button')) {
                    if (event.button === 0) {
                        this.leftMouseButtonPressed = true;
                    }
                }
            });

            document.addEventListener('mouseup', (event) => {
                if (event.target == null || !(event.target as Element).matches('input, button')) {
                    if (event.button === 0) {
                        this.leftMouseButtonPressed = false;
                    }
                }
            });
        }

        private IsKeyDown(key: string): boolean {
            return this.keyStates[key.toUpperCase()];
        }


        /** Retrieves user input from current keyboard + mouse state */
        GetUsersInput(): GameDesign.ClientInput {
            const result = new GameDesign.ClientInput();

            result.angle = this.lastAngle;
            result.movementPower = this.lastMovementPower;
            result.isFire = this.leftMouseButtonPressed;

            return result;
        }
    }



}


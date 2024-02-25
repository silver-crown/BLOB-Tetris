using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Tetris
{


    public class InputManager
    {
        public float timeSinceLastMovement = 0f;
        private readonly float movementCooldown = 0.1f;
        private bool spacePressedLastFrame = false;
        private readonly TetrisGame _game;
        public InputManager(TetrisGame game) => _game = game;
        public void ProcessInput() {
            KeyboardState state = Keyboard.GetState();

            if ((state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A)) && timeSinceLastMovement >= movementCooldown) {
                _game.pieceManager.MovePiece(-1, 0);
                timeSinceLastMovement = 0f;
            }
            else if ((state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D)) && timeSinceLastMovement >= movementCooldown) {
                _game.pieceManager.MovePiece(1, 0);
                timeSinceLastMovement = 0f;
            }
            else if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
                _game.pieceManager.MovePieceDown();
            else if (state.IsKeyDown(Keys.Space) && !spacePressedLastFrame)
                _game.pieceManager.RotatePiece();
            else if (state.IsKeyDown(Keys.E) && !_game.pieceManager.GetAlreadyStoredAPiece()) {
                _game.pieceManager.StorePiece();
            }
            spacePressedLastFrame = state.IsKeyDown(Keys.Space);
        }
    }
}

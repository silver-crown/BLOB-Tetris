using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Tetris
{
    internal class GameplayState : State
    {
        private TetrisGame _game;
        private bool _contentLoaded = false;
        private bool _pausing = false;
        private bool _playingTetrisSong = false;
        private bool escapePressedLastFrame = false;
        public GameplayState(TetrisGame game) => _game = game;
        public override void Execute(float deltaTime, ContentManager contentManager) {
            if (!_contentLoaded) {
                LoadContent(contentManager); 
            }
            Update(deltaTime);
        }

        private void LoadContent(ContentManager contentManager) {
            _game.soundManager.LoadContent();
            _game.pieceManager.LoadContent();
            _game.gameRenderer.LoadContent(contentManager);
            _game.pieceManager.ShowNextPiece();
            _game.pieceManager.SpawnNewPiece();
            _contentLoaded = true;
        }

        private void Update(float deltaTime) {
            if (!_playingTetrisSong) {
                _game.soundManager.PlaySong(_game.soundManager.tetrisMusic, 0.5f, true);
                _playingTetrisSong = true;
            }

            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape) && !escapePressedLastFrame) {
                    _game.stateMachine.PauseState(this);
                    _game.stateMachine.SetState(new PauseState(_game));
                    _playingTetrisSong = false;
            }
            if(_game.pieceManager.GetGameOverStatus() == true) {
                _game.stateMachine.SetState(new GameOverState());
            }
            _game.inputManager.timeSinceLastMovement += deltaTime;
            _game.pieceManager.LineClearGlow(deltaTime);
            _game.pieceManager.GravityAndInputLogic(deltaTime);
            _game.pieceManager.LevelLogic();
            escapePressedLastFrame = state.IsKeyDown(Keys.Escape);
        }
    }
}

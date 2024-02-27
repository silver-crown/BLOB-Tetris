using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Tetris
{
    public class PauseState : State
    {
        private bool playedPauseSound;
        private bool escapePressedLastFrame = true;
        private TetrisGame _game;

        public PauseState(TetrisGame game) => _game = game;
        public override void Execute(float deltaTime, ContentManager contentManager) {
            if (!playedPauseSound) {
                _game.soundManager.StopSong();
                _game.soundManager.PlaySoundEffect(_game.soundManager.ZA_WARUDO);
                playedPauseSound = true;
            }
            Update(deltaTime);
        }
        public void Update(float deltaTime) {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape) && !escapePressedLastFrame) {
                _game.stateMachine.ResumeState();
                playedPauseSound = false;
            }
            escapePressedLastFrame = state.IsKeyDown(Keys.Escape);
        }
    }
}


using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Tetris
{
    public class StateMachine {
        public enum States {
            NONE,
            OPENING,
            PLAYING,
            GAMEOVER,
            PAUSE
        }
        private GameplayState _gameplayState;
        private GameOverState _gameoverState;
        private PauseState _pausedState;
        private OpeningScreenState _openingScreenState;
        private TetrisGame _game;
        States currentState = States.PLAYING;
        public StateMachine(TetrisGame game) {
            _game = game;
        }
        public void Initialize() {
            _gameplayState = new GameplayState(_game);
            _gameoverState = new GameOverState();
            _pausedState = new PauseState(_game);
            _openingScreenState = new OpeningScreenState();
        }
        public void SetState(States state) => currentState = state;
        public States GetState() => currentState;

        public void RunState(float deltaTime, ContentManager contentManager) {
            switch (currentState) {
                case States.NONE:
                    break;
                case States.OPENING:
                    break;
                case States.PLAYING:
                    _gameplayState.Execute(deltaTime, contentManager);
                    break;
                case States.GAMEOVER:

                    break;
                case States.PAUSE:
                    _pausedState.Execute(deltaTime, contentManager);
                    break;
            }

        }
    }
}


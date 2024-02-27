using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Tetris
{
    public class StateMachine {
        private State _currentState;
        private GameplayState _gameplayState;
        private TetrisGame _game;
        private State _pausedState;
        public StateMachine(TetrisGame game) {
            _game = game;
        }
        public void Initialize() {
            _gameplayState = new GameplayState(_game);
            _currentState = _gameplayState;
        }
        public void Update(float deltaTime, ContentManager contentManager) {
            RunState(deltaTime, contentManager, _currentState);
        } 

        private void RunState(float deltaTime, ContentManager contentManager, State state) {
            state.Execute(deltaTime, contentManager);
        }
        public State GetState() => _currentState;
        public void SetState(State state) => _currentState  = state;
        public void PauseState(State state) => _pausedState = state;
        public void ResumeState() => SetState(_pausedState);
    }
}


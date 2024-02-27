using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Monogame_Tetris
{
    public class TetrisGame : Game
    {

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Random random;
        private readonly float BlockSize = 40.0f;
        private const int BoardWidth = 10;
        private const int BoardHeight = 20;
        private int linesCleared = 0;
        private int playerScore;

        private int level = 1;

        public StateMachine stateMachine;
        public GameRenderer gameRenderer;
        public PieceManager pieceManager;
        public SoundManager soundManager;
        public InputManager inputManager;

        public int GetPlayerScore() => playerScore;
        public int IncreasePlayerScore(int i) => playerScore += i;
        public int GetLevel() => level;
        public int SetLevel(int i) => level = i;
        public int GetLinesCleared() => linesCleared;
        public void IncreaseLinesCleared() => linesCleared++;

        public TetrisGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Set the window size based on the board size and block size
            _graphics.PreferredBackBufferWidth = (int)(BoardWidth * BlockSize * 2);
            _graphics.PreferredBackBufferHeight = (int)(BoardHeight * BlockSize + 20);
        }

        protected override void Initialize() {
            stateMachine = new StateMachine(this);
            stateMachine.Initialize();
            random = new Random();
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            soundManager = new SoundManager(Content);
            pieceManager = new PieceManager(Content, this);
            inputManager = new InputManager(this);
            gameRenderer = new GameRenderer(_spriteBatch);
        }

        protected override void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //run the state machine, which dictates the behavior of the game
            stateMachine.Update(deltaTime, Content);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);    
            // draw the game board
            gameRenderer.DrawGame(this);
            base.Draw(gameTime);
        }
    }
}
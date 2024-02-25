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
    

        public GameRenderer gameRenderer;
        public PieceManager pieceManager;
        public SoundManager soundManager;
        public InputManager inputManager;

        public int GetPlayerScore() => playerScore;
        public int SetPlayerScore(int i) => playerScore += i;
        public int GetLevel() => level;
        public int SetLevel(int i) => level += i;
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
         
            random = new Random();
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            soundManager = new SoundManager(Content);
            soundManager.LoadContent();
            pieceManager = new PieceManager(Content, this);
            pieceManager.LoadContent();
            soundManager.PlaySong(soundManager.tetrisMusic,0.5f,true);
            inputManager = new InputManager(this);
            gameRenderer = new GameRenderer(_spriteBatch);
            gameRenderer.LoadContent(Content);
            pieceManager.ShowNextPiece();
            pieceManager.SpawnNewPiece();
        }

        protected override void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            inputManager.timeSinceLastMovement += deltaTime;
            pieceManager.LineClearGlow(deltaTime);
            pieceManager.GravityAndInputLogic(deltaTime);
            
            LevelLogic();

            base.Update(gameTime);
        }
        //logic for moving to the next level and increasing fallspeed
        void LevelLogic() {
            switch (linesCleared) {
                default:
                    pieceManager.SetFallSpeed(0.5f);
                    break;
                case var _ when linesCleared >= 20:
                    pieceManager.SetFallSpeed(0.07f);
                    level = 5;
                    break;
                case var _ when linesCleared >= 15:
                    pieceManager.SetFallSpeed(0.1f);
                    level = 4;
                    break;
                case var _ when linesCleared >= 10:
                    pieceManager.SetFallSpeed(0.2f);
                    level = 3;
                    break;
                case var _ when linesCleared >= 5:
                    pieceManager.SetFallSpeed(0.3f);
                    level = 2;
                    break;
            }
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();
            // draw the game board
            gameRenderer.DrawGame(this);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
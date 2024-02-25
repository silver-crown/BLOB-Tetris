using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Monogame_Tetris
{
    public class GameRenderer
    {
        private readonly SpriteBatch _spriteBatch;
        private const int _boardWidth = 10;
        private const int _boardHeight = 20;
        private float _blockSize = 40.0f;
        private Effect _glowEffect;
        public Texture2D blockTexture;
        public Texture2D backgroundImage;

        public GameRenderer( SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;
        }

   
        public void LoadContent(ContentManager c) {
            _glowEffect = c.Load<Effect>("GlowEffect");
            blockTexture = c.Load<Texture2D>("block");
            backgroundImage = c.Load<Texture2D>("purpleCastleBackground");
        }
        

        public void DrawGame(TetrisGame game) {
            DrawGameBackground(); 
            DrawGameBoard(game);
            DrawGameBorders();
            DrawCurrentPiece(game);
            DrawNextPiece(game);
            DrawSavedPiece(game);
            DrawPlayerScore(game);
            DrawCurrentLevel(game);
            DrawNumberOfLinesCleared(game);
        }
       
        private void DrawGameBackground() {
            //Draw the background
            _spriteBatch.Draw(
                texture: backgroundImage,
                position: new Vector2(1, 1),
                color: Color.White);
        }
        private void DrawGameBoard(TetrisGame game) {
            for (int x = 1; x < _boardWidth; x++) {
                for (int y = 0; y < _boardHeight; y++) {
                    int cellValue = game.pieceManager.GetGameBoard()[x, y].PieceType;
                    if (cellValue > 0) {
                        //If the cells are supposed to glow, apply the shader effect, else just draw normally.
                        if (game.pieceManager.GetGameBoard()[x, y].ShouldGlow) {
                            _spriteBatch.End();
                            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect: _glowEffect);
                            // Apply the shader
                            _glowEffect.CurrentTechnique.Passes[0].Apply();

                            _spriteBatch.Draw(
                                texture: blockTexture,
                                position: new Vector2(x * _blockSize, y * _blockSize),
                                color: game.pieceManager.GetPieceColors()[cellValue - 1]); // Use pieceColors array for landed pieces
                            _spriteBatch.End();
                            _spriteBatch.Begin();
                        }
                        else {
                            // Use cellValue - 1 to index into pieceColors array
                            _spriteBatch.Draw(
                                texture: blockTexture,
                                position: new Vector2(x * _blockSize, y * _blockSize),
                                color: game.pieceManager.GetPieceColors()[cellValue - 1]); // Use pieceColors array for landed pieces

                        }
                    }
                }
            }
        }
        private void DrawGameBorders() {
            // Draw the game board border on the left side
            for (int y = 0; y < _boardHeight; y++) {
                _spriteBatch.Draw(
                    texture: blockTexture,
                    position: new Vector2(0, y * _blockSize),
                    color: Color.Gray);
            }

            // Draw the game board border on the right side
            for (int y = 0; y <= _boardHeight; y++) {
                _spriteBatch.Draw(
                    texture: blockTexture,
                    position: new Vector2(_boardWidth * _blockSize, y * _blockSize),
                    color: Color.Gray);
            }
            // Draw the game board border on the right side
            for (int x = 1; x < _boardWidth; x++) {
                for (int y = 0; y <= _boardHeight; y++) {
                    _spriteBatch.Draw(
                        texture: blockTexture,
                        position: new Vector2((_boardWidth * _blockSize) + x * _blockSize, y * _blockSize),
                        color: Color.Black);
                }
            }

            // Draw the game board border on the bottom side
            for (int x = 0; x <= _boardWidth; x++) {
                _spriteBatch.Draw(
                    texture: blockTexture,
                    position: new Vector2(x * _blockSize, _boardHeight * _blockSize),
                    color: Color.Gray);
            }

        }
        private void DrawCurrentPiece(TetrisGame game) {
            // Draw the current piece
            foreach (var block in game.pieceManager.GetCurrentPiece() ) {
                _spriteBatch.Draw(
                    texture: blockTexture, // You can replace null with an actual texture for the blocks
                    position: new Vector2((block.X + game.pieceManager.GetCurrentPiecePosition().X) * _blockSize, (block.Y + game.pieceManager.GetCurrentPiecePosition().Y) * _blockSize),
                    color: game.pieceManager.GetCurrentPieceColor());
            }
        }
        private void DrawNextPiece(TetrisGame game) {
            //Draw the text for the next piece
            _spriteBatch.DrawString(
               game.Content.Load<SpriteFont>("default font"),
               $"NEXT",
               new Vector2(650 + _boardWidth, _boardHeight + 300),
               Color.White);
            //Draw the next piece
            foreach (var block in game.pieceManager.GetNextPiece()) {
                _spriteBatch.Draw(
                    texture: blockTexture, // You can replace null with an actual texture for the blocks
                    position: new Vector2((block.X + _boardWidth + 5) * _blockSize, (block.Y + _boardHeight - 10) * _blockSize),
                    color: game.pieceManager.GetNextPieceColor());
            }
        }
        private void DrawSavedPiece(TetrisGame game) {
            //Draw the text for the saved/stored piece
            _spriteBatch.DrawString(
               game.Content.Load<SpriteFont>("default font"),
               $"SAVED",
               new Vector2(650 + _boardWidth, _boardHeight + 530),
               Color.White);
            if (game.pieceManager.GetStoredPiece() != null) {

                //Draw the stored piece
                foreach (var block in game.pieceManager.GetStoredPiece()) {
                    _spriteBatch.Draw(
                        texture: blockTexture, // You can replace null with an actual texture for the blocks
                        position: new Vector2((block.X + _boardWidth + 5) * _blockSize, (block.Y + _boardHeight - 5) * _blockSize),
                        color: game.pieceManager.GetStoredPieceColor());
                }
            }
        }
        private void DrawPlayerScore(TetrisGame game) {
            //Draw the player score
            _spriteBatch.DrawString(
              game.Content.Load<SpriteFont>("default font"),
              $"Score: " + game.GetPlayerScore(),
              new Vector2(600 + _boardWidth, _boardHeight + 50),
              Color.White);
        }
        private void DrawCurrentLevel(TetrisGame game) {
            //Draw the current level
            _spriteBatch.DrawString(
              game.Content.Load<SpriteFont>("default font"),
              $"Level: " + game.GetLevel(),
              new Vector2(600 + _boardWidth, _boardHeight + 25),
              Color.White);
        }
        private void DrawNumberOfLinesCleared(TetrisGame game) {

            //Draw number of lines cleared
            _spriteBatch.DrawString(
              game.Content.Load<SpriteFont>("default font"),
              $"Lines cleared: " + game.GetLinesCleared(),
              new Vector2(600 + _boardWidth, _boardHeight + 100),
              Color.White);
        }
    }
}

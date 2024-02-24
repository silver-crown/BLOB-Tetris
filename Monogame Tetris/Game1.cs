using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Monogame_Tetris
{
    public class TetrisGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int BlockSize = 40;
        private const int BoardWidth = 10;
        private const int BoardHeight = 20;
        private int linesCleared = 0;
        private int playerScore = 0;

        private int[,] gameBoard;
        private List<Vector2> currentPiece;
        private List<Vector2> nextPiece;
        private List<Vector2> storedPiece;
        private Color currentPieceColor;
        private Color nextPieceColor;
        private Color storedPieceColor;
        private Vector2 currentPiecePosition;
        private float fallTime = 0f;
        private const float FallSpeed = 0.5f;
        private Texture2D blockTexture;
        private Texture2D backgroundImage;
        private bool spacePressedLastFrame = false;
        private float movementCooldown = 0.1f;
        private float timeSinceLastMovement = 0f;
        private int currentPieceType;
        private int storedPieceType;
        private Song tetrisMusic;
        TetrisBlock currentTetrisBlock;
        private bool alreadyStoredAPiece = false;
        private bool storedPieceNotNull = false; 
        private Random random;

        private Color[] pieceColors = new Color[]
        {
            Color.Cyan,    // I-shape
            Color.Yellow,  // O-shape
            Color.Orange,  // L-shape
            Color.Blue,    // J-shape
            Color.Green,   // S-shape
            Color.Red,     // Z-shape
            Color.Purple   // T-shape
        };

        public TetrisGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Set the window size based on the board size and block size
            _graphics.PreferredBackBufferWidth = BoardWidth * BlockSize * 2;
            _graphics.PreferredBackBufferHeight = BoardHeight * BlockSize + 20;
        }

        protected override void Initialize() {
            gameBoard = new int[BoardWidth, BoardHeight];
            currentPiece = new List<Vector2>();
            currentPiecePosition = new Vector2(4, 0); // Starting position of the piece
            random = new Random();
            currentTetrisBlock = new TetrisBlock();
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the Tetris music
            tetrisMusic = Content.Load<Song>("tetris music"); 
            MediaPlayer.Play(tetrisMusic);
            MediaPlayer.IsRepeating = true; // Set to true if you want the music to loop
            blockTexture = Content.Load<Texture2D>("block");
            backgroundImage = Content.Load<Texture2D>("purpleCastleBackground");
            ShowNextPiece();
            SpawnNewPiece();
        }

        protected override void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            fallTime += deltaTime;
            timeSinceLastMovement += deltaTime;

            if (fallTime >= FallSpeed) {
                MovePieceDown();
                fallTime = 0f;
            }

            ProcessInput();

            base.Update(gameTime);
        }

        private void ProcessInput() {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left) && timeSinceLastMovement >= movementCooldown) {
                MovePiece(-1, 0);
                timeSinceLastMovement = 0f;
            }
            else if (state.IsKeyDown(Keys.Right) && timeSinceLastMovement >= movementCooldown) {
                MovePiece(1, 0);
                timeSinceLastMovement = 0f;
            }
            else if (state.IsKeyDown(Keys.Down))
                MovePieceDown();
            else if (state.IsKeyDown(Keys.Up))
                MovePiece(0, 1);
            else if (state.IsKeyDown(Keys.Space) && !spacePressedLastFrame)
                RotatePiece();
            else if (state.IsKeyDown(Keys.E) && !alreadyStoredAPiece) {
                StorePiece();
            }
            spacePressedLastFrame = state.IsKeyDown(Keys.Space);
        }

        private void MovePiece(int xOffset, int yOffset) {
            Vector2 newPosition = currentPiecePosition + new Vector2(xOffset, yOffset);

            if (IsMoveValid(newPosition, currentPiece)) {
                currentPiecePosition = newPosition;
            }
        }

        private void MovePieceDown() {
            Vector2 newPosition = currentPiecePosition + new Vector2(0, 1);

            if (IsMoveValid(newPosition, currentPiece)) {
                currentPiecePosition = newPosition;
            }
            else {
                // Lock the piece in place
                LockPiece();
                ClearLines();
                alreadyStoredAPiece = false;
                SpawnNewPiece();
            }
        }

        private void RotatePiece() {
            List<Vector2> rotatedPiece = new List<Vector2>();


            // Compute the center of the piece
            Vector2 center = currentPiecePosition + new Vector2(1.5f, 1.5f);
            if (currentPieceType == 1) // If it's a square piece, don't rotate
    {
                rotatedPiece = currentPiece;
                return;
            }
            foreach (var block in currentPiece) {
                // Rotate the block around the center of the piece
                Vector2 relativePosition = block - new Vector2(1.5f, 1.5f);
                Vector2 rotatedPosition = new Vector2(relativePosition.Y, -relativePosition.X);
                Vector2 absolutePosition = rotatedPosition + new Vector2(1.5f, 1.5f);

                rotatedPiece.Add(absolutePosition);
            }

            if (IsMoveValid(currentPiecePosition, rotatedPiece)) {
                currentPiece = rotatedPiece;
            }

            // Special handling for the line piece
            if (currentPieceType == 0) {
                // Attempt additional wall kicks for the line piece
                int[] lineKicksX = { 0, 1, -1, 2, -2 }; // Adjust these values based on your needs
                int[] lineKicksY = { 0, 1, -1, 2, -2 }; // Adjust these values based on your needs

                for (int i = 0; i < lineKicksX.Length; i++) {
                    Vector2 kick = new Vector2(lineKicksX[i], lineKicksY[i]);

                    if (IsMoveValid(currentPiecePosition + kick, rotatedPiece)) {
                        currentPiece = rotatedPiece;
                        currentPiecePosition += kick;
                        return;
                    }
                }
            }

            // Attempt wall kicks
            int[] wallKicksX = { 0, 1, -1, 2, -2 }; // Adjust these values based on your needs
            int[] wallKicksY = { 0, 1, -1, 2, -2 }; // Adjust these values based on your needs

            for (int i = 0; i < wallKicksX.Length; i++) {
                Vector2 kick = new Vector2(wallKicksX[i], wallKicksY[i]);

                if (IsMoveValid(currentPiecePosition + kick, rotatedPiece)) {
                    currentPiece = rotatedPiece;
                    currentPiecePosition += kick;
                    return;
                }
            }
        }

        private void LockPiece() {
            foreach (var block in currentPiece) {
                int x = (int)block.X + (int)currentPiecePosition.X;
                int y = (int)block.Y + (int)currentPiecePosition.Y;

                // Check if the cell is not already occupied before locking
                if (x >= 1 && x < BoardWidth && y >= 0 && y < BoardHeight && gameBoard[x, y] == 0) {
                    gameBoard[x, y] = currentPieceType + 1; // Use currentPieceType + 1 as the value for the piece type
                }
            }
            
        }

        private void ClearLines() {
            int cleared = 0;
            for (int y = BoardHeight - 1; y >= 0; y--) {
                bool lineIsFull = true;

                for (int x = 1; x < BoardWidth; x++) {
                    if (gameBoard[x, y] == 0) {
                        lineIsFull = false;
                        break;
                    }
                }

                if (lineIsFull) {
                    // Clear the line
                    for (int newY = y; newY > 0; newY--) {
                        for (int x = 0; x < BoardWidth; x++) {
                            gameBoard[x, newY] = gameBoard[x, newY - 1];
                        }
                    }

                    // Add a new empty line at the top
                    for (int x = 1; x < BoardWidth; x++) {
                        gameBoard[x, 0] = 0;
                    }

                    // Check the same line again
                    y++;
                    linesCleared++;
                    cleared++;
                }
            }
            switch (cleared){
                default:
                    break;
                case 1: playerScore += 40;
                    break;
                case 2: playerScore += 100;
                    break;
                case 3: playerScore += 300;
                    break;
                case 4: playerScore += 1200;
                    break;
            }
        }

        private void SpawnNewPiece() {
            currentPiece = currentTetrisBlock.Spawn();
            currentPieceColor = currentTetrisBlock.GetColor();
            currentPieceType = currentTetrisBlock.GetPieceType();
            ShowNextPiece();
            nextPieceColor = currentTetrisBlock.GetColor();
            // Set the initial position of the new piece
            currentPiecePosition = new Vector2((BoardWidth - 4) / 2, 0);
        }

        private void ShowNextPiece() {
            nextPiece = currentTetrisBlock.Init(random);
        }

        private void StorePiece() {
            var p = currentPiece;
            var c = currentPieceColor;
            var t = currentPieceType;
            if (storedPieceNotNull) {
                SpawnStoredPiece();
            }
            else {
                storedPieceNotNull = true;
                SpawnNewPiece();
            }
            storedPiece = p;
            storedPieceColor = c;
            storedPieceType = t;
            alreadyStoredAPiece = true;
        }

        //set current piece to being the stored piece, and spawn the stored piece
        private void SpawnStoredPiece() {
                currentPiece = storedPiece;
                currentPieceColor = storedPieceColor;
                currentPieceType = storedPieceType;
                // Set the initial position of the new piece
                currentPiecePosition = new Vector2((BoardWidth - 4) / 2, 0);
        }
      
        private bool IsMoveValid(Vector2 position, List<Vector2> piece) {
            foreach (var block in piece) {
                int x = (int)block.X + (int)position.X;
                int y = (int)block.Y + (int)position.Y;
                if (x < 1 || x >= BoardWidth || y >= BoardHeight || (y >= 0 && gameBoard[x, y] > 0)) {
                    return false; // Collision with the game board or boundaries
                }
            }
            return true;
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();


            //Draw the background
            _spriteBatch.Draw(
                texture: backgroundImage,
                position: new Vector2(1, 1),
                color: Color.White);

            // Draw the game board
            for (int x = 1; x < BoardWidth; x++) {
                for (int y = 0; y < BoardHeight; y++) {
                    int cellValue = gameBoard[x, y];
                    if (cellValue > 0) {
                        // Use cellValue - 1 to index into pieceColors array
                        _spriteBatch.Draw(
                            texture: blockTexture,
                            position: new Vector2(x * BlockSize, y * BlockSize),
                            color: pieceColors[cellValue - 1]); // Use pieceColors array for landed pieces
                    }
                }
            }


            // Draw the game board border on the left side
            for (int y = 0; y < BoardHeight; y++) {
                _spriteBatch.Draw(
                    texture: blockTexture,
                    position: new Vector2(0, y * BlockSize),
                    color: Color.Gray);
            }

            // Draw the game board border on the right side
                for (int y = 0; y <= BoardHeight; y++) {
                    _spriteBatch.Draw(
                        texture: blockTexture,
                        position: new Vector2(BoardWidth * BlockSize, y * BlockSize),
                        color: Color.Gray);
                }
            // Draw the game board border on the right side
            for (int x = 1; x < BoardWidth; x++) { 
                for (int y = 0; y <= BoardHeight; y++) {
                    _spriteBatch.Draw(
                        texture: blockTexture,
                        position: new Vector2((BoardWidth * BlockSize)+x*BlockSize, y * BlockSize),
                        color: Color.Black);
                }
            }

            // Draw the game board border on the bottom side
            for (int x = 0; x <= BoardWidth; x++) {
                _spriteBatch.Draw(
                    texture: blockTexture,
                    position: new Vector2(x * BlockSize, BoardHeight * BlockSize),
                    color: Color.Gray);
            }
            // Draw the current piece
            foreach (var block in currentPiece) {
                _spriteBatch.Draw(
                    texture: blockTexture, // You can replace null with an actual texture for the blocks
                    position: new Vector2((block.X + currentPiecePosition.X) * BlockSize, (block.Y + currentPiecePosition.Y) * BlockSize),
                    color: currentPieceColor);
            }

            //Draw the text for the next piece
            _spriteBatch.DrawString(
               Content.Load<SpriteFont>("default font"),
               $"NEXT",
               new Vector2(650 + BoardWidth, BoardHeight + 300),
               Color.White);
            //Draw the next piece
            foreach (var block in nextPiece) {
                _spriteBatch.Draw(
                    texture: blockTexture, // You can replace null with an actual texture for the blocks
                    position: new Vector2((block.X + BoardWidth + 5) * BlockSize, (block.Y + BoardHeight - 10) * BlockSize),
                    color: nextPieceColor);
            }

            //Draw the text for the saved/stored piece
            _spriteBatch.DrawString(
               Content.Load<SpriteFont>("default font"),
               $"SAVED",
               new Vector2(650 + BoardWidth, BoardHeight + 530),
               Color.White);
            if (storedPiece != null) {
                //Draw the stored piece
                foreach (var block in storedPiece) {
                    _spriteBatch.Draw(
                        texture: blockTexture, // You can replace null with an actual texture for the blocks
                        position: new Vector2((block.X + BoardWidth + 5) * BlockSize, (block.Y + BoardHeight - 5) * BlockSize),
                        color: storedPieceColor);
                }
            }

            //Draw the player score
            _spriteBatch.DrawString(
              Content.Load<SpriteFont>("default font"),
              $"Score: " + playerScore,
              new Vector2(600 + BoardWidth, BoardHeight + 50),
              Color.White);

            //Draw number of lines cleared
             _spriteBatch.DrawString(
              Content.Load<SpriteFont>("default font"),
              $"Lines cleared: " + linesCleared,
              new Vector2(600 + BoardWidth, BoardHeight + 100),
              Color.White);

         

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        protected override void UnloadContent() {
            // Stop the Tetris music
            MediaPlayer.Stop();
        }
    }
}
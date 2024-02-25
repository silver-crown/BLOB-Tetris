using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

namespace Monogame_Tetris
{
    public class PieceManager {
        private bool alreadyStoredAPiece = false;
        private const int BoardWidth = 10;
        private const int BoardHeight = 20;

        private GameBoardCell[,] gameBoard;
        private List<Vector2> currentPiece;
        private List<Vector2> nextPiece;
        private List<Vector2> storedPiece;
        private Color currentPieceColor;
        private Color nextPieceColor;
        private Color storedPieceColor;
        private Vector2 currentPiecePosition;
        private int currentPieceType;
        private int storedPieceType;
        private bool storedPieceNotNull = false;
        private ContentManager _contentManager;
        private TetrisGame _game;
        private Effect glowEffect; //currently not in use
        private float fallTime = 0f;
        private float FallSpeed = 0.5f;
   
        private float _amount = 0f;
        private float _dir = -1f;
        private bool lineIsGlowing;
        private int loopCount = 0; // the amount of times you want the piece to glow before moving on
        private TetrisBlock tetrisBlock;

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

        public bool GetAlreadyStoredAPiece() => alreadyStoredAPiece;

        public PieceManager(ContentManager c, TetrisGame game) {
            _contentManager = c;  
            _game = game;
        } 

        public void LoadContent() {
            gameBoard = new GameBoardCell[BoardWidth, BoardHeight];
            for (int x = 0; x < BoardWidth; x++) {
                for (int y = 0; y < BoardHeight; y++) {
                    gameBoard[x, y] = new GameBoardCell();
                }
            }
            tetrisBlock = new TetrisBlock(_game.random);
            glowEffect = _contentManager.Load<Effect>("GlowEffect");
        }
        
        public void MovePiece(int xOffset, int yOffset) {
            Vector2 newPosition = currentPiecePosition + new Vector2(xOffset, yOffset);

            if (IsMoveValid(newPosition, currentPiece)) {
                currentPiecePosition = newPosition;
            }
        }

        public void MovePieceDown() {
            Vector2 newPosition = currentPiecePosition + new Vector2(0, 1);

            if (IsMoveValid(newPosition, currentPiece)) {
                currentPiecePosition = newPosition;
            }
            else {
                // Lock the piece in place
                LockPiece();
                alreadyStoredAPiece = false;
                SpawnNewPiece();
            }
        }

        public void RotatePiece() {
            FallSpeed += 0.1f;
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

        public void LockPiece() {
            foreach (var block in currentPiece) {
                int x = (int)block.X + (int)currentPiecePosition.X;
                int y = (int)block.Y + (int)currentPiecePosition.Y;

                // Check if the cell is not already occupied before locking
                if (x >= 1 && x < BoardWidth && y >= 0 && y < BoardHeight && gameBoard[x, y].PieceType == 0) {
                    gameBoard[x, y].PieceType = currentPieceType + 1; // Use currentPieceType + 1 as the value for the piece type
                    //Set ShouldGlow for locked pieces
                    //gameBoard[x, y].ShouldGlow = true;
                    //pieceIsGlowing = true;
                }
            }

        }

        public void SpawnNewPiece() {
            currentPiece = tetrisBlock.Spawn();
            currentPieceColor = tetrisBlock.GetColor();
            currentPieceType = tetrisBlock.GetPieceType();
            ShowNextPiece();
            nextPieceColor = tetrisBlock.GetColor();
            // Set the initial position of the new piece
            currentPiecePosition = new Vector2((BoardWidth - 4) / 2, 0);
        }

        public void ShowNextPiece() {
            nextPiece = tetrisBlock.Init(_game.random);
        }

        public void StorePiece() {
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
        public void SpawnStoredPiece() {
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
                if (x < 1 || x >= BoardWidth || y >= BoardHeight || (y >= 0 && gameBoard[x, y].PieceType > 0)) {
                    return false; // Collision with the game board or boundaries
                }
            }
            return true;
        }

        //apply glow on lines about to be cleared
        public void GlowLines() {
            //Make line glow
            for (int y = BoardHeight - 1; y >= 0; y--) {
                bool lineIsFull = true;

                for (int x = 1; x < BoardWidth; x++) {
                    if (gameBoard[x, y].PieceType == 0) {
                        lineIsFull = false;
                        break;
                    }
                }
                if (lineIsFull) {
                    for (int x = 1; x < BoardWidth; x++) {
                        if (gameBoard[x, y].PieceType > 0) {
                            gameBoard[x, y].ShouldGlow = true;
                        }
                    }
                    lineIsGlowing = true;
                }
            }
        }
        public void ClearLines(SoundManager soundManager) {
            int cleared = 0;
            for (int y = BoardHeight - 1; y >= 0; y--) {
                bool lineIsFull = true;

                for (int x = 1; x < BoardWidth; x++) {
                    if (gameBoard[x, y].PieceType == 0) {
                        lineIsFull = false;
                        break;
                    }
                }
                if (lineIsFull) {
                    // Clear the line
                    for (int newY = y; newY > 0; newY--) {
                        for (int x = 0; x < BoardWidth; x++) {
                            gameBoard[x, newY].PieceType = gameBoard[x, newY - 1].PieceType;
                        }
                    }

                    // Add a new empty line at the top
                    for (int x = 1; x < BoardWidth; x++) {
                        gameBoard[x, 0].PieceType = 0;
                    }

                    // Check the same line again
                    y++;
                    _game.IncreaseLinesCleared();
                    cleared++;
                }
            }
            switch (cleared) {
                default:
                    break;
                case 1:
                    _game.SetPlayerScore(40);
                    soundManager.PlaySoundEffect(soundManager.drOakWonderful);
                    break;
                case 2:
                    _game.SetPlayerScore(100);
                    soundManager.PlaySoundEffect(soundManager.drOakWonderful);
                    break;
                case 3:
                    _game.SetPlayerScore(300);
                    soundManager.PlaySoundEffect(soundManager.drOakWellDone);
                    break;
                case 4:
                    _game.SetPlayerScore(1200);
                    soundManager.PlaySoundEffect(soundManager.drOakPerfect);
                    break;
            }
        }

        public void LineClearGlow(float deltaTime){

            GlowLines();
            if (lineIsGlowing) {
                _amount += deltaTime * _dir;
                if (_amount < 0 || _amount > 0.15) {
                    _dir *= -1;
                    if (loopCount <= 3) {
                        loopCount++;
                    }
                }
                if (loopCount >= 3) {
                    ClearLines(_game.soundManager);
                    for (int y = BoardHeight - 1; y >= 0; y--) {
                        for (int x = 1; x < BoardWidth; x++) {
                            gameBoard[x, y].ShouldGlow = false;
                        }
                    }
                    lineIsGlowing = false;
                    loopCount = 0;
                    _amount = 0;
                    _dir = -1;
                }
            }
            glowEffect.Parameters["amount"].SetValue(_amount);
            

        }
        public void GravityAndInputLogic(float deltaTime) {
            fallTime += deltaTime;
            if (fallTime >= FallSpeed && !lineIsGlowing) {
                MovePieceDown();
                fallTime = 0f;
            }
            if (!lineIsGlowing)
                _game.inputManager.ProcessInput();
        }
        public List<Vector2> GetCurrentPiece() => currentPiece;
        public Vector2 GetCurrentPiecePosition() => currentPiecePosition;
        public Color GetCurrentPieceColor() => currentPieceColor;

        public List<Vector2> GetNextPiece() => nextPiece;
        public Color GetNextPieceColor() => nextPieceColor;

        public List<Vector2> GetStoredPiece() => storedPiece;
        public Color GetStoredPieceColor() => storedPieceColor;
        public GameBoardCell[,] GetGameBoard() => gameBoard;
        public Color[] GetPieceColors() => pieceColors;
        public void SetFallSpeed(float fallSpeed) => FallSpeed = fallSpeed;
    }

}

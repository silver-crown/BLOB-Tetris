using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Tetris
{
    internal class TetrisBlock
    {
        private Texture2D blockTexture;
        private Vector2 blockPosition;
        private List<Microsoft.Xna.Framework.Vector2> blockShape;
        private Microsoft.Xna.Framework.Color[] pieceColors = new Microsoft.Xna.Framework.Color[]
          {
                Microsoft.Xna.Framework.Color.Cyan,    // I-shape
                Microsoft.Xna.Framework.Color.Yellow,  // O-shape
                Microsoft.Xna.Framework.Color.Orange,  // L-shape
                Microsoft.Xna.Framework.Color.Blue,    // J-shape
                Microsoft.Xna.Framework.Color.Green,   // S-shape
                Microsoft.Xna.Framework.Color.Red,     // Z-shape
                Microsoft.Xna.Framework.Color.Purple   // T-shape
          };
        private int pieceType; 
        private Microsoft.Xna.Framework.Color pieceColor;

        // Define different Tetris shapes
        readonly List<List<Microsoft.Xna.Framework.Vector2>> tetrominoes = new List<List<Microsoft.Xna.Framework.Vector2>>
        {
            new List<Microsoft.Xna.Framework.Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0) }, // I-shape
            new List<Microsoft.Xna.Framework.Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) }, // O-shape
            new List<Microsoft.Xna.Framework.Vector2> { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 0) }, // L-shape
            new List<Microsoft.Xna.Framework.Vector2> { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 0) }, // J-shape
            new List<Microsoft.Xna.Framework.Vector2> { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(2, 0) }, // S-shape
            new List<Microsoft.Xna.Framework.Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1) }, // Z-shape
            new List<Microsoft.Xna.Framework.Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 0) }  // T-shape
        };

        //initializes the piece ahead of time so the player can see the next piece in line
        public List<Microsoft.Xna.Framework.Vector2> Init(Random r) {
            // Randomly select a Tetris shape
            int index = r.Next(tetrominoes.Count);
            blockShape = tetrominoes[index];
            pieceColor = pieceColors[index];
            pieceType = index;
            return blockShape;
        }

        public List<Microsoft.Xna.Framework.Vector2> Spawn() {
            return blockShape;
        }
        public Microsoft.Xna.Framework.Color GetColor() {
            return pieceColor;
        }
        public int GetPieceType() {
            return pieceType;
        }

    }
}

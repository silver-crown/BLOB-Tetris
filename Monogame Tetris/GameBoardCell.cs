using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Tetris
{
    public class GameBoardCell
    {
        public Texture2D cellTexture; 
        public int PieceType { get; set; } // 0 for empty, 1 for piece type 1, etc.
        public bool ShouldGlow { get; set;}
    }
}

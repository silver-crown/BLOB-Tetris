using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Tetris
{
    public abstract class State
    {
        public State() { }

        public abstract void Execute(float deltaTime, ContentManager contentManager);
    }
}

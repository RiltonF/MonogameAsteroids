using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// This was an idea to make a settings menu when the screen was paused. 
// Probably implement it in another version of the game.

namespace Alpha_Build
{
    class Settings : Level
    {
        public Settings(Game1 game) : base(game)
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
    }
}

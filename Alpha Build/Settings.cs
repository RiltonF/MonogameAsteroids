using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

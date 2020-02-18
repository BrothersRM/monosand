using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace monosand
{
    public class Config
    {
        public const int TileWidth = 30;
        public const int BoardWidth = 30;
        public const int BoardHeight = 30;
        public const int BrushWidth = 3;
        public const int FluidTileCapacity = TileWidth;
        public const int TicksToEvaporation = 5;
        public readonly Random GlobalRng = new Random();

        public static readonly List<Color> DirtColors = new List<Color>()
        {
            new Color(124, 88, 53),
            new Color(102, 68, 44),
            new Color(76, 43, 33),
            new Color(46, 25, 21)
        };

        public static readonly List<Color> GrassColors = new List<Color>()
        {
            new Color(120,144,48),
            new Color(72,144,48),
            new Color(103,146,103),
            new Color(103,146,125)
        };
        

    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Xml.Schema;
using Microsoft.Xna.Framework.Content;
using Optional;

namespace monosand
{
    
    public abstract class Tile 
    {
        public Color Color { get; set; }

        public Tile(Color color)
        {
            Color = color;
        }
    }
    
    public interface ISolid
    {

    }

    public interface IPowder
    {
    }

    public interface ILiquid
    {
        public int Infill { get; set; }
    }

    public interface IGas
    {
    }

    public class Water : Tile, ILiquid
    {
        public int Infill { get; set; }
        
        public int EvaporationTicks { get; set; }

        public Water(Color color) : base(color)
        {
        }
    }

    public class Sand : Tile, ISolid, IPowder
    {
        public Sand(Color color) : base(color)
        {
        }
    }

    public class Wall : Tile, ISolid
    {
        public Wall(Color color) : base(color)
        {
        }
    }
    
    public class Terrain
    {
        private Config Config { get; set; }

        private Option<Tile>[,] Board;

        private Texture2D Texture;
        
        public Terrain(Config config, Texture2D texture)
        {
            Config = config;
            Texture = texture;
            Board = new Option<Tile>[Config.BoardWidth, Config.BoardHeight];
        }
        
        public Color GetRandomDirt()
        {
            var idx = Config.GlobalRng.Next(100);
            if (idx < 5)
            {
                return Config.DirtColors[3];
            }
            else if (idx < 20)
            {
                return Config.DirtColors[2];
            }
            else if (idx < 60)
            {
                return Config.DirtColors[1];
            }
            else
                return Config.DirtColors[0];
        }

        public void UpdateLiquids()
        {
            for (var y = Config.BoardHeight - 1; y >= 0; y--)
            {
                for (var x = Config.BoardWidth - 1; x >= 0; x--)
                {
                    
                }
            }
        }
        
        private void UpdatePowders()
        {
            for (var y = Config.BoardHeight - 1; y >= 0; y--)
            {
                for (var x = Config.BoardWidth - 1; x >= 0; x--)
                {
                    var ldx = x;
                    var ldy = y;
                    var current = Board[x, y];
                    current.MatchSome(item =>
                    {
                        if (!(item is IPowder)) return;
                        if (ldy + 1 >= Config.BoardHeight || ldx >= Config.BoardWidth - 1 || ldx <= 0) return;
                        if (!Board[ldx, ldy + 1].HasValue) //Down
                        {
                            var tmp = Board[ldx, ldy + 1];
                            Board[ldx, ldy + 1] = current;
                            Board[ldx, ldy] = tmp;
                        }
                        else if(!Board[ldx + 1, ldy + 1].HasValue && !Board[ldx - 1, ldy + 1].HasValue) //Down Right and Down Left
                        {
                            var chance = Config.GlobalRng.Next(100);
                            if (chance > 50)
                            {
                                var tmp = Board[ldx + 1, ldy + 1];
                                Board[ldx + 1, ldy + 1] = current;
                                Board[ldx, ldy] = tmp;
                            }
                            else
                            {
                                var tmp = Board[ldx - 1, ldy + 1] = current;
                                Board[ldx - 1, ldy + 1] = current;
                                Board[ldx, ldy] = tmp;
                            }
                        }
                        else if (!Board[ldx + 1, ldy + 1].HasValue) //Down Right 
                        {
                            var tmp = Board[ldx + 1, ldy + 1];
                            Board[ldx + 1, ldy + 1] = current;
                            Board[ldx, ldy] = tmp;
                        }
                        else if (!Board[ldx -1, ldy + 1].HasValue) //Down Left
                        {
                            var tmp = Board[ldx - 1, ldy + 1];
                            Board[ldx - 1, ldy + 1] = current;
                            Board[ldx, ldy] = tmp;
                        }
                    });
                }
            }
        }

        public void Update()
        {
            UpdatePowders();
            
            var state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed)
            {
                var position = GridPosition.FromScreenPosition(state.Position);
                for (var y = position.Y - Config.BrushWidth; y < position.Y + Config.BrushWidth; y++)
                {
                    for(var x = position.X - Config.BrushWidth; x < position.X + Config.BrushWidth; x++)
                    {
                        var r = Config.GlobalRng.Next(100);
                        if (r <= 96|| x >= Config.BoardWidth || x < 0 || y >= Config.BoardHeight || y < 0) continue;
                        if(Board[x, y] == Option.None<Tile>())
                            Board[x, y] = Option.Some<Tile>(new Sand(GetRandomDirt()));
                    }
                }
            }

            if (state.MiddleButton == ButtonState.Pressed)
            {
                var position = GridPosition.FromScreenPosition(state.Position);
                for (var y = position.Y - Config.BrushWidth; y < position.Y + Config.BrushWidth; y++)
                {
                    for(var x = position.X - Config.BrushWidth; x < position.X + Config.BrushWidth; x++)
                    {
                        var r = Config.GlobalRng.Next(100);
                        if (r <= 96 || x >= Config.BoardWidth || x < 0 || y >= Config.BoardHeight || y < 0) continue;
                        if(Board[x, y] == Option.None<Tile>())
                            Board[x, y] = Option.Some<Tile>(new Water(Color.Blue){Infill=Config.TileWidth});
                    }
                }
            }

            if (state.RightButton == ButtonState.Pressed)
            {
                var position = GridPosition.FromScreenPosition(state.Position);
                Board[position.X, position.Y] = Option.Some<Tile>(new Wall(Color.Black));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Config.BoardHeight; y++)
            {
                for (int x = 0; x < Config.BoardWidth; x++)
                {
                    Board[x, y].MatchSome(
                        item =>
                        {
                            switch (item)
                            {
                                case ISolid t: 
                                    spriteBatch.Draw(Texture, new Vector2(x * Config.TileWidth, y * Config.TileWidth), item.Color);
                                    break;
                                case ILiquid f:
                                    //spriteBatch.Draw(rect, new Vector2(x * Config.TileWidth, y * Config.TileWidth), item.Color);
                                    spriteBatch.Draw(Texture, new Rectangle(x*Config.TileWidth, y*Config.TileWidth + (Config.TileWidth - f.Infill), Config.TileWidth, f.Infill), item.Color);
                                    break;                                  
                            }
                        });
                }
            }
        }
            
    }
}
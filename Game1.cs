using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Optional;

namespace monosand
{
    public static class Config
    {
        public static int TileWidth = 4;
        public static int BoardWidth = 300;
        public static int BoardHeight = 200;
        public static int ScreenWidth = TileWidth * BoardWidth;
        public static int ScreenHeight = TileWidth * BoardHeight;

        public static List<Color> DirtColors = new List<Color>()
        {
            new Color(124, 88, 53),
            new Color(102, 68, 44),
            new Color(76, 43, 33),
            new Color(46, 25, 21)
        };

        public static List<Color> GrassColors = new List<Color>()
        {
            new Color(120,144,48),
            new Color(72,144,48),
            new Color(103,146,103),
            new Color(103,146,125)
        };
        
        public static Color GetRandomDirt(Random rng)
        {
            var idx = rng.Next(100);
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
    }
    

    
    public class Sand
    {
        public Color Color;

        public bool UpdatedLastTurn = false;

        public Sand(Random rng)
        {
            Color = Config.GetRandomDirt(rng);
        }
    }

    public class GridPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static GridPosition FromScreenPosition(Point pos)
        {
            var newx = (int)(pos.X / Config.TileWidth);
            var newy = (int)(pos.Y / Config.TileWidth);
            return new GridPosition{X = newx, Y = newy};
        }
    }
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D rect; 

        Color[] data = new Color[Config.TileWidth*Config.TileWidth];

        //Option<Sand>[,] board = new Option<Sand>[Config.BoardWidth, Config.BoardHeight];
        
        Dictionary<GridPosition, Sand> board = new Dictionary<GridPosition, Sand>();
        
        private System.Random rng = new System.Random();

        public void MoveBoardDown()
        {
            for (int y = Config.BoardHeight - 1; y >= 0 ; y--)
            {
                for (int x = Config.BoardWidth - 1; x >=0; x--)
                {
                    var ldx = x;
                    var ldy = y;
                    var current = board[x, y];
                    current.MatchSome(item =>
                    {
                        item.UpdatedLastTurn = true;
                        if (ldy + 1 < Config.BoardHeight && ldx < Config.BoardWidth - 1 && ldx > 0) 
                        {
                            if (!board[ldx, ldy + 1].HasValue)
                            {
                                board[ldx, ldy + 1] = current;
                                board[ldx, ldy] = Option.None<Sand>();
                            }
                            else if(!board[ldx + 1, ldy + 1].HasValue && !board[ldx - 1, ldy + 1].HasValue)
                            {
                                var chance = rng.Next(100);
                                if (chance > 50)
                                {
                                    board[ldx + 1, ldy + 1] = current;
                                    board[ldx, ldy] = Option.None<Sand>();
                                }
                                else
                                {
                                    board[ldx - 1, ldy + 1] = current;
                                    board[ldx, ldy] = Option.None<Sand>();
                                }
                            }
                            else if (!board[ldx + 1, ldy + 1].HasValue)
                            {
                                board[ldx + 1, ldy + 1] = current;
                                board[ldx, ldy] = Option.None<Sand>();
                            }
                            else if (!board[ldx -1, ldy + 1].HasValue)
                            {
                                board[ldx - 1, ldy + 1] = current;
                                board[ldx, ldy] = Option.None<Sand>();
                            }
                            else
                            {
                                item.UpdatedLastTurn = false;
                            }
                        }
                        else
                        {
                            item.UpdatedLastTurn = false;
                        }
                    });
                }
            }
        }
        
        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Config.BoardWidth * Config.TileWidth;
            graphics.PreferredBackBufferHeight = Config.BoardHeight * Config.TileWidth;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            rect = new Texture2D(graphics.GraphicsDevice, Config.TileWidth, Config.TileWidth);
            for(int i=0; i < data.Length; ++i) data[i] = Color.White;
                rect.SetData(data);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        
            // for (int y = 0; y < Config.BoardHeight; y++)
            // {
            //     for (int x = 0; x < Config.BoardWidth; x++)
            //     {
            //         var r = rng.Next(255);
            //         var g = rng.Next(255);
            //         var b = rng.Next(255);
            //         board[x, y] = Option.Some(new Sand{Color = new Color(r, g, b) });
            //     }
            // }
            //
            // TODO: use this.Content to load your game content here
        }

        private long elapsedticks = 0;
        
        protected override void Update(GameTime gameTime)
        {
  
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            // for (int y = 0; y < Config.BoardHeight; y++)
            // {
            //     for (int x = 0; x < Config.BoardWidth; x++)
            //     {
            //         var r = rng.Next(255);
            //         var g = rng.Next(255);
            //         var b = rng.Next(255);
            //         board[x, y] = Option.Some(new Sand{Color = new Color(r, g, b) });
            //     }
            // }
            if (gameTime.TotalGameTime.Ticks - elapsedticks >= 10000000)
            {
                MoveBoardDown();
                elapsedticks = gameTime.ElapsedGameTime.Ticks;
            }
            

            var state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed)
            {
                var position = GridPosition.FromScreenPosition(state.Position);
                for (int y = position.Y - 0; y < position.Y + 1; y++)
                {
                    for(int x = position.X - 0; x < position.X + 1; x++)
                    {
                        board[x, y] = Option.Some<Sand>(new Sand(rng));
                    }
                }
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            
            spriteBatch.Draw(rect, new Vector2(0, 0), Color.Black);
            
            for (int y = 0; y < Config.BoardHeight; y++)
            {
                for (int x = 0; x < Config.BoardWidth; x++)
                {
                    board[x, y].MatchSome(
                        item => spriteBatch.Draw(rect, new Vector2(x*Config.TileWidth, y*Config.TileWidth), item.Color)
                    );
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Xml.Schema;
using Optional;

namespace monosand
{
    public class GridPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static GridPosition FromScreenPosition(Point pos)
        {
            var newx = (pos.X / Config.TileWidth);
            var newy = (pos.Y / Config.TileWidth);
            return new GridPosition{X = newx, Y = newy};
        }
    }
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        
        SpriteBatch spriteBatch;

        private Texture2D rect; 

        Color[] data = new Color[Config.TileWidth*Config.TileWidth];

        private System.Random rng = new System.Random();

        private Terrain t;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Config.BoardWidth * Config.TileWidth,
                PreferredBackBufferHeight = Config.BoardHeight * Config.TileWidth
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            rect = new Texture2D(graphics.GraphicsDevice, Config.TileWidth, Config.TileWidth);
            for(int i=0; i < data.Length; ++i) data[i] = Color.White;
                rect.SetData(data);

            t = new Terrain(new Config(), rect);
                
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        
        protected override void Update(GameTime gameTime)
        {
  
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            t.Update();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();

            t.Draw(spriteBatch);
            
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

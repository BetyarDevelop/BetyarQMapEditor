using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TileEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont pericles6;

        static int CAMERA_SPEED = 7;

        int selectedTileID = 0;
        private KeyboardState keyboardState;
        private MouseState mouseState;
        private MouseState oldMouseState;

        private SelectorPanel selectPanel;

        TileMap myMap;
        int squaresAcross = 17;        
        int squaresDown = 37;
        int baseOffsetX = -32;
        int baseOffsetY = -64;
        float heightRowDepthMod = 0.00001f;

        Texture2D hilight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            Content.RootDirectory = "Content";
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            squaresAcross = (GraphicsDevice.Viewport.Width / 64) + 2;
            squaresDown = (GraphicsDevice.Viewport.Height / 16) + 4;
            // Make changes to handle the new window size.
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            myMap = new TileMap(Content.Load<Texture2D>(@"Textures\TileSets\mousemap"));
            hilight = Content.Load<Texture2D>(@"Textures\TileSets\hilight");

            Tile.TileSetTexture = Content.Load<Texture2D>(@"Textures\TileSets\part4_tileset");

            pericles6 = Content.Load<SpriteFont>(@"Fonts\Pericles6");
            //pericless6 = Content

            Camera.ViewWidth = this.graphics.PreferredBackBufferWidth;
            Camera.ViewHeight = this.graphics.PreferredBackBufferHeight;
            Camera.WorldWidth = ((myMap.MapWidth-2) * Tile.TileStepX);
            Camera.WorldHeight = ((myMap.MapHeight-2) * Tile.TileStepY);
            Camera.DisplayOffset = new Vector2(baseOffsetX, baseOffsetY);

            selectPanel.addTexture(Tile.TileSetTexture);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if(keyboardState.IsKeyDown(Keys.Q))
            {
                selectedTileID = 0;
            }
            else if(keyboardState.IsKeyDown(Keys.W))
            {
                selectedTileID = 1;
            }
            else if(keyboardState.IsKeyDown(Keys.E))
            {
                selectedTileID = 2;
            }

            mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released && !selectPanel.Rect.Contains(new Point( mouseState.X, mouseState.Y)))
            {
                Vector2 MouseWorldPos = Camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));
                Point MouseCell = myMap.WorldToMapCell(new Point((int)MouseWorldPos.X, (int)MouseWorldPos.Y));
                myMap.Rows[MouseCell.Y].Columns[MouseCell.X].TileID = selectedTileID;
            }

            oldMouseState = mouseState;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Left))
            {
                Camera.Move(new Vector2(-CAMERA_SPEED, 0));
            }

            if (ks.IsKeyDown(Keys.Right))
            {
                Camera.Move(new Vector2(CAMERA_SPEED, 0));
            }

            if (ks.IsKeyDown(Keys.Up))
            {
                Camera.Move(new Vector2(0, -CAMERA_SPEED));
            }

            if (ks.IsKeyDown(Keys.Down))
            {
                Camera.Move(new Vector2(0, CAMERA_SPEED));
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            Vector2 firstSquare = new Vector2(Camera.Location.X / Tile.TileStepX, Camera.Location.Y / Tile.TileStepY);
            int firstX = (int)firstSquare.X;
            int firstY = (int)firstSquare.Y;

            Vector2 squareOffset = new Vector2(Camera.Location.X % Tile.TileStepX, Camera.Location.Y % Tile.TileStepY);
            int offsetX = (int)squareOffset.X;
            int offsetY = (int)squareOffset.Y;

            float maxdepth = ((myMap.MapWidth + 1) * ((myMap.MapHeight + 1) * Tile.TileWidth)) / 10;
            float depthOffset;

            for (int y = 0; y < squaresDown; y++)
            {
                int rowOffset = 0;
                if ((firstY + y) % 2 == 1)
                    rowOffset = Tile.OddRowXOffset;

                for (int x = 0; x < squaresAcross; x++)
                {
                    int mapx = (firstX + x);
                    int mapy = (firstY + y);
                    depthOffset = 0.7f - ((mapx + (mapy * Tile.TileWidth)) / maxdepth);

                    if ((mapx >= myMap.MapWidth) || (mapy >= myMap.MapHeight))
                        continue;

                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].BaseTiles)
                    {
                        spriteBatch.Draw(
                            Tile.TileSetTexture,
                            Camera.WorldToScreen(
                                new Vector2((mapx * Tile.TileStepX) + rowOffset, mapy * Tile.TileStepY)),
                            Tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            1.0f);
                    }
                    int heightRow = 0;

                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].HeightTiles)
                    {
                        spriteBatch.Draw(
                            Tile.TileSetTexture,
                            Camera.WorldToScreen(
                                new Vector2(
                                    (mapx * Tile.TileStepX) + rowOffset,
                                    mapy * Tile.TileStepY - (heightRow * Tile.HeightTileOffset))),
                            Tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod));
                        heightRow++;
                    }

                    foreach (int tileID in myMap.Rows[y + firstY].Columns[x + firstX].TopperTiles)
                    {
                        spriteBatch.Draw(
                            Tile.TileSetTexture,
                            Camera.WorldToScreen(
                                new Vector2((mapx * Tile.TileStepX) + rowOffset, mapy * Tile.TileStepY)),
                            Tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod));
                    }


                    //spriteBatch.DrawString(pericles6, (x + firstX).ToString() + ", " + (y + firstY).ToString(),
                    //    new Vector2((x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX + 24,
                    //        (y * Tile.TileStepY) - offsetY + baseOffsetY + 48), Color.White, 0f, Vector2.Zero,
                    //        1.0f, SpriteEffects.None, 0.0f);
                }
            }

            Point mousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);

            if (!selectPanel.Rect.Contains(mousePoint))
            {
                Vector2 hilightLoc = Camera.ScreenToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                Point hilightPoint = myMap.WorldToMapCell(new Point((int)hilightLoc.X, (int)hilightLoc.Y));

                //spriteBatch.DrawString(pericles6, hilightPoint.X.ToString() + " " + hilightPoint.Y.ToString(), new Vector2(10, 10), Color.White);

                int hilightrowOffset = 0;
                if ((hilightPoint.Y) % 2 == 1)
                    hilightrowOffset = Tile.OddRowXOffset;

                spriteBatch.Draw(
                                hilight,
                                Camera.WorldToScreen(
                                new Vector2(
                                    (hilightPoint.X * Tile.TileStepX) + hilightrowOffset,
                                    (hilightPoint.Y + 2) * Tile.TileStepY)),
                                new Rectangle(0, 0, 64, 32),
                                Color.White * 0.3f,
                                0.0f,
                                Vector2.Zero,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
            }

            selectPanel.Draw(spriteBatch);

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

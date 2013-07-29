using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TileEngine
{
    class SelectorPanel
    {
        Rectangle rect = new Rectangle(20, 20, 600, 200);
        Color panelColor = new Color(0, 0, 0);
        Texture2D tileMap;
        Texture2D blackTexture;
        int selected = 0;

        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public SelectorPanel(Texture2D blackTexture)
        {
            this.blackTexture = blackTexture;
        }

        public void addTexture(Texture2D texture)
        {
            tileMap = texture;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(blackTexture, rect, panelColor);

            for (int i = 0; i < 3; i++)
            {
                spritebatch.Draw(
                tileMap,
                new Rectangle(20+i*64,20,64,64),
                Tile.GetSourceRectangle(i),
                Color.White);
            }
        }

        public void update()
        {

        }
    }
}

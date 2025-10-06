using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Classes
{
    internal class Platform
    {
        public Rectangle Rect { get; set; }

        public Platform(Rectangle rect)
        {
            Rect = rect;
        }

        public void MovePlatform(int X)
        {
            Rect = new Rectangle(X, Rect.Y, Rect.Width, Rect.Height);
        }

    }
}

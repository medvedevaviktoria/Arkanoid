using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Classes
{
    internal class Ball 
    {
        public Rectangle Rect { get; set; }

        public int SpeedX { get; set; }

        public int SpeedY { get; set; }

        public Ball(Rectangle rect)
        {
            Rect = rect;
        }

        public void SetBallPos(int x, int y)
        {
            Rect = new Rectangle(x, y, Rect.Width, Rect.Height);
        }
    }
}

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
        public Point Position { get; set; }

        public Ball(Rectangle rect, Point position)
        {
            Rect = rect;
            Position = position;
        }
    }
}

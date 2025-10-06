using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Classes
{
    internal class Block
    {
        public Rectangle Rect { get; set; }
        public int Health { get; set; }
        public bool IsDestroyed { get; set; }

        public Block(Rectangle rect, int health = 1)
        {
            Rect = rect;
            Health = health;
        }

        public void HitBlock()
        {
            Health -= 1;
            if (Health <= 0) IsDestroyed = true;
        }
    }
}

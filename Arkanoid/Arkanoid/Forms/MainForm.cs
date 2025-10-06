using Arkanoid.Classes;
using System.Net.Http.Headers;

namespace Arkanoid
{
    public partial class MainForm : Form
    {
        private bool gameStarted = false;
        private Ball ball;
        private Platform platform;
        private int MinX;
        private int MaxX;
        private int MaxY;
        private int MinY;
        private List<Block> Blocks;
        private const int Rows = 10;
        private const int Cols = 6;
        private readonly Random random = new();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(Brushes.White, ball.Rect);
            e.Graphics.FillRectangle(Brushes.Orange, platform.Rect);

            foreach (var block in Blocks)
            {
                if (!block.IsDestroyed)
                {
                    e.Graphics.FillRectangle(Brushes.Blue, block.Rect);
                    e.Graphics.DrawRectangle(Pens.Black, block.Rect);
                }
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            MinX = 0;
            MaxX = ClientSize.Width;
            MinY = 0;
            MaxY = ClientSize.Height;
            var ballWidth = 20;
            var ballHeight = 20;
            var platformWidth = 110;
            var platformHeight = 20;
            var centerXBall = (MaxX - ballWidth) / 2;
            var startYBall = MaxY - 175;

            
            ball = new Ball(new Rectangle(centerXBall, startYBall, ballWidth, ballHeight));
            var direction = random.Next(0, 2) == 0 ? -1 : 1;
            ball.SpeedX = random.Next(1, 5) * direction;
            ball.SpeedY = random.Next(-10, -5);
            var centerXPlatform = (MaxX - platformWidth) / 2;
            var startYPlatform = startYBall + ballHeight;
            platform = new Platform(new Rectangle(centerXPlatform, startYPlatform, platformWidth, platformHeight));

            var blockWidth = MaxX / Cols;
            var blockHeight = (MaxY / Rows * 2) / Cols;
            Blocks = new List<Block>();
            for (var row = 0; row < Rows; row++)
                for (var col = 0; col < Cols; col++)
                {
                    var x = col * blockWidth;
                    var y = 50 + row * blockHeight;
                    Blocks.Add(new Block(new Rectangle(x + 1, y + 1, blockWidth - 2, blockHeight - 2)));
                }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            var newXPlatform = e.X - platform.Rect.Width / 2;
            if (newXPlatform < MinX) newXPlatform = MinX;
            if (newXPlatform > MaxX - platform.Rect.Width) newXPlatform = MaxX - platform.Rect.Width;
            platform.MovePlatform(newXPlatform);

            if (!gameStarted)
            {
                var ballX = platform.Rect.X + (platform.Rect.Width - ball.Rect.Width) / 2;
                if (ballX < MinX) ballX = MinX;
                if (ballX > MaxX - ball.Rect.Width) ballX = MaxX - ball.Rect.Width;
                ball.MoveBall(ballX, ball.Rect.Y);
                Invalidate();
            }
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameStarted == false) gameStarted = true;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ball.MoveBall(ball.Rect.X + ball.SpeedX, ball.Rect.Y + ball.SpeedY);

            //границы для шара по X
            if (ball.Rect.Left <= MinX || ball.Rect.Right >= MaxX)
                ball.SpeedX = -ball.SpeedX;
            //границы для шара по Y
            if (ball.Rect.Top <= MinY)
                ball.SpeedY = -ball.SpeedY;
            //если шар коснулся платформы
            if (ball.Rect.IntersectsWith(platform.Rect) && ball.SpeedY > 0)
            {
                ball.SpeedY = -ball.SpeedY;
                var hitPosition = ball.Rect.Width / 2 + ball.Rect.X - platform.Rect.X;//расстояние от левого края платформы до шарика
                var thirdWidth = platform.Rect.Width / 3;
                if (hitPosition < thirdWidth) 
                {
                    ball.SpeedX = random.Next(1, 5) * -1; // летит влево
                }
                else if (hitPosition < 2 * thirdWidth)
                {
                    ball.SpeedX = 0; // летит прямо вверх
                }
                else
                {
                    ball.SpeedX = random.Next(1, 5); // летит вправо
                }
                ball.SpeedY = random.Next(-10, -5);
            }

            foreach (var block in Blocks)
            {
                if (!block.IsDestroyed && ball.Rect.IntersectsWith(block.Rect))
                {
                    block.HitBlock();
                    ball.SpeedY = -ball.SpeedY;
                    break;
                }
            }
            Invalidate();
            //проверка на победу
            if (IsGameWon())
            {
                timer.Stop();
                MessageBox.Show("Вы выиграли", "Победа", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            //проверка на проигрыш
            if (ball.Rect.Top > MaxY)
            {
                timer.Stop();
                MessageBox.Show("Вы проиграли", "Проигрыш", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }

        private bool IsGameWon()
        {
            foreach (var block in Blocks)
            {
                if (!block.IsDestroyed) return false;
            }
            return true;
        }
    }
}

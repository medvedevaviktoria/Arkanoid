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
        private Block[,] Blocks;
        private const int Rows = 10;
        private const int Cols = 6;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(Brushes.White, ball.Rect);
            e.Graphics.FillRectangle(Brushes.Orange, platform.Rect);

            for (var row = 0; row < Rows; row++)
                for (var col = 0; col < Cols; col++)
                {
                    var block = Blocks[row, col];
                    if (block.IsDestroyed == false)
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

            Random random = new Random();
            ball = new Ball(new Rectangle(centerXBall, startYBall, ballWidth, ballHeight));
            ball.SpeedX = random.Next(-5, 5);
            ball.SpeedY = random.Next(-5, 0);
            var centerXPlatform = (MaxX - platformWidth) / 2;
            var startYPlatform = startYBall + ballHeight;
            platform = new Platform(new Rectangle(centerXPlatform, startYPlatform, platformWidth, platformHeight));

            var blockWidth = MaxX / Cols;
            var blockHeight = (MaxY / Rows * 2) / Cols;
            Blocks = new Block[Rows, Cols];
            for (var row = 0; row < Rows; row++)
                for (var col = 0; col < Cols; col++)
                {
                    var x = col * blockWidth;
                    var y = 50 + row * blockHeight;
                    Blocks[row, col] = new Block(new Rectangle(x + 1, y + 1, blockWidth - 2, blockHeight - 2));
                }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            var newXPlatform = e.X - platform.Rect.Width / 2;
            newXPlatform = Math.Max(MinX, Math.Min(MaxX - platform.Rect.Width, newXPlatform));
            platform.MovePlatform(newXPlatform);
            if (gameStarted == false)
            {
                var ballX = platform.Rect.X + (platform.Rect.Width - ball.Rect.Width) / 2;
                ball.MoveBall(ballX, ball.Rect.Y);
            }
            Invalidate();
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
                ball.SpeedY = -ball.SpeedY;

            for (var row = 0; row < Rows; row++)
                for (var col = 0; col < Cols; col++)
                {
                    var block = Blocks[row, col];
                    if (block != null && !block.IsDestroyed && ball.Rect.IntersectsWith(block.Rect))
                    {
                        block.HitBlock();
                        ball.SpeedY = -ball.SpeedY;
                        if (block.IsDestroyed) block = null;
                        break;
                    }
                }

            if (IsGameWon())
            {
                timer.Stop();
                MessageBox.Show("Вы выиграли", "Победа", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }

            if (ball.Rect.Top > MaxY)
            {
                timer.Stop();
                MessageBox.Show("Вы проиграли", "Проигрыш", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }

            Invalidate();
        }

        private bool IsGameWon()
        {
            for (var row = 0; row < Rows; row++)
                for (var col = 0; col < Cols; col++)
                {
                    var block = Blocks[row, col];
                    if (block != null) return false;
                }
            return true;
        }
    }
}

using Arkanoid.Classes;
using System.Net.Http.Headers;

namespace Arkanoid
{
    public partial class MainForm : Form
    {
        private bool gameStarted = false;
        private Ball ball;
        private Platform platform;
        private int platformMinX;
        private int platformMaxX;
        private Block[,] Blocks;
        private int rows = 10;
        private int cols = 6;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(Brushes.White, ball.Rect);
            e.Graphics.FillRectangle(Brushes.Orange, platform.Rect);

            for (var row = 0; row < rows; row++)
                for (var col = 0; col < cols; col++)
                {
                    if (Blocks[row, col].IsDestroyed == false)
                    {
                        e.Graphics.FillRectangle(Brushes.Blue, Blocks[row, col].Rect);
                        e.Graphics.DrawRectangle(Pens.Black, Blocks[row, col].Rect);
                    }
                }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            platformMinX = 0;
            platformMaxX = ClientSize.Width;
            var ballWidth = 20;
            var ballHeight = 20;
            var platformWidth = 110;
            var platformHeight = 20;
            var centerXBall = (platformMaxX - ballWidth) / 2;
            var startYBall = ClientSize.Height - 175;

            ball = new Ball(new Rectangle(centerXBall, startYBall, ballWidth, ballHeight));
            var centerXPlatform = (platformMaxX - platformWidth) / 2;
            var YPlatform = startYBall + ballHeight;
            platform = new Platform(new Rectangle(centerXPlatform, YPlatform, platformWidth, platformHeight));

            var blockWidth = platformMaxX / cols;
            var blockHeight = (ClientSize.Height / rows * 2) / cols;
            Blocks = new Block[rows, cols];
            for (var row = 0; row < rows; row++)
                for (var col = 0; col < cols; col++)
                {
                    var x = col * blockWidth;
                    var y = 50 + row * blockHeight;
                    Blocks[row, col] = new Block(new Rectangle(x + 1, y + 1, blockWidth - 2, blockHeight - 2));
                }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            var newXPlatform = e.X - platform.Rect.Width / 2;
            newXPlatform = Math.Max(platformMinX, Math.Min(platformMaxX - platform.Rect.Width, newXPlatform));
            platform.MovePlatform(newXPlatform);
            if (gameStarted == false)
            {
                int ballX = platform.Rect.X + (platform.Rect.Width - ball.Rect.Width) / 2;
                ball.MoveBall(new Point(ballX, ball.Rect.Y));
            }
            Invalidate();
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameStarted == false ) gameStarted = true;
        }
    }
}

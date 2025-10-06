using Arkanoid.Classes;
using System.Net.Http.Headers;

namespace Arkanoid
{
    public partial class MainForm : Form
    {
        private Ball ball;
        private Platform platform;
        Image platformImage = Properties.Resources.platformImg;
        Image ballImage = Properties.Resources.ballImg;
        private Block[,] Blocks;
        private int rows = 10;
        private int cols = 6;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(ballImage, ball.Rect);
            e.Graphics.DrawImage(platformImage, platform.Rect);

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
            var ballWidth = 20;
            var ballHeight = 20;
            var platformWidth = 110;
            var platformHeight = 20;
            var centerXBall = (ClientSize.Width - ballWidth) / 2;
            var YBall = ClientSize.Height - 175; //интересное название переменной

            ball = new Ball(new Rectangle(centerXBall, YBall, ballWidth, ballHeight));
            var centerXPlatform = (ClientSize.Width - platformWidth) / 2;
            var YPlatform = YBall + ballHeight;
            platform = new Platform(new Rectangle(centerXPlatform, YPlatform, platformWidth, platformHeight));

            var blockWidth = ClientSize.Width / cols;
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

    }
}

using Arkanoid.Classes;
using System.Net.Http.Headers;

namespace Arkanoid
{
    public partial class MainForm : Form
    {
        private bool gameStarted = false;
        private Ball ball = default!;
        private Platform platform = default!;
        private int MinX, MaxX, MaxY, MinY;
        private List<Block> Blocks = [];
        private const int Rows = 10;
        private const int Cols = 6;
        private readonly Random random = new();

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события перерисовки формы.
        /// </summary>
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Отрисовка шара и платформы.
            e.Graphics.FillEllipse(Brushes.White, ball.Rect);
            e.Graphics.FillRectangle(Brushes.Orange, platform.Rect);
            // Отрисовка блоков.
            foreach (var block in Blocks)
            {
                if (!block.IsDestroyed)
                {
                    e.Graphics.FillRectangle(Brushes.Blue, block.Rect);
                    e.Graphics.DrawRectangle(Pens.Black, block.Rect);
                }
            }
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // -------- ПАРАМЕТРЫ ОКНА --------
            MinX = 0; MaxX = ClientSize.Width; // Ширина окна.
            MinY = 0; MaxY = ClientSize.Height; // Высота окна.

            // -------- ШАР --------
            var ballWidth = 20; var ballHeight = 20; 

            var startXBall = (MaxX - ballWidth) / 2;
            var startYBall = MaxY - 175;
            ball = new Ball(new Rectangle(startXBall, startYBall, ballWidth, ballHeight));

            var direction = random.Next(0, 2) == 0 ? -1 : 1; // Случайный выбор направления шара.
            ball.SpeedX = random.Next(1, 5) * direction;
            ball.SpeedY = random.Next(-10, -5);

            // -------- ПЛАТФОРМА --------
            var platformWidth = 110; var platformHeight = 20;
            
            var startXPlatform = (MaxX - platformWidth) / 2;
            var startYPlatform = startYBall + ballHeight;
            platform = new Platform(new Rectangle(startXPlatform, startYPlatform, platformWidth, platformHeight));

            // -------- БЛОКИ --------
            // Параметры блоков.
            var blockWidth = MaxX / Cols;
            var blockHeight = (MaxY / Rows * 2) / Cols;
            Blocks = [];
            for (var row = 0; row < Rows; row++)
                for (var col = 0; col < Cols; col++)
                {
                    var x = col * blockWidth;
                    var y = 50 + row * blockHeight;
                    Blocks.Add(new Block(new Rectangle(x + 1, y + 1, blockWidth - 2, blockHeight - 2)));
                }
        }

        /// <summary>
        /// Обработчик события движения мышки.
        /// </summary>
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            var newXPlatform = e.X - platform.Rect.Width / 2;
            // Ограничение пределов экрана для платформы.
            if (newXPlatform < MinX) newXPlatform = MinX;
            if (newXPlatform > MaxX - platform.Rect.Width) newXPlatform = MaxX - platform.Rect.Width;
            platform.MovePlatform(newXPlatform);

            if (!gameStarted)
            {
                var ballX = platform.Rect.X + (platform.Rect.Width - ball.Rect.Width) / 2;
                // Ограничение пределов экрана для шара.
                if (ballX < MinX) ballX = MinX;
                if (ballX > MaxX - ball.Rect.Width) ballX = MaxX - ball.Rect.Width;
                ball.MoveBall(ballX, ball.Rect.Y);
                Invalidate();
            }
        }

        /// <summary>
        /// Обработчик события щелчка мышки.
        /// </summary>
        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameStarted == false) gameStarted = true;
            timer.Start();
        }

        /// <summary>
        /// Обработчик события таймера.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            ball.MoveBall(ball.Rect.X + ball.SpeedX, ball.Rect.Y + ball.SpeedY);

            // Границы для шара по X.
            if (ball.Rect.Left <= MinX || ball.Rect.Right >= MaxX)
                ball.SpeedX = -ball.SpeedX;
            // Границы для шара по Y.
            if (ball.Rect.Top <= MinY)
                ball.SpeedY = -ball.SpeedY;
            // Если шар коснулся платформы.
            if (ball.Rect.IntersectsWith(platform.Rect) && ball.SpeedY > 0)
            {
                ball.SpeedY = -ball.SpeedY;
                var hitPosition = ball.Rect.Width / 2 + ball.Rect.X - platform.Rect.X;// Расстояние от левого края платформы до шара
                var thirdWidth = platform.Rect.Width / 3;
                if (hitPosition < thirdWidth) // Шар попал в левую часть платформы.
                {
                    ball.SpeedX = random.Next(1, 5) * -1; 
                }
                else if (hitPosition < 2 * thirdWidth) // Шар попал в среднюю часть платформы.
                {
                    ball.SpeedX = 0;
                }
                else // Шар попал в правую часть платформы.
                {
                    ball.SpeedX = random.Next(1, 5);
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

            //Проверка на победу.
            if (IsGameWon())
            {
                timer.Stop();
                MessageBox.Show("Поздравляю! Вы выиграли :)", "Победа", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            // Проверка на проигрыш.
            if (ball.Rect.Top > MaxY)
            {
                timer.Stop();
                MessageBox.Show("К сожалению, Вы проиграли :(", "Проигрыш", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }

        /// <summary>
        /// Метод определения победы.
        /// </summary>
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

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    namespace Caro_Lợi
    {
        public class Cls_TienLoi
        {
            private bool isBlinking = false;
            private Timer blinkTimer;
            private Panel chessBoard;
            private List<Player> player;
            private int currentPlayer;
            private TextBox playerName;
            private PictureBox playerMark;
            private List<List<Button>> matrix;
            private List<Button> winningButtons; // Danh sách các ô đã chiến thắng
            private int gameTimeInSeconds;
            public TextBox textBox5; // Tham chiếu đến TextBox5 trong lớp khác
            public TextBox textBox2; // Tham chiếu đến TextBox2 trong lớp khác
            private frmLoi frmLoi;

            public int GameTimeInSeconds
            {
                get { return gameTimeInSeconds; }
                set { gameTimeInSeconds = value; }
            }
        

            public Cls_TienLoi(Panel chessBoard, TextBox playerName, PictureBox mark, frmLoi frmLoi)
            {
                this.ChessBoard = chessBoard;
                this.PlayerName = playerName;
                this.PlayerMark = mark;
                isBlinking = false;
                this.frmLoi = frmLoi;


                // Khởi tạo và cấu hình Timer
                blinkTimer = new Timer();
                blinkTimer.Interval = 300; // Đặt khoảng thời gian nhấp nháy (miliseconds)
                blinkTimer.Tick += BlinkTimer_Tick;
                blinkTimer.Start();
                this.Player = new List<Player>()
                {
                    new Player(frmLoi.playerName1, Image.FromFile(Application.StartupPath + "\\Resources\\x.PNG")),
                    new Player(frmLoi.playerName2, Image.FromFile(Application.StartupPath + "\\Resources\\o.PNG"))
                };
          

            }
            private Stack<PlayInfo> playTimeLine;
        private bool hasUsedUndo = false;
        public bool Undo()
        {
            if (PlayTimeLine.Count <= 0||hasUsedUndo)
                return false;
            // Loại bỏ tọa độ của nước đi hiện tại ra khỏi danh sách playedPositions
            if (playedPositions.Count > 0)
            {
                playedPositions.RemoveAt(playedPositions.Count - 1);
            }


            // Kiểm tra số lần undo của người chơi hiện tại
            int currentPlayerUndoCount = (CurrentPlayer == 1) ? player1UndoCount : player2UndoCount;

            // Kiểm tra xem có vượt quá giới hạn undo cho người chơi hiện tại không (3 lần)
            if (currentPlayerUndoCount >= 3)
            {
                MessageBox.Show("Bạn đã undo quá 3 lần cho lượt chơi này.");
                return false;
            }

            PlayInfo oldPoint = PlayTimeLine.Pop();
            Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];
            btn.BackgroundImage = null;

            // Tăng số lần undo của người chơi hiện tại
            if (CurrentPlayer == 1)
            {
                player1UndoCount++;
                frmLoi.player1Undo++;
            }
            else
            {
                player2UndoCount++;
                frmLoi.player2Undo++;
            }

            // Cập nhật TextBox hiển thị số lần undo
            frmLoi.tbResult3.Text = "Undo: " + (frmLoi.player1Undo).ToString();
            frmLoi.tbResult4.Text = "Undo: " + (frmLoi.player2Undo).ToString();

            if (PlayTimeLine.Count <= 0)
            {
                CurrentPlayer = 0;
            }
            else
            {
                oldPoint = PlayTimeLine.Peek();
                CurrentPlayer = oldPoint.CurrentPlayer == 1 ? 0 : 1;
            }

            ChangePlayer();
            hasUsedUndo = true;
            return true;
        }
            private int player1UndoCount = 0;
            private int player2UndoCount = 0;


        private bool isGameEnded = false;
            private void BlinkTimer_Tick(object sender, EventArgs e)
            {
                if (isBlinking == false)
                {
                    // Duyệt qua tất cả các ô và thay đổi `BackgroundImage` của các ô đã chiến thắng
                    foreach (var row in Matrix)
                    {
                        foreach (var btn in row)
                        {
                            if (winningButtons.Contains(btn))
                            {
                                // Thay đổi `BackgroundImage` của các ô đã chiến thắng tùy thuộc vào người chiến thắng
                                if (CurrentPlayer == 0)
                                {
                                    btn.BackgroundImage = (btn.BackgroundImage == null) ? Properties.Resources.o : null;
                                }
                                else
                                {
                                    btn.BackgroundImage = (btn.BackgroundImage == null) ? Properties.Resources.x : null;
                                }
                            }
                        }
                    }
                }
            }



            public Panel ChessBoard
            {
                get { return chessBoard; }
                set { chessBoard = value; }
            }

            public List<Player> Player
            {
                get { return player; }
                set { player = value; }
            }

            public int CurrentPlayer
            {
                get => currentPlayer;
                set => currentPlayer = value;
            }

            public TextBox PlayerName
            {
                get => playerName;
                set => playerName = value;
            }

            public PictureBox PlayerMark
            {
                get => playerMark;
                set => playerMark = value;
            }

            public List<List<Button>> Matrix
            {
                get => matrix;
                set => matrix = value;
            }
            public Stack<PlayInfo> PlayTimeLine { get => playTimeLine; set => playTimeLine = value; }
        public List<Point> PlayedPositions { get => playedPositions; set => playedPositions = value; }

        private event EventHandler playerMarked;
            public event EventHandler PlayerMarked
            {
                add
                {
                    playerMarked += value;
                }
                remove
                {
                    playerMarked -= value;
                }
            }
            private event EventHandler endedGame;
            public event EventHandler EndedGame
            {
                add
                {
                    endedGame += value;
                }
                remove
                {
                    endedGame -= value;
                }
            }

            public void DrawChessBoard()
            {
                playedPositions.Clear();
                isGameEnded = false;
                ChessBoard.Enabled = true;
                ChessBoard.Controls.Clear();
                PlayTimeLine = new Stack<PlayInfo>();
                CurrentPlayer = 0;
            player1UndoCount = 0;
            player2UndoCount = 0;
            frmLoi.player1Undo = 0;
            frmLoi.player2Undo = 0;
            frmLoi.lbll.Text = "";
            ChangePlayer();
                Matrix = new List<List<Button>>();
                winningButtons = new List<Button>(); // Khởi tạo danh sách các ô đã chiến thắng

                Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
                for (int i = 0; i < Cls_Loi.CHESS_BOARD_HEIGHT; i++)
                {
                    Matrix.Add(new List<Button>());
                    for (int j = 0; j < Cls_Loi.CHESS_BOARD_WIDTH; j++)
                    {
                        Button btn = new Button
                        {
                            Width = Cls_Loi.CHESS_WIDTH,
                            Height = Cls_Loi.CHESS_HEIGHT,
                            Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                            BackgroundImageLayout = ImageLayout.Stretch,
                            Tag = i.ToString()
                        };
                        ChessBoard.Controls.Add(btn);
                        Matrix[i].Add(btn);
                        oldButton = btn;
                        btn.Click += btn_Click;
                    }
                    oldButton.Location = new Point(0, oldButton.Location.Y + Cls_Loi.CHESS_HEIGHT);
                    oldButton.Width = 0;
                    oldButton.Height = 0;
                }
            }
        private Point currentMovePoint; // Thêm biến này
        public void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null || isGameEnded)
                return;
            // Lấy tọa độ (x, y) của ô và thêm vào danh sách playedPositions
            Point clickedPoint = GetChessPoint(btn);
            playedPositions.Add(clickedPoint);
            currentMovePoint = clickedPoint;
            Mark(btn);

            PlayTimeLine.Push(new PlayInfo(GetChessPoint(btn), CurrentPlayer));
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();

            // Đặt lại biến hasUsedUndo khi chuyển lượt chơi
            hasUsedUndo = false;

            if (playerMarked != null)
            {
                playerMarked(this, new EventArgs());
            }

            if (isEndGame(btn))
            {
                isGameEnded = true;
                EndGame();
            }
        }


        private void ChangePlayer()
            {
                PlayerName.Text = Player[CurrentPlayer].Name;

                PlayerMark.Image = Player[CurrentPlayer].Mark;
            }

            public void EndGame()
            {
                if (endedGame != null)
                {
                    endedGame(this, new EventArgs());
                }

                string winner = Player[CurrentPlayer == 1 ? 0 : 1].Name; // Người chiến thắng là người chơi còn lại
                string winningSymbol = (CurrentPlayer == 1) ? "X" : "O";
                MessageBox.Show($"{winner} đã chiến thắng với quân {winningSymbol}!");

                // Cập nhật số ván thắng cho người chơi thắng
                if (CurrentPlayer == 1)
                {
                    frmLoi.player1Wins++; // Người chơi 1 thắng
                }
                else
                {
                    frmLoi.player2Wins++; // Người chơi 2 thắng
                }

                frmLoi.tbResult1.Text = frmLoi.player1Wins.ToString() + "/" + (frmLoi.player1Wins + frmLoi.player2Wins).ToString();
                frmLoi.tbResult2.Text = frmLoi.player2Wins.ToString() + "/" + (frmLoi.player1Wins + frmLoi.player2Wins).ToString();

                // Duyệt qua tất cả các ô và kiểm tra xem chúng có nằm trong danh sách ô đã chiến thắng không
            }

            private bool isEndGame(Button btn)
            {
                Point point = GetChessPoint(btn);

                // Kiểm tra ngang, dọc, chéo chính và chéo phụ
                bool horizontalWin = isEndHorizontal(point, btn.BackgroundImage);
                bool verticalWin = isEndVertical(point, btn.BackgroundImage);
                bool primaryWin = isEndPrimary(point, btn.BackgroundImage);
                bool subWin = isEndSub(point, btn.BackgroundImage);

                if (horizontalWin || verticalWin || primaryWin || subWin)
                {
                    // Thêm các ô đã chiến thắng vào danh sách
                    winningButtons.AddRange(GetWinningButtons(point, btn.BackgroundImage));
                    return true;
                }

                return false;
            }

            private Point GetChessPoint(Button btn)
            {
                int vertical = Convert.ToInt32(btn.Tag);
                int horizontal = Matrix[vertical].IndexOf(btn);
                Point point = new Point(horizontal, vertical);
                return point;
            }

            private bool isEndHorizontal(Point point, Image backgroundImage)
            {
                int countLeft = 0;
                int countRight = 0;

                for (int i = point.X; i >= 0; i--)
                {
                    if (matrix[point.Y][i].BackgroundImage == backgroundImage)
                    {
                        countLeft++;
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = point.X + 1; i < Cls_Loi.CHESS_BOARD_WIDTH; i++)
                {
                    if (matrix[point.Y][i].BackgroundImage == backgroundImage)
                    {
                        countRight++;
                    }
                    else
                    {
                        break;
                    }
                }
                return countLeft + countRight >= 5;
            }
            private bool isEndVertical(Point point, Image backgroundImage)
            {
                int countTop = 0;
                int countBottom = 0;

                for (int i = point.Y; i >= 0; i--)
                {
                    if (matrix[i][point.X].BackgroundImage == backgroundImage)
                    {
                        countTop++;
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = point.Y + 1; i < Cls_Loi.CHESS_BOARD_HEIGHT; i++)
                {
                    if (matrix[i][point.X].BackgroundImage == backgroundImage)
                    {
                        countBottom++;
                    }
                    else
                    {
                        break;
                    }
                }
                return countTop + countBottom >= 5;
            }
            private bool isEndPrimary(Point point, Image backgroundImage)
            {
                int countTop = 0;
                int countBottom = 0;

                for (int i = 0; i <= point.X; i++)
                {
                    if (point.X - i < 0 || point.Y - i < 0)
                        break;
                    if (matrix[point.Y - i][point.X - i].BackgroundImage == backgroundImage)
                    {
                        countTop++;
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= Cls_Loi.CHESS_BOARD_WIDTH - point.X; i++)
                {
                    if (point.Y + i >= Cls_Loi.CHESS_BOARD_HEIGHT || point.X + i >= Cls_Loi.CHESS_BOARD_WIDTH)
                        break;
                    if (matrix[point.Y + i][point.X + i].BackgroundImage == backgroundImage)
                    {
                        countBottom++;
                    }
                    else
                    {
                        break;
                    }
                }

                return countTop + countBottom >= 5;
            }
            private bool isEndSub(Point point, Image backgroundImage)
            {
                int countTop = 0;
                int countBottom = 0;

                for (int i = 0; i <= point.X; i++)
                {
                    if (point.X + i > Cls_Loi.CHESS_BOARD_WIDTH || point.Y - i < 0)
                        break;
                    if (matrix[point.Y - i][point.X + i].BackgroundImage == backgroundImage)
                    {
                        countTop++;
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = 1; i <= Cls_Loi.CHESS_BOARD_WIDTH - point.X; i++)
                {
                    if (point.Y + i >= Cls_Loi.CHESS_BOARD_HEIGHT || point.X - i < 0)
                        break;
                    if (matrix[point.Y + i][point.X - i].BackgroundImage == backgroundImage)
                    {
                        countBottom++;
                    }
                    else
                    {
                        break;
                    }
                }

                return countTop + countBottom >= 5;
            }
            private List<Button> GetWinningButtons(Point point, Image backgroundImage)
            {
                List<Button> winningButtons = new List<Button>();

                if (isEndHorizontal(point, backgroundImage))
                {
                    for (int i = point.X; i >= 0; i--)
                    {
                        if (matrix[point.Y][i].BackgroundImage == backgroundImage)
                        {
                            winningButtons.Add(matrix[point.Y][i]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = point.X + 1; i < Cls_Loi.CHESS_BOARD_WIDTH; i++)
                    {
                        if (matrix[point.Y][i].BackgroundImage == backgroundImage)
                        {
                            winningButtons.Add(matrix[point.Y][i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (isEndVertical(point, backgroundImage))
                {
                    for (int i = point.Y; i >= 0; i--)
                    {
                        if (matrix[i][point.X].BackgroundImage == backgroundImage)
                        {
                            winningButtons.Add(matrix[i][point.X]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = point.Y + 1; i < Cls_Loi.CHESS_BOARD_HEIGHT; i++)
                    {
                        if (matrix[i][point.X].BackgroundImage == backgroundImage)
                        {
                            winningButtons.Add(matrix[i][point.X]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (isEndPrimary(point, backgroundImage))
                {
                    for (int i = 0; i <= point.X; i++)
                    {
                        if (point.X - i < 0 || point.Y - i < 0)
                            break;
                        if (matrix[point.Y - i][point.X - i].BackgroundImage == backgroundImage)
                        {
                            winningButtons.Add(matrix[point.Y - i][point.X - i]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = 1; i <= Cls_Loi.CHESS_BOARD_WIDTH - point.X; i++)
                    {
                        if (point.Y + i >= Cls_Loi.CHESS_BOARD_HEIGHT || point.X + i >= Cls_Loi.CHESS_BOARD_WIDTH)
                            break;
                        if (matrix[point.Y + i][point.X + i].BackgroundImage == backgroundImage)
                        {
                            winningButtons.Add(matrix[point.Y + i][point.X + i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (isEndSub(point, backgroundImage))
                {
                    for (int i = 0; i <= point.X; i++)
                    {
                        if (point.X + i > Cls_Loi.CHESS_BOARD_WIDTH || point.Y - i < 0)
                            break;
                        if (matrix[point.Y - i][point.X + i].BackgroundImage == backgroundImage)
                        {
                            winningButtons.Add(matrix[point.Y - i][point.X + i]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = 1; i <= Cls_Loi.CHESS_BOARD_WIDTH - point.X; i++)
                    {
                        if (point.Y + i >= Cls_Loi.CHESS_BOARD_HEIGHT || point.X - i < 0)
                            break;
                        if (matrix[point.Y + i][point.X - i].BackgroundImage == backgroundImage)
                        {
                            winningButtons.Add(matrix[point.Y + i][point.X - i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return winningButtons;
            }
        public List<Point> playedPositions = new List<Point>();
        private void Mark(Button btn)
            {
                btn.BackgroundImage = Player[CurrentPlayer].Mark;
          
            }
            public void ClearBoard()
            {
                // Duyệt qua tất cả các ô trên bảng cờ và xóa hình nền của chúng
                foreach (var row in Matrix)
                {
                    foreach (var btn in row)
                    {
                        btn.BackgroundImage = null;
                    }
                }

            // Đặt lại trạng thái của ván đấu và các biến liên quan
            playedPositions.Clear();
            isGameEnded = false;
                currentPlayer = 0;
                playerName.Text = Player[0].Name; // Đặt lại tên người chơi hiển thị
                playerMark.Image = Player[0].Mark; // Đặt lại hình đánh dấu của người chơi hiển thị
                winningButtons.Clear();
                player1UndoCount = 0;
                player2UndoCount = 0;
            frmLoi.tbResult3.Text = "";
            frmLoi.tbResult4.Text = "";
            frmLoi.tbResult5.Value = 0;
            frmLoi.abc();
            frmLoi.aaa();
            frmLoi.lbll.Text = "";
        }
        }
    }
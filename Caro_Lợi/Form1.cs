using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caro_Lợi
{
    public partial class frmLoi : Form
    {
        #region Properties
        Cls_TienLoi ChessBoard;
        public int a;
        public int b;
        public string playerName1;// biến lưu tên người chơi 1
        public string playerName2; // biến lưu tên người chơi 2
        public bool newgame = true;
        public static int player1Wins; // Để lưu số ván thắng của người chơi 1
        public static int player2Wins; // Để lưu số ván thắng của người chơi 2
        public static int player1Undo; // Để lưu số ván Undo của người chơi 1
        public static int player2Undo; // Để lưu số ván Undo của người chơi 2
        public TextBox tbResult1;
        public TextBox tbResult2;
        public TextBox tbResult3;
        public TextBox tbResult4;
        public ProgressBar tbResult5;
        private int playedTimeInSeconds = 0;
        public Timer gameTimer = new Timer();
        public Label lbll;
        #endregion
        Timer labelTimer = new Timer();
        int counter = 0;
        
        public frmLoi()
        {
            InitializeComponent();
            gameTimer.Interval = 1000;  // Cập nhật mỗi giây (1000 milliseconds)
            gameTimer.Tick += GameTimer_Tick;
            undoToolStripMenuItem.Enabled = false;
          

        }

        public void GameTimer_Tick(object sender, EventArgs e)
        {
            playedTimeInSeconds++;

            // Cập nhật label5 để hiển thị thời gian đã chơi 
            TimeSpan timePlayed = TimeSpan.FromSeconds(playedTimeInSeconds);
            label5.Text = string.Format("Thời gian đã chơi trong ván: {0:D2}:{1:D2}", timePlayed.Minutes, timePlayed.Seconds);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pnl1.Visible = false;
            pnl2.Visible = false;
            panel5.Visible = false;
            btnClear.Visible = false;
            label2.Visible = true;
            // Thiết lập timer
            labelTimer.Interval = 500;  // Thời gian nhấp nháy (500 milliseconds)
            labelTimer.Tick += LabelTimer_Tick;
            labelTimer.Start();

            tbResult1 = textBox2;
            tbResult2 = textBox5;
            tbResult3 = textBox3;
            tbResult4 = textBox4;
            tbResult5 = prcbCoolDown;
            lbll = lbl;
           
          

        }
        private void LabelTimer_Tick(object sender, EventArgs e)
        {
            counter++;
            // Nhấp nháy label tại các số lẻ
            if (counter % 2 == 1)
            {
                label2.Visible = !label2.Visible;
            }
        }
        private string InputBox(string prompt, string title)
        {
            Form promptForm = new Form();
            promptForm.Width = 400;
            promptForm.Height = 150;
            promptForm.Text = title;
            Label promptLabel = new Label() { Left = 50, Top = 20, Text = prompt,Width = 300 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200 };
            Button confirmButton = new Button() { Text = "OK", Left = 150, Width = 80, Top = 80 };
            confirmButton.Click += (sender, e) => { promptForm.Close(); };
            promptForm.Controls.Add(textBox);
            promptForm.Controls.Add(promptLabel);
            promptForm.Controls.Add(confirmButton);
            promptForm.ShowDialog();
            return textBox.Text;
        }
         void EndGame()
        {
            gameTimer.Stop();
            tmCoolDown.Stop();
            pnlChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;

            MessageBox.Show("Hết thời gian");
            lbll.Text = "Vui lòng chọn game mới trên Menu hoặc tổ hợp phím Ctr+N";


        }
        void NewGame()
        {

            undoToolStripMenuItem.Enabled = true;
            prcbCoolDown.Value = 0;
            tmCoolDown.Stop();
            ChessBoard.DrawChessBoard();
            label4.Text = "";
            player2Undo = 0;
            player1Undo = 0;
            tbResult3.Text = "Undo:0";
            tbResult4.Text = "Undo:0";

        }

        private void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmCoolDown.Start();
            prcbCoolDown.Value = 0;
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            prcbCoolDown.PerformStep();
            if (prcbCoolDown.Value >= prcbCoolDown.Maximum)
            {
                tmCoolDown.Stop();
                EndGame();
            }

        }
       

        public void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (newgame == true)
            {
                
                playerName1 = InputBox("Nhập tên người chơi đầu tiên với quân X:", "Nhập tên");
                playerName2 = InputBox("Nhập tên người chơi tiếp theo với quân O:", "Nhập tên");
                while (string.IsNullOrWhiteSpace(playerName1) || string.IsNullOrWhiteSpace(playerName2))
                {
                    MessageBox.Show("Vui lòng nhập tên cho cả hai người chơi.");
                    playerName1 = InputBox("Nhập tên người chơi đầu tiên với quân X:", "Nhập tên");
                    playerName2 = InputBox("Nhập tên người chơi tiếp theo với quân O:", "Nhập tên");
                }
                player1Wins = 0;
                player2Wins = 0;
                tbResult1.Text = player1Wins.ToString() + "/" + (player2Wins+player1Wins).ToString();
                tbResult2.Text = player2Wins.ToString() + "/" + (player2Wins + player1Wins).ToString();
                tbResult3.Text = player1Undo.ToString() ; 
                tbResult4.Text = player2Undo.ToString()  ; 
                ChessBoard = new Cls_TienLoi(pnlChessBoard, txbPlayerName, pctbMark, this);
                ChessBoard.EndedGame += ChessBoard_EndedGame;
                ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;

                ChessBoard.DrawChessBoard();
                
                gameTimer.Start();
                label5.Visible = true;
                prcbCoolDown.Step = Cls_Loi.COOL_DOWN_STEP;
                prcbCoolDown.Maximum = Cls_Loi.COOL_DOWN_TIME;
                prcbCoolDown.Value = 0;
                tmCoolDown.Interval = Cls_Loi.COOL_DOWN_INTERVAL;
                undoToolStripMenuItem.Enabled = true;

                labelTimer.Stop();
                labelTimer.Dispose();
                label2.Visible = false;
                txbPlayerName1.Text = playerName1.ToString();
                txbPlayerName2.Text = playerName2.ToString();
                pnl1.Visible = true;
                pnl2.Visible = true;
                panel5.Visible = true;
                btnClear.Visible = true;
                newgame = false;
                NewGame();
                playedTimeInSeconds = 0;
                pnlChessBoard.Enabled = true;

            }
            else
            {
                pnlChessBoard.Enabled = true;
                NewGame();
                playedTimeInSeconds = 0;
                
                gameTimer.Start();
                label5.Visible = true;  

            }
        }
       
        public void abc()
        {
            tmCoolDown.Stop();
        }
        public void aaa()
        {
            undoToolStripMenuItem.Enabled = false;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu ván đấu
            ChessBoard.ClearBoard();

            // Xóa tên người chơi
            txbPlayerName1.Text = "";
            txbPlayerName2.Text = "";
            label4.Text = "Vui lòng chọn game mới hoặc tổ hợp phím Ctr+N";

            // Đặt lại biến newgame về true để cho phép tạo ván đấu mới
            newgame = true;
            tbResult1.Text = "";
            tbResult2.Text = "";
            txbPlayerName.Text = "";
            pctbMark.Image = null;
            pnlChessBoard.Enabled = false;
            label5.Visible = false;
          


        }
        private int currentPlayerIndex = 0;
        private void lưuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Mở hộp thoại để chọn nơi lưu tệp tin
    SaveFileDialog saveFileDialog = new SaveFileDialog();
    saveFileDialog.Filter = "Tệp tin văn bản (*.txt)|*.txt";
    if (saveFileDialog.ShowDialog() == DialogResult.OK)
    {
        string filePath = saveFileDialog.FileName;
        SaveResultsToFile(filePath, player1Wins.ToString(), player2Wins.ToString());
    }
        }
        private void SaveResultsToFile(string filePath, string result1, string result2)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Ghi dữ liệu từ result1 và result2 vào tệp tin
                    writer.WriteLine("Tổng trận thắng của người chơi " + playerName1 + " với quân X:" + player1Wins);
                    writer.WriteLine("Tổng trận thắng của người chơi " + playerName2 + " với quân O:" + player2Wins);
                    writer.WriteLine("Tổng số trận của cả 2 người chơi " + Convert.ToInt32(player1Wins + player2Wins));
                    writer.WriteLine("-----------------------------------------------------------------------------------");
                    writer.WriteLine("Tổng số lần Undo của người chơi " + playerName1 + " trong ván đấu: " + player1Undo);
                    writer.WriteLine("Tổng số lần Undo của người chơi " + playerName2 + " trong ván đấu: " + player2Undo);
                    writer.WriteLine("-----------------------------------------------------------------------------------");
                    writer.WriteLine("---- Vì là mảng 2 chiều nên vị trí tọa độ x và y bắt đầu là 0 thay vì 1------------");
                    foreach (Point position in ChessBoard.playedPositions)
                    {
                        int x = position.X; // Tọa độ x
                        int y = position.Y; // Tọa độ y

                        // Xác định người chơi hiện tại và ghi tọa độ tương ứng vào tệp tin
                        if (currentPlayerIndex == 0)
                        {
                            writer.WriteLine("Người chơi " + playerName1 + ": Tọa độ x: " + x + ", Tọa độ y: " + y);
                            currentPlayerIndex = 1; // Chuyển sang người chơi thứ hai
                        }
                        else
                        {
                            writer.WriteLine("Người chơi " + playerName2 + ": Tọa độ x: " + x + ", Tọa độ y: " + y);
                            currentPlayerIndex = 0; // Chuyển lại người chơi thứ nhất
                        }
                    }
                }

                MessageBox.Show("Dữ liệu đã được lưu vào tệp tin thành công!");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }


        private void btnSaveResults_Click(object sender, EventArgs e)
        {
            // Mở hộp thoại để chọn nơi lưu tệp tin
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Tệp tin văn bản (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                SaveResultsToFile(filePath, player1Wins.ToString(), player2Wins.ToString());
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

          //  prcbCoolDown.Value = 0;
            ChessBoard.Undo();
            
         

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

     

        private void frmLoi_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)

                e.Cancel = true;
        }

        private void gameGuidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tạo một đối tượng của form mới
            Guide myForm = new Guide();

            // Sử dụng phương thức ShowDialog để hiển thị form (nó sẽ hiển thị và chờ đến khi form đó được đóng)
            myForm.ShowDialog();

            // Sau khi form đã được đóng, bạn có thể thực hiện các thao tác tiếp theo ở đây

        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pnlChessBoard_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

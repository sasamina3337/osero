using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Internal;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osero
{
    public partial class Form1 : Form
    {

        public int[,] board = new int[8, 8];

        public int[,] resetBoard = new int[8, 8]{{0, 0, 0, 0, 0, 0, 0, 0,},
                                                 {0, 0, 0, 0, 0, 0, 0, 0,},
                                                 {0, 0, 0, 0, 0, 0, 0, 0,},
                                                 {0, 0, 0, 0, 0, 0, 0, 0,},
                                                 {0, 0, 0, 0, 0, 0, 0, 0,},
                                                 {0, 0, 0, 0, 0, 0, 0, 0,},
                                                 {0, 0, 0, 0, 0, 0, 0, 0,},
                                                 {0, 0, 0, 0, 0, 0, 0, 0,}};

        public int[,] firstStone = new int[8, 8]{{0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 1, 2, 0, 0, 0},
                                                 {0, 0, 0, 2, 1, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0}};

        public int player, gameCount, step;

        public Boolean stoneJudge = true;

        public string[] gameStep = { "スタート", "リセット", "リスタート" };

        public string[] turnName = { "先手(黒)", "後手(白)" };

        public enum stoneColor
        {
            none = 0,
            black = 1,
            white = 2,
        }

        public Dictionary<stoneColor, Image> stoneImg = new Dictionary<stoneColor, Image>()
        {
            {stoneColor.none, null},
            {stoneColor.black, osero.Properties.Resources.black},
            {stoneColor.white, osero.Properties.Resources.white}
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void start_Click(object sender, EventArgs e)
        {
            player = step = 0;
            gameCount = 4;
            label.Text = turnName[player] + "の番です";
            Array.Copy(firstStone, board, resetBoard.Length);
            boardEnable(true);
            Drow(board);
            gameStepUp();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            stoneJudge = true;
            PictureBox clickBox = (PictureBox)sender;
            stoneColor nowStone = (stoneColor)(player + 1);
            string nameBox = clickBox.Name;
            int numberBox = int.Parse(nameBox.Replace("pictureBox", ""));
            int row = numberBox / board.GetLength(0);
            int col = numberBox % board.GetLength(1);
            board[row, col] = player + 1;
            selectStone(numberBox);
            if (!stoneJudge)
            {
                board[row, col] = 0;
                return;
            }

            Image img = stoneImg[nowStone];
            clickBox.Image = img;
            clickBox.Enabled = false;

            if (gameCount + 1 >= board.Length)
            {
                boardEnable(false);
                gameStepUp();
                judge(board);
                return;
            }

            gameCount++;
            player = (player + 1) % 2;
            label.Text = turnName[player] + "の番です";
        }

        public void boardEnable(Boolean b)
        {
            Control[] c;
            PictureBox pic;
            for (int i = 0; i < board.Length; i++)
            {
                c = this.Controls.Find("pictureBox" + i.ToString(), true);
                pic = (PictureBox)c[0];
                pic.Enabled = b;
            }
        }

        public void Drow(int[,] n)
        {
            Control[] c;
            PictureBox pic;
            string boxName;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(1); j++)
                {
                    boxName = Convert.ToString(i * board.GetLength(1) + j);
                    c = this.Controls.Find("pictureBox" + boxName, true);
                    pic = (PictureBox)c[0];
                    pic.Enabled = (n[i,j] == 0) ;
                    pic.Image = stoneImg[(stoneColor)n[i,j]];
                }
            }
        }

        public void gameStepUp()
        {
            step = (step + 1) % 3;
            start.Text = gameStep[step];
            if (step != 1)
            {
                label.Text = gameStep[step] + "を押してください";
            }
        }

        public int[,] aroundStone(int x, int y)
        {
            int[,] cross = new int[3, 3];
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int p = i + x;
                    int q = j + y;
                    if (p >= 0 && p < board.GetLength(0) && q >= 0 && q < board.GetLength(1))
                    {
                        cross[i + 1, j + 1] = board[p, q];
                    }
                }
            }
            return cross;
        }

        public void selectStone(int n)
        {
            int row = n / board.GetLength(0);
            int col = n % board.GetLength(1);
            int[,] cross = aroundStone(row, col);

            List<int> rows = new List<int>();
            List<int> cols = new List<int>();

            int m = (n + 1) % 2;

            for (int i = 0; i < cross.GetLength(0); i++)
            {
                for (int j = 0; j < cross.GetLength(1); j++)
                {
                    int x = row + (i - 1);
                    int y = col + (j - 1);
                    if (x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1))
                    {
                        if (cross[i, j] == m)
                        {
                            rows.Add(i);
                            cols.Add(j);
                        }
                    }
                }
            }

            for (int i = 2; i <= board.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < rows.Count; j++)
                {
                    int x = row + (rows[j] * i);
                    int y = col + (cols[j] * i);
                    if (x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1))
                    {
                        int d = board[x, y];
                        if (d != n)
                        {
                            rows.RemoveAt(j);
                            cols.RemoveAt(j);
                            stoneJudge = false;
                            return;

                        }
                    }
                }
            }

            for (int i = 2; i <= board.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < rows.Count; j++)
                {
                    int x = row + (rows[j] * i);
                    int y = col + (cols[j] * i);
                    if (x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1))
                    {
                        int d = board[x, y];
                        if (d != n)
                        {
                            while (board[x, y] != n)
                            {
                                board[x, y] = n;
                                x -= rows[j];
                                y -= cols[j];
                            }
                            break;
                        }
                    }
                }
            }

            if (!stoneJudge)
            {
                player = (player + 1) % 2;
                label.Text = turnName[player] + "の番です";
            }
        }

        private void judge(int[,] f)
        {
            int b, w;
            b = w = 0;

            foreach (int n in f)
            {
                if (n == 1)
                {
                    b++;
                }
                else if (n == 2)
                {
                    w++;
                }
            }

            if (b > w)
            {
                MessageBox.Show("先行(黒)の勝利");
            }
            else if (w > b)
            {
                MessageBox.Show("後攻(白)の勝利");
            }
            else
            {
                MessageBox.Show("引き分けです");
            }
        }
    }
}
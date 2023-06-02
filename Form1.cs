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

        public int[,] board = new int[8, 8]; //盤面の配列

        //初期配置
        public int[,] firstStone = new int[8, 8]{{0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 1, 2, 0, 0, 0},
                                                 {0, 0, 0, 2, 1, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0},
                                                 {0, 0, 0, 0, 0, 0, 0, 0}};

        //周りのストーン数
        int[,] aroundStone = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

        public int currentPlayer, counterPlayer, gameCount, step; //stepはgamestepを扱うために設定した数字

        private int passCount = 0; //passの数を数える数字

        public string[] gameStep = { "スタート", "リセット", "リスタート" }; //startボタンの変更文字

        public string[] turnName = { "先手(黒)", "後手(白)" }; //labelに表示する現在のプレイヤー名

        //石の色
        public enum stoneColor
        {
            none = 0,
            black = 1,
            white = 2,
        }

        //置ける石のリスト
        public List<int> selectedStone = new List<int>();

        //石の画像と色の対応辞書
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

        //スタートクリック時の操作
        private void start_Click(object sender, EventArgs e)
        {
            currentPlayer = step = 0;
            counterPlayer = (currentPlayer + 1) % 2;
            gameCount = 4;
            label.Text = turnName[currentPlayer] + "の番です";
            Array.Copy(firstStone, board, board.Length);
            boardEnable(true);
            Drow(board);
            gameStepUp();
        }
        //盤面クリック時の操作
        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickBox = (PictureBox)sender;
            stoneColor nowStone = (stoneColor)(currentPlayer + 1);
            string nameBox = clickBox.Name;
            int numberBox = int.Parse(nameBox.Replace("pictureBox", ""));
            int row = numberBox / board.GetLength(0);
            int col = numberBox % board.GetLength(1);
            if (!selectStone(row, col))
            {
                return;
            }

            board[row, col] = currentPlayer + 1;

            Image img = stoneImg[nowStone];
            clickBox.Image = img;
            clickBox.Enabled = false;

            gameCount++;

            if (gameCount >= board.Length)
            {
                boardEnable(false);
                gameStepUp();
                judge(board);
                return;
            }

            changePlayer();
            label.Text = turnName[currentPlayer] + "の番です";
        }
        //盤面をtrueで操作可能にする
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

        //図を書く
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
        //labelの管理
        public void gameStepUp()
        {
            step = (step + 1) % 3;
            start.Text = gameStep[step];
            if (step != 1)
            {
                label.Text = gameStep[step] + "を押してください";
            }
        }

        //石を置いた場所から八方向に探索し、ひっくり返す関数
        private bool selectStone(int row, int col)
        {
            stoneColor nowStone = (stoneColor)(currentPlayer + 1);
            stoneColor enemyStone = (stoneColor)(counterPlayer + 1);
            bool canPut = false;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int x = col + dx;
                    int y = row + dy;
                    bool isInBoard = (x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1));
                    bool isEnemyStone = (isInBoard && board[y, x] == (int)enemyStone);
                    List<int> reverseList = new List<int>();

                    while (isEnemyStone)
                    {
                        reverseList.Add(y * board.GetLength(0) + x);
                        x += dx;
                        y += dy;
                        isInBoard = (x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1));
                        isEnemyStone = (isInBoard && board[y, x] == (int)enemyStone);
                    }

                    if (isInBoard && board[y, x] == (int)nowStone)
                    {
                        if (reverseList.Count > 0)
                        {
                            canPut = true;
                            foreach (int n in reverseList)
                            {
                                board[n / board.GetLength(0), n % board.GetLength(1)] = currentPlayer + 1;
                                Control[] c = this.Controls.Find("pictureBox" + n.ToString(), true);
                                PictureBox pic = (PictureBox)c[0];
                                pic.Image = stoneImg[nowStone];
                            }
                        }
                    }
                }
            }

            return canPut;
        }



        private void changePlayer()
        {
            int x = currentPlayer;
            currentPlayer = counterPlayer;
            counterPlayer = x;

            if (checkPass())
            {
                passCount++;
                if (passCount >= 2)
                {
                    judge(board);
                    return;
                }
                MessageBox.Show(turnName[currentPlayer] + "はパスです。");
                changePlayer();
            }
            else
            {
                passCount = 0;
            }

            label.Text = turnName[currentPlayer] + "の手番です。";
        }

        //現在の状態がパスか判定する
        private bool checkPass()
        {
            stoneColor nowStone = (stoneColor)(currentPlayer + 1);
            bool canPut = false;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col] != (int)stoneColor.none) continue;
                    if (selectStone(row, col))
                    {
                        canPut = true;
                        break;
                    }
                }
                if (canPut) break;
            }

            return !canPut;
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
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

        public int currentPlayer, gameCount, step; //stepはgamestepを扱うために設定した数字

        public string[] gameStep = { "スタート", "リセット", "リスタート" }; //startボタンの変更文字

        public string[] turnName = { "先手(黒)", "後手(白)" }; //labelに表示する現在のプレイヤー名

        //石の色
        public enum stoneColor
        {
            none = 0,
            black = 1,
            white = 2,
        }

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
            board[row, col] = currentPlayer + 1;
            if (selectStone(row, col))
            {
                board[row, col] = 0;
                return;
            }

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

            currentPlayer = (currentPlayer + 1) % 2;
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


        public Boolean selectStone(int n, int m)
        {

            return false;
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
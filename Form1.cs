using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public int[] board1 = new int[64];

        public int[,] board2 = new int[8, 8];

        public int player = 0;

        public int gameCount = 0;

        public Boolean turn = true;

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
            boardEnable(true);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

            PictureBox clickBox = (PictureBox)sender;
            stoneColor nowStone = (stoneColor)(player + 1);
            string nameBox = clickBox.Name;
            int numberBox = int.Parse(nameBox.Replace("pictureBox", ""));
            board1[numberBox] = player + 1;
            boardIn(board1);

            Image img = stoneImg[nowStone];
            clickBox.Image = img;
            clickBox.Enabled = false;

            gameCount++;
            player = (player + 1) % 2;
        }

        public void boardEnable(Boolean b)
        {
            Control[] c;
            PictureBox pic;
            for (int i = 0; i < board1.Length; i++)
            {
                c = this.Controls.Find("pictureBox" + i.ToString(), true);
                pic = (PictureBox)c[0];
                pic.Enabled = b;
            }
        }

        public void boardIn(int[] n)
        {
            for(int i = 0; i < board1.Length; i++)
            {
                int row = i / board2.GetLength(0);
                int col = i % board2.GetLength(1);

                board2[row, col] = board1[i];
            }
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace TicTacToe
{
    public partial class dialog : Form
    {
        private Button[] array;
        public dialog()
        {
            InitializeComponent();

            array = new[] {b11, b12, b13, b21, b22, b23, b31, b32, b33};
            isItMyTurn = false;
            baseColor = b11.BackColor;
            restart_Click(null,null);
        }

        private Color baseColor;

        private bool isItMyTurn;

        public enum ECase
        {
            X,O,NULL
        }

        bool checkIfOver() => array.All(button => string.IsNullOrEmpty(button.Text) != true);

        void resetBoard()
        {
            Array.ForEach(array,a => a.Text = "");
        }

        void Log(string str)
        {
            status.Text = str;
        }

        private void gamebut_Click(object sender, EventArgs e)
        {
            if (gameOver) return;
            Button b = (Button) sender;
            if (string.IsNullOrEmpty(b.Text) == false) return;
            b.Text = @"O";
            checkWhoWins();
            letAIShowItsMagic();
        }

        

        private int scoreBen, scoreSen;
        private bool gameOver;
        private void checkWhoWins()
        {
            if (gameOver) return;
            gameOver = true;
            ECase winner = checkFor();
            switch (winner)
            {
            case ECase.X:
                scoreBen ++;
                ben_score.Text = scoreBen.ToString();
                Log("Ben kazandım...");
                break;
            case ECase.O:
                scoreSen++;
                sen_score.Text = scoreSen.ToString();
                if (babymode.Checked) bebeScore++;
                Log("Sen kazandın, tebrikler.");
                break;
            case ECase.NULL:
                if (checkIfOver())
                    Log("Oyun berabere bitti");
                else gameOver = false;
                break;
            }
        }

        private int bebeScore;
        private int[][] checkArray =
        {
            new[] {0,1,2},
            new[] {3,4,5},
            new[] {6,7,8},
            new[] {0,3,6},
            new[] {1,4,7},
            new[] {2,5,8},
            new[] {0,4,8},
            new[] {2,4,6}
        };

        ECase checkFor()
        {
            foreach (int[] ints in checkArray)
            {
                bool lineup = true;
                string first = array[ints[0]].Text;
                if (string.IsNullOrEmpty(first)) continue;
                for (int i = 1; i < ints.Length; i++) { lineup = lineup && array[ints[i]].Text.Equals(first); }
                if (lineup == false) continue;
                foreach (var i in ints) { array[i].BackColor = Color.Red; }
                return ECase.O.ToString().Equals(array[ints[0]].Text) ? ECase.O : ECase.X;
            }
            return ECase.NULL;
        }


        private void restart_Click(object sender, EventArgs e)
        {
            resetBoard();
            gameOver = false;
            isItMyTurn = !isItMyTurn;
            foreach (var a in array) { a.BackColor = baseColor; }
            if (isItMyTurn) letAIShowItsMagic();
            Log(isItMyTurn ? "Bu el ben başladım" : "Sen başlıyorsun");
            if (isItMyTurn) return;
            if (scoreBen < 3 || babymode.Checked) return;
            if (scoreSen == 0) Log("Umarım bu eli alırsın benden.");
            else if(scoreSen - bebeScore == 0) Log("Bebe modu kapalıykende kazanabilirsin.");
        }

        Random random = new Random();
        void letAIShowItsMagic()
        {
            if (checkIfOver() || gameOver) return;
            if (babymode.Checked)
            {
                while (true)
                {
                    int i = random.Next(array.Length);
                    if (!string.IsNullOrEmpty(array[i].Text)) continue;
                    array[i].Text = @"X";
                    break;
                }
            }
            else
            {
                do
                {
                    if (checkIf1MoveLeft()) break;
                    if (checkIfCenterEmpty()) break;
                    if (checkIfBestCornerAvailable()) break;
                    JustMakeAMove();
                } while (false);
            }
            checkWhoWins();            
        }

        bool checkIfSame(int a, int b, int c)
        {
            if (string.IsNullOrEmpty(array[c].Text) == false) return false;
            if (string.IsNullOrEmpty(array[a].Text)) return false;
            if (!array[a].Text.Equals(array[b].Text)) return false;
            array[c].Text = @"X";
            return true;
        } 

        bool checkIf1MoveLeft()
        {
            foreach (var ca in checkArray)
            {
                if (checkIfSame(ca[0], ca[1], ca[2])) return true;
                if (checkIfSame(ca[0], ca[2], ca[1])) return true;
                if (checkIfSame(ca[2], ca[1], ca[0])) return true;
            }
            return false;
        }


        bool checkIfCenterEmpty()
        {
            if (string.IsNullOrEmpty(array[4].Text) == false) return false;
            array[4].Text = @"X";
            return true;
        }


        bool isEnemy(params int[] x) => x.All(i => array[i].Text.Equals("O"));

        bool checkOneOfThem(params int[] places)
        {
            foreach (var place in places) {
                if (!string.IsNullOrEmpty(array[place].Text)) continue;
                array[place].Text = @"X";
                return true;
            }
            return false;
        }

        bool AllEmpty(params int[] a) => a.All(i => string.IsNullOrEmpty(array[i].Text));

        bool checkIfBestCornerAvailable()
        {
            if (isEnemy(0,8) || isEnemy(2,6)) { return checkOneOfThem(1, 3, 5, 7); }

            if (isEnemy(3, 1) && AllEmpty(0, 2, 6)) return checkOneOfThem(0, 2, 6);
            if (isEnemy(5, 1) && AllEmpty(0, 2, 8)) return checkOneOfThem(2, 0, 8);
            if (isEnemy(3, 7) && AllEmpty(0, 8, 6)) return checkOneOfThem(6, 0, 8);
            if (isEnemy(7, 5) && AllEmpty(8, 2, 6)) return checkOneOfThem(8, 2, 6);

            if (isEnemy(0, 5) && AllEmpty(1,2)) return checkOneOfThem(1, 2);
            if (isEnemy(1, 8) && AllEmpty(5, 2)) return checkOneOfThem(2, 5);
            if (isEnemy(2, 7) && AllEmpty(5, 8)) return checkOneOfThem(5, 8);
            if (isEnemy(3, 2) && AllEmpty(1, 0)) return checkOneOfThem(0, 1);
            if (isEnemy(5, 6) && AllEmpty(7, 8)) return checkOneOfThem(7, 8);
            if (isEnemy(6, 1) && AllEmpty(3, 0)) return checkOneOfThem(3, 0);
            if (isEnemy(7, 0) && AllEmpty(3, 6)) return checkOneOfThem(3, 6);
            if (isEnemy(8, 3) && AllEmpty(7, 6)) return checkOneOfThem(6, 7);

           

            int[] d = {0, 2, 6, 8};
            foreach (var i in d) {
                if (!string.IsNullOrEmpty(array[i].Text)) continue;
                array[i].Text = @"X";
                return true;
            }
            return false;
        }


        void JustMakeAMove()
        {
            foreach (var a in array) {
                if (!string.IsNullOrEmpty(a.Text)) continue;
                a.Text = @"X";
                return;
            }
        }

    }
}

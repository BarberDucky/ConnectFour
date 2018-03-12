using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data;
using System.Diagnostics;
using System.IO;

namespace ConnectFour1
{
    public partial class MainForm : Form
    {
        int row = 6;
        int col = 7;
        Game game;
        List<String> _captionList = new List<string>();     //Sluzi za labelu koja prikazuje koji je igrac
        List<String> _winnerList = new List<string>();      //Sluzi da prikaze poruku ko je pobedio
        
        public MainForm()
        {
            this.DoubleBuffered = true;
            game = new Game();
            InitializeComponent();
            InitializeTlp();
            InitializeLists();
            lblIgrac.Text = _captionList[0];
        }

        #region Inicijalizacije
        private void InitializeTlp()
        {
            tlp.ColumnStyles.Clear();
            tlp.RowStyles.Clear();

            tlp.ColumnCount = col;
            tlp.RowCount = row;
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); //dovoljno jednom
            for (int i = 0; i < row; i++)
            {
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
                for (int j = 0; j < col; j++)
                {
                    tlp.Controls.Add(new Cell(j, this), j, i);
                }
            }
        }
        private void InitializeLists()
        {
            lblIgrac.Text = String.Empty;
            _captionList.Add("Igra crveni igrac");
            _captionList.Add("Igra zuti igrac");
            _winnerList.Add("Pobedio je crveni igrac.");
            _winnerList.Add("Pobedio je zuti igrac.");
            _winnerList.Add("Nereseno je.");
        }
        #endregion

        #region Metode

        //Postavlja prazno polje kao pozadinu kad se pokrene new game
        public void SetTlpToEmpty()
        {
            tlp.SuspendLayout();
            for (int i = 0; i < row; i++)
            {
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
                for (int j = 0; j < col; j++)
                {
                    ((Cell)tlp.GetControlFromPosition(j, i)).SetToEmpty();
                }
            }
            tlp.ResumeLayout();
        }

        private void NewGame()
        {
            game.RestartGame();
            tlp.Enabled = true;
            SetTlpToEmpty();
            lblIgrac.Text = _captionList[0];
        }

        public void CallGame(int col)
        {           
            Move lastMove = game.HumanPlay(col);
            if(!UpdateForm(lastMove))
                return;
            this.Refresh();

            if (CheckGameOver(lastMove))
                return;
       
            tlp.Enabled = false;
            lastMove = game.ComputerPlay();
            UpdateForm(lastMove);

            tlp.Enabled = true;
            CheckGameOver(lastMove);
        }

        private bool UpdateForm(Move m)
        {
            if (m != null)
            {
                ((Cell)tlp.GetControlFromPosition(m.Col, m.Row)).MarkCurrentPlayer(m.Current);
                lblIgrac.Text = _captionList[1 - m.Current];
                return true;
            }
            return false;
        }

        private bool CheckGameOver(Move lastMove)
        {
            if (game.State != GameState.NijeKraj)
            {              
                List<Move> listaPoteza=game.Tabla.CheckFourInARow(lastMove);
                MarkConnectFour(listaPoteza, lastMove);
                tlp.Enabled = false;
                String displayString = _winnerList[(int)game.State] + " Da li zelite da pocnete novu igru?";
                DialogResult dlg = MessageBox.Show(displayString, "Igra gotova", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                lblIgrac.Text = "Igra gotova";
                if (dlg == DialogResult.Yes)
                {
                    NewGame();
                }               
                return true;
            }
            return false;
        }

        private void MarkConnectFour(List<Move> listaPoteza, Move lastMove)
        {
            if (listaPoteza == null)
                return;
            foreach(Move m in listaPoteza)
            {
                ((Cell)tlp.GetControlFromPosition(m.Col, m.Row)).MarkWinner(lastMove.Current);
            }
        }       

        #endregion

        #region EventHandleri
        private void btn_newGame_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tlp.Enabled = false;
            Stopwatch s = new Stopwatch();
            s.Start();
            LoadingForm lf = new LoadingForm();
            lf.Show();
            lf.Enabled = false;
           // lf.Refresh();
            game.Computer.TranspTable = TranspositionTable.Deserialize();
            s.Stop();
            if (s.ElapsedMilliseconds < 2000)
                System.Threading.Thread.Sleep(2000);
            lf.Close();
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            DialogResult dlg = MessageBox.Show("Da li ste sigurni da zelite da zatvorite igru?","Zatvaranje igre", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dlg == DialogResult.Cancel)
                e.Cancel = true;
            this.Hide();
            TranspositionTable.Serialize(game.Computer.TranspTable);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            MessageBox.Show("Dobrodosli u igru Connect Four", "Dobrodosli", MessageBoxButtons.OK);
            tlp.Enabled = true;
        }

        #endregion


    }
}

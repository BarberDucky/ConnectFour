using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Data;

namespace ConnectFour1
{
    public class Cell : Button
    {
        int _col;
        MainForm _parent;
        static List<Image> _imageList = new List<Image>();
        static List<Image> _winnerList = new List<Image>();     //ima svetlece kuglice 

        public Cell(int c, MainForm mf) 
            : base()
        {
            _imageList.Add(Resource.red);
            _imageList.Add(Resource.yellow);
            _imageList.Add(Resource.blue);
            _winnerList.Add(Resource.red_glow);
            _winnerList.Add(Resource.yellow_glow);

            _col = c;
            _parent = mf;

            this.Height = 80;
            this.Width = 80;
            this.FlatStyle = FlatStyle.Flat;
            this.Margin = new Padding(0, 0, 0, 0);
            this.FlatAppearance.BorderSize = 0;
            this.FlatAppearance.MouseDownBackColor = this.BackColor;
            this.FlatAppearance.MouseOverBackColor = this.BackColor;
            this.BackgroundImage = _imageList[2];
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.SetStyle(ControlStyles.Selectable, false);
        }

        protected override void OnClick(EventArgs e)
        {
            _parent.CallGame(_col);
        }
        
        //Sluzi za reinicijalizaciju tlp
        public void SetToEmpty()
        {
            this.BackgroundImage = _imageList[2];
        }

        //Postavlja crvenu ili zutu kuglicu na osnovu igraca koji je poslednji igrao
        public void MarkCurrentPlayer(int curPlayerId)
        {
            this.BackgroundImage = _imageList[curPlayerId];
        }

        //Postavlja svetlece kuglice na osn igraca koji je poslednji igrao
        public void MarkWinner(int curPlayerId)
        {
            this.BackgroundImage = _winnerList[curPlayerId];
        }
    }
}

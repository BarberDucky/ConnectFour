using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Move
    {
        private int _row;
        private int _col;
        private int _current;
        private int _score;

#region Property
        public int Row { get => _row; set => _row = value; }
        public int Col { get => _col; set => _col = value; }
        public int Current { get => _current; set => _current = value; }
        public int Score { get => _score; set => _score = value; }
#endregion
       
        public Move(int s)
        {
            _score = s;
        }
        public Move(int c, int score, bool b)
        {
            _col = c;
            _score = score;
        }
        public Move(int r, int c)
        {
            _row = r;
            _col = c;
        }        
    }
}

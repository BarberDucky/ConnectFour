using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public abstract class Player
    {
        protected int id; //0- prvi igrac; 1-drugi igrac

        public int Id
        {
            get { return id; }

        }

        public Player(int i)
        {
            id = i;
        }

    }
}

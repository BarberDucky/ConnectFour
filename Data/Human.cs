using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Human : Player
    {
        public Human(int id) 
            : base(id)
        { }

        public Move Play(Board b,int i)
        {
            if(!b.CantPlay(i))
                return b.MakeMove(i);
            return null;
            
        }
    }
}

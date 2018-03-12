using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public enum GameState
    {
        CrveniWin = 0,
        ZutiWin = 1,
        Nereseno = 2,
        NijeKraj = -1
    }

    public class Game
    {
        Human _human;
        Computer _computer;
        GameState _state;
        Board _board;

        #region Property
        public GameState State { get { return _state; }}

        public Board Tabla { get { return _board; } }

        public Computer Computer { get { return _computer; } set { _computer = value; } }
        #endregion

        public Game()
        {
            _board = new Board();
            _human = new Human(0);
            _computer = new Computer(1);
            _state = GameState.NijeKraj;
           
        }

        public Move HumanPlay(int col)
        {
            Move m=_human.Play(_board,col);
            if (m != null)
            {
                _board.SwitchCurrent();
                m.Current = _human.Id;
                _state = _board.CheckWinner(_human.Id);
               _board.SwitchCurrent();
            }
            return m;
        }

        public Move ComputerPlay()
        {
            Move m = _computer.Play(_board);
            if (m != null)
            {
                _board.SwitchCurrent();
                m.Current = _computer.Id;
                _state =_board.CheckWinner(_computer.Id);
                _board.SwitchCurrent();
            }
            return m;
        }

        //Human ne mora, Comp se ne resetuje zbog transp table
        public void RestartGame()
        {
            _board = new Board();                       
            _state = GameState.NijeKraj;
        }

    }
}

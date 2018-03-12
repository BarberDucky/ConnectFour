using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Computer : Player
    {       
        private int[] moveOrder;
        public static TranspositionTable transpTable;
        private int col = 7; //Da ne bi stalno zvala b.Col

        public TranspositionTable TranspTable
        {
            get { return transpTable; }
            set { transpTable = value; }
        }

        public Computer(int id)
            : base(id)
        {
            moveOrder = new int[col];
            if(transpTable == null)
                transpTable = new TranspositionTable();
            for (int k = 0; k < col;  k++)
                moveOrder[k] = col / 2 + (1 - 2 * (k % 2)) * (k + 1) / 2;
        }

        #region DRUGI ALGORITMI
        //minimax sa alfabeta, prepravila sam ga da koristi IsWinningMove i da nema playera
        //Malo sporije radi od negamax ali cuvamo za svaki slucaj ako se nesto spetljamo oko evaluacije
        public Move GetBestMoveMinimax(Board b,int alfa, int beta, bool computer, int color)
        {
            if (b.Moves == 42)
                return new Move(0);
            for (int i = 0; i < col; i++)
                if (!b.CantPlay(moveOrder[i]) && b.IsWinningMove(moveOrder[i]))
                    return new Move(moveOrder[i], color * ((44 - b.Moves) / 2), true);

            if (computer)//ai move
            {
                Move bestMove = new Move(beta);
                for (int i = 0; i < col; i++)
                {
                    if(!b.CantPlay(moveOrder[i]))
                    {
                        Move m = b.MakeMove(moveOrder[i]);
                        m.Score = GetBestMoveMinimax(b,alfa, beta, false,-color).Score;
                        b.RemoveMove(m);
                        if (m.Score < bestMove.Score)
                        {
                            bestMove = m;
                        }
                        if (beta > bestMove.Score)
                            beta = bestMove.Score;                       
                    }
                    if (alfa >= beta)
                    {
                        break;
                    }
                }
                return bestMove;
            }
            else
            {
                Move bestMove = new Move(alfa);
                for (int i = 0; i < col; i++)
                {
                    if (!b.CantPlay(moveOrder[i]))
                    {
                        Move m = b.MakeMove(moveOrder[i]);
                        m.Score = GetBestMoveMinimax(b, alfa, beta, true, -color).Score;
                        b.RemoveMove(m);
                        if (m.Score > bestMove.Score)
                        {
                            bestMove = m;
                        }
                        if (alfa < bestMove.Score)
                            alfa = bestMove.Score;
                    }
                    if (alfa >= beta)
                    {
                        break;
                    }
                }
                return bestMove;
            }
        }
        #endregion

        //Negamax sa evaluacijom
        Move GetBestMoveNegamaxEval(Board b, int alpha, int beta, int depth)
        {         
            for (int x = 0; x < 7; x++)
                if (!b.CantPlay(moveOrder[x]) && b.IsWinningMove(moveOrder[x]))
                    return new Move(moveOrder[x], (10000 - b.Moves)/2, true);
            
            if (depth == 0)           
            {
               return new Move(b.EvaluateThreats());
            }

            int maxScore = (9998 - b.Moves)/2;
            TableData  entry = transpTable.Return(b.Current, b.Mask);
            if(entry != null)
            {
                int val = Convert.ToInt32(entry.value);
                if (val < -3000 || val > 3000 || Convert.ToInt32(entry.depth) >= depth)
                {
                    maxScore = val;
                }
            }

            if (beta > maxScore)
            {
                beta = maxScore;

                if (alpha >= beta)
                {
                    return new Move(beta);
                }
            }

            Move bestMove = new Move(alpha);
            for (int x = 0; x < 7; x++)
            {            
                if (!b.CantPlay(moveOrder[x]))
                {
                    Move m = b.MakeMove(moveOrder[x]);
                    m.Score = -GetBestMoveNegamaxEval(b, -beta, -alpha, depth-1).Score;
                    b.RemoveMove(m);
                    if (m.Score >= beta)
                    {
                        return m;
                    }
                    if (m.Score > bestMove.Score)
                    {
                        alpha = m.Score;
                        bestMove = m;
                    }
                }
            }
            transpTable.Add(b.Current, b.Mask, Convert.ToInt16(bestMove.Score), Convert.ToByte(depth));       //ovi castovi su safe za opsege koje koristimo             
            return bestMove;
        }

        //Negamax sa ab, ovaj algoritam koristimo

        Move GetBestMoveNegamax(Board b, int alpha, int beta)
        {
            if (b.Moves == 42) // check for draw game
                return new Move(3001);                                                                  

            for (int x = 0; x < 7; x++)
                if (!b.CantPlay(moveOrder[x]) && b.IsWinningMove(moveOrder[x]))
                    return new Move(moveOrder[x], (10000 - b.Moves) / 2, true);

            int maxScore = (9998 - b.Moves) / 2;
            TableData entry = transpTable.Return(b.Current, b.Mask);
            if(entry != null)
            {
                int val = Convert.ToInt32(entry.value);
                if (val > 3000 || val < -2500)                             //nereseno je 3001
                {
                    maxScore = Convert.ToInt32(entry.value);
                }
            }
           
            if (beta > maxScore)
            {
                beta = maxScore;
                if (alpha == -3001)
                    alpha = 3001;
                if (alpha >= beta)
                    return new Move(beta);
            }

            Move bestMove = new Move(alpha);
            for (int x = 0; x < 7; x++)
            {
                if (!b.CantPlay(moveOrder[x]))
                {
                    Move m = b.MakeMove(moveOrder[x]);
                    m.Score = -GetBestMoveNegamax(b, -beta, -alpha).Score;
                    b.RemoveMove(m);

                    if (m.Score == -3001)
                        m.Score = 3001;
                    
                    if (m.Score >= beta)
                        return m;
                    if (m.Score > bestMove.Score)
                    {
                        alpha = m.Score;
                        bestMove = m;
                    }
                }
            }
            transpTable.Add(b.Current, b.Mask, Convert.ToInt16(bestMove.Score)); 
            return bestMove;
        }

        //Pravi potez
        public Move Play(Board b)
        {
            Move m;
            if (b.Moves < 14)
                m = GetBestMoveNegamaxEval(b, -1000000, 1000000, 10);
            else
                m = GetBestMoveNegamax(b, -1000000, 1000000);
            return b.MakeMove(m.Col);
        }

    }
}

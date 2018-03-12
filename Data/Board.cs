using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Data
{
    public class Board
    {
        const int row = 6;
        const int col = 7;

        static int[,] _matrixScores = new int[,]
            {
                {3,4,5,7,5,4,3},
                {4,6,8,9,8,6,4},
                {5,8,11,13,11,8,5},
                {5,8,11,13,11,8,5},
                {4,6,8,9,8,6,4},
                {3,4,5,7,5,4,3},
            };

        UInt64 current;
        UInt64 mask;
        int moves;

        int[,] _matrix;
        int[] _heightArray; // visine kolona

        #region Property
        public int Moves
        {
            get { return moves; }
        }

        public UInt64 Current
        {
            get { return current; }
        }

        public UInt64 Mask
        {
            get { return mask; }
        }
        #endregion

        public Board() {

            _matrix = new int[row, col];
            _heightArray = new int[col];
            InitBoard();                 
        }

        public void InitBoard()
        {
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                    _matrix[i, j] = -1;
            for (int i = 0; i < col; i++)
            {
                _heightArray[i] = row - 1;
            }
            moves = 0;
            current = 0;
            mask = 0;
        }

        public int EvaluateGreedy()
        {
            int score = 0;
            int current = moves % 2;
            int opposite = (moves + 1) % 2; 
            for(int j=0;j<col;j++)
            {
                if (_heightArray[j] < row - 1)
                {
                    for(int i=_heightArray[j]+1;i<row;i++)
                    {
                        if (_matrix[i, j] == current)
                            score += _matrixScores[i, j];
                        if (_matrix[i, j] == opposite)
                            score -= _matrixScores[i, j];
                    }
                }
            }

            return score;
       
        }

        //checkwinner za pretnje
        public bool CheckWinnerEval(UInt64 Board, UInt64 Move)
        {
            UInt64 pos = Board | Move;

            UInt64 m = pos & (pos >> (row + 1));
            if ((m & (m >> (2 * (row + 1)))) != 0)
                return true;

            m = pos & (pos >> row);
            if ((m & (m >> (2 * row))) != 0) return true;

            m = pos & (pos >> (row + 2));
            if ((m & (m >> (2 * (row + 2)))) != 0) return true;

            m = pos & (pos >> 1);
            if ((m & (m >> 2)) != 0) return true;

            return false;
        }

        public UInt64 EliminatorMask()
        {

            UInt64 eliminator = mask;
            eliminator |= mask >> 7;
            eliminator |= eliminator << 1;
            eliminator |= eliminator << 7;
            return eliminator;
        }

        public int EvaluateThreats()
        {

            int P1Odd, P1Even, P2Odd, P2Even, Mixed, Odd, curr;
            bool parnost;
            bool ignoreEvenP1, ignoreEvenP2, ignoreOddP1, ignoreOddP2;
            UInt64 P1, P2, p, eliminator;
            //setovanje bitmapa za oba igraca u zavisnosti od trenutnog poteza
            if (moves % 2 == 0)
            {
                P1 = current;
                P2 = current ^ mask;
                curr = 1;
            }
            else
            {
                P1 = current ^ mask;
                P2 = current;
                curr = -1;
            }
            //p: 64bitni int koji se sastoji od bitskog niza nula i jedinice koja na pocetku predstavlja donje levo polje u matrici 
            //i zatim se shiftuje, sto modelira prolazak kroz matricu
            //u svakoj iteraciji p ima bit 1 na mestu u matrici za koje treba da se proveri pretnja
            p = 1;
            eliminator = EliminatorMask();
            eliminator = mask | (~eliminator);
            //eliminator = mask;
            P1Odd = P1Even = P2Odd = P2Even = Mixed = 0;

            for (int i = 0; i < 7; i++)
            {
                if (_heightArray[i] > -1)
                {
                    ignoreEvenP1 = ignoreEvenP2 = ignoreOddP1 = ignoreOddP2 = false;
                    for (int j = 0; j < 6; j++)
                    {
                        
                        //proverava da li je posmatrano polje (jedinica u p) slobodno
                        if ((eliminator & p) == 0)
                        {
                            //racuna da li je red paran
                            parnost = ((j + 1) % 2) == 0;
                            //proverava da li se pretnja za trenutni red vazi
                            if (((parnost && !ignoreEvenP1) || (!parnost && !ignoreOddP1)))
                            {
                                if (CheckWinnerEval(P1, p))
                                {
                                    if (!parnost)
                                    {
                                        ignoreEvenP2 = true;
                                        P1Odd++;
                                    }
                                    else
                                    {
                                        ignoreOddP2 = true;
                                        P1Even++;
                                    }
                                }
                            }
                            //isto to za drugog igraca
                            if (((parnost && !ignoreEvenP2) || (!parnost && !ignoreOddP2)))
                            {
                                if (CheckWinnerEval(P2, p))
                                {
                                    if (!parnost)
                                    {
                                        ignoreEvenP1 = true;
                                        P2Odd++;
                                    }
                                    else
                                    {
                                        ignoreOddP1 = true;
                                        P2Even++;
                                    }
                                }
                            }
                        }
                        p = p << 1;
                    }
                    //ako su oba ignoreEven-a true, znaci da postoji Odd threat za oba igraca, pa se povecava mixed
                    if (ignoreEvenP1 && ignoreEvenP2)
                        Mixed++;

                    //ovo dodatno pomeranje se vrsi zbog dodatne vrste na vrhu table koja regulise njenu popunjenost
                    p = p << 1;
                }
                else
                    p = p << 7;

            }


            Odd = P2Odd - P1Odd;

            //Console.WriteLine(Odd + " " + Mixed + " " + P2Even + " " + P1Even);

            int greedyScore = EvaluateGreedy();
            if (P2Even == 0 && P2Odd == 0 && P1Odd == 0)
                return greedyScore;
            //deo koji daje poene na osnovu izracunatog
            if (Odd < 0)
            {
                // Console.WriteLine("1 wins");
                return curr * 1000 + greedyScore;
            }
            if (Odd == 0)
            {
                if (Mixed % 2 == 1)
                {
                    //Console.WriteLine("1 wins");
                    return curr * 1000 + greedyScore;
                }
                else
                {
                    if (Mixed == 0)
                    {
                        if (P2Even == 0)
                        {
                            // Console.WriteLine("draw");
                            return greedyScore;
                        }
                        else
                        {
                            // Console.WriteLine("2 wins");
                            return -1000 * curr + greedyScore;
                        }
                    }
                    else
                    {
                        //Console.WriteLine("2 wins");
                        return -1000 * curr + greedyScore;
                    }
                }
            }
            if (Odd == 1)
            {
                if (Mixed == 0)
                {
                    if (P2Even == 0)
                    {
                        //Console.WriteLine("draw");
                        return greedyScore;
                    }
                    else
                    {
                        //Console.WriteLine("2 wins");
                        return -1000 * curr + greedyScore;
                    }
                }
                else
                {
                    if (Mixed % 2 == 1)
                    {
                        //Console.WriteLine("2 wins");
                        return -1000 * curr + greedyScore;
                    }
                    else
                    {
                        // Console.WriteLine("1 wins");
                        return curr * 1000 + greedyScore;
                    }
                }
            }
            if (Odd > 1)
            {
                // Console.WriteLine("2 wins");
                return -1000 * curr + greedyScore;
            }
            return 0;
        }


        // vraca true ako ne moze da se igra vise u toj koloni
        public bool CantPlay(int col) {
            return (mask & TopMask(col)) != 0; 
        }

        //Da li je potez koji bi se sledeci odigrao pobednicki
        //Koristi AI
        public bool IsWinningMove(int col) {
            UInt64 pos = current;
            pos |= (mask + BottomMask(col)) & ColumnMask(col);

            UInt64 m = pos & (pos >> (row + 1));
            if ((m & (m >> (2 * (row + 1)))) != 0) return true;

            m = pos & (pos >> row);
            if ((m & (m >> (2 * row))) != 0) return true;

            m = pos & (pos >> (row + 2));
            if ((m & (m >> (2 * (row + 2)))) != 0) return true;

            m = pos & (pos >> 1);
            if ((m & (m >> 2)) != 0) return true;

            return false;
        }

        //Pravi potez, ne koristi Player 
        public Move MakeMove(int index) {
            current ^= mask;
            UInt64 b = BottomMask(index);
            mask |= mask + b;
            _matrix[_heightArray[index], index] = moves%2;
            moves++;
            _heightArray[index]--;
            return new Move(_heightArray[index] + 1, index);
        }
        
        //Proverava da li je doslo do pobede a ne koristi Playera
        //Klasican ChackWinner
        public GameState CheckWinner(int id) {
            
            UInt64 pos = current;

            UInt64 m = pos & (pos >> (row + 1));
            if ((m & (m >> (2 * (row + 1)))) != 0)
                return (GameState)id;

            m = pos & (pos >> row);
            if ((m & (m >> (2 * row))) != 0) return (GameState)id;
            
            m = pos & (pos >> (row + 2));
            if ((m & (m >> (2 * (row + 2)))) != 0) return (GameState)id;
            
            m = pos & (pos >> 1);
            if ((m & (m >> 2)) != 0) return (GameState)id;

            if (moves == 42)
                return GameState.Nereseno;

            return GameState.NijeKraj;

        }

        //Skida potez sa table
        public void RemoveMove(Move m) {
            mask &= mask - GetAt(m.Row, m.Col);
            current &= mask;
            current ^= mask;
            _heightArray[m.Col]++;
            _matrix[m.Row, m.Col] = -1;
            moves--;
        }

  
        private UInt64 TopMask(int col) {
            return ((UInt64.MinValue + 1) << (row - 1)) << col * (row + 1);
        }

       
        private UInt64 BottomMask(int col) {
            return (UInt64.MinValue + 1) << col * (row + 1);
        }

       
        private UInt64 ColumnMask(int col) {
            return (((UInt64.MinValue + 1) << row) - 1) << col * (row + 1);
        }

        public void SwitchCurrent() {
            current ^= mask;
        }

     
        private UInt64 GetAt(int x, int y) {
            return ((UInt64.MinValue + 1) << (row - x - 1)) << y * (row + 1);
        }

       
        public List<Move> CheckFourInARow(Move move)
        {
            int m = move.Row;
            int n = move.Col;
            int stone = move.Current;  //zeton
            List<Move> listaPoteza = new List<Move>();
            
            //PROVERA KOLONE
            int buffer = 0;
            int i = m;
            while (i < row && buffer < 4)
            {
                if (_matrix[i, n] == stone)
                {
                    listaPoteza.Add(new Move(i, n));
                    buffer++;
                }
                else
                {
                    buffer = 0;
                    listaPoteza.Clear();
                }
                i++;
            }
            if (buffer == 4)
            {
                return listaPoteza;
            }
            listaPoteza.Clear();
            // PROVERA REDA
            buffer = 0;
            i = 0;
            while (i < col && buffer < 4)
            {
                if (_matrix[m, i] == stone)
                {
                    listaPoteza.Add(new Move(m,i));
                    buffer++;
                }
                else
                {
                    buffer = 0;
                    listaPoteza.Clear();
                }
                    
                i++;
            }
            if (buffer == 4)
            {
                return listaPoteza;
            }
            listaPoteza.Clear();
                
            //PROVERA OPADAJUCE DIJAGONALE

            int mUp = m - Math.Min(m, (col - n - 1));
            int nUp = n + Math.Min(m, (col - n - 1));

            i = 0;
            buffer = 0;
            while (i < Math.Min(row - mUp, nUp + 1) && buffer < 4)
            {
                if (_matrix[mUp + i, nUp - i] == stone)
                {
                    listaPoteza.Add(new Move(mUp + i, nUp - i));
                    buffer++;
                }
                else
                {
                    listaPoteza.Clear();
                    buffer = 0;
                }
                i++;
            }

            if (buffer == 4)
                return listaPoteza;
            listaPoteza.Clear();
            //PROVERA RASTUCE DIJAGONALE

            int mDown = m - Math.Min(m, n);
            int nDown = n - Math.Min(m, n);

            i = 0;
            buffer = 0;
            while (i < Math.Min(row - mDown, col - nDown) && buffer < 4)
            {
                if (_matrix[mDown + i, nDown + i] == stone)
                {
                    listaPoteza.Add(new Move(mDown + i, nDown + i));
                    buffer++;
                }
                else
                {
                    listaPoteza.Clear();
                    buffer = 0;
                }
                i++;
            }

            if (buffer == 4)
                return listaPoteza;

            return null;
        }

    }
}

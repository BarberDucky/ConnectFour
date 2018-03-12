using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Data
{
    public class TableData
    {

        public UInt64 current;
        public UInt64 mask;
        public short value;
        public byte depth;                 

        public TableData(UInt64 c, UInt64 m, short v, byte d)
        {
            current = c;
            mask = m;
            value = v;
            depth = d;
        }

    }

    public class TranspositionTable
    {
        const int size = 8388593;
        
        public TableData[] table;

        public TranspositionTable()
        {
            table = new TableData[size];
        }

        public void Add(UInt64 curr, UInt64 mask, short value, byte depth)
        {
            ulong ind = ((curr + mask) % size);
            if ((table[ind] != null && (table[ind].value < 3000 && table[ind].value > -2500)) || table[ind] == null)
                table[ind] = new TableData(curr, mask, value, depth);
        }

        public void Add(UInt64 curr, UInt64 mask, short value)
        {
            ulong ind = ((curr + mask) % size);
                table[ind] = new TableData(curr, mask, value, 0);
        }

        public TableData Return(UInt64 curr, UInt64 mask)
        {
            TableData td = table[(curr + mask) % size];
            if (td != null && td.current == curr && td.mask == mask)
                return td;
            else
                return null;
        }

        //public void ResetTable()
        //{
        //    Array.Clear(table, 0, size);
        //}

        public static void Serialize(TranspositionTable tabla) 
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream("table.bin", FileMode.Create)))
                {
                    TableData td;

                    for (int i = 0; i < size; i++)
                    {
                        td = tabla.table[i];
                        if (td != null)
                        {
                            bw.Write(td.current);
                            bw.Write(td.mask);
                            bw.Write(td.value);
                            bw.Write(td.depth);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static TranspositionTable Deserialize()   
        {
            TranspositionTable newTable = new TranspositionTable();
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream("table.bin", FileMode.Open)))
                {
                    while (true)  //posto ne znamo koliko ce puta da procita, kada dodje do kraja streama, baci exception EndOfStream
                    {
                        newTable.Add(br.ReadUInt64(), br.ReadUInt64(), br.ReadInt16(), br.ReadByte());
                    } 
                }
            }
            catch (EndOfStreamException e)
            {
                Console.WriteLine(e.Message);
                return newTable;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return new TranspositionTable();
            }
        }
    }
}







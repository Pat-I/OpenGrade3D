using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGrade
{
    public class CUDPComm
    {
        public byte CRC(byte[] Data, int Length, byte Start = 0)
        {
            byte Result = 0;
            if (Length <= Data.Length)
            {
                int CK = 0;
                for (int i = Start; i < Length; i++)
                {
                    CK += Data[i];
                }
                Result = (byte)CK;
            }
            return Result;
        }

        public bool GoodCRC(byte[] Data, byte Start = 0)
        {
            bool Result = false;
            int Length = Data.Length;
            byte cr = CRC(Data, Length - 1, Start);
            Result = cr == Data[Length - 1];
            return Result;
        }

        public bool GoodCRC(string[] Data, byte Start = 0)
        {
            bool Result = false;
            byte tmp;
            int Length = Data.Length;
            byte[] BD = new byte[Length];
            for (int i = 0; i < Length; i++)
            {
                if (byte.TryParse(Data[i], out tmp)) BD[i] = tmp;
            }
            byte cr = CRC(BD, Length - 1, Start);   // exclude existing crc
            Result = cr == BD[Length - 1];
            return Result;
        }



    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpUtils.Extensions;

namespace CSharpUtils.Compression.Lz
{
    public class LzBuffer
    {
        public int Position;
        public int GlobalMinSearchSize;
        List<byte> Data = new List<byte>();
        Dictionary<int, List<int>> Waypoints = new Dictionary<int, List<int>>();

        public LzBuffer(int GlobalMinSearchSize = 1)
        {
            this.GlobalMinSearchSize = GlobalMinSearchSize;
            this.Position = 0;
        }

        public void AddBytes(byte[] Bytes)
        {
            /*
            Data.AddRange(Bytes);
            Position = (uint)Data.Count;
            */
            foreach (var Byte in Bytes)
            {
                AddByte(Byte);
            }
        }

        public int Size
        {
            get
            {
                return Data.Count;
            }
        }

        int GetOffsetHash(int Offset)
        {
            int HashData = 0;
            for (int n = 0; n < GlobalMinSearchSize; n++)
            {
                HashData |= this.Data[Offset] << (n * 8);
            }
            return HashData;
        }

        public void AddByte(byte Byte)
        {
            
            Data.Add(Byte);
            Position = Size;
            if (Size >= GlobalMinSearchSize)
            {
                var Hash = GetOffsetHash(Size - GlobalMinSearchSize);
                if (!Waypoints.ContainsKey(Hash))
                {
                    Waypoints[Hash] = new List<int>();
                }
                Waypoints[Hash].Add(Data.Count - GlobalMinSearchSize);
            }
        }

        public struct FindSequenceResult
        {
            public bool Found
            {
                get
                {
                    return Size > 0;
                }
            }
            public int Offset;
            public int Size;

            public override string ToString()
            {
                return String.Format("LzBuffer.FindSequenceResult(Offset={0}, Size={1})", Offset, Size);
            }
        }

        public FindSequenceResult FindMaxSequence(int FindOffset, int MaxSearchOffset, int MaxDistanceFromSearchOffset = 0x1000, int MinSearchSize = 3, int MaxSearchSize = 16, bool AllowOverlapping = true)
        {
            var FindSequenceResult = new FindSequenceResult()
            {
                Offset = 0,
                Size = 0,
            };
            MaxSearchSize = Math.Min(MaxSearchSize, Size - FindOffset);

            if (MinSearchSize < this.GlobalMinSearchSize)
            {
                throw(new Exception("Local MinSearchSize can't be lower than the GlobalMinSearchSize"));
            }

            if (MaxSearchSize < MinSearchSize)
            {
                return FindSequenceResult;
            }

            if ((Size - FindOffset) < MinSearchSize)
            {
                return FindSequenceResult;
            }

            var Hash = GetOffsetHash(FindOffset);

            if (Waypoints.ContainsKey(Hash))
            {
                FindSequenceResult.Size = 0;
                //foreach (var Index in Waypoints[Hash].Where((int Index) => (Index < MaxSearchOffset)))
                foreach (var Index in Waypoints[Hash].LowerBound(MaxSearchOffset, false))
                {
                    if (Index < (MaxSearchOffset - MaxDistanceFromSearchOffset))
                    {
                        continue;
                    }

                    int SequenceCount = 0;
                    for (SequenceCount = 0; SequenceCount < MaxSearchSize; SequenceCount++)
                    {
                        //Console.WriteLine(this.Data[Index + SequenceCount] + "," + this.Data[FindOffset + SequenceCount]);
                        if (this.Data[Index + SequenceCount] != this.Data[FindOffset + SequenceCount]) break;
                    }

                    if ((SequenceCount >= MinSearchSize) && SequenceCount >= (FindSequenceResult.Size))
                    {
                        FindSequenceResult.Size = SequenceCount;
                        FindSequenceResult.Offset = Index;
                    }
                }
            }

            return FindSequenceResult;
        }
    }
}

using System;
using System.Diagnostics;

namespace DadanBinHeap
{

    public class TheBlock
    {
        public int idx;
        public int gCost;
        public int hCost;
        public TheBlock(int gVal, int hVal)
        {
            gCost = gVal;
            hCost = hVal;
        }
    }

    public class PFBinHeapLoopMode<T> where T : TheBlock
    {
        public T[] Blocks;

        public int mbrCount { get; private set; }

        public PFBinHeapLoopMode(int size)
        {
            Blocks = new T[size];
        }

        public void Add(T member)
        {
            member.idx = mbrCount;
            Blocks[mbrCount] = member;
            BeParent(mbrCount);
            mbrCount++;
        }

        /// <summary>
        /// Apply Modification
        /// </summary>
        public void ApplyMod(T block)
        {
            //Blocks[block.idx] = block;
            BeParent(block.idx);
        }

        public T PopPriority()
        {
            T result = Blocks[0];
            if (mbrCount < 1) return null;
            Blocks[0] = Blocks[mbrCount - 1];
            BeChild(0);
            mbrCount--;
            return result;
        }

        void BeParent(int idx)
        {
            while (idx > 0)
            {
                int currParent = ((idx + 1) >> 1) - 1;
                if (ComFunc(Blocks[idx], Blocks[currParent]))
                {
                    Exchange(ref idx, currParent);
                }
                else
                    break;
            }
        }

        void BeChild(int idx)
        {

            while (true)
            {
                int RN = (idx + 1) << 1, LN = RN - 1;
                if (RN < mbrCount && ComFunc(Blocks[RN], Blocks[idx]) && ComFunc(Blocks[RN], Blocks[LN]))
                {
                    Exchange(ref idx, RN);
                }
                else if (LN < mbrCount && ComFunc(Blocks[LN], Blocks[idx]))
                {
                    Exchange(ref idx, LN);
                }
                else break;
            }
        }

        /// <summary>
        /// Exchange "from" content to "to" content
        /// </summary>
        void Exchange(ref int from, int to)
        {
            T curBlock = Blocks[from];
            Blocks[from] = Blocks[to];
            Blocks[from].idx = from;
            curBlock.idx = from = to;
            Blocks[to] = curBlock;
        }

        /// <summary>
        /// Is "a" smaller than "b"
        /// </summary>
        bool ComFunc(TheBlock a, TheBlock b)
        {
            int aFcost = a.gCost + a.hCost, bFcost = b.gCost + b.hCost;
            return aFcost < bFcost || (aFcost == bFcost && a.hCost < b.hCost);
        }
    }

    //Recursive Mode
    public class PFBinHeapRecMode<T> where T : TheBlock
    {
        public T[] Blocks;
        //private T[] Blocks;

        public int mbrCount { get; private set; }

        public PFBinHeapRecMode(int size)//(T[] blocks)
        {
            //Blocks = blocks;
            Blocks = new T[size];
        }

        public void Add(T member)
        {
            member.idx = mbrCount;
            Blocks[mbrCount] = member;
            BeParent(mbrCount);
            mbrCount++;
        }

        /// <summary>
        /// Apply Modification
        /// </summary>
        public void ApplyMod(T block)
        {
            //Blocks[block.idx] = block;// ??
            BeParent(block.idx);
        }

        public T PopPriority()
        {
            T result = Blocks[0];
            if (mbrCount < 1) return null;
            Blocks[0] = Blocks[mbrCount - 1];
            BeChild(0);
            mbrCount--;
            return result;
        }

        void BeParent(int idx)
        {
            if (idx <= 0) return;
            int currParent = ((idx + 1) >> 1) - 1;
            if (ComFunc(Blocks[idx], Blocks[currParent]))
            {
                Exchange(ref idx, currParent);
                BeParent(idx);
            }
        }

        void BeChild(int idx)
        {
            int RN = (idx + 1) << 1, LN = RN - 1;
            if (RN < mbrCount && ComFunc(Blocks[RN], Blocks[idx]) && ComFunc(Blocks[RN], Blocks[LN]))
            {
                Exchange(ref idx, RN);
                BeChild(idx);
            }
            else if (LN < mbrCount && ComFunc(Blocks[LN], Blocks[idx]))
            {
                Exchange(ref idx, LN);
                BeChild(idx);
            }
        }
        /// <summary>
        /// Exchange "from" content to "to" content
        /// </summary>
        void Exchange(ref int from, int to)
        {
            T curBlock = Blocks[from];
            Blocks[from] = Blocks[to];
            Blocks[from].idx = from;
            curBlock.idx = from = to;
            Blocks[to] = curBlock;
        }

        /// <summary>
        /// Is "a" smaller than "b"
        /// </summary>
        bool ComFunc(TheBlock a, TheBlock b)
        {
            int aFcost = a.gCost + a.hCost, bFcost = b.gCost + b.hCost;
            return aFcost < bFcost || (aFcost == bFcost && a.hCost < b.hCost);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int arrSize = 2000000;
            Console.WriteLine("\nExtract and Updating test uses " + arrSize + " array data");
            Console.WriteLine("\n Press enter to start");

            while (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                int seed = new Random().Next();
                //Loop Mode test Speed
                {
                    var random = new Random(seed);
                    int rand;
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    var heap = new PFBinHeapLoopMode<TheBlock>(arrSize);
                    for (int i = 0; i < heap.Blocks.Length; i++)
                    {
                        rand = random.Next();
                        heap.Add(new TheBlock(rand, random.Next()));
                    }

                    //Pop, Remove and update test
                    while (heap.mbrCount > 0)
                    {
                        TheBlock currBlock = heap.PopPriority();
                        if (heap.mbrCount > 0)
                        {
                            TheBlock block = heap.Blocks[random.Next(heap.mbrCount)];
                            int newgCost = block.gCost - random.Next(10);
                            if (newgCost < block.gCost)
                            {
                                block.gCost = newgCost;
                                heap.ApplyMod(block);
                            }
                        }
                    }
                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;

                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}",
                        ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    Console.WriteLine("This Heap Loop Mode      = " + elapsedTime);
                }
                //Recursive Mode test Speed
                {
                    var random = new Random(12);//new Random(seed);
                    int rand;
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    var heap = new PFBinHeapRecMode<TheBlock>(arrSize);
                    for (int i = 0; i < heap.Blocks.Length; i++)
                    {
                        rand = random.Next();
                        heap.Add(new TheBlock(rand, random.Next()));
                    }

                    //Pop, Remove and update test
                    while (heap.mbrCount > 0)
                    {
                        TheBlock currBlock = heap.PopPriority();
                        if (heap.mbrCount > 0)
                        {
                            TheBlock block = heap.Blocks[random.Next(heap.mbrCount)];
                            int newgCost = block.gCost - random.Next(10);
                            if (newgCost < block.gCost)
                            {
                                block.gCost = newgCost;
                                heap.ApplyMod(block);
                            }
                        }
                    }
                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;

                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}",
                        ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    Console.WriteLine("This Heap Recursive Mode = " + elapsedTime);
                }
                Console.WriteLine("\n Press enter to repeat\n");
            }
        }
    }
}

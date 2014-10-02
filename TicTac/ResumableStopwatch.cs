using System;
using System.Diagnostics;

namespace TicTac
{
    public class ResumableStopwatch : Stopwatch
    {
        public TimeSpan StartOffset { get; private set; }


        public ResumableStopwatch(TimeSpan startOffset)
        {
            StartOffset = startOffset;
        }

        public ResumableStopwatch()
        {
            StartOffset = TimeSpan.Zero;
        }

        public new TimeSpan Elapsed
        {
            get
            {
                return base.Elapsed + this.StartOffset;
            }
        }

        public new long ElapsedMilliseconds
        {
            get
            {
                return base.ElapsedMilliseconds + (long)StartOffset.TotalMilliseconds;
            }
        }

        public new long ElapsedTicks
        {
            get
            {
                return base.ElapsedTicks + StartOffset.Ticks;
            }
        }
    }
}

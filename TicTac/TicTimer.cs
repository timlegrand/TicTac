using System;
using System.Timers;


namespace TicTac
{
    class TicTimer
    {
        private Timer _timer;
        private ResumableStopwatch _stopWatch;
        private ElapsedEventHandler _callback;

        public TicTimer(ElapsedEventHandler callback)
        {
            _callback = callback;

            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.AutoReset = true; //Stops it from repeating
            _timer.Start();
        }


        public TimeSpan Elapsed
        {
            get
            {
                return _stopWatch.Elapsed;
            }
        }

        internal void Start()
        {
            _stopWatch = new ResumableStopwatch();
            _stopWatch.Start();
            _timer.Elapsed += new ElapsedEventHandler(_callback);
        }

        internal void Stop()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();
            _timer.Elapsed -= new ElapsedEventHandler(_callback);
        }

        internal void Resume(TimeSpan startOffset)
        {
            _stopWatch = new ResumableStopwatch(startOffset);
            _stopWatch.Start();
            _timer.Elapsed += new ElapsedEventHandler(_callback);
        }
    }
}

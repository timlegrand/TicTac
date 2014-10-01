using System;
using System.Timers;


namespace TicTac
{
    class TicTimer
    {
        private Timer _timer;
        private ResumableStopwatch _stopWatch;
        private ElapsedEventHandler _callback;
        private bool _fireAtLaunch;

        public TicTimer()
        {
            _stopWatch = new ResumableStopwatch();
            _callback = null;
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.AutoReset = true; //Stops it from repeating
        }
        
        public TicTimer(ElapsedEventHandler callback)
            : this()
        {
            _callback = callback;
        }

        public TicTimer(ElapsedEventHandler callback, bool fireAtLaunch)
            : this(callback)
        {
            _fireAtLaunch = fireAtLaunch;
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
            if (_callback != null)
            {
                if (_fireAtLaunch == true)
                {
                    _callback(this, null);
                }
                _timer.Elapsed += new ElapsedEventHandler(_callback);
            }
            _timer.Start();
            _stopWatch.Start();
        }

        internal void Stop()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();
            if (_callback != null)
            {
                _timer.Elapsed -= new ElapsedEventHandler(_callback);
            }
        }

        internal void Resume(TimeSpan startOffset)
        {
            _stopWatch = new ResumableStopwatch(startOffset);
            _stopWatch.Start();
            if (_callback != null)
            {
                _timer.Elapsed += new ElapsedEventHandler(_callback);
            }
        }
    }
}

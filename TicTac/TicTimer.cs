using System;
using System.Timers;

/// <summary>
/// Timer class that fires a callback every tick and keeps elapsed time.</summary>
/// <remarks>
/// Supports callback firing right on start (no wait for first trigger).
/// Supports standard Pause and Resume.
/// Supports long term resume (start with a given offset).</remarks>
namespace TicTac
{
    class TicTimer
    {
        private Timer _timer;
        private ResumableStopwatch _stopWatch;
        private ElapsedEventHandler _callback;
        private bool _fireAtLaunch = false;

        /// <summary>
        /// The main class constructor.</summary>
        /// <remarks>
        /// Cannot be called.</remarks>
        private TicTimer()
        {
            _stopWatch = new ResumableStopwatch();
            _callback = null;
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.AutoReset = true;
        }

        /// <summary>
        /// The class constructor.</summary>
        /// <param name="callback">
        /// Function to call every timer's tick.</param>
        public TicTimer(ElapsedEventHandler callback)
            : this()
        {
            _callback = callback;
        }

        /// <summary>
        /// Overloaded class constructor.</summary>
        /// <param name="callback">
        /// Function to call every timer's tick.</param>
        /// <param name="interval">
        /// Period of timer's ticks.</param>
        public TicTimer(ElapsedEventHandler callback, int interval)
            : this(callback)
        {
            _timer.Interval = interval;
        }

        /// <summary>
        /// Overloaded class constructor.</summary>
        /// <param name="callback">
        /// Function to call every timer's tick.</param>
        /// <param name="interval">
        /// Period of timer's ticks.</param>
        /// <param name="fireAtLaunch">
        /// Enables Fire At Launch. When set to true, prevents from
        /// waiting the first tick (a whole period from the start)
        /// to call the callback function.</param>
        public TicTimer(ElapsedEventHandler callback, int interval, bool fireAtLaunch)
            : this(callback, interval)
        {
            _fireAtLaunch = fireAtLaunch;
        }

        /// <summary>
        /// Elapsed time property. Returns elapsed time
        /// from the first Start (pauses excluded).</summary>
        public TimeSpan Elapsed
        {
            get
            {
                return _stopWatch.Elapsed;
            }
        }

        /// <summary>
        /// Starts the watch. Starts counting elapsed time.</summary>
        public void Start()
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

        /// <summary>
        /// Starts the watch. Starts counting elapsed time from
        /// a given value, just as if it was paused.</summary>
        public void Start(TimeSpan startOffset)
        {
            _stopWatch = new ResumableStopwatch(startOffset);
            this.Start();
        }

        /// <summary>
        /// Stops the watch. Resets elapsed time to zero.</summary>
        public void Stop()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();
            if (_callback != null)
            {
                _timer.Elapsed -= new ElapsedEventHandler(_callback);
            }
        }

        /// <summary>
        /// Pauses the watch. Stops counting elapsed time but keeps
        /// value for later resume, if any.</summary>
        /// <remarks>Keeps triggering the callback.</remarks>
        public void Pause()
        {
            _stopWatch.Stop();
        }

        /// <summary>
        /// Restarts the watch. Resumes counting elapsed time from
        /// last value (before pause).</summary>
        public void Resume()
        {
            _stopWatch.Start();
        }
    }
}

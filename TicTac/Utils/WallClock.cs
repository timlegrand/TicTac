using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TicTac
{
    class WallClock
    {
        private Stopwatch stopWatch;
        private TimeSpan lastTimeStamp;
        private List<WallClockEntry> entries;
        private uint unnamedProbes;

        public WallClock()
        {
            this.unnamedProbes = 0;
            this.entries = new List<WallClockEntry>();
            this.stopWatch = new System.Diagnostics.Stopwatch();
            this.stopWatch.Start();
            this.lastTimeStamp = this.stopWatch.Elapsed;
        }

        public void Probe()
        {
            Probe(null);
        }

        public void Probe(string location)
        {
            var now = this.stopWatch.Elapsed;
            var duration = now - this.lastTimeStamp;
            this.lastTimeStamp = now;

            var e = new WallClockEntry()
            {
                Duration = duration,
                Time = now,
                Location = (location != null && location != String.Empty) ? location : "#" + ++this.unnamedProbes
            };

            entries.Add(e);
        }

        public void Print()
        {
            this.stopWatch.Stop();

            foreach (WallClockEntry entry in this.entries)
            {
                Console.WriteLine(entry.ToString());
            }            
        }
    }

    [Serializable()]
    class WallClockEntry
    {
        public TimeSpan Duration { get; set; } // Timespan between previous registered event (or cloc start if any)
        public TimeSpan Time { get; set; } // Wallclock since clock start
        public string Location { get; set; }

        public override string ToString()
        {
            string duration = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", Duration.Hours, Duration.Minutes, Duration.Seconds, Duration.Milliseconds);
            string timeStamp = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", Time.Hours, Time.Minutes, Time.Seconds, Time.Milliseconds);

            return "[" + timeStamp + "](" + duration + ") => "+ this.Location;
        }
    }
}

using System;
using System.ComponentModel;


namespace TicTac
{
    public class AnimatedBackgroundWorker : BackgroundWorker
    {
        public AnimatedBackgroundWorker(
            DoWorkEventHandler ShowBusyAnimation,
            DoWorkEventHandler LongOperation,
            RunWorkerCompletedEventHandler HideBusyAnimation)
            : base()
        {
            this.DoWork += ShowBusyAnimation;
            this.DoWork += LongOperation;
            this.RunWorkerCompleted += HideBusyAnimation;
        }
            
    }
}

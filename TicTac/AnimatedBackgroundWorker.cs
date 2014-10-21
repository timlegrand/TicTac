using System.ComponentModel;


namespace TicTac
{
    public class AnimatedBackgroundWorker : BackgroundWorker
    {
        public AnimatedBackgroundWorker(
            DoWorkEventHandler showBusyAnimation,
            DoWorkEventHandler longOperation,
            RunWorkerCompletedEventHandler hideBusyAnimation)
        {
            DoWork += showBusyAnimation;
            DoWork += longOperation;
            RunWorkerCompleted += hideBusyAnimation;
        }
            
    }
}

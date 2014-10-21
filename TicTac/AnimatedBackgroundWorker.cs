using System.ComponentModel;


namespace TicTac
{
    // Since explicit name qualifier is required in attribute
// ReSharper disable once RedundantNameQualifier
    [System.ComponentModel.DesignerCategory("")]
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

using System;

namespace TicTac
{
    public class WorkSession
    {
        public Architect Architect { get; set; }
        public Project Project { get; set; }
        public Phase Phase { get; set; }
        public uint? RunningSessionId { get; set; }
        public DateTime StartTime { get; set; } //in DateTime format
        public DateTime StopTime { get; set; } //in DateTime format

        public WorkSession()
        {
            Architect = null;
            Project = null;
            Phase = null;
            RunningSessionId = null;
            StartTime = DateTime.MinValue;
            StopTime = DateTime.MinValue;
        }

        /*
        public bool Validate()
        {
            if (Architect.Id == null || Project.Id == null || Phase.Id == null)
            {
                return false;
            }
            Architect = _service.SelectArchitectFromId((int)Architect.Id);
            Project = _service.SelectProjectFromId((int)Project.Id);
            Phase = _service.SelectPhaseFromId((int)Phase.Id);
            return IsValid();
        }
        */
        public bool IsValid()
        {
            return (Architect != null && Project != null && Phase != null);
        }

        public bool IsTerminated()
        {
            return (StartTime != DateTime.MinValue && StopTime != DateTime.MinValue);
        }

        public override string ToString()
        {
            var s = " -- Work session #" + RunningSessionId ?? "unknown";
            s += "\nArchitect: " + Architect ?? "unknown";
            s += "\nProject: " + Project ?? "unknown";
            s += "\nPhase: " + Phase ?? "unknown";
            s += "\nStart time: " + StartTime ?? "unknown";
            s += "\nStop time: " + StopTime ?? "unknown";
            return s;
        }

        internal void PrettyPrint()
        {
            Console.Write(this.ToString());
        }
    }
}

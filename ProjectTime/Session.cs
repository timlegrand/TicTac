﻿using System;

namespace TicTac
{
    class Session
    {
        public Architect Architect { get; set; }
        public Project Project { get; set; }
        public Phase Phase { get; set; }
        public uint? RunningSessionId { get; set; }
        public DateTime StartTime { get; set; } //in DateTime format
        public DateTime StopTime { get; set; }
//in DateTime format

        public Session()
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
            Architect = _dao.SelectArchitectFromId((int)Architect.Id);
            Project = _dao.SelectProjectFromId((int)Project.Id);
            Phase = _dao.SelectPhaseFromId((int)Phase.Id);
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
    }
}

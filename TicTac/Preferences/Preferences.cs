using System;
using System.Drawing;
using System.Windows.Forms;


namespace TicTac
{
    public abstract class Preferences
    {
        public string Version { get; set; }
        public Point StartLocation { get; set; }
        public Architect LastArchitect { get; set; }
        public Project LastProject { get; set; }
        public Phase LastPhase { get; set; }
        public string DbServerAddress { get; set; }
        public string DbName { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }

        // Constructor
        public Preferences()
        {
            StartLocation = new Point(-1,-1);
            LastArchitect = null;
            LastProject = null;
            LastPhase = null;
        }

        public Preferences(RecordWindow recordWindow)
            : this()
        {            
            if (recordWindow.WorkSession != null)
            {
                LastArchitect = recordWindow.WorkSession.Architect ?? null;
                LastProject = recordWindow.WorkSession.Project ?? null;
                LastPhase = recordWindow.WorkSession.Phase ?? null;

                // If WorkSession exists consider that RecordWindow location is approved by user
                StartLocation = recordWindow.Location;

                DbServerAddress = Database.ServerAddress;
                DbName = Database.Name;
                DbUserName = Database.UserName;
                DbPassword = Database.Password;
            }
        }

        public abstract void Load();
        public abstract void Save();

        public virtual bool IsValid()
        {
            return (StartLocation != new Point(-1, -1) &&
                    LastArchitect != null && LastArchitect.Id != null &&
                    LastProject != null && LastProject.Id != null &&
                    LastPhase != null && LastPhase.Id != null);
        }
    }
}

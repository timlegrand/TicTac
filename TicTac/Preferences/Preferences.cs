using System;
using System.Drawing;
using System.Windows.Forms;


namespace TicTac
{
    public abstract class Preferences
    {
        public static readonly Point DefaultLocation = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);

        public string Version { get; set; }
        public Point StartLocation { get; set; }
        public Architect LastArchitect { get; set; }
        public Project LastProject { get; set; }
        public Phase LastPhase { get; set; }
        public string DbServerAddress { get; set; }
        public string DbName { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }

        protected readonly RecordWindow _parent = null;

        // Constructor
        public Preferences()
        {
            StartLocation = DefaultLocation;
            LastArchitect = null;
            LastProject = null;
            LastPhase = null;
        }

        public Preferences(RecordWindow recordWindow)
            : this()
        {
            _parent = recordWindow;
            
            if (recordWindow.WorkSession != null)
            {
                LastArchitect = recordWindow.WorkSession.Architect ?? null;
                LastProject = recordWindow.WorkSession.Project ?? null;
                LastPhase = recordWindow.WorkSession.Phase ?? null;

                // If WorkSession exists consider that RecordWindow location is apporved by user
                StartLocation = recordWindow.Location;

                DbServerAddress = Database.ServerAddress;
                DbName = Database.Name;
                DbUserName = Database.UserName;
                DbPassword = Database.Password;
            }
        }

        public abstract void Load();
        public abstract void Save();

        protected Point BoundLocation(int x, int y)
        {
            if (x != -1 && y != -1)
            {
                x = (x < Screen.PrimaryScreen.Bounds.Width - _parent.Width) ? x : Screen.PrimaryScreen.Bounds.Width - _parent.Width;
                x = (x >= 0) ? x : 0;
                y = (y < Screen.PrimaryScreen.Bounds.Height - _parent.Height) ? y : Screen.PrimaryScreen.Bounds.Height - _parent.Height;
                x = (x >= 0) ? x : 0;
                return new Point(x, y);
            }

            return DefaultLocation;
        }

        public virtual bool IsValid()
        {
            return (StartLocation != null &&
                    LastArchitect != null && LastArchitect.Id != null &&
                    LastProject != null && LastProject.Id != null &&
                    LastPhase != null && LastPhase.Id != null);
        }
    }
}

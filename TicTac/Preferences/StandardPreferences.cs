using System;


namespace TicTac
{
    public class StandardPreferences : Preferences
    {
        // Constructor
        public StandardPreferences()
        {
            StartLocation = new System.Drawing.Point(-1,-1);
            LastArchitect = null;
            LastProject = null;
            LastPhase = null;
        }

        public override void Load()
        {
            StartLocation = Properties.Settings.Default.Location;

            Database.Name = Properties.Settings.Default.DbName;
            Database.ServerAddress = Properties.Settings.Default.DbAddress;
            Database.UserName = Properties.Settings.Default.DbUserName;
            Database.Password = Properties.Settings.Default.DbPassword;

            LastArchitect = new Architect { Id = Properties.Settings.Default.ArchitectId };
            LastProject = new Project { Id = Properties.Settings.Default.ProjectId };
            LastPhase = new Phase { Id = Properties.Settings.Default.PhaseId };
        }

        public override void Save()
        {
            Properties.Settings.Default.Location = StartLocation;

            Properties.Settings.Default.DbName = Database.Name;
            Properties.Settings.Default.DbAddress = Database.ServerAddress;
            Properties.Settings.Default.DbUserName = Database.UserName;
            Properties.Settings.Default.DbPassword = Database.Password;

            Properties.Settings.Default.ArchitectId = LastArchitect.Id ?? 0;
            Properties.Settings.Default.ProjectId = LastProject.Id ?? 0;
            Properties.Settings.Default.PhaseId = LastPhase.Id ?? 0;

            Properties.Settings.Default.Save();
        }

        public override bool IsValid()
        {
            return (StartLocation != null &&
                    LastArchitect != null && LastArchitect.Id != null &&
                    LastProject != null && LastProject.Id != null &&
                    LastPhase != null && LastPhase.Id != null);
        }
    }
}

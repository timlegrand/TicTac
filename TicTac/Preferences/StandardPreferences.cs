using System;


namespace TicTac
{
    public class StandardPreferences : Preferences
    {
        // Constructor
        public StandardPreferences()
        {
            StartLocation = DefaultLocation;
            LastArchitect = null;
            LastProject = null;
            LastPhase = null;
        }

        public override void Load()
        {
            StartLocation = Properties.Settings.Default.Location;

            Database.DbName = Properties.Settings.Default.DbName;
            Database.DbServerIp = Properties.Settings.Default.DbAddress;
            Database.DbUserName = Properties.Settings.Default.DbUserName;
            Database.DbPassword = Properties.Settings.Default.DbPassword;

            LastArchitect = new Architect() { Id = Properties.Settings.Default.ArchitectId };
            LastProject = new Project() { Id = Properties.Settings.Default.ProjectId };
            LastPhase = new Phase() { Id = Properties.Settings.Default.PhaseId };
        }

        public override void Save()
        {
            Properties.Settings.Default.Location = StartLocation;

            Properties.Settings.Default.DbName = Database.DbName;
            Properties.Settings.Default.DbAddress = Database.DbServerIp;
            Properties.Settings.Default.DbUserName = Database.DbUserName;
            Properties.Settings.Default.DbPassword = Database.DbPassword;

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

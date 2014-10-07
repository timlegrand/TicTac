using System;


namespace TicTac
{
    public class DualModePreferences : Preferences
    {
        BackupPreferences FileMode;
        StandardPreferences IntegratedMode;

        public DualModePreferences()
            : base()
        {
            FileMode = new BackupPreferences();
            IntegratedMode = new StandardPreferences();
        }

        public DualModePreferences(RecordWindow recordWindow)
            : base(recordWindow)
        {
            FileMode = new BackupPreferences(recordWindow)
            {
                StartLocation = this.StartLocation,
                LastArchitect = this.LastArchitect,
                LastProject = this.LastProject,
                LastPhase = this.LastPhase,
            };
            IntegratedMode = new StandardPreferences()
            {
                StartLocation = this.StartLocation,
                LastArchitect = this.LastArchitect,
                LastProject = this.LastProject,
                LastPhase = this.LastPhase,
            };
        }

        public override void Load()
        {
            try
            {
                FileMode.Load();
                Synchronize(FileMode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                IntegratedMode.Load();
                Synchronize(IntegratedMode);
            }
        }

        public override void Save()
        {
            IntegratedMode.Save();
            FileMode.Save();
        }

        protected void Synchronize(Preferences mode)
        {
            this.StartLocation = mode.StartLocation;
            this.LastArchitect = mode.LastArchitect;
            this.LastProject = mode.LastProject;
            this.LastPhase = mode.LastPhase;
        }
    }
}

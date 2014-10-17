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
                Logger.Write(e);
                IntegratedMode.Load();
                Synchronize(IntegratedMode);
            }
        }

        public override void Save()
        {
            try
            {
                IntegratedMode.Save();
                FileMode.Save();
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }
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

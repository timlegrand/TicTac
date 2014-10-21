using System;


namespace TicTac
{
    public class DualModePreferences : Preferences
    {
        readonly BackupPreferences _fileMode;
        readonly StandardPreferences _integratedMode;

        public DualModePreferences()
            : base()
        {
            _fileMode = new BackupPreferences();
            _integratedMode = new StandardPreferences();
        }

        public DualModePreferences(RecordWindow recordWindow)
            : base(recordWindow)
        {
            _fileMode = new BackupPreferences(recordWindow)
            {
                StartLocation = StartLocation,
                LastArchitect = LastArchitect,
                LastProject = LastProject,
                LastPhase = LastPhase,
            };
            _integratedMode = new StandardPreferences()
            {
                StartLocation = StartLocation,
                LastArchitect = LastArchitect,
                LastProject = LastProject,
                LastPhase = LastPhase,
            };
        }

        public override void Load()
        {
            try
            {
                _fileMode.Load();
                Synchronize(_fileMode);
            }
            catch (Exception e)
            {
                Logger.Write(e);
                _integratedMode.Load();
                Synchronize(_integratedMode);
            }
        }

        public override void Save()
        {
            try
            {
                Logger.Write("IntegratedMode preferences: " + _integratedMode.ToString());
                _integratedMode.Save();
                Logger.Write("FileMode preferences: " + _fileMode.ToString());
                _fileMode.Save();
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }
        }

        protected void Synchronize(Preferences mode)
        {
            StartLocation = mode.StartLocation;
            LastArchitect = mode.LastArchitect;
            LastProject = mode.LastProject;
            LastPhase = mode.LastPhase;
        }
    }
}

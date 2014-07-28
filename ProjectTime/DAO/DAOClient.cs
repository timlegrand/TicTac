using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTac
{
    class DAOClient
    {
        private DbClient _db;
        //private FileClient _fc;
        private DbClient _fc;

        //Constructor
        public DAOClient()
        {
            _db = new DbClient();
            //_fc = new FileClient();
            _fc = new DbClient();
        }

        public DbClient getDb()
        {
            return _db;
        }

        private void SwitchDAO()
        {
            // Method may switch the _dao internal variable between _db and _fc according to current connectivity state
        }

        public List<Session> GetStartedWorkSessions(Architect archi)
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectStartedWorkSessions(archi);
            }
            else
            {
                return _fc.SelectStartedWorkSessions(archi);
            }
        }

        public void StartWorkSession(Session cfg)
        {
            if (Program.ConnectedMode)
            {
                _db.StartWorkSession(cfg);
            }
            else
            {
                _fc.StartWorkSession(cfg);
            }
        }

        public void EndWorkSession(Session s)
        {
            if (Program.ConnectedMode)
            {
                _db.EndWorkSession(s);
            }
            else
            {
                _fc.EndWorkSession(s);
            }
        }

        public List<Session> SelectStartedWorkSessions(Architect archi)
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectStartedWorkSessions(archi);
            }
            else
            {
                return _fc.SelectStartedWorkSessions(archi);
            }
        }

        public TimeSpan SelectTimeCount(int? archiId, int? projectId, int? phaseId)
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectTimeCount(archiId, projectId, phaseId);
            }
            else
            {
                return _fc.SelectTimeCount(archiId, projectId, phaseId);
            }
        }

        public double SelectTimeCountFromProjectId(int? id)
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectTimeCountFromProjectId(id);
            }
            else
            {
                return _fc.SelectTimeCountFromProjectId(id);
            }
        }

        public Architect SelectArchitectFromId(int id)
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectArchitectFromId(id);
            }
            else
            {
                return _fc.SelectArchitectFromId(id);
            }
        }

        public Project SelectProjectFromId(int id)
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectProjectFromId(id);
            }
            else
            {
                return _fc.SelectProjectFromId(id); ;
            }
        }

        public Phase SelectPhaseFromId(int id)
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectPhaseFromId(id);
            }
            else
            {
                return _fc.SelectPhaseFromId(id);
            }
        }

        public Company SelectCompanyFromId(int id)
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectCompanyFromId(id);
            }
            else
            {
                return _fc.SelectCompanyFromId(id);
            }
        }

        public List<Architect> SelectAllArchitects()
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectAllArchitects();
            }
            else
            {
                return _fc.SelectAllArchitects();
            }
        }

        public List<Project> SelectAllProjects()
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectAllProjects();
            }
            else
            {
                return _fc.SelectAllProjects();
            }
        }

        public List<Phase> SelectAllPhases()
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectAllPhases();
            }
            else
            {
                return _fc.SelectAllPhases();
            }
        }

        public List<Company> SelectAllCompanies()
        {
            if (Program.ConnectedMode)
            {
                return _db.SelectAllCompanies();
            }
            else
            {
                return _fc.SelectAllCompanies();
            }
        }

        public int InsertArchitect(Architect a)
        {
            if (Program.ConnectedMode)
            {
                return _db.InsertArchitect(a);
            }
            else
            {
                return _fc.InsertArchitect(a);
            }
        }

        public int InsertProject(Project p)
        {
            if (Program.ConnectedMode)
            {
                return _db.InsertProject(p);
            }
            else
            {
                return _fc.InsertProject(p);
            }
        }

        public int InsertPhase(Phase p)
        {
            if (Program.ConnectedMode)
            {
                return _db.InsertPhase(p);
            }
            else
            {
                return _fc.InsertPhase(p);
            }
        }

        public bool DeleteArchitect(Architect a)
        {
            if (Program.ConnectedMode)
            {
                return _db.DeleteArchitect(a);
            }
            else
            {
                return _fc.DeleteArchitect(a);
            }
        }

        public bool DeleteProject(Project p)
        {
            if (Program.ConnectedMode)
            {
                return _db.DeleteProject(p);
            }
            else
            {
                return _fc.DeleteProject(p);
            }
        }

        public bool DeletePhase(Phase p)
        {
            if (Program.ConnectedMode)
            {
                return _db.DeletePhase(p);
            }
            else
            {
                return _fc.DeletePhase(p);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicTac.DAO;

namespace TicTac
{
    class DAOClient
    {
        private DbClient _db;
        private DAOClientClass _dao;
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

        // This method may switch the _dao internal variable between _db and _fc
        // according to current connectivity state
        private void SwitchDAO()
        {
            if (Program.ConnectedMode)
            {
                _dao = _db;
            }
            else
            {
                _dao = _fc;
            }
        }

        public List<Session> GetStartedWorkSessions(Architect archi)
        {
            SwitchDAO();
            return _dao.SelectStartedWorkSessions(archi);
        }

        public void StartWorkSession(Session cfg)
        {
            SwitchDAO();
            _dao.StartWorkSession(cfg);
        }

        public void EndWorkSession(Session s)
        {
            SwitchDAO();
            _dao.EndWorkSession(s);
        }

        public List<Session> SelectStartedWorkSessions(Architect archi)
        {
            SwitchDAO();
            return _dao.SelectStartedWorkSessions(archi);
        }

        public TimeSpan SelectTimeCount(int? archiId, int? projectId, int? phaseId)
        {
            SwitchDAO();
            return _dao.SelectTimeCount(archiId, projectId, phaseId);
        }

        public double SelectTimeCountFromProjectId(int? id)
        {
            SwitchDAO();
            return _dao.SelectTimeCountFromProjectId(id);
        }

        public Architect SelectArchitectFromId(int id)
        {
            SwitchDAO();
            return _dao.SelectArchitectFromId(id);
        }

        public Project SelectProjectFromId(int id)
        {
            SwitchDAO();
            return _dao.SelectProjectFromId(id);
        }

        public Phase SelectPhaseFromId(int id)
        {
            SwitchDAO();
            return _dao.SelectPhaseFromId(id);
        }

        public Company SelectCompanyFromId(int id)
        {
            SwitchDAO();
            return _dao.SelectCompanyFromId(id);
        }

        public List<Architect> SelectAllArchitects()
        {
            SwitchDAO();
            return _dao.SelectAllArchitects();
        }

        public List<Project> SelectAllProjects()
        {
            SwitchDAO();
            return _dao.SelectAllProjects();
        }

        public List<Phase> SelectAllPhases()
        {
            SwitchDAO();
            return _dao.SelectAllPhases();
        }

        public List<Company> SelectAllCompanies()
        {
            SwitchDAO();
            return _dao.SelectAllCompanies();
        }

        public int InsertArchitect(Architect a)
        {
            SwitchDAO();
            return _dao.InsertArchitect(a);
        }

        public int InsertProject(Project p)
        {
            SwitchDAO();
            return _dao.InsertProject(p);
        }

        public int InsertPhase(Phase p)
        {
            SwitchDAO();
            return _dao.InsertPhase(p);
        }

        public bool DeleteArchitect(Architect a)
        {
            SwitchDAO();
            return _dao.DeleteArchitect(a);
        }

        public bool DeleteProject(Project p)
        {
            SwitchDAO();
            return _dao.DeleteProject(p);
        }

        public bool DeletePhase(Phase p)
        {
            SwitchDAO();
            return _dao.DeletePhase(p);
        }
    }
}

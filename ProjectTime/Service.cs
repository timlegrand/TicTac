using System;
using System.Collections.Generic;
using TicTac.DAO;

namespace TicTac
{
    class Service
    {
        private DAOClient _dao;

        //Constructor
        public Service()
        {
            _dao = new DAOClient();
        }

        public List<Session> GetStartedWorkSessions(Architect archi)
        {
            return _dao.SelectStartedWorkSessions(archi);
        }

        public void StartWorkSession(Session cfg)
        {
            _dao.InsertWorkSession(cfg);
        }

        public void EndWorkSession(Session s)
        {
            _dao.UpdateWorkSession(s);
        }

        public TimeSpan SelectTimeCount(int? archiId, int? projectId, int? phaseId)
        {
            return _dao.SelectTimeCount(archiId, projectId, phaseId);
        }

        public double SelectTimeCountFromProjectId(int? id)
        {
            return _dao.SelectTimeCountFromProjectId(id);
        }

        public Architect SelectArchitectFromId(int id)
        {
            return _dao.SelectArchitectFromId(id);
        }

        public Project SelectProjectFromId(int id)
        {
            return _dao.SelectProjectFromId(id);
        }

        public Phase SelectPhaseFromId(int id)
        {
            return _dao.SelectPhaseFromId(id);
        }

        public Company SelectCompanyFromId(int id)
        {
            return _dao.SelectCompanyFromId(id);
        }

        public List<Architect> SelectAllArchitects()
        {
            return _dao.SelectAllArchitects();
        }

        public List<Project> SelectAllProjects()
        {
            return _dao.SelectAllProjects();
        }

        public List<Phase> SelectAllPhases()
        {
            return _dao.SelectAllPhases();
        }

        public List<Company> GetAllCompanies()
        {
            return _dao.SelectAllCompanies();
        }

        public int AddArchitect(Architect a)
        {
            return _dao.InsertArchitect(a);
        }

        public int InsertProject(Project p)
        {
            return _dao.InsertProject(p);
        }

        public int InsertPhase(Phase p)
        {
            return _dao.InsertPhase(p);
        }

        public bool DeleteArchitect(Architect a)
        {
            return _dao.DeleteArchitect(a);
        }

        public bool DeleteProject(Project p)
        {
            return _dao.DeleteProject(p);
        }

        public bool DeletePhase(Phase p)
        {
            return _dao.DeletePhase(p);
        }
    }
}

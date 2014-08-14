using System;
using System.Collections.Generic;
using TicTac.DAO;

namespace TicTac
{
    class DAOClient
    {
        //private DAOClientClass _dao;
        private DbClient _db;
        private MessageQueue _mq;

        //Constructor
        public DAOClient()
        {
            _db = new DbClient();
            _mq = new MessageQueue();
        }

        public DbClient getDb()
        {
            return _db;
        }

        // This method may switch the _service internal variable between _service and _mq
        // according to current connectivity state
        private void SwitchDAO()
        {
            if (Program.ConnectedMode)
            {
                // Check and process waiting messages if any
                //_mq.ProcessWaitingMessages();
            }
            else
            {
//                string action = System.Reflection.MethodBase.GetCurrentMethod().Name
//;
//                string data = ;
//                Message msg = new Message(action, data);
//                _mq.StoreMessage();
            }
        }

        public void InsertWorkSession(Session s)
        {
            SwitchDAO();
            _db.InsertWorkSession(s);
        }

        public void UpdateWorkSession(Session s)
        {
            SwitchDAO();
            _db.UpdateWorkSession(s);
        }

        public List<Session> SelectStartedWorkSessions(Architect archi)
        {
            SwitchDAO();
            return _db.SelectStartedWorkSessions(archi);
        }

        public TimeSpan SelectTimeCount(int? archiId, int? projectId, int? phaseId)
        {
            SwitchDAO();
            return _db.SelectTimeCount(archiId, projectId, phaseId);
        }

        public double SelectTimeCountFromProjectId(int? id)
        {
            SwitchDAO();
            return _db.SelectTimeCountFromProjectId(id);
        }

        public Architect SelectArchitectFromId(int id)
        {
            SwitchDAO();
            return _db.SelectArchitectFromId(id);
        }

        public Project SelectProjectFromId(int id)
        {
            SwitchDAO();
            return _db.SelectProjectFromId(id);
        }

        public Phase SelectPhaseFromId(int id)
        {
            SwitchDAO();
            return _db.SelectPhaseFromId(id);
        }

        public Company SelectCompanyFromId(int id)
        {
            SwitchDAO();
            return _db.SelectCompanyFromId(id);
        }

        public List<Architect> SelectAllArchitects()
        {
            SwitchDAO();
            return _db.SelectAllArchitects();
        }

        public List<Project> SelectAllProjects()
        {
            SwitchDAO();
            return _db.SelectAllProjects();
        }

        public List<Phase> SelectAllPhases()
        {
            SwitchDAO();
            return _db.SelectAllPhases();
        }

        public List<Company> SelectAllCompanies()
        {
            SwitchDAO();
            return _db.SelectAllCompanies();
        }

        public int InsertArchitect(Architect a)
        {
            SwitchDAO();
            return _db.InsertArchitect(a);
        }

        public int InsertProject(Project p)
        {
            SwitchDAO();
            return _db.InsertProject(p);
        }

        public int InsertPhase(Phase p)
        {
            SwitchDAO();
            return _db.InsertPhase(p);
        }

        public bool DeleteArchitect(Architect a)
        {
            SwitchDAO();
            return _db.DeleteArchitect(a);
        }

        public bool DeleteProject(Project p)
        {
            SwitchDAO();
            return _db.DeleteProject(p);
        }

        public bool DeletePhase(Phase p)
        {
            SwitchDAO();
            return _db.DeletePhase(p);
        }
    }
}

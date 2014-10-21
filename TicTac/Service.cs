using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;


namespace TicTac
{
    // Singleton
    public sealed class Service
    {
        private static ManualResetEvent _ready;
        private static volatile Service _instance;
        private static readonly object SyncRoot = new Object();

        private readonly DAOClient _dao;

        private List<Project> _projectList;
        private List<Phase> _phaseList;
        private List<Architect> _architectList;
        private List<Company> _companyList;

        public List<Project> ProjectList        { get { return Instance._projectList; }      private set { _projectList = value; } }
        public List<Phase> PhaseList            { get { return Instance._phaseList; }        private set { _phaseList = value; } }
        public List<Architect> ArchitectList    { get { return Instance._architectList; }    private set { _architectList = value; } }
        public List<Company> CompanyList        { get { return Instance._companyList; }      private set { _companyList = value; } }

        // Static constructor
        static Service()
        {
            _ready = new ManualResetEvent(false);
        }

        // Singleton getter & lazy constructor
        public static Service Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new Service();
                    }
                }

                return _instance;
            }
        }

        // Instance constructor
        private Service()
        {
            var ar = new Thread(delegate()
            {
                ArchitectList = new DAOClient().SelectAllArchitects();
            });
            ar.Start();

            var pr = new Thread(delegate()
            {
                ProjectList = new DAOClient().SelectAllProjects();
            });
            pr.Start();

            var ph = new Thread(delegate()
            {
                PhaseList = new DAOClient().SelectAllPhases();
            });
            ph.Start();

            var co = new Thread(delegate()
            {
                CompanyList = new DAOClient().SelectAllCompanies();
            });
            co.Start();

            _dao = new DAOClient();

            ar.Join();
            pr.Join();
            ph.Join();
            co.Join();

            _ready.Set(); // Tell I'm ready
        }

        public static void StartAsync()
        {
            var startUpThread = new Thread(delegate()
            {
                // Since call to Instance needed to initiate Service
                // ReSharper disable once UnusedVariable
                var s = Instance;
            });
            startUpThread.Start();
        }

        public bool IsReady()
        {
            // Non-blocking check
            return _ready.WaitOne(0);
        }

        internal static Service Ready()
        {
            // Blocking wait
            _ready.WaitOne();

            return _instance;
        }

        internal List<WorkSession> GetStartedWorkSessions(Architect archi)
        {
            return _dao.SelectStartedWorkSessions(archi);
        }

        internal void StartWorkSession(WorkSession cfg)
        {
            _dao.InsertWorkSession(cfg);
        }

        internal void EndWorkSession(WorkSession s)
        {
            _dao.UpdateWorkSession(s);
        }

        internal List<WorkSession> GetDailyWorkSessions(Architect archi)
        {
            var dr = new DateRange(DateTime.Today, DateTime.Today.AddDays(1));
            return _dao.SelectWorkSessions(archi, dr);
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

        internal Company SelectCompanyFromId(int id)
        {
            return _dao.SelectCompanyFromId(id);
        }

        public List<Architect> GetAllArchitects()
        {
            ArchitectList = _dao.GetAllArchitects();
            return ArchitectList;
        }

        public List<Project> GetAllProjects()
        {
            ProjectList = _dao.GetAllProjects();
            return ProjectList;
        }

        public List<Phase> GetAllPhases()
        {
            PhaseList = _dao.GetAllPhases();
            return PhaseList;
        }

        internal List<Company> GetAllCompanies()
        {
            CompanyList = _dao.GetAllCompanies();
            return CompanyList;
        }

        public void SaveAllArchitects()
        {
            _dao.SaveAllArchitects(ArchitectList);
        }

        public void SaveAllProjects()
        {
            _dao.SaveAllProjects(ProjectList);
        }

        public void SaveAllPhases()
        {
            _dao.SaveAllPhases(PhaseList);
        }

        public int AddArchitect(Architect a)
        {
            ArchitectList.Add(a);
            return _dao.InsertArchitect(a);
        }

        public int AddProject(Project p)
        {
            ProjectList.Add(p);
            return _dao.InsertProject(p);
        }

        public int AddPhase(Phase p)
        {
            PhaseList.Add(p);
            return _dao.InsertPhase(p);
        }

        public bool DeleteArchitect(Architect a)
        {
            ArchitectList.Remove(ArchitectList.Single(i => i.Id == a.Id));
            return _dao.DeleteArchitect(a);
        }

        public bool DeleteProject(Project p)
        {
            ProjectList.Remove(ProjectList.Single(i => i.Id == p.Id));
            return _dao.DeleteProject(p);
        }

        public bool DeletePhase(Phase p)
        {
            PhaseList.Remove(PhaseList.Single(i => i.Id == p.Id));
            return _dao.DeletePhase(p);
        }

        internal int EditArchitect(int id, Architect architect)
        {
            ArchitectList.Single(a => a.Id == id).CopyIn(architect);
            return _dao.UpdateArchitect(id, architect);
        }

        internal object EditProject(int id, Project project)
        {
            ProjectList.Single(a => a.Id == id).CopyIn(project);
            return _dao.UpdateProject(id, project);
        }

        internal object EditPhase(int id, Phase phase)
        {
            PhaseList.Single(a => a.Id == id).CopyIn(phase);
            return _dao.UpdatePhase(id, phase);
        }

        internal DataTable GetWorkSessionDataTable(int id)
        {
            return _dao.GetWorkSessionDataTable(id);
        }
    }
}

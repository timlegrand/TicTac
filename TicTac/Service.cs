using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;


namespace TicTac
{
    // Singleton
    public sealed class Service
    {
        private static ManualResetEvent ready;
        private static volatile Service instance;
        private static object syncRoot = new Object();

        private readonly DAOClient _dao;
        private readonly BinaryFormatter _formatter;

        public List<Project> ProjectList { get; private set; }
        public List<Phase> PhaseList { get; private set; }
        public List<Architect> ArchitectList { get; private set; }
        public List<Company> CompanyList { get; private set; }

        // Static constructor
        static Service()
        {
            Service.ready = new ManualResetEvent(false);
        }

        // Singleton getter & lazy constructor
        public static Service Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Service();
                    }
                }

                return instance;
            }
        }

        // Instance constructor
        private Service()
        {
            Thread ar = new Thread(delegate()
            {
                ArchitectList = new DAOClient().SelectAllArchitects();
            });
            ar.Start();

            Thread pr = new Thread(delegate()
            {
                ProjectList = new DAOClient().SelectAllProjects();
            });
            pr.Start();

            Thread ph = new Thread(delegate()
            {
                PhaseList = new DAOClient().SelectAllPhases();
            });
            ph.Start();

            _dao = new DAOClient();
            _formatter = new BinaryFormatter();

            ar.Join();
            pr.Join();
            ph.Join();

            CompanyList = null; // On demand only

            Service.ready.Set(); // Tell I'm ready
        }

        public static void StartAsync()
        {
            Thread startUpThread = new Thread(delegate()
            {
                var s = Service.Instance;
            });
            startUpThread.Start();
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
            if (ConnectionPing.DatabaseConnexionAvailable)
            {
                // Retrieve data from server
                ArchitectList = _dao.SelectAllArchitects();
            }
            else
            {
                // Deserialize from file if any
                // Use serialization to files until real DB
                using (var architectsFile = File.Open("Architects.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des architectes (depuis un fichier)");
                    ArchitectList = (List<Architect>)_formatter.Deserialize(architectsFile);
                }
            }
            return ArchitectList;
        }

        public List<Project> GetAllProjects()
        {
            if (ConnectionPing.DatabaseConnexionAvailable)
            {
                ProjectList = _dao.SelectAllProjects();
            }
            else
            {
                using (var projectsFile = File.Open("Projects.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des projets (depuis un fichier)");
                    ProjectList = (List<Project>)_formatter.Deserialize(projectsFile);
                }
            }
            return ProjectList;
        }

        public List<Phase> GetAllPhases()
        {
            if (ConnectionPing.DatabaseConnexionAvailable)
            {
                PhaseList = _dao.SelectAllPhases();
            }
            else
            {
                using (var phasesFile = File.Open("Phases.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des phases (depuis un fichier)");
                    PhaseList = (List<Phase>)_formatter.Deserialize(phasesFile);
                }
            }
            return PhaseList;
        }

        internal List<Company> GetAllCompanies()
        {
            if (CompanyList == null && ConnectionPing.DatabaseConnexionAvailable)
            {
                CompanyList = _dao.SelectAllCompanies();
            }
            return CompanyList;
        }

        public void SaveAllArchitects()
        {
            using (var architectsFile = File.Open(Path.Combine(Program.ApplicationDataFolder, "Architects.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des architectes (dans un fichier)");
                _formatter.Serialize(architectsFile, ArchitectList);
            }
        }

        public void SaveAllProjects()
        {
            using (var projectsFile = File.Open(Path.Combine(Program.ApplicationDataFolder, "Projects.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des projets (dans un fichier)");
                _formatter.Serialize(projectsFile, ProjectList);
            }
        }

        public void SaveAllPhases()
        {
            using (var phasesFile = File.Open(Path.Combine(Program.ApplicationDataFolder, "Phases.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des phases (dans un fichier)");
                _formatter.Serialize(phasesFile, PhaseList);
            }
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

        public bool IsReady()
        {
            // Non-blocking check
            return ready.WaitOne(0);
        }

        internal static bool Ready()
        {
            // Blocking check
            return ready.WaitOne();
        }

        internal System.Data.DataTable GetWorkSessionDataTable(int id)
        {
            return _dao.GetWorkSessionDataTable(id);
        }
    }
}

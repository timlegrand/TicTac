using System;
using System.Collections.Generic;
using TicTac.DAO;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TicTac
{
    class DAOClient
    {
        //private DAOClientClass _dao;
        private readonly DbClient _db;
        private MessageQueue _mq;
        private readonly BinaryFormatter _formatter;

        //Constructor
        public DAOClient()
        {
            _db = new DbClient();
            _mq = new MessageQueue();
            _formatter = new BinaryFormatter();
        }

        // This method may switch the _service internal variable between _service and _mq
        // according to current connectivity state
        private void SwitchDAO()
        {
            if (Database.CheckConnexion())
            {
                // Check and process waiting messages if any
                //_mq.ProcessWaitingMessages();
            }
            else
            {
//                string action = System.Reflection.MethodBase.GetCurrentMethod().Name;
//                string data = ;
//                Message msg = new Message(action, data);
//                _mq.StoreMessage();
            }
        }

        public void InsertWorkSession(WorkSession s)
        {
            SwitchDAO();
            _db.InsertWorkSession(s);
        }

        public void UpdateWorkSession(WorkSession s)
        {
            SwitchDAO();
            _db.UpdateWorkSession(s);
        }

        public List<WorkSession> SelectStartedWorkSessions(Architect archi)
        {
            SwitchDAO();
            if (!Database.DatabaseConnexionAvailable)
            {
                throw new NotImplementedException(@"file-based session not yet implemented");
            }

            return _db.SelectStartedWorkSessions(archi);
        }

        public List<WorkSession> SelectWorkSessions(Architect archi, DateRange dr = null)
        {
            SwitchDAO();
            return _db.SelectWorkSessions(archi, dr);
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

        internal int UpdateArchitect(int id, Architect architect)
        {
            SwitchDAO();
            return _db.UpdateArchitect(id, architect);
        }


        internal object UpdateProject(int id, Project project)
        {
            SwitchDAO();
            return _db.UpdateProject(id, project);
        }

        internal object UpdatePhase(int id, Phase phase)
        {
            SwitchDAO();
            return _db.UpdatePhase(id, phase);
        }

        internal System.Data.DataTable GetWorkSessionDataTable(int archi_id, int? project_id, int? phase_id)
        {
            SwitchDAO();
            return _db.GetWorkSessionDataTable(archi_id, project_id, phase_id);
        }

        internal void SaveAllPhases(List<Phase> PhaseList)
        {
            using (var phasesFile = File.Open(Path.Combine(Program.ApplicationDataFolder, "Phases.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des phases (dans un fichier)");
                _formatter.Serialize(phasesFile, PhaseList);
            }
        }

        internal void SaveAllProjects(List<Project> ProjectList)
        {
            using (var projectsFile = File.Open(Path.Combine(Program.ApplicationDataFolder, "Projects.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des projets (dans un fichier)");
                _formatter.Serialize(projectsFile, ProjectList);
            }
        }

        internal void SaveAllArchitects(List<Architect> ArchitectList)
        {
            using (var architectsFile = File.Open(Path.Combine(Program.ApplicationDataFolder, "Architects.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des architectes (dans un fichier)");
                _formatter.Serialize(architectsFile, ArchitectList);
            }
        }

        internal List<Architect> GetAllArchitects()
        {
            List<Architect> architectList;
            if (Database.DatabaseConnexionAvailable)
            {
                // Retrieve data from server
                architectList = SelectAllArchitects();
            }
            else
            {
                // Deserialize from file if any
                // Use serialization to files until real DB
                using (var architectsFile = File.Open("Architects.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des architectes (depuis un fichier)");
                    architectList = (List<Architect>)_formatter.Deserialize(architectsFile);
                }
            }
            return architectList;
        }

        internal List<Project> GetAllProjects()
        {
            List<Project> projectList;
            if (Database.DatabaseConnexionAvailable)
            {
                projectList = SelectAllProjects();
            }
            else
            {
                using (var projectsFile = File.Open("Projects.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des projets (depuis un fichier)");
                    projectList = (List<Project>)_formatter.Deserialize(projectsFile);
                }
            }
            return projectList;
        }

        internal List<Phase> GetAllPhases()
        {
            List<Phase> phaseList;
            if (Database.DatabaseConnexionAvailable)
            {
                phaseList = SelectAllPhases();
            }
            else
            {
                using (var phasesFile = File.Open("Phases.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des phases (depuis un fichier)");
                    phaseList = (List<Phase>)_formatter.Deserialize(phasesFile);
                }
            }
            return phaseList;
        }

        internal List<Company> GetAllCompanies()
        {
            List<Company> companyList = null;
            if (Database.DatabaseConnexionAvailable)
            {
                companyList = SelectAllCompanies();
            }
            return companyList;
        }
    }
}

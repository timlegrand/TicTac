﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TicTac
{
    // Singleton
    public sealed class Service
    {
        private static readonly Service instance = new Service();

        private readonly DAOClient _dao;
        private readonly BinaryFormatter _formatter;
        public List<Project> ProjectList { get; private set; }
        public List<Phase> PhaseList { get; private set; }
        public List<Architect> ArchitectList { get; private set; }

        // Constructor
        private Service()
        {
            _dao = new DAOClient();
            _formatter = new BinaryFormatter();
            ArchitectList = GetAllArchitects();
            Program.clk.Probe("GetAllArchitects");
            ProjectList = GetAllProjects();
            Program.clk.Probe("GetAllProjects");
            PhaseList = GetAllPhases();
            Program.clk.Probe("GetAllPhases");
        }

        // Singleton getter
        public static Service Instance
        {
            get
            {
                return instance;
            }
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
            if (Program.DatabaseConnexionAvailable)
            {
                // Retrieve data from server
                return _dao.SelectAllArchitects();
            }
            else
            {
                // Deserialize from file if any
                List<Architect> architectList;
                // Use serialization to files until real DB
                using (var architectsFile = File.Open("Architects.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des architectes (depuis un fichier)");
                    architectList = (List<Architect>)_formatter.Deserialize(architectsFile);
                }
                return architectList;
            }
        }

        public List<Project> GetAllProjects()
        {
            if (Program.DatabaseConnexionAvailable)
            {
                return _dao.SelectAllProjects();
            }
            else
            {
                List<Project> projectList;
                using (var projectsFile = File.Open("Projects.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des projets (depuis un fichier)");
                    projectList = (List<Project>) _formatter.Deserialize(projectsFile);
                }
                return projectList;
            }
        }

        public List<Phase> GetAllPhases()
        {
            if (Program.DatabaseConnexionAvailable)
            {
                return _dao.SelectAllPhases();
            }
            else
            {
                List<Phase> phaseList;
                using (var phasesFile = File.Open("Phases.osl", FileMode.Open))
                {
                    Console.WriteLine("Lecture de la table des phases (depuis un fichier)");
                    phaseList = (List<Phase>)_formatter.Deserialize(phasesFile);
                }
                return phaseList ;
            }
        }

        public void SaveAllArchitects()
        {
            using (var architectsFile = File.Open(Path.Combine(Preferences.ApplicationDataFolder, "Architects.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des architectes (dans un fichier)");
                _formatter.Serialize(architectsFile, ArchitectList);
            }
        }

        public void SaveAllProjects()
        {
            using (var projectsFile = File.Open(Path.Combine(Preferences.ApplicationDataFolder, "Projects.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des projets (dans un fichier)");
                _formatter.Serialize(projectsFile, ProjectList);
            }
        }

        public void SaveAllPhases()
        {
            using (var phasesFile = File.Open(Path.Combine(Preferences.ApplicationDataFolder, "Phases.osl"), FileMode.Create))
            {
                Console.WriteLine("Ecriture de la table des phases (dans un fichier)");
                _formatter.Serialize(phasesFile, PhaseList);
            }
        }

        internal List<Company> GetAllCompanies()
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

        internal int EditArchitect(int id, Architect architect)
        {
            return _dao.UpdateArchitect(id, architect);
        }
    }
}

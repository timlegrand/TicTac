using System;
using System.Collections.Generic;

namespace TicTac.DAO
{
    abstract class DAOClientClass
    {
        // Work-session management
        abstract public void InsertWorkSession(WorkSession cfg);
        abstract public void UpdateWorkSession(WorkSession s);

        // Get statistics
        abstract public List<WorkSession> SelectStartedWorkSessions(Architect archi);
        abstract public List<WorkSession> SelectWorkSessions(Architect archi, DateRange dr = null);
        abstract public TimeSpan SelectTimeCount(int? archiId, int? projectId, int? phaseId);
        abstract public double SelectTimeCountFromProjectId(int? id);

        // Create
        abstract public int InsertArchitect(Architect a);
        abstract public int InsertProject(Project p);
        abstract public int InsertPhase(Phase p);

        // Read
        abstract public Architect SelectArchitectFromId(int id);
        abstract public Project SelectProjectFromId(int id);
        abstract public Phase SelectPhaseFromId(int id);
        abstract public Company SelectCompanyFromId(int id);
        abstract public List<Architect> SelectAllArchitects();
        abstract public List<Project> SelectAllProjects();
        abstract public List<Phase> SelectAllPhases();
        abstract public List<Company> SelectAllCompanies();

        // Update
        //abstract public Architect UpdateArchitectFromId(int id);
        //abstract public Project UpdateProjectFromId(int id);
        //abstract public Phase UpdatePhaseFromId(int id);
        //abstract public Company UpdateCompanyFromId(int id);

        // Delete
        abstract public bool DeleteArchitect(Architect a);
        abstract public bool DeleteProject(Project p);
        abstract public bool DeletePhase(Phase p);
    }
}

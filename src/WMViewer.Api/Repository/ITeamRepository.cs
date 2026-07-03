using VMViewer.Model;

namespace VMViewer.Repository;

public interface ITeamRepository
{
    public Team? GetByID(int Id);
    public bool DoTeamExistsWithId(int Id);
    public bool DoTeamExistsWithName(string Name);
    public (Team?, SaveStatus) SaveTeam(Team team);
    public SaveStatus DeleteTeam(int Id);

    SaveStatus UpdateTeam(Team team);
}
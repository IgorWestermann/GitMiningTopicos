using Atividade1.model;

public class AnalysisResult
{
    public int TotalCommits { get; set; }
    public List<CommitDetails> Commits { get; set; }
    public List<Contributor> TopContributors { get; set; }
}
using Atividade1.model;

public class AnalysisResult
{
    public int TotalCommits { get; set; }
    public List<CommitDetails> TestCommits { get; set; }
    public List<Contributor> TopContributors { get; set; }
}
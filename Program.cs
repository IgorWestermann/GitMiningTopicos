using Atividade1.model;
using System.Text.RegularExpressions;
using System.Text;

namespace Atividade1;

class Program
{
    static void Main(string[] args)
    {
        string repoPath = "/Users/igorwestermann/Dev/UFJF/Topicos/antlr4";
        var analysis = AnalyzeTestCommits(repoPath, true);

        Console.WriteLine("Total commits: " + analysis.TotalCommits);
        Console.WriteLine("Test-related commits: " + analysis.TestCommits.Count);
        Console.WriteLine("Total files related to tests: " + analysis.TestCommits.SelectMany(tc => tc.Files).Count());
        Console.WriteLine("Top 5 contributors: ");
        foreach (var contributor in analysis.TopContributors)
        {
            Console.WriteLine($"{contributor.Author}: {contributor.Count}");
        }

        Console.WriteLine("\nDetalhes dos commits relacionados a testes:");
        foreach (var testCommit in analysis.TestCommits.Take(5))
        {
            Console.WriteLine(@$"
Commit Hash: {testCommit.Hash.Replace("commit ", "")} 
Author: {testCommit.Author}
Date: {testCommit.Date} 
Message: {testCommit.Message}");
            Console.WriteLine("Files changed:");
            foreach (var file in testCommit.Files)
            {
                Console.WriteLine($" - {file}");
            }
            Console.WriteLine();
        }
    }

    public static AnalysisResult AnalyzeTestCommits(string repoPath, bool countAllCommits = false)
    {
        var results = new AnalysisResult
        {
            TestCommits = new List<CommitDetails>(),
            TopContributors = new List<Contributor>()
        };

        if (countAllCommits)
        {
            results.TotalCommits = GitMethods.GetTotalCommits(repoPath);
        }

        var gitLog = GitMethods.GetLogOutput(repoPath);
        var sections = SplitCommitLog(gitLog);
        string pattern = @"(?=commit \w{40})";
        Regex regex = new Regex(pattern);

        var filteredLines = regex.Split(gitLog).Where(line => !string.IsNullOrEmpty(line)).ToList();
        foreach (var commit in filteredLines)
        {
            var files = commit.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            var commitDetail = GitMethods.GetCommitDetails(files);
            if (commitDetail.Hash != null)
            {
                results.TestCommits.Add(commitDetail);
            }
        }
        results.TopContributors = GitMethods.GetTopContributors(repoPath).Take(5).ToList();
        return results;
    }

    public static List<string> SplitCommitLog(string log)
    {
        var commits = new List<string>();
        var currentCommit = new StringBuilder();

        foreach (var line in log.Split('\n'))
        {
            if (line.StartsWith("commit "))
            {
                if (currentCommit.Length > 0)
                {
                    commits.Add(currentCommit.ToString());
                }
                currentCommit.Clear();
                currentCommit.AppendLine(line);
            }
            else
            {
                currentCommit.AppendLine(line);
            }
        }
        if (currentCommit.Length > 0)
        {
            commits.Add(currentCommit.ToString());
        }
        return commits;
    }
}

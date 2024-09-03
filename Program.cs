using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Atividade1.model;
using System.Text.RegularExpressions;

namespace Atividade1;

class Program
{
    static void Main(string[] args)
    {
        string repoPath = "/Users/igorwestermann/Dev/UFJF/Topicos/antlr4";
        var analysis = AnalyzeTestCommits(repoPath, true);

        Console.WriteLine("Total commits: " + analysis.TotalCommits);
        Console.WriteLine("Test-related commits: " + analysis.TestCommits.Count);
        Console.WriteLine("Top 5 contributors: ");
        foreach (var contributor in analysis.TopContributors)
        {
            Console.WriteLine($"{contributor.Author}: {contributor.Count}");
        }
        System.Console.WriteLine("_____________");
        foreach (var commit in analysis.TestCommits.Take(5))
        {
            Console.WriteLine(commit.Author);
            Console.WriteLine(commit.Message);
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
        var sections = SplitGitLogByCommitWithHash(gitLog);
        foreach (var section in sections)
        {
            var commits = section.Split(new[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var commit in commits)
            {
                // Console.WriteLine(commit.Trim().Replace("Author: ", "").Replace("Date:   ", ""));
                var files = commit.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                var commitDetail = GitMethods.GetCommitDetails(files);
                if (commitDetail is not null)
                {
                    results.TestCommits.Add(commitDetail);
                }

            }
        }

        results.TopContributors = GitMethods.GetTopContributors(repoPath).Take(5).ToList();

        return results;
    }


    public static string[] SplitGitLogByCommitWithHash(string gitLog)
    {
        string pattern = @"(?<=commit\s+[0-9a-f]{40})\s*(?=\n)";

        return Regex.Split(gitLog, pattern, RegexOptions.Multiline);
    }

}

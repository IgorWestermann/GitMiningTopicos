using System.Diagnostics;
using Atividade1.model;

public class GitMethods
{
    public static int GetTotalCommits(string repoPath)
    {
        try
        {
            return int.Parse(ExecuteGitCommand($"--git-dir={repoPath}/.git rev-list --count HEAD"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting total commits: {ex.Message}");
            return 0;
        }
    }
    public static string GetLogOutput(string repoPath)
    {
        return ExecuteGitCommand($"--git-dir={repoPath}/.git log --name-only");
    }

    public static List<Contributor> GetTopContributors(string repoPath)
    {
        try
        {
            var topContributorsOutput = ExecuteGitCommand($"--git-dir={repoPath}/.git shortlog -sn --grep=\"test\"")
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var parts = line.Split('\t');
                    return new Contributor
                    {
                        Count = int.TryParse(parts[0], out int count) ? count : 0,
                        Author = parts.Length > 1 ? parts[1] : string.Empty
                    };
                }).ToList();

            return topContributorsOutput;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting top contributors: {ex.Message}");
            return new List<Contributor>();
        }
    }
    public static CommitDetails GetCommitDetails(string[] commit)
    {
        if (commit is null || commit.Length < 4)
        {
            // Console.WriteLine("Commit data is insufficient.");
            return new CommitDetails();
        }
        var hash = commit[0].Trim();
        var author = commit[1].Trim().Replace("Author: ", "");
        var date = commit[2].Replace("Date:   ", "").Trim();
        var message = commit.Length > 3 ? commit[3].Trim() : string.Empty;
        var bCommit = new CommitDetails();
        if (IsTestRelatedInMessage(message))
        {
            var files = commit.Skip(4).Where(IsTestRelated).ToArray();

            bCommit.Hash = hash;
            bCommit.Author = author;
            bCommit.Date = date;
            bCommit.Message = IsTestRelatedInMessage(message) ? message : "NO MESSAGE FOUND";
            bCommit.Files = [];

            if (commit.Any(IsTestRelated))
            {
                bCommit.Files = files.Distinct().ToArray();
            }
            return bCommit;
        }
        return new CommitDetails();
    }
    private static bool IsTestRelated(string fileName)
    {
        return fileName.EndsWith(".test.cs") ||
            fileName.EndsWith(".spec.cs") ||
            fileName.Contains("test") ||
            fileName.Contains("spec");
    }

    private static bool IsTestRelatedInMessage(string commit)
    {
        return !commit.Contains("Merge") && (commit.Contains("test", StringComparison.CurrentCultureIgnoreCase) ||
            commit.Contains("fix", StringComparison.CurrentCultureIgnoreCase));
    }
    private static string ExecuteGitCommand(string arguments)
    {
        using var process = new Process();
        process.StartInfo.FileName = "git";
        process.StartInfo.Arguments = arguments;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        try
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output.Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred executing git command: {ex.Message}");
            return string.Empty;
        }
    }
}
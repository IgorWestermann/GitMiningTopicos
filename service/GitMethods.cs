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
            return new List<Contributor>(); // Return an empty list if there's an error.
        }
    }
    public static CommitDetails GetCommitDetails(string[] commit)
    {
        if (commit.Length > 4 && commit.Any(file => IsTestRelated(file) || IsTestRelatedInMessage(commit[3])))
        {
            return new CommitDetails()
            {
                Hash = commit[0].Trim(),
                Author = commit[1].Trim().Replace("Author: ", ""),
                Date = commit[2].Replace("Date:   ", ""),
                Message = commit[3].Trim(),
                Files = commit.Skip(4).ToArray()
            };
        }
        return new CommitDetails();
    }
    public static string GetCommitAuthor(string commit)
    {
        string author = SplitCommit(commit, 1);
        return author;
    }

    public static DateTime GetCommitDate(string commit)
    {
        var dateLine = SplitCommit(commit, 2);
        return DateTime.Parse(dateLine.Split(' ')[0] + " " + dateLine.Split(' ')[1]);
    }

    public static string GetCommitMessage(string commit)
    {
        var lines = GetCommitLines(commit);
        return string.Join("\n", lines.Skip(2));
    }

    private static string SplitCommit(string commit, int index)
    {
        string[] lines = GetCommitLines(commit);
        var line = lines[1];
        var parts = line.Split(' ');
        return parts[index];
    }

    private static string[] GetCommitLines(string commit)
    {
        return commit.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
        return commit.Contains("test", StringComparison.CurrentCultureIgnoreCase) ||
            commit.Contains("fix", StringComparison.CurrentCultureIgnoreCase);
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
            return string.Empty; // Return empty string if there's any error.
        }
    }
}
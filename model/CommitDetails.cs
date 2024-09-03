namespace Atividade1.model;

public class CommitDetails
{
    public string Hash { get; set; }
    public string Author { get; set; }
    public string Date { get; set; }
    public string Message { get; set; }
    public string[] Files { get; set; }

    public CommitDetails(string hash, string author, string date, string message, string[] files)
    {
        Hash = hash;
        Author = author;
        Date = date;
        Message = message;
        Files = files;
    }

    public CommitDetails()
    {
    }
}


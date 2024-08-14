using System;

public class Email
{
    public int Id { get; set; }
    public string EmailId { get; set; }
    public string ParentId { get; set; }
    public string Subject { get; set; }
    public string TextBody { get; set; }
    public DateTime DateTime { get; set; }

    public Email(int id, string subject)
    {
        Id = id;
        Subject = subject;
    }

    public Email() { }
}

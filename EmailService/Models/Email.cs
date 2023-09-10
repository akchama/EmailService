using System.ComponentModel;

namespace EmailService.Models;

public class EmailAccount
{
    public int Id { get; set; }
    [DefaultValue("user@example.com")]
    public string Address { get; set; }

    public virtual ICollection<EmailMessage> SentEmails { get; set; } = new List<EmailMessage>();
}

public class EmailMessage
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime SentDate { get; set; }
    public int EmailAccountId { get; set; }
    
}
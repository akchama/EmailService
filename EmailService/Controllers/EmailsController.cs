using System.Net;
using EmailService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace EmailService.Controllers
{
    /// <summary>
    /// Prefixing with 'api' is a common convention for RESTful APIs
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmailAccountsController : ControllerBase
    {
        private readonly EmailDbContext _context;

        /// <param name="context"></param>
        public EmailAccountsController(EmailDbContext context)
        {
            _context = context;
            _context.EnsureDatabaseAndSeedData();
        }

        /// <summary>
        /// Get all EmailAccounts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.EmailAccounts.Include(e => e.SentEmails).Distinct().ToList());
        }


        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var emailAccount = _context.EmailAccounts.Include(e => e.SentEmails).FirstOrDefault(e => e.Id == id);
            if (emailAccount == null)
            {
                return NotFound();
            }

            return Ok(emailAccount);
        }


        /// <param name="emailAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(EmailAccount emailAccount)
        {
            if (!IsValidEmail(emailAccount.Address))
                return BadRequest("Invalid email format.");

            _context.EmailAccounts.Add(emailAccount);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = emailAccount.Id },
                emailAccount); // Return the newly created email account
        }

        /// <summary>
        /// Update an existing EmailAccount
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var emailAccount = _context.EmailAccounts.Find(id);
            if (emailAccount == null)
            {
                return NotFound();
            }

            _context.EmailAccounts.Remove(emailAccount);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Get all EmailMessages of a specific EmailAccount
        /// </summary>
        /// <param name="emailAccountId"></param>
        /// <param name="emailMessage"></param>
        /// <returns></returns>

        [HttpPost("{emailAccountId:int}/emailmessages")]
        public IActionResult SendEmailMessage(int emailAccountId, [FromBody] string content)
        {
            var emailAccount = _context.EmailAccounts.Find(emailAccountId);
            if (emailAccount == null)
            {
                return NotFound("Email Account not found.");
            }

            EmailMessage emailMessage = new EmailMessage
            {
                Content = content,
                EmailAccountId = emailAccountId,
                SentDate = DateTime.UtcNow  
            };

            _context.EmailMessages.Add(emailMessage);
            _context.SaveChanges();

            // Actually send the email
            // SendEmail(emailAccount.Address, content);

            return CreatedAtAction(nameof(GetById), new { id = emailMessage.Id }, emailMessage);
        }



        /// <summary>
        /// Get all EmailMessages of a specific EmailAccount
        /// </summary>
        /// <param name="emailAccountId"></param>
        /// <param name="emailMessageId"></param>
        /// <param name="updatedEmailMessage"></param>
        /// <returns></returns>
        [HttpPut("{emailAccountId}/emailmessages/{emailMessageId}")]
        public IActionResult UpdateEmailMessage(int emailAccountId, int emailMessageId,
            EmailMessage updatedEmailMessage)
        {
            var emailAccount = _context.EmailAccounts.Find(emailAccountId);
            if (emailAccount == null)
            {
                return NotFound("Email Account not found.");
            }

            var emailMessage = _context.EmailMessages.Find(emailMessageId);
            if (emailMessage == null)
            {
                return NotFound("Email Message not found.");
            }

            emailMessage.Content = updatedEmailMessage.Content;
            emailMessage.SentDate = updatedEmailMessage.SentDate;
            _context.SaveChanges();

            return NoContent();
        }

        /// <param name="emailAccountId"></param>
        /// <param name="emailMessageId"></param>
        /// <returns></returns>
        [HttpDelete("{emailAccountId:int}/emailmessages/{emailMessageId}")]
        public IActionResult DeleteEmailMessage(int emailAccountId, int emailMessageId)
        {
            var emailAccount = _context.EmailAccounts.Find(emailAccountId);
            if (emailAccount == null)
            {
                return NotFound("Email Account not found.");
            }

            var emailMessage = _context.EmailMessages.Find(emailMessageId);
            if (emailMessage == null)
            {
                return NotFound("Email Message not found.");
            }

            _context.EmailMessages.Remove(emailMessage);
            _context.SaveChanges();

            return NoContent();
        }


        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        private void SendEmail(string recipientAddress, string content)
        {
            // Create a new MailMessage object
            MailMessage mail = new MailMessage();

            // Set the sender (From), recipient (To), subject, and body of the message
            mail.From = new MailAddress("akcamdev@gmail.com", "Abdullah");
            mail.To.Add(recipientAddress);
            mail.Subject = "Test";
            mail.Body = content;

            // Define the SMTP client to send the mail message
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 465,
                Credentials = new NetworkCredential("username", "password"),
                EnableSsl = true,
            };

            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                // Handle exception - e.g. log the failure, return a response to the client, etc.
                throw; // or log the exception
            }
        }
    }
}   
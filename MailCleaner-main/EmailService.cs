using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace EmailDataApp
{
    public class EmailService
    {
        private string connectionString = "server=localhost;database=mailgette;uid=root;password=Root;";

        public async Task<string> GetSalesforceApiKeyAsync()
        {
            string oauthEndpoint = "https://test.salesforce.com/services/oauth2/token";
            string clientId = "3MVG9hz9IjkO5fmUKrCdpGXV9lkDK1qDahfkAedmhgPyajcQJGKsteV3fUdsKZX6kdOboWNG3_WHFiFNGo4Ob";
            string clientSecret = "DD8C619C9DEE22B319DA805565CC8FC5B6740350C385EBFB6BCF6D3F8EA25683";
            string username = "support@ellab.com.testing";
            string password = "Service2024!";

            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, oauthEndpoint);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject jsonResponse = JObject.Parse(responseString);
                    return jsonResponse["access_token"].ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<Email>> FetchEmailsFromApiAsync(string apiKey)
        {
            string getEmailsUrl = "https://ellab--testing.sandbox.my.salesforce.com/services/data/v58.0/query?q=SELECT+Id,Subject,TextBody,ParentId,MessageDate+FROM+EmailMessage";
            List<Email> emails = new List<Email>();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
                HttpResponseMessage response = await client.GetAsync(getEmailsUrl);
                string responseString = await response.Content.ReadAsStringAsync();

                JObject jsonResponse = JObject.Parse(responseString);
                JArray records = (JArray)jsonResponse["records"];

                foreach (var record in records)
                {
                    emails.Add(new Email
                    {
                        EmailId = record["Id"].ToString(),
                        Subject = record["Subject"].ToString(),
                        TextBody = record["TextBody"].ToString(),
                        ParentId = record["ParentId"].ToString(),
                        DateTime = record["MessageDate"] != null ? Convert.ToDateTime(record["MessageDate"]) : DateTime.MinValue
                    });
                }
            }

            return emails;
        }

        public void InsertEmailsIntoDatabase(List<Email> emails)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                foreach (Email email in emails)
                {
                    string sql = "INSERT INTO email (EmailId, ParentId, Subject, TextBody, MessageDate) VALUES (@EmailId, @ParentId, @Subject, @TextBody, @MessageDate)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmailId", email.EmailId);
                        command.Parameters.AddWithValue("@ParentId", email.ParentId);
                        command.Parameters.AddWithValue("@Subject", email.Subject);
                        command.Parameters.AddWithValue("@TextBody", email.TextBody);
                        command.Parameters.AddWithValue("@MessageDate", email.DateTime != DateTime.MinValue ? email.DateTime : (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void SplitEmails()
        {
            string sourceConnectionString = "server=localhost;database=test;uid=dev;password={ANHePa3%d{,uae;";
            string testConnectionString = "server=localhost;database=hop01c;uid=dev;password={ANHePa3%d{,uae;";

            //DataAccessService DAS = new DataAccessService();
            List<Email> emailsDB = new List<Email>();

            using (MySqlConnection connection = new MySqlConnection(sourceConnectionString))
            {
                connection.Open();
                string sql = "SELECT Id, EmailId, ParentId, Subject, TextBody, MessageDate FROM email";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Email email = new Email
                            {
                                Id = reader.GetInt32("Id"),
                                EmailId = reader.IsDBNull(reader.GetOrdinal("EmailId")) ? "" : reader.GetString("EmailId"),
                                ParentId = reader.IsDBNull(reader.GetOrdinal("ParentId")) ? "" : reader.GetString("ParentId"),
                                Subject = reader.IsDBNull(reader.GetOrdinal("Subject")) ? "" : reader.GetString("Subject"),
                                TextBody = reader.IsDBNull(reader.GetOrdinal("TextBody")) ? "" : reader.GetString("TextBody"),
                                DateTime = reader.IsDBNull(reader.GetOrdinal("MessageDate")) ? DateTime.MinValue : reader.GetDateTime("MessageDate")
                            };
                            emailsDB.Add(email);
                        }
                    }
                }
            }

            List<Email> splitEmails = new List<Email>();
            foreach (Email email in emailsDB)
            {
                List<Email> splitMessages = LetsSplit(email);
                splitEmails.AddRange(splitMessages);
            }

            //DAS.InsertEmails(splitEmails, testConnectionString);
            InsertEmailsIntoDatabase(splitEmails);

            Console.WriteLine("Emails have been split and inserted into the test database.");


        }

        static List<Email> LetsSplit(Email email) // Split email into multiple messages
        {
            List<Email> messages = new List<Email>();
            string messagePattern = @"(?:^|\n)From:[^\n]*";
            Regex messageRegex = new Regex(messagePattern);
            MatchCollection splitMessages = messageRegex.Matches(email.TextBody);

            if (splitMessages.Count > 0)
            {
                // Add the first message (from the beginning to the first "From:" field) with original datetime
                messages.Add(new Email
                {
                    Id = email.Id,
                    EmailId = email.EmailId,
                    ParentId = email.ParentId,
                    Subject = email.Subject,
                    TextBody = email.TextBody.Substring(0, splitMessages[0].Index).Trim(),
                    DateTime = email.DateTime
                });




                // Add subsequent messages (from each "From:" field to the next) with extracted Sent datetime. 
                for (int i = 0; i < splitMessages.Count - 1; i++)
                {
                    int startIndex = splitMessages[i].Index;
                    int endIndex = splitMessages[i + 1].Index;
                    string messageText = email.TextBody.Substring(startIndex, endIndex - startIndex).Trim();
                    DateTime sentDateTime = ExtractSentDateTime(messageText);
                    string subject = ExtractSubject(messageText);

                    messages.Add(new Email
                    {
                        Id = email.Id,
                        EmailId = email.EmailId,
                        ParentId = email.ParentId,
                        Subject = subject,
                        TextBody = messageText,
                        DateTime = sentDateTime != DateTime.MinValue ? sentDateTime : email.DateTime // Use extracted Sent datetime if available or original datetime otherwise 
                    });
                }

                // Add the last message (from the last "From:" field to the end) with extracted Sent datetime
                int lastMessageIndex = splitMessages.Count - 1;
                int lastMessageStartIndex = splitMessages[lastMessageIndex].Index;
                string lastMessageText = email.TextBody.Substring(lastMessageStartIndex).Trim();
                DateTime lastSentDateTime = ExtractSentDateTime(lastMessageText);
                string lastSubject = ExtractSubject(lastMessageText);

                messages.Add(new Email
                {
                    Id = email.Id,
                    EmailId = email.EmailId,
                    ParentId = email.ParentId,
                    Subject = lastSubject,
                    TextBody = lastMessageText,
                    DateTime = lastSentDateTime != DateTime.MinValue ? lastSentDateTime : email.DateTime
                });
            }
            else
            {
                // If there's no "From:" field, consider the entire text body as one message with original datetime
                messages.Add(new Email
                {
                    Id = email.Id,
                    EmailId = email.EmailId,
                    ParentId = email.ParentId,
                    Subject = email.Subject,
                    TextBody = email.TextBody.Trim(),
                    DateTime = email.DateTime
                });
            }

            return messages;
        }


        static DateTime ExtractSentDateTime(string messageText) // Extract Sent datetime from message text used in SplitEmail method
        {
            string sentPattern = @"Sent:\s+(.*)";
            Regex sentRegex = new Regex(sentPattern);
            Match sentMatch = sentRegex.Match(messageText);

            if (sentMatch.Success)
            {
                string sentDateStr = sentMatch.Groups[1].Value;
                if (DateTime.TryParse(sentDateStr, out DateTime sentDateTime))
                {
                    return sentDateTime;
                }
            }

            return DateTime.MinValue; // Return DateTime.MinValue if Sent datetime is not found or cannot be parsed
        }

        static string ExtractSubject(string messageText) // Extract subject from message text used in SplitEmail method
        {
            string subjectPattern = @"Subject:\s+(.*)";
            Regex subjectRegex = new Regex(subjectPattern);
            Match subjectMatch = subjectRegex.Match(messageText);

            if (subjectMatch.Success)
            {
                // Extract the subject from the matched group
                string subject = subjectMatch.Groups[1].Value;

                // Return the extracted subject
                return subject;
            }

            // If no subject is found, return a default subject
            return "Default Subject";
        }
    }
}

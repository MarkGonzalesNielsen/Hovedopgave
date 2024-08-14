using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace EmailDataApp
{
    public class EmailCleaner
    {
        private string connectionString = "server=localhost;database=mailgette;uid=root;password=Root;";

        public List<Email> CleanEmails()
        {
            List<Email> emails = new List<Email>();

            // Mønsterarray til fjernelse af uønskede mønstre i e-mailteksten
            string[] patterns = {
                @"From:.*?\n",
                @"Sent:.*?\n",
                @"To:.*?\n",
                @"Cc:.*?\n",
                @"Subject:.*?\n",
                @"Importance:.*?\n",
                @"<[^>]*>",
                @"\[[^\]]*\]",
                @"\S*@\S*\s?",
                @"(\bwww\.\S*|\b\S*\.com\b|\bhttps:\S*)",
                @"(?i)^.*\bregards[\.,\s].*?$",
                @"\bhttps?://\S+\b",
                @"\[cid:[^\]]+\]",
                @"<img\b[^>]*>",
                @"\bhttps?:\/\/\S+\b",
                @"www\.\w+\.\w+<>",
                @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b",
                @"\+?\b[1-9]\d{0,2}[-.\s]?\d{1,3}[-.\s]?\d{2,3}[-.\s]?\d{2,3}\b",
                @"^\s*$\n",
                @"From:.*$",
                @"Sent:.*$",
                @"To:.*$",
                @"Cc:.*$",
                @"Fra:.*$",
                @"Sendt:.*$",
                @"De :.*$",
                @"Envoyé :.*$",
                @"A :.*$",
                @"Cc :.*$",
                @"Objet :.*$",
                @"Best Regards\s*,\s*[\r\n]+.*$",
                @"Best Regards,.*$",
                @"Best Regards\s*.*?$",
                @"(?i)best\s*regards\s*.*?$",
                @"Best Regards\s*\/\s*.*$",
                @"Regards\s*,\s*[\r\n]+.*$",
                @"Regards,.*$",
                @"Regards\s*.*?$",
                @"Regards\s*\/\s*.*$",
                @"BR\s*,\s*[\r\n]+.*$",
                @"BR,.*$",
                @"BR\s*.*?$",
                @"BR\s*\/\s*.*$",
                @"Kind Regards\s*,\s*[\r\n]+.*$",
                @"Kind Regards,.*$",
                @"Kind Regards\s*.*?$",
                @"Kind Regards\s*\/\s*.*$",
                @"Med venlig hilsen\s*,\s*[\r\n]+.*$",
                @"Med venlig hilsen,.*$",
                @"Med venlig hilsen\s*.*?$",
                @"Med venlig hilsen\s*\/\s*.*$",
                @"Thanks again\s*,\s*[\r\n]+.*$",
                @"Thanks again,.*$",
                @"Thanks again\s*.*?$",
                @"Thanks again\s*\/\s*.*$",
                @"Thanks,.*$",
                @"Thanks\s*,\s*[\r\n]+.*$",
                @"Thanks\s*.*?$",
                @"Thanks \s*\/\s*.*$",
                @"Thank you,.*$",
                @"Takker,.*$",
                @"Dear.*$",
                @"Hello.*$",
                @"Hej.*$",
                @"Hey.*$",
                @"Hi.*$",
                @"HI.*$",
                @"Good Morning.*$",
                @"Good morning.*$",
                @"Godmorgen.*$",
                @"Bonjour.*$",
                @"TEL:.*$",
                @"Email:.*$",
                @"E-mail:.*$",
                @"Direct:.*$",
                @"CVR:.*$",
                @"Cell:.*$",
                @"Web:.*$",
                @"SV:.*$",
                @"W :.*$",
                @"mailto:>\s*",
                @"Office:.*$",
                @"Fax:.*$",
                @"Mobile:.*$",
                @"Phone :.*$",
                @"Phone:.*$",
                @"Description:.*$",
                @"Suite:.*$",
                @"EXTERNAL:.*$",
                @"CAUTION:.*$",
                @"Password:.*$",
                @"s*www\.ellab\.com\s*",
                @"s*www\.ellab\.fr\s*",
                @"s*ellab\.de\s*",
                @"C:.*$",
                @":\s*.*$",
                @"\[\s*.*?\]",
                @"\[",
                @"\]",
                @"Key\s+\d+:\s*.*?$",
                @"\[Inline image .*?$",
                @"\[Inline image (URL|name).*?\]",
                @"Inline image .*?$",
                @"^>\s*",
                @"\>",
                @"\<",
                @"\|",
                @"\*",
                @"\/",
                @"\?",
                @"^<>\s*$",
                @"[\*<>]",
                @"^<\/\>\s *$",
                @"^<\$>\s*$",
                @"\?\<\$>",
                @"^\?\<\$>\s*$",
                @"\[\$",
                @"\$",
                @"^\?<>\s*$",
                @"^\?<>",
                @"\?<>\s*",
                @"^(?:\[\]|\|)\s*$",
                @"\?\/.*",
                @".*?<\/\>.*",
                @"\bkey\s+\w+\b",
                @"\bKey\s+\w+\b",
                @"\blicense\s+\w+\b",
                @"\bLicense\s+\w+\b",
                @"\😊",
                @"CaseId:\s*[A-Za-z0-9]+",
                @"MessageDate:\s*\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}"
            };

            using (MySqlConnection connection = new MySqlConnection(connectionString))
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
                            emails.Add(email);
                        }
                    }
                }
            }

            foreach (Email email in emails)
            {
                StringBuilder subjectBuilder = new StringBuilder(email.Subject);
                subjectBuilder.Replace("RE: ", "").Replace("FW: ", "");
                email.Subject = subjectBuilder.ToString();

                StringBuilder bodyBuilder = new StringBuilder(email.TextBody);
                bodyBuilder.Replace("\r\n", " ");
                email.TextBody = bodyBuilder.ToString();

                foreach (string pattern in patterns)
                {
                    bodyBuilder = new StringBuilder(Regex.Replace(bodyBuilder.ToString(), pattern, string.Empty, RegexOptions.Multiline));
                }

                email.TextBody = bodyBuilder.ToString();
            }
            return emails;
        }
    }
}

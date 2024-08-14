using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EmailDataApp
{
    public class EmailPdfGenerator
    {
        private const int BatchSize = 100; // Adjust batch size as needed

        public void SaveEmailsAsPdf(List<Email> emails, bool includeEmailId, bool includeParentId, bool includeSubject, bool includeTextBody, bool includeMessageDate)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "SelectedEmails.pdf");

            Document document = new Document();
            PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            document.Open();

            // Process emails in batches to optimize memory usage
            for (int i = 0; i < emails.Count; i += BatchSize)
            {
                List<Email> batch = emails.GetRange(i, Math.Min(BatchSize, emails.Count - i));
                foreach (Email email in batch)
                {
                    // Add email details to PDF
                    AddEmailDetailsToPdf(document, email, includeEmailId, includeParentId, includeSubject, includeTextBody, includeMessageDate);
                }
            }
            document.Close();
        }

        private void AddEmailDetailsToPdf(Document document, Email email, bool includeEmailId, bool includeParentId, bool includeSubject, bool includeTextBody, bool includeMessageDate)
        {
            // Create a paragraph for each email detail
            if (includeEmailId)
                document.Add(new Paragraph("EmailId: " + email.EmailId));
            if (includeParentId)
                document.Add(new Paragraph("ParentId: " + email.ParentId));
            if (includeSubject)
                document.Add(new Paragraph("Subject: " + email.Subject));
            if (includeTextBody)
            {
                // Process TextBody with regex to remove unwanted patterns
                string cleanedTextBody = CleanTextBody(email.TextBody);
                document.Add(new Paragraph("TextBody: " + cleanedTextBody));
            }
            if (includeMessageDate)
                document.Add(new Paragraph("MessageDate: " + email.DateTime.ToString()));

            document.Add(new Paragraph("\n------------------------------------------------\n"));
        }

        private string CleanTextBody(string textBody)
        {
            // Define an array of regex patterns to remove unwanted content
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

            // Apply each regex pattern to clean up the text body
            foreach (string pattern in patterns)
            {
                textBody = Regex.Replace(textBody, pattern, "");
            }

            return textBody.Trim();
        }
    }
}

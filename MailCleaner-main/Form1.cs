using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Threading.Tasks; 
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace EmailDataApp
{
    public partial class Form1 : Form
    {
        private EmailService emailService;
        private EmailCleaner emailCleaner;
        private EmailPdfGenerator emailPdfGenerator;

        public Form1()
        {
            InitializeComponent();
            emailService = new EmailService();
            emailCleaner = new EmailCleaner();
            emailPdfGenerator = new EmailPdfGenerator();
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            string apiKey = await emailService.GetSalesforceApiKeyAsync();
            if (apiKey == null)
            {
                MessageBox.Show("Failed to retrieve Salesforce API key.");
                return;
            }

            List<Email> emails = await emailService.FetchEmailsFromApiAsync(apiKey);
            emailService.InsertEmailsIntoDatabase(emails);
            List<Email> cleanedEmails = emailCleaner.CleanEmails();
            DisplayEmails(cleanedEmails);
        }

        private void DisplayEmails(List<Email> emails)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("EmailId");
            dataTable.Columns.Add("ParentId");
            dataTable.Columns.Add("Subject");
            dataTable.Columns.Add("TextBody");
            dataTable.Columns.Add("MessageDate");

            foreach (Email email in emails)
            {
                dataTable.Rows.Add(email.EmailId, email.ParentId, email.Subject, email.TextBody, email.DateTime);
            }

            dataGridView1.DataSource = dataTable;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            List<Email> cleanedEmails = emailCleaner.CleanEmails();
            bool includeEmailId = checkBoxEmailId.Checked;
            bool includeParentId = checkBoxParentId.Checked;
            bool includeSubject = checkBoxSubject.Checked;
            bool includeTextBody = checkBoxTextBody.Checked;
            bool includeMessageDate = checkBoxMessageDate.Checked;

            emailPdfGenerator.SaveEmailsAsPdf(cleanedEmails, includeEmailId, includeParentId, includeSubject, includeTextBody, includeMessageDate);
            MessageBox.Show("PDF saved to desktop successfully.");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}

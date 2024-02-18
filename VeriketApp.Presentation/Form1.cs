using VeriketApp.Core;
using Microsoft.Extensions.Options;
using System.ServiceProcess;
using System.Windows.Forms;

namespace VeriketApp.Presentation
{
    public partial class Form1 : Form
    {
        private readonly AppSettings _appSettings;
        public Form1(IOptions<AppSettings> appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings.Value;
            CheckServiceStatus();
        }
        private void CheckServiceStatus()
        {
            try
            {
                ServiceController sc = new ServiceController(_appSettings.ServiceName);
                string status = sc.Status.ToString();
                label1.Text = $"Service Status : {status}";
            }
            catch (Exception)
            {
                label1.Text = $"Service Doesnt load";
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckServiceStatus();
         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckServiceStatus();
            FillGrid(Path.Combine(_appSettings.BaseFilePath, _appSettings.ServiceName, _appSettings.LogFileName));
        }
        private void FillGrid(string filePath)
        {
            List<string[]> data = new List<string[]>();
            List<CustomLogRecord> itmSrc = new();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        data.Add(values);
                    }
                }


                foreach (var row in data)
                {
                    itmSrc.Add(new CustomLogRecord { CreatedOnUtc = row[0], MachineName = row[1], UserName = row[2] });
                }
                var source = new BindingSource();
                source.DataSource = itmSrc.ToList().OrderByDescending(x => x.CreatedOnUtc);
                dataGridView1.DataSource = source;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Something went wrong {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public class CustomLogRecord
        {
            public string CreatedOnUtc { get; set; }
            public string MachineName { get; set; }
            public string UserName { get; set; }
        }
    }
}

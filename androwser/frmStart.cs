using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace androwser
{
    public partial class frmStart : Form
    {
        Schedule schedule = new Schedule();

        public frmStart()
        {
            InitializeComponent();
        }

        private void frmStart_Load(object sender, EventArgs e)
        {
            if (StartApp())
            {
                if (IsValidSchedule())
                {
                    this.timVerify.Start();
                    LogText("Verificação iniciada...");
                }
            }
        }

        private void LogText(string text)
        {
            this.txtLog.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {text}\r\n";
            this.txtLog.SelectionStart = this.txtLog.Text.Length;
            this.txtLog.SelectionLength = 0;
        }

        private bool StartApp()
        {
            string scheduleFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
            scheduleFilePath += $"\\UrlList.json";
            string jsonText = string.Empty;
            LogText("Aplicação iniciada");
            LogText($"Carregando arquivo de agendamento: {scheduleFilePath}");
            try
            {
                jsonText = File.ReadAllText(scheduleFilePath);
            }
            catch (Exception ex)
            {
                LogText("Falha durante a carga do arquivo de agendamento:");
                LogText($"{ex.Message}");
                LogText("Execução interrompida.");
                return false;
            }
            try
            {
                schedule = JsonConvert.DeserializeObject<Schedule>(jsonText);
            }
            catch (Exception ex)
            {
                LogText("Falha durante a carga do arquivo de agendamento:");
                LogText($"{ex.Message}");
                LogText("Execução interrompida.");
                return false;
            }
            return true;
        }

        private bool IsValidSchedule()
        {
            bool valid = true;
            foreach (ScheduleData scheduleEntry in schedule.urls)
            {
                string url = scheduleEntry.url;
                string wakeup = $"{DateTime.Now.ToString("yyyy-MM-dd")} {scheduleEntry.wakeup}";
                string die = $"{DateTime.Now.ToString("yyyy-MM-dd")} {scheduleEntry.die}";
                string logText = $"Wakeup: {wakeup} Die: {die} Url: {url}";
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    valid = false;
                    logText += " (URL inválida)";
                }
                if (!IsDateTime(wakeup))
                {
                    valid = false;
                    logText += " (Wakeup Inválido)";
                }
                if (!IsDateTime(die))
                {
                    valid = false;
                    logText += " (Die Inválido)";
                }
                if (string.Compare(wakeup, die, StringComparison.Ordinal) >= 0)
                {
                    valid = false;
                    logText += " (Die inferior ou igual ao wakeup)";
                }
                LogText(logText);
            }
            if (!valid)
            {
                LogText("Corrija seu aruivo schedule!");
                LogText("Execução interrompida.");
            }
            return valid;
        }
        public static bool IsDateTime(string txtDate)
        {
            DateTime tempDate;
            return DateTime.TryParse(txtDate, out tempDate);
        }

        private void timVerify_Tick(object sender, EventArgs e)
        {
            DateTime rightNow = DateTime.Now;
            var notFired = schedule.urls.Where(item =>
            {
                string wakeup = $"{rightNow.ToString("yyyy-MM-dd")} {item.wakeup}";
                string passed = $"{rightNow.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm")}";
                if (string.Compare(wakeup, passed, StringComparison.Ordinal) <= 0)
                    return false;
                else
                    return true;
            });
            string now = $"{rightNow.ToString("yyyy-MM-dd HH:mm")}";
            foreach (var item in notFired)
            {
                string wakeup = $"{rightNow.ToString("yyyy-MM-dd")} {item.wakeup}";
                if (wakeup == now)
                {
                    LogText($"Wakeup {wakeup}! Abrindo navegador: ${item.url}");
                    frmBrowser browser = new frmBrowser(item.url);
                    item.browser = browser;
                    browser.Show();
                }
            }
            foreach (var item in schedule.urls)
            {
                string die = $"{rightNow.ToString("yyyy-MM-dd")} {item.die}";
                if (die == now && item.browser != null)
                {
                    LogText($"Die {die}! Fechando navegador: ${item.url}");
                    try
                    {
                        item.browser.Dispose();
                        item.browser = null;
                    }
                    catch { }
                }
            }
        }
    }
}

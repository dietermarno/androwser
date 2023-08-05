using System.Windows.Forms;

namespace androwser
{
    public partial class frmBrowser : Form
    {
        private EO.WinForm.WebControl webControl1;
        private EO.WebBrowser.WebView webView1;

        public frmBrowser(string url)
        {
            InitializeComponent();
            webControl1 = new EO.WinForm.WebControl();
            webView1 = new EO.WebBrowser.WebView();
            webControl1.Dock = DockStyle.Fill;
            webControl1.WebView = webView1;
            webView1.Url = url;
            Controls.Add(webControl1);
        }
    }
}

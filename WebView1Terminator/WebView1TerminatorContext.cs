using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebView1Terminator
{
    public sealed class WebView1TerminatorContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;

        public WebView1TerminatorContext()
        {
            var icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("WebView1Terminator.App.ico"), 64, 64);
            trayIcon = new NotifyIcon()
            {
                Icon = icon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Terminate WebView1", TerminateProcesses),
                    new MenuItem("Exit", (s, e) =>
                    {
                        trayIcon.Visible = false;
                        Application.Exit();
                    })
                }),
                Visible = true
            };
            trayIcon.Text = "WebView1 terminator";
            trayIcon.DoubleClick += TerminateProcesses;
        }

        private static void TerminateProcesses(object sender, EventArgs e)
        {
            var count = 0;
            var failed = 0;
            foreach (var process in Process.GetProcesses().
                Where(process =>
                    process.StartInfo.Arguments.Contains("-ServerName:Windows.Internal.WebView.OopWebViewServer") ||
                    process.StartInfo.Arguments.Contains("Win32WebViewHost.exe")))
            {
                try
                {
                    process.Kill();
                    count++;
                }
                catch
                {
                    failed++;
                }
            }

            if ((count >= 1) || (failed >= 1))
            {
                MessageBox.Show($"Terminated {count}/{count + failed} processes.", "WebView1 terminator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show($"WebView1 related process not found.", "WebView1 terminator", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }
    }
}

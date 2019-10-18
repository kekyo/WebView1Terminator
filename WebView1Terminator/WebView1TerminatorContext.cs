using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace WebView1Terminator
{
    public sealed class WebView1TerminatorContext : ApplicationContext
    {
        private static readonly string title =
            $"WebView1 terminator {typeof(WebView1TerminatorContext).Assembly.GetName().Version}";

        private readonly NotifyIcon trayIcon;

        public WebView1TerminatorContext()
        {
            var icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("WebView1Terminator.App.ico"), 64, 64);
            trayIcon = new NotifyIcon()
            {
                Icon = icon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Terminate WebView1", TerminateProcesses),
                    new MenuItem("About...", (s, e) =>
                    {
                        MessageBox.Show(
                            "WebView1 terminator - WPF WebView1 stub process terminator utility.\r\nCopyright (c) 2019 Kouji Matsui.\r\nhttps://github.com/kekyo/WebView1Terminator\r\nLicense under Apache-v2",
                            title,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }),
                    new MenuItem("Exit", (s, e) =>
                    {
                        trayIcon.Visible = false;
                        Application.Exit();
                    })
                }),
                Visible = true
            };
            trayIcon.Text = title;
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
                MessageBox.Show(
                    $"Terminated {count}/{count + failed} processes.",
                    title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show(
                    $"WebView1 related process not found.",
                    title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Question);
            }
        }
    }
}

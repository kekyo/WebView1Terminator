using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
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

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process"))
            {
                using (var objects = searcher.Get())
                {
                    foreach (var process in
                        from obj in objects.Cast<ManagementBaseObject>()
                        let properties = obj.Properties.Cast<PropertyData>().ToDictionary(data => data.Name, data => data.Value)
                        let argument = (properties.TryGetValue("CommandLine", out var value) ? value?.ToString().ToLowerInvariant() : null) ?? string.Empty
                        where
                            argument.Contains("-servername:windows.internal.webview.oopwebviewserver") ||
                            argument.Contains("win32webviewhost.exe")
                        let process = int.TryParse(obj.GetPropertyValue("ProcessId")?.ToString() ?? string.Empty, out var id) ? Process.GetProcessById(id) : null
                        where process != null
                        select process)
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

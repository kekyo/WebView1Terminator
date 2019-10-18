# WebView1 terminator

# What's this?

Windows [WPF WebView1 (Legacy Edge-based web browser components)](https://github.com/windows-toolkit/WindowsCommunityToolkit) stub process terminator utility.

The component invoke background proxy-stub processes named "Win32WebViewHost.exe" and "WWAHost.exe". It will turn to zombie each if crash hosted (our) application process, and they cause heavy loading CPU cycles!

![zombie processes](Images/zombie-processes.png)

This utility will terminate all WebView1 zombie processes (included not zombies :) You have to understand when use it.)

# How to use

Download built binary (net45): [WebView1Terminator-1.0.2.zip](https://github.com/kekyo/WebView1Terminator/releases/download/1.0.2/WebView1Terminator-1.0.2.zip)

Easy, double-click system tray icon or click context menu.

![system tray icon](Images/screen-shot.png)

# License

Apache-v2

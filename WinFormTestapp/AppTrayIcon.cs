namespace SimpleLogitechLcdApp
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    class AppTrayIcon : IDisposable
    {
        private NotifyIcon notifyIcon;
        private Program program;

        public AppTrayIcon(Program p)
        {
            notifyIcon = new NotifyIcon();
            program = p;
        }

        public void Display()
        {
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); ;
            notifyIcon.Text = "Logitech LCD App";
            notifyIcon.Visible = true;

            // Attach a context menu.
            notifyIcon.ContextMenuStrip = new ContextMenu().Create();
        }

        public void Dispose()
        {
            notifyIcon.Visible = false;
            program.Terminate();
            notifyIcon.Dispose();
        }
    }
}

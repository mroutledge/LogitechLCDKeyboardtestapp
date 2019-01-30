namespace SimpleLogitechLcdApp
{
    using System;
    using System.Windows.Forms;

    class ContextMenu
    {
        private bool isAboutLoaded = false;

        public ContextMenuStrip Create()
        {
            // Add the default menu options.
            ContextMenuStrip menu = new ContextMenuStrip();

            // About.
            ToolStripMenuItem about = new ToolStripMenuItem();
            about.Text = "About";
            about.Click += new EventHandler(About_Click);
            menu.Items.Add(about);

            // Separator
            menu.Items.Add(new ToolStripSeparator());

            // Exit.
            ToolStripMenuItem exit = new ToolStripMenuItem();
            exit.Text = "Exit";
            exit.Click += new EventHandler(Exit_Click);
            menu.Items.Add(exit);

            return menu;
        }

        void About_Click(object sender, EventArgs e)
        {
            if (!isAboutLoaded)
            {
                isAboutLoaded = true;
                new About().ShowDialog();
                isAboutLoaded = false;
            }
        }

        void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

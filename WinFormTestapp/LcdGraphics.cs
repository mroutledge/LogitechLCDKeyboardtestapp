namespace SimpleLogitechLcdApp
{
    using GammaJul.LgLcd;
    using System.Drawing;
    using System.IO;
    using System.Reflection;

    class LcdGraphics
    {
        private LcdDevice lcd;
        private readonly Image mainIcon;
        private readonly Image xMainIcon;

        private LcdGdiPage mainPage;
        private LcdGdiPage warningPage;

        public LcdGraphics(LcdDevice lcd)
        {
            //set device
            this.lcd = lcd;

            // Get the images from the assembly
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WinFormTestapp.derpIcon.bmp"))
                mainIcon = Image.FromStream(stream);

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WinFormTestapp.derpIcon_offline.bmp"))
                xMainIcon = Image.FromStream(stream);

            CreateGdiPage();
        }

        /// <summary>
        /// Generate the GDI pages
        /// </summary>
        public void CreateGdiPage()
        {
            mainPage = new LcdGdiPage(lcd)
            {
                Children = {
                    new LcdGdiImage(mainIcon),
                    new LcdGdiText {
                        Text = "This has been a test",
                        Margin = new MarginF(34.0f, 0.0f, 2.0f, 0.0f)
                    }
                }
            };

            warningPage = new LcdGdiPage(lcd)
            {
                Children = {
                    new LcdGdiImage(xMainIcon),
                    new LcdGdiText {
                        Text = "Don't do that again",
                        Margin = new MarginF(34.0f, 0.0f, 2.0f, 0.0f)
                    }
                }
            };

            lcd.Pages.Add(mainPage);
            //I've not added the warning page but it doesn't appear to be an issue
            lcd.CurrentPage = mainPage;
        }

        /// <summary>
        /// Sets the current page to the warning page
        /// </summary>
        public void SetWarning()
        {
            lcd.CurrentPage = warningPage;
        }

        /// <summary>
        /// Sets the current page to the main 'test' page
        /// </summary>
        public void SetMain()
        {
            lcd.CurrentPage = mainPage;
        }

    }
}

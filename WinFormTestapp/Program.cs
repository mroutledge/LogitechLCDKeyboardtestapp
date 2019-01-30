/// <summary>
/// Testing the LCD keyboard from logitech
/// </summary>
namespace SimpleLogitechLcdApp
{
    using GammaJul.LgLcd;
    using System;
    using System.Threading;
    using System.Windows.Forms;

    internal class Program
    {
        /// <summary>
        /// The LCD device type
        /// </summary>
        private LcdDeviceType deviceType;

        /// <summary>
        /// Track device arrival event
        /// </summary>
        private readonly AutoResetEvent waitAutoResetEvent = new AutoResetEvent(false);

        //keyboard state
        private volatile bool arrived = false;

        /// <summary>
        /// Graphics to be pushed to LCD
        /// </summary>
        private LcdGraphics lcdGraphics;

        //track button keypress
        private bool button1Pressed = false;
        private bool button2Pressed = false;
        private bool button3Pressed = false;
        private bool button4Pressed = false;

        /// <summary>
        /// Terminate flag for program
        /// </summary>
        private volatile bool stillAlive = true;

        /// <summary>
        /// Tray app thread
        /// </summary>
        private Thread trayApp;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            Program program = new Program();

            program.StartTrayIcon();

            LcdApplet app = new LcdApplet("Test Logitech LCD Keyboard", LcdAppletCapabilities.Both);

            //set event handling
            app.Configure += AppletConfigure;
            app.DeviceArrival += program.DeviceArrival;
            app.DeviceRemoval += program.DeviceRemoval;
            app.IsEnabledChanged += program.AppletIsEnabledChanged;
            app.Connect();

            // We are waiting for the handler thread to warn us for device arrival
            LcdDeviceMonochrome monoDevice = null;
            LcdDeviceQvga qvgaDevice = null;

            program.waitAutoResetEvent.WaitOne();

            //enter main loop
            do
            {
                //Handle device arrival
                if (program.arrived && program.deviceType == LcdDeviceType.Monochrome)
                {
                    if (monoDevice == null)
                    {
                        monoDevice = (LcdDeviceMonochrome)app.OpenDeviceByType(program.deviceType);
                        monoDevice.SoftButtonsChanged += program.MonoDeviceSoftButtonsChanged;
                        program.lcdGraphics = new LcdGraphics(monoDevice);
                        monoDevice.SetAsForegroundApplet = true;
                    }
                    else
                    {
                        monoDevice.ReOpen();
                    }

                    //reset the arrival flag
                    program.arrived = false;
                }
                if (program.arrived && program.deviceType == LcdDeviceType.Qvga)
                {
                    if (qvgaDevice == null )
                    {
                        qvgaDevice = (LcdDeviceQvga)app.OpenDeviceByType(program.deviceType);
                        qvgaDevice.SoftButtonsChanged += program.MonoDeviceSoftButtonsChanged;
                        program.lcdGraphics = new LcdGraphics(qvgaDevice);
                        qvgaDevice.SetAsForegroundApplet = true;
                    }
                    else
                    {
                        qvgaDevice.ReOpen();
                    }

                    //reset the arrival flag
                    program.arrived = false;
                }

                //if there is no lcd keyboard, sleep for a while and look again                
                if (monoDevice == null && qvgaDevice == null)
                {
                    Thread.Sleep(20000);
                    continue;
                }

                //Disposed, skip draw, next check loop exits
                if (monoDevice != null && monoDevice.IsDisposed 
                    || qvgaDevice != null && qvgaDevice.IsDisposed)
                {
                    continue;
                }

                //update and draw
                if (app.IsEnabled)
                {
                    if (program.button1Pressed)
                    {
                        //set warning page in object and redraw
                        program.lcdGraphics.SetWarning();
                        monoDevice.DoUpdateAndDraw();

                        //wait and reset
                        Thread.Sleep(2000);
                        program.lcdGraphics.SetMain();

                        //reset flag
                        program.button1Pressed = false;
                    }

                    monoDevice.DoUpdateAndDraw();

                    //let the user enjoy the magical LCD graphics
                    Thread.Sleep(30);
                }
            }
            while (program.stillAlive);
        }

        /// <summary>
        /// start tray icon thread
        /// </summary>
        private void StartTrayIcon()
        {
            trayApp = new Thread(delegate ()
            {
                using (AppTrayIcon ati = new AppTrayIcon(this))
                {
                    ati.Display();
                    Application.Run();
                }
            });

            trayApp.SetApartmentState(ApartmentState.STA);
            trayApp.Start();
        }

        // Event handler for new device arrical in the system.
        // Monochrome devices include (G510, G13, G15, Z10)
        private void DeviceArrival(object sender, LcdDeviceTypeEventArgs e)
        {
            SetDeviceType(e.DeviceType);
            arrived = true;
            //inform main thread the device is avalable
            waitAutoResetEvent.Set();
        }

        /// Event handler for Configure button click (for this applet) in the LCD Manager.
        private static void AppletConfigure(object sender, EventArgs e)
        {
            // No action required
        }

        // Event handler for device removal
        private void DeviceRemoval(object sender, LcdDeviceTypeEventArgs e)
        {
            // No action required
        }

        // Event handler for applet enablement or disablement in the LCD Manager
        private void AppletIsEnabledChanged(object sender, EventArgs e)
        {
            // No action required
        }

        /// This event handler is called whenever the soft buttons are pressed or released.
        private void MonoDeviceSoftButtonsChanged(object sender, LcdSoftButtonsEventArgs e)
        {
            var device = (LcdDevice)sender;

            // First button 
            if ((e.SoftButtons & LcdSoftButtons.Button0) == LcdSoftButtons.Button0)
            {
                button1Pressed = true;
            }
            if ((e.SoftButtons & LcdSoftButtons.Button1) == LcdSoftButtons.Button0)
            {
                button2Pressed = true;
            }
            if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button0)
            {
                button3Pressed = true;
            }
            if ((e.SoftButtons & LcdSoftButtons.Button3) == LcdSoftButtons.Button0)
            {
                button4Pressed = true;
            }
        }

        /// <summary>
        /// set the device type
        /// </summary>
        /// <param name="LcdType"></param>
        private void SetDeviceType(LcdDeviceType LcdType)
        {
            deviceType = LcdType;
        }

        public void Terminate()
        {
            stillAlive = false;
            lcdGraphics = null;
            trayApp.Abort();
        }

    }
}

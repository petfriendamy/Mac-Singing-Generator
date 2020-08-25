using System;

using Foundation;
using AppKit;

namespace MacSingingGenerator
{
    public partial class MainWindowController : NSWindowController
    {
        public MainWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MainWindowController(NSCoder coder) : base(coder)
        {
        }

        public MainWindowController() : base("MainWindow")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            Window.WillClose += Window_WillClose;
            Window.GetSystemVoices();
            Window.UpdateTitle();
        }

        public void Window_WillClose(object sender, EventArgs e)
        {
            if (!AppDelegate.IsQuitting) //stops from hanging on exit
            {
                Window.AlertIfUnsavedChanges(true);
                AppDelegate.IsQuitting = true;
            }
        }

        public new MainWindow Window
        {
            get { return (MainWindow)base.Window; }
        }
    }
}

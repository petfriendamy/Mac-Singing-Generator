using System;

using Foundation;
using AppKit;

namespace MacSingingGenerator
{
    public partial class TrackChooserWindowController : NSWindowController
    {
        public TrackChooserWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public TrackChooserWindowController(NSCoder coder) : base(coder)
        {
        }

        public TrackChooserWindowController() : base("TrackChooserWindow")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new TrackChooserWindow Window
        {
            get { return (TrackChooserWindow)base.Window; }
        }
    }
}

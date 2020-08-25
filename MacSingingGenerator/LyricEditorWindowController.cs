using System;

using Foundation;
using AppKit;

namespace MacSingingGenerator
{
    public partial class LyricEditorWindowController : NSWindowController
    {
        public LyricEditorWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public LyricEditorWindowController(NSCoder coder) : base(coder)
        {
        }

        public LyricEditorWindowController() : base("LyricEditorWindow")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new LyricEditorWindow Window
        {
            get { return (LyricEditorWindow)base.Window; }
        }
    }
}

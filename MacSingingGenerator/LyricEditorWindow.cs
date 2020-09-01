using System;

using Foundation;
using AppKit;

using SingingGenerator.Core;

namespace MacSingingGenerator
{
    public partial class LyricEditorWindow : NSWindow
    {
        public Phoneme[][] Lyrics { get; private set; } = null;

        public LyricEditorWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public LyricEditorWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public override bool WorksWhenModal()
        {
            return true;
        }

        partial void OKButton(NSObject sender)
        {
            try
            {
                Lyrics = SpeechSynthesizer.GetPhonemesFromText(LyricTextView.Value);
                NSApplication.SharedApplication.StopModal();
                Close();
            }
            catch (FormatException ex)
            {
                Alert.CreateAlert(ex.Message, AlertType.Caution);
            }
        }

        partial void CancelButton(NSObject sender)
        {
            NSApplication.SharedApplication.StopModal();
            Close();
        }
    }
}

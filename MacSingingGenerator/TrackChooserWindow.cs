using System;

using Foundation;
using AppKit;

namespace MacSingingGenerator
{
    public partial class TrackChooserWindow : NSWindow
    {
        public int Track { get; private set; }

        public TrackChooserWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public TrackChooserWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public void SetTrackList(string[] tracks)
        {
            if (tracks.Length > 0)
            {
                TrackPopup.RemoveAllItems();
                foreach (var track in tracks)
                {
                    TrackPopup.AddItem(track);
                }
                TrackPopup.SelectItem(0);
            }
        }

        partial void ChooseTrack(NSObject sender)
        {
            TrackInfoTextField.StringValue = "testing";
        }

        partial void OKButton(NSObject sender)
        {
            Track = (int)TrackPopup.IndexOfSelectedItem;
            NSApplication.SharedApplication.StopModal();
            Close();
        }

        partial void CancelButton(NSObject sender)
        {
            Track = -1;
            NSApplication.SharedApplication.StopModal();
            Close();
        }
    }
}

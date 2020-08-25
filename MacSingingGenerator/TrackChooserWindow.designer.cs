// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacSingingGenerator
{
	[Register ("TrackChooserWindow")]
	partial class TrackChooserWindow
	{
		[Outlet]
		AppKit.NSTextField TrackInfoTextField { get; set; }

		[Outlet]
		AppKit.NSPopUpButton TrackPopup { get; set; }

		[Action ("CancelButton:")]
		partial void CancelButton (Foundation.NSObject sender);

		[Action ("ChooseTrack:")]
		partial void ChooseTrack (Foundation.NSObject sender);

		[Action ("OKButton:")]
		partial void OKButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (TrackPopup != null) {
				TrackPopup.Dispose ();
				TrackPopup = null;
			}

			if (TrackInfoTextField != null) {
				TrackInfoTextField.Dispose ();
				TrackInfoTextField = null;
			}
		}
	}
}

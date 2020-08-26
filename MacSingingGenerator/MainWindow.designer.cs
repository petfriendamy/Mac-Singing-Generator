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
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		AppKit.NSButton IsDottedCheck { get; set; }

		[Outlet]
		AppKit.NSButton IsTripletCheck { get; set; }

		[Outlet]
		AppKit.NSPopUpButton LengthMenu { get; set; }

		[Outlet]
		AppKit.NSPopUpButton NoteMenu { get; set; }

		[Outlet]
		AppKit.NSPopUpButton OctaveMenu { get; set; }

		[Outlet]
		AppKit.NSTextView OutputTextView { get; set; }

		[Outlet]
		AppKit.NSPopUpButton SystemVoicesMenu { get; set; }

		[Outlet]
		AppKit.NSTextField TempoField { get; set; }

		[Action ("AddButton:")]
		partial void AddButton (Foundation.NSObject sender);

		[Action ("AddRestButton:")]
		partial void AddRestButton (Foundation.NSObject sender);

		[Action ("AddTempoChangeButton:")]
		partial void AddTempoChangeButton (Foundation.NSObject sender);

		[Action ("ChangeTempoButton:")]
		partial void ChangeTempoButton (Foundation.NSObject sender);

		[Action ("PlayVoiceButton:")]
		partial void PlayVoiceButton (Foundation.NSObject sender);

		[Action ("RemoveButton:")]
		partial void RemoveButton (Foundation.NSObject sender);

		[Action ("RemoveTempoChangeButton:")]
		partial void RemoveTempoChangeButton (Foundation.NSObject sender);

		[Action ("SaveVoiceButton:")]
		partial void SaveVoiceButton (Foundation.NSObject sender);

		[Action ("StopVoiceButton:")]
		partial void StopVoiceButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (IsDottedCheck != null) {
				IsDottedCheck.Dispose ();
				IsDottedCheck = null;
			}

			if (IsTripletCheck != null) {
				IsTripletCheck.Dispose ();
				IsTripletCheck = null;
			}

			if (LengthMenu != null) {
				LengthMenu.Dispose ();
				LengthMenu = null;
			}

			if (NoteMenu != null) {
				NoteMenu.Dispose ();
				NoteMenu = null;
			}

			if (OctaveMenu != null) {
				OctaveMenu.Dispose ();
				OctaveMenu = null;
			}

			if (OutputTextView != null) {
				OutputTextView.Dispose ();
				OutputTextView = null;
			}

			if (SystemVoicesMenu != null) {
				SystemVoicesMenu.Dispose ();
				SystemVoicesMenu = null;
			}

			if (TempoField != null) {
				TempoField.Dispose ();
				TempoField = null;
			}
		}
	}
}

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
	[Register ("LyricEditorWindow")]
	partial class LyricEditorWindow
	{
		[Outlet]
		AppKit.NSTextView LyricTextView { get; set; }

		[Action ("CancelButton:")]
		partial void CancelButton (Foundation.NSObject sender);

		[Action ("OKButton:")]
		partial void OKButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (LyricTextView != null) {
				LyricTextView.Dispose ();
				LyricTextView = null;
			}
		}
	}
}

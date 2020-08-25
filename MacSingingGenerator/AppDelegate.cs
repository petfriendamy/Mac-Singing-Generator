using AppKit;
using Foundation;

namespace MacSingingGenerator
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        MainWindowController mainWindowController;
        LyricEditorWindow lyricEditor;
        public static bool IsQuitting = false;

        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            mainWindowController = new MainWindowController();
            mainWindowController.Window.MakeKeyAndOrderFront(this);
        }

        public override void WillTerminate(NSNotification notification)
        {
            if (!IsQuitting) //prevents duplicate notifications
            {
                mainWindowController.Window.AlertIfUnsavedChanges(true);
                IsQuitting = true;
            }
        }

        [Export("applicationShouldTerminateAfterLastWindowClosed:")]
        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }

        partial void NewFile(NSObject sender)
        {
            mainWindowController.Window.NewSong();
        }

        partial void OpenFile(NSObject sender)
        {
            mainWindowController.Window.OpenFile();
        }

        partial void CloseFile(NSObject sender)
        {
            mainWindowController.Window.NewSong();
        }

        partial void SaveFile(NSObject sender)
        {
            mainWindowController.Window.SaveFile(false);
        }

        partial void SaveFileAs(NSObject sender)
        {
            mainWindowController.Window.SaveFile(true);
        }

        partial void ChangeLyrics(NSObject sender)
        {
            using (var temp = new LyricEditorWindowController())
            {
                lyricEditor = temp.Window;
            }
            NSApplication.SharedApplication.RunModalForWindow(lyricEditor);
            if (lyricEditor.Lyrics != null)
            {
                mainWindowController.Window.ChangeLyrics(lyricEditor.Lyrics);
            }
            lyricEditor = null;
        }

        partial void OctaveUp(NSObject sender)
        {
            mainWindowController.Window.TransposeSong(1);
        }

        partial void OctaveDown(NSObject sender)
        {
            mainWindowController.Window.TransposeSong(-1);
        }

        partial void ImportMidi(NSObject sender)
        {
            mainWindowController.Window.ImportMidi();
            /*using (var alert = new NSAlert())
            {
                alert.MessageText = "This feature is a work in progress, and may not work as intended. Continue anyway?";
                alert.AddButton("Yes"); //1000
                alert.AddButton("No"); //1001
                var result = alert.RunModal();
                if (result == 1000)
                {
                    mainWindowController.Window.ImportMidi();
                }
            }*/
        }
    }
}

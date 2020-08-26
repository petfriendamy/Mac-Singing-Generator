using System;
using System.Collections.Generic;
using System.IO;

using Foundation;
using AppKit;

using SingingGenerator.Core;

namespace MacSingingGenerator
{
    public partial class MainWindow : NSWindow
    {
        private Song song = new Song();
        private NSUrl currentFile;
        public bool UnsavedChanges { get; private set; }
        private struct NoteLength
        {
            public int Length;
            public int NotesPerBeat;
        }

        public MainWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MainWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        //get the available system voices and add them to the dropdown list
        public void GetSystemVoices()
        {
            SystemVoicesMenu.RemoveAllItems();
            foreach (var voice in SpeechSynthesizer.SystemVoices)
            {
                SystemVoicesMenu.AddItem(voice.Name);
            }
            SystemVoicesMenu.SelectItem(0);
        }

        //update the window title with the current file (and saved status)
        public void UpdateTitle()
        {
            string title = "Mac Singing Generator - ";
            if (currentFile == null)
            {
                title += "<untitled>";
            }
            else
            {
                title += currentFile.LastPathComponent;
            }
            if (UnsavedChanges)
            {
                title += "*";
            }
            Title = title;
        }

        //update the output with the new speech script
        private void UpdateOutput(bool changed)
        {
            OutputTextView.Value = song.GetSpeechSynthesisScript();
            UnsavedChanges = changed;
            UpdateTitle();
        }

        //alert the user of unsaved changes and allow them the chance to save the file
        public bool AlertIfUnsavedChanges(bool appIsClosing = false)
        {
            if (UnsavedChanges)
            {
                using (var alert = new NSAlert())
                {
                    alert.MessageText = "There are unsaved changes! Do you want to save them first?";
                    alert.AddButton("Yes"); //1000
                    alert.AddButton("No"); //1001
                    if (!appIsClosing) { alert.AddButton("Cancel"); } //1002 
                    var result = alert.RunModal();
                    if (result == 1000)
                    {
                        SaveFile(false);
                    }
                    if (result == 1002)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //create a blank file
        public void NewSong()
        {
            if (AlertIfUnsavedChanges())
            {
                song = new Song();
                OutputTextView.Value = "";
                UnsavedChanges = false;
                currentFile = null;
                UpdateTitle();
            }
        }

        //open an existing speech script
        public void OpenFile()
        {
            if (AlertIfUnsavedChanges())
            {
                NSUrl loadPath = null;
                using (var openDialog = new NSOpenPanel())
                {
                    openDialog.AllowedFileTypes = new string[] { "txt" };
                    var result = openDialog.RunModal();
                    if (result == 1)
                    {
                        loadPath = openDialog.Url;
                    }
                }
                if (loadPath != null)
                {
                    try
                    {
                        song.ImportFromSpeechSynthesisScript(loadPath.RelativePath);
                        TempoField.StringValue = song.Tempo.ToString();
                        currentFile = loadPath;
                        UpdateOutput(false);
                    }
                    catch (IOException ex)
                    {
                        using (var alert = new NSAlert())
                        {
                            alert.MessageText = $"An error occured while loading the file. ({ex.Message})";
                            alert.RunModal();
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        using (var alert = new NSAlert())
                        {
                            alert.MessageText = "File cannot be accessed.";
                            alert.RunModal();
                        }
                    }
                    catch (FormatException ex)
                    {
                        using (var alert = new NSAlert())
                        {
                            alert.MessageText = ex.Message;
                            alert.RunModal();
                        }
                    }
                }
            }
        }

        //save the existing speech script to a text file
        public void SaveFile(bool newFile)
        {
            NSUrl savePath = null;
            if (newFile || currentFile == null) //new file or save as
            {
                using (var saveDialog = new NSSavePanel())
                {
                    var result = saveDialog.RunModal();
                    if (result == 1)
                    {
                        if (!saveDialog.Url.RelativePath.EndsWith(".txt", StringComparison.CurrentCulture))
                        {
                            savePath = saveDialog.Url.AppendPathExtension("txt");
                        }
                    }
                }
            }
            else { savePath = currentFile; }

            //if a file path is specified, save the file
            if (savePath != null)
            {
                try
                {
                    using (var fileStream = new FileStream(savePath.RelativePath, FileMode.Create))
                    {
                        using (var streamWriter = new StreamWriter(fileStream))
                        {
                            streamWriter.WriteLine(song.GetSpeechSynthesisScript());
                        }
                    }
                    currentFile = savePath;
                    UnsavedChanges = false;
                    UpdateTitle();
                }
                catch (IOException ex)
                {
                    using (var alert = new NSAlert())
                    {
                        alert.MessageText = $"An error occured while saving the file. ({ex.Message})";
                        alert.RunModal();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    using (var alert = new NSAlert())
                    {
                        alert.MessageText = "File cannot be accessed.";
                        alert.RunModal();
                    }
                }
            }
        }

        //set song lyrics from a phoneme array
        public void ChangeLyrics(Phoneme[][] phonemes)
        {
            song.SetLyricsFromPhonemeArray(phonemes);
            UpdateOutput(true);
        }

        //change key/octave of the whole song
        public void TransposeSong(int octave)
        {
            if (song.TransposeSong(octave))
            {
                UpdateOutput(true);
            }
            else
            {
                using (var alert = new NSAlert())
                {
                    if (octave > 0)
                    {
                        alert.MessageText = "Can't go any higher!";
                    }
                    else
                    {
                        alert.MessageText = "Can't go any lower!";
                    }
                    alert.RunModal();
                }
            }
        }

        //import a midi file
        public void ImportMidi()
        {
            if (AlertIfUnsavedChanges())
            {
                NSUrl loadPath = null;
                using (var openDialog = new NSOpenPanel())
                {
                    openDialog.AllowedFileTypes = new string[] { "mid", "midi" };
                    var result = openDialog.RunModal();
                    if (result == 1)
                    {
                        loadPath = openDialog.Url;
                    }
                }
                if (loadPath != null)
                {
                    try
                    {
                        var midi = new Midi(loadPath.RelativePath);
                        if (midi.TrackCount > 1)
                        {
                            TrackChooserWindow trackChooser;
                            using (var temp = new TrackChooserWindowController())
                            {
                                trackChooser = temp.Window;
                                trackChooser.SetTrackList(midi.GetTrackNames());
                            }
                            NSApplication.SharedApplication.RunModalForWindow(trackChooser);

                            if (trackChooser.Track != -1)
                            {
                                song.ImportFromMidiTrack(midi, trackChooser.Track);
                                currentFile = null;
                                UpdateOutput(true);
                            }
                        }
                        else
                        {
                            song.ImportFromMidiTrack(midi, 0);
                            currentFile = null;
                            UpdateOutput(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var alert = new NSAlert())
                        {
                            alert.MessageText = ex.Message;
                            alert.RunModal();
                        }
                    }
                }
            }
        }

        //change the tempo of the song
        partial void ChangeTempoButton(NSObject sender)
        {
            if (int.TryParse(TempoField.StringValue, out int tempo) && tempo > 0)
            {
                song.Tempo = tempo;
                UpdateOutput(true);
            }
            else
            {
                using (var alert = new NSAlert())
                {
                    alert.MessageText = "Invalid tempo!";
                    alert.RunModal();
                }
            }
        }

        //get length data from the user controls
        private NoteLength GetNoteLength()
        {
            int fraction = int.Parse(LengthMenu.SelectedItem.Title.Substring(2));
            bool isDotted = IsDottedCheck.State == NSCellStateValue.On,
                isTriplet = IsTripletCheck.State == NSCellStateValue.On;
            var n = new NoteLength
            {
                Length = 1,
                NotesPerBeat = 1
            };

            if (isTriplet)
            {
                if (fraction > 8)
                {
                    n.Length = 1;
                    n.NotesPerBeat = fraction / 16;
                }
                else
                {
                    n.NotesPerBeat = 3;
                    n.Length = 8 / fraction;
                }
            }
            else
            {
                if (fraction < 4)
                {
                    if (isDotted) { n.Length = 3; }
                    else { n.Length = 2; }
                    if (fraction == 1) { n.Length *= 2; }
                }
                else
                {
                    n.NotesPerBeat = fraction / 4;
                    if (isDotted)
                    {
                        n.Length = 3;
                        n.NotesPerBeat *= 2;
                    }
                }
            }
            return n;
        }

        //add a new note
        partial void AddButton(NSObject sender)
        {
            char note = NoteMenu.SelectedItem.Title[0];
            bool isSharp = NoteMenu.SelectedItem.Title.EndsWith('#');
            int octave = int.Parse(OctaveMenu.SelectedItem.Title);
            var length = GetNoteLength();

            try
            {
                song.AppendNote(new Note(Note.GetNoteFromLetter(note, isSharp, octave), length.Length, length.NotesPerBeat));
                UpdateOutput(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                using (var alert = new NSAlert())
                {
                    alert.MessageText = ex.Message;
                    alert.RunModal();
                }
            }
        }

        //add a rest
        partial void AddRestButton(NSObject sender)
        {
            var length = GetNoteLength();
            song.AppendRest(length.Length, length.NotesPerBeat);
            UpdateOutput(true);
        }

        //remove the last note or rest
        partial void RemoveButton(NSObject sender)
        {
            song.RemoveLastNote();
            UpdateOutput(true);
        }

        //speak the script as it is
        partial void PlayVoiceButton(NSObject sender)
        {
            if (!SpeechSynthesizer.IsSpeaking)
            {
                SpeechSynthesizer.StopSpeaking();
            }
            SpeechSynthesizer.StartSpeaking(song.GetSpeechSynthesisScript(), SpeechSynthesizer.SystemVoices[SystemVoicesMenu.IndexOfSelectedItem]);
        }

        //stop speaking
        partial void StopVoiceButton(NSObject sender)
        {
            SpeechSynthesizer.StopSpeaking();
        }

        //save the script to an AIFF file
        partial void SaveVoiceButton(NSObject sender)
        {
            NSUrl savePath = null;
            using (var saveDialog = new NSSavePanel())
            {
                var result = saveDialog.RunModal();
                if (result == 1)
                {
                    if (!saveDialog.Url.RelativePath.EndsWith(".aif", StringComparison.CurrentCulture))
                    {
                        savePath = saveDialog.Url.AppendPathExtension("aif");
                    }
                }
            }
            if (savePath != null)
            {
                if (SpeechSynthesizer.SaveToFile(song.GetSpeechSynthesisScript(), SpeechSynthesizer.SystemVoices[SystemVoicesMenu.IndexOfSelectedItem], savePath))
                {
                    using (var alert = new NSAlert())
                    {
                        alert.MessageText = "Saved successfully.";
                        alert.RunModal();
                    }
                }
            }
        }
    }
}

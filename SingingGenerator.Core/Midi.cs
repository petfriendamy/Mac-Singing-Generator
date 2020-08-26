using System;
using System.Collections.Generic;
using System.IO;
using Commons.Music.Midi;

namespace SingingGenerator.Core
{
    //class to access MIDI data externally without needing the extension
    public class Midi
    {
        //private member
        private MidiMusic file;
        int tempo = 120; //fix later

        //properties
        public int Tempo
        {
            get { return tempo; }
            private set { tempo = value; }
        }
        public int TrackCount
        {
            get { return file.Tracks.Count; }
        }

        public Midi(string filepath)
        {
            if (File.Exists(filepath))
            {
                if (Path.GetExtension(filepath).ToLower() == ".mid")
                {
                    using (var fileStream = new FileStream(filepath, FileMode.Open))
                    {
                        file = MidiMusic.Read(fileStream);

                        //get tempo information from the midi
                        var t = file.GetMetaEventsOfType(MidiMetaType.Tempo).GetEnumerator();
                        t.MoveNext();
                    }
                }
                else
                {
                    throw new IOException("Incorrect file type.");
                }
            }
            else
            {
                throw new IOException("File doesn't exist.");
            }
        }

        //returns track names
        public string[] GetTrackNames()
        {
            var names = new string[TrackCount];
            var messages = file.GetMetaEventsOfType(MidiMetaType.TrackName).GetEnumerator();
            for (int i = 0; i < TrackCount; ++i)
            {
                string name;
                if (messages.MoveNext())
                {
                    if (messages.Current.Event.ExtraDataLength > 0)
                    {
                        name = "";
                        foreach (var d in messages.Current.Event.ExtraData)
                        {
                            name += (char)d;
                        }
                    }
                    else { name = "Untitled"; }
                }
                else { name = "Untitled"; }

                names[i] = $"Track {i + 1}: {name}";
            }
            return names;
        }

        //returns a list of notes from a track
        public List<SongEvent> GetNotesFromTrack(int track)
        {
            var notes = new List<SongEvent> { };
            int newNote = 0, currTime = 0, noteStart = -1, noteEnd = -1;
            int? currNote = null;

            foreach (var m in file.Tracks[track].Messages)
            {
                /*MIDI note data:
                 * http://newt.phys.unsw.edu.au/jw/notes.html
                 * https://mido.readthedocs.io/en/latest/midi_files.html
                 */
                currTime += m.DeltaTime;

                if (m.Event.EventType == MidiEvent.NoteOn) //start of note
                {
                    if (m.Event.Lsb != 0) //start of note
                    {
                        newNote = m.Event.MetaType - 69;

                        if (currNote == null) //start new note
                        {
                            if (noteEnd >= 0 && currTime > noteEnd) //if there was a gap between this and the last note, add a rest
                            {
                                notes.Add(MakeNoteOrRest(noteStart, currTime));
                            }
                        }
                        else //end previous note
                        {
                            if (currTime == noteStart) //ignore chords
                            {
                                continue;
                            }
                            noteEnd = currTime;
                            notes.Add(MakeNoteOrRest(noteStart, noteEnd, currNote.Value));
                        }
                        currNote = newNote;
                        noteStart = currTime;
                    }
                }
                else if (m.Event.EventType == MidiEvent.NoteOff || currTime == file.GetTotalTicks()) //end of note or song
                {
                    if (currNote != null && newNote == currNote.Value)
                    {
                        noteEnd = currTime;
                        notes.Add(MakeNoteOrRest(noteStart, noteEnd, currNote.Value));
                        noteStart = currTime;
                        currNote = null;
                    }
                }
            }
            return notes;
        }

        //private function that makes notes from midi data
        private Beat MakeNoteOrRest(int start, int finish, int? noteValue = null)
        {
            short ticksPerBeat = file.DeltaTimeSpec;
            int ticks = finish - start;
            if (noteValue == null)
            {
                return new Rest(ticks, ticksPerBeat);
            }
            return new Note(noteValue.Value, ticks, ticksPerBeat);
        }
    }
}

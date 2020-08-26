using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SingingGenerator.Core
{
    public class Song
    {
        //private list to hold notes
        private List<SongEvent> events;

        //properties
        public int GlobalTempo { get; set; }
        public bool HasEvents
        {
            get
            {
                return events.Count > 0;
            }
        }

        //constructor
        public Song(int tempo = 120)
        {
            events = new List<SongEvent> { };
            GlobalTempo = tempo;
        }

        //append an event to the song
        private void Append(SongEvent songEvent)
        {
            events.Add(songEvent);
        }

        //append a note to the end of the song
        public void AppendNote(int offset, int length, int notesPerBeat)
        {
            Append(new Note(offset, length, notesPerBeat));
        }

        //append a rest to the end of the song
        public void AppendRest(int length, int notesPerBeat)
        {
            Append(new Rest(length, notesPerBeat));
        }

        //append a tempo change event
        public bool AppendTempoChange(int tempo)
        {
            //if events list is empty, change global tempo instead
            if (events.Count == 0) 
            {
                GlobalTempo = tempo;
            }
            //if last event is already a tempo change, overwrite it
            else if (events[events.Count - 1] is TempoChangeEvent)
            {
                var tempoChange = events[events.Count - 1] as TempoChangeEvent;
                tempoChange.Tempo = tempo;
            }
            else
            {
                //check if there are other tempo change events already
                bool changeExists = false;
                for (int i = events.Count - 2; i > 0 && !changeExists; ++i)
                {
                    if (events[i] is TempoChangeEvent)
                    {
                        //if tempo is the same as last event's tempo, ignore new event
                        if ((events[i] as TempoChangeEvent).Tempo == tempo) 
                        {
                            return false;
                        }
                        changeExists = true;
                    }
                }
                if (!changeExists) //if no change events exist, compare to global tempo
                {
                    if (tempo == GlobalTempo)
                    {
                        return false;
                    }
                }
                Append(new TempoChangeEvent(tempo));
            }
            return true;
        }

        //remove a note or rest from the end of the song
        public void RemoveLastNoteOrRest()
        {
            for (int i = events.Count - 1; i > 0; --i)
            {
                if (events[i] is Beat)
                {
                    events.RemoveAt(i);
                    return;
                }
            }
        }

        //remove the last non-Beat event from the song
        public void RemoveLastNonNote()
        {
            for (int i = events.Count - 1; i > 0; --i)
            {
                if (!(events[i] is Beat))
                {
                    events.RemoveAt(i);
                    return;
                }
            }
        }

        //get a string of phonemes and set the lyrics accordingly
        public void SetLyricsFromPhonemeArray(Phoneme[][] phonemes)
        {
            for (int i = 0, j = 0; i < phonemes.Length && j < events.Count; ++i, ++j)
            {
                //skip rests
                while (events[j] is Rest && j < events.Count) { ++j; }

                if (j < events.Count)
                {
                    (events[j] as Note).SetPhonemes(phonemes[i]);
                }
            }
        }

        //transpose whole song
        public bool TransposeSong(int octave)
        {
            //fix for halfsteps later
            try
            {
                foreach (var note in events)
                {
                    if (note is Note)
                    {
                        var n = note as Note;
                        n.NoteValue += octave * 12;
                    }
                }
            }
            catch (ArgumentOutOfRangeException) //too high or low
            {
                return false;
            }
            return true;
        }

        //get whole song in Apple speech synthesis script
        public string GetSpeechSynthesisScript()
        {
            string output = $"[[cmnt Tempo: {GlobalTempo}]]\n[[inpt TUNE]]\n";
            int currTempo = GlobalTempo;
            for (int i = 0; i < events.Count; ++i)
            {
                if (events[i] is Beat)
                {
                    Beat curr = events[i] as Beat, next = null;
                    int j = 1;
                    while (i + j < events.Count && next == null)
                    {
                        if (events[i + j] is Beat) //get next note's data
                        {
                            next = events[i + j] as Beat;
                        }
                        j++;
                    }
                    output += curr.GetSpeechSynthesisScript(currTempo, next);
                }
                else if (events[i] is TempoChangeEvent)
                {
                    var tempoChange = events[i] as TempoChangeEvent;
                    currTempo = tempoChange.Tempo;
                    output += tempoChange.GetSpeechSynthesisScript();
                }
                output += "~\n";
            }
            output += "[[inpt TEXT]]";

            return output;
        }

        //import from an existing Apple speech synthesis script
        public void ImportFromSpeechSynthesisScript(string filepath)
        {
            using (var fileStream = new FileStream(filepath, FileMode.Open))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    string read;
                    int initialTempo = 0;
                    var readNotes = new List<SongEvent> { };
                    double length, pitch, noteLength = 0, notePitch = 0;
                    List<Phoneme> phonemes = null;

                    while (!streamReader.EndOfStream)
                    {
                        read = streamReader.ReadLine().Trim();
                        string pattern;
                        if (initialTempo == 0)
                        {
                            pattern = @"^\[\[cmnt Tempo: ([\d]+)\]\]$";
                            if (Regex.IsMatch(read, pattern))
                            {
                                var groups = Regex.Match(read, pattern).Groups;
                                initialTempo = int.Parse(groups[1].Value);
                            }
                            else
                            {
                                throw new FormatException("Missing tempo information.");
                            }
                        }
                        else
                        {
                            pattern = @"^(\w+|%) {D ([\d]+(\.*[\d]+)?)(; P ([\d]+(\.*[\d]+)?):0)?}$";
                            if (read == "~")
                            {
                                if (phonemes != null)
                                {
                                    var note = new Note(initialTempo, notePitch, noteLength);
                                    note.SetPhonemes(phonemes.ToArray());
                                    readNotes.Add(note);
                                    phonemes = null;
                                }
                            }
                            else if (Regex.IsMatch(read, pattern))
                            {
                                var groups = Regex.Match(read, pattern).Groups;
                                length = double.Parse(groups[2].Value);

                                try
                                {
                                    var phon = new Phoneme(groups[1].Value);
                                    if (phon.IsPause())
                                    {
                                        readNotes.Add(new Rest(initialTempo, length));
                                    }
                                    else
                                    {
                                        pitch = double.Parse(groups[5].Value);
                                        if (phonemes == null)
                                        {
                                            notePitch = pitch;
                                            phonemes = new List<Phoneme> { phon };
                                            noteLength = length;
                                        }
                                        else
                                        {
                                            //to add: compare pitch to notePitch
                                            phonemes.Add(phon);
                                            noteLength += length;
                                        }
                                    }

                                }
                                catch (ArgumentException ex)
                                {
                                    throw new FormatException("File is formatted incorrectly.", ex);
                                }
                            }
                            else if (!string.IsNullOrEmpty(read) && !read.StartsWith("[[inpt", StringComparison.CurrentCulture))
                            {
                                throw new FormatException("File is formatted incorrectly.");
                            }
                        }
                    }
                    //if everything goes through, pass the loaded song to the object
                    GlobalTempo = initialTempo;
                    events = readNotes;
                }
            }
        }

        //import from a midi track
        public void ImportFromMidiTrack(Midi midi, int track)
        {
            GlobalTempo = midi.Tempo;
            events = midi.GetNotesFromTrack(track);
        }
    }
}

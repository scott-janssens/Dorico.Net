using CommunityToolkit.Diagnostics;

namespace DoricoNet.Commands;

/// <summary>
/// Represent values associate with a note/pitch. Microtonality is not currently supported.
/// </summary>
public record Note
{
    private static readonly string[] _pitchesSharp = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
    private static readonly string[] _pitchesFlat = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

    private readonly bool _internalUseSharps;
    private readonly string? _accidentalType;

    /// <summary>
    /// Globally sets default use of sharps (true) or flats (false). Notes created in opposition
    /// to this will retain their set value. Ex: if UseSharps is true, but a note is created as Ab,
    /// the note will show as Ab.
    /// </summary>
    public static bool UseSharps { get; set; } = true;

    /// <summary>
    /// The MIDI value of the note.
    /// </summary>
    public int Midi { get; }

    /// <summary>
    /// The pitch of the note.
    /// </summary>
    public char Pitch { get; }

    /// <summary>
    /// Accidental, if any, of the note.
    /// </summary>
    public string Accidental { get; }

    /// <summary>
    /// The octave of the note.
    /// </summary>
    public int Octave { get; }

    /// <summary>
    /// The piano key value of the note.
    /// </summary>
    public int PianoKey { get; private set; }

    /// <summary>
    /// The organ key value of the note.
    /// </summary>
    public int OrganKey { get; private set; }

    /// <summary>
    /// The frequency of the note in equal temperament where A=440.
    /// </summary>
    public double Frequency { get; private set; }

    /// <summary>
    /// Creates an immutable Note record for the specified MIDI note value.
    /// </summary>
    /// <param name="midi">A midi note value</param>
    public Note(int midi)
    {
        Guard.IsInRange(midi, 0, 128, nameof(midi));

        _internalUseSharps = UseSharps;

        Midi = midi;
        Octave = (int)Math.Floor((Midi - 12) / 12.0);

        var pitchIndex = midi % 12;

        if (_internalUseSharps)
        {
            Pitch = _pitchesSharp[pitchIndex][0];
            Accidental = _pitchesSharp[pitchIndex][1..];
        }
        else
        {
            Pitch = _pitchesFlat[pitchIndex][0];
            Accidental = _pitchesFlat[pitchIndex][1..];
        }

        CalculateMidiRelatedValues();
    }

    /// <summary>
    /// Creates an immutable Note record for the specified pitch and octave.
    /// </summary>
    /// <param name="pitch">A note pitch</param>
    /// <param name="octave">An octave value</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "lower case is deliberate and necessary here")]
    public Note(string pitch, int octave)
    {
        Guard.IsNotNullOrWhiteSpace(pitch);

        Pitch = pitch.ToUpperInvariant()[0];

        // Not currently supporting the German "H"
        Guard.IsInRange(Pitch, 'A', 'H');
        Guard.IsInRange(octave, -1, 10, nameof(octave));

        Accidental = pitch[1..]?.ToLowerInvariant() ?? string.Empty;
        Octave = octave;
        _internalUseSharps = Accidental != "b" && UseSharps;

        Midi = octave * 12 + 12 + (_internalUseSharps ? Array.FindIndex(_pitchesSharp, PitchMatch) : Array.FindIndex(_pitchesFlat, PitchMatch));

        _accidentalType = GetAccidentalType(Accidental);
        CalculateMidiRelatedValues();
    }

    /// <summary>
    /// A helper method that returns the command(s) necessary to set a note value in Dorico.
    /// </summary>
    /// <returns>A collection of Command object.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Not a concern with octave values")]
    public IList<Command> GetNoteCommands()
    {
        var result = new List<Command>();

        if (!string.IsNullOrEmpty(_accidentalType))
        {
            result.Add(new("NoteInput.SetAccidental", new CommandParameter("Type", _accidentalType)));
        }

        result.Add(new("NoteInput.Pitch", new CommandParameter("Pitch", Pitch.ToString()), new CommandParameter("OctaveValue", Octave.ToString())));

        return result;
    }

    /// <summary>
    /// returns a new Note object with the enharmonic value of the original. Ex: if the original note pitch
    /// is C#4, the returned enharmonic pitch is Db4. If the original pitch is not sharp or flat, the new 
    /// Note object is equal to (but not the same as) the original.
    /// </summary>
    /// <returns></returns>
    public Note ToEnharmonic()
    {
        string[] pitchSetFrom;
        string[] pitchSetTo;
        if (_internalUseSharps)
        {
            pitchSetFrom = _pitchesSharp;
            pitchSetTo = _pitchesFlat;
        }
        else
        {
            pitchSetFrom = _pitchesFlat;
            pitchSetTo = _pitchesSharp;
        }
        return new Note(pitchSetTo[Array.FindIndex(pitchSetFrom, PitchMatch)], Octave);
    }

    /// <summary>
    /// Returns a new Note object offset by the specified value. Ex: "myNote + 12" returns a Note
    /// object an octave higher than the original.
    /// </summary>
    /// <param name="value">The number of half steps to add (can be negative)</param>
    /// <returns>A new Note object</returns>
    public Note Add(int value) => new(value + Midi);

    /// <inheritdoc/>
    public override string ToString() => $"{Pitch}{Accidental}{Octave}";

    #region Operators

    /// <summary>
    /// Returns a new Note object offset by the specified value. Ex: "myNote + 12" returns a Note
    /// object an octave higher than the original.
    /// </summary>
    /// <param name="value">The number of half steps to add</param>
    /// <returns>A new Note object</returns>
    public static Note operator +(Note note, int value)
    {
        Guard.IsNotNull(note, nameof(note));
        return note.Add(value);
    }

    /// <summary>
    /// Returns a new Note object offset by the specified value. Ex: "myNote - 12" returns a Note
    /// object an octave lower than the original.
    /// </summary>
    /// <param name="value">The number of half steps to subtract</param>
    /// <returns>A new Note object</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Unnecessary, can use Add()")]
    public static Note operator -(Note note, int value)
    {
        Guard.IsNotNull(note, nameof(note));
        return note.Add(-value);
    }

    #endregion

    #region Private Methods

    private static string? GetAccidentalType(string accidental)
    {
        switch (accidental)
        {
            case "0":
                return "kNatural";
            case "#":
                return "kSharp";
            case "x":
                return "kDoubleSharp";
            case "#x":
                return "kTripleSharp";
            case "b":
                return "kFlat";
            case "bb":
                return "kDoubleFlat";
            case "bbb":
                return "kTripleFlat";
            default:
                break;
        }

        return null;
    }

    private void CalculateMidiRelatedValues()
    {
        var value = Midi - 20;
        PianoKey = (value < 1 || value > 88) ? -1 : value;

        value = Midi - 35;
        OrganKey = (value < 1 || value > 61) ? -1 : value;

        Frequency = Math.Round(440 * Math.Pow(2.0, (Midi - 69) / 12.0), 2);
    }
    private bool PitchMatch(string pitch)
    {
        return pitch == $"{Pitch}{Accidental}";
    }
    #endregion
}

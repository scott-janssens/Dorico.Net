using DoricoNet.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.Commands;

[ExcludeFromCodeCoverage]
[TestFixture]
public class NoteTests
{
    [SetUp]
    public void SetUp()
    {
        Note.UseSharps = true;
    }

    [Test]
    public void CtorMidi()
    {
        var actualA0 = new Note(21);
        var actualC1 = new Note(24);
        var actualB1 = new Note(35);
        var actualFSharp4 = new Note(66);
        var actualCSharp7 = new Note(97);
        var actualC8 = new Note(108);
        var actualPianoLow = new Note(20);
        var actualPianoVeryLow = new Note(8);
        var actualPianoHigh = new Note(109);
        var actualPianoVeryHigh = new Note(122);

        Assert.Multiple(() =>
        {
            Assert.That(actualA0.Midi, Is.EqualTo(21));
            Assert.That(actualA0.Octave, Is.EqualTo(0));
            Assert.That(actualA0.Pitch, Is.EqualTo('A'));
            Assert.That(actualA0.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualA0.PianoKey, Is.EqualTo(1));
            Assert.That(actualA0.OrganKey, Is.EqualTo(-1));
            Assert.That(actualA0.Frequency, Is.EqualTo(27.5));

            Assert.That(actualC1.Midi, Is.EqualTo(24));
            Assert.That(actualC1.Octave, Is.EqualTo(1));
            Assert.That(actualC1.Pitch, Is.EqualTo('C'));
            Assert.That(actualC1.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualC1.PianoKey, Is.EqualTo(4));
            Assert.That(actualC1.OrganKey, Is.EqualTo(-1));
            Assert.That(actualC1.Frequency, Is.EqualTo(32.7));

            Assert.That(actualB1.Midi, Is.EqualTo(35));
            Assert.That(actualB1.Octave, Is.EqualTo(1));
            Assert.That(actualB1.Pitch, Is.EqualTo('B'));
            Assert.That(actualB1.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualB1.PianoKey, Is.EqualTo(15));
            Assert.That(actualB1.OrganKey, Is.EqualTo(-1));
            Assert.That(actualB1.Frequency, Is.EqualTo(61.74));

            Assert.That(actualFSharp4.Midi, Is.EqualTo(66));
            Assert.That(actualFSharp4.Octave, Is.EqualTo(4));
            Assert.That(actualFSharp4.Pitch, Is.EqualTo('F'));
            Assert.That(actualFSharp4.Accidental, Is.EqualTo("#"));
            Assert.That(actualFSharp4.PianoKey, Is.EqualTo(46));
            Assert.That(actualFSharp4.OrganKey, Is.EqualTo(31));
            Assert.That(actualFSharp4.Frequency, Is.EqualTo(369.99));

            Assert.That(actualCSharp7.Midi, Is.EqualTo(97));
            Assert.That(actualCSharp7.Octave, Is.EqualTo(7));
            Assert.That(actualCSharp7.Pitch, Is.EqualTo('C'));
            Assert.That(actualCSharp7.Accidental, Is.EqualTo("#"));
            Assert.That(actualCSharp7.PianoKey, Is.EqualTo(77));
            Assert.That(actualCSharp7.OrganKey, Is.EqualTo(-1));
            Assert.That(actualCSharp7.Frequency, Is.EqualTo(2217.46));

            Assert.That(actualC8.Midi, Is.EqualTo(108));
            Assert.That(actualC8.Octave, Is.EqualTo(8));
            Assert.That(actualC8.Pitch, Is.EqualTo('C'));
            Assert.That(actualC8.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualC8.PianoKey, Is.EqualTo(88));
            Assert.That(actualC8.OrganKey, Is.EqualTo(-1));
            Assert.That(actualC8.Frequency, Is.EqualTo(4186.01));

            Assert.That(actualPianoLow.Midi, Is.EqualTo(20));
            Assert.That(actualPianoLow.Octave, Is.EqualTo(0));
            Assert.That(actualPianoLow.Pitch, Is.EqualTo('G'));
            Assert.That(actualPianoLow.Accidental, Is.EqualTo("#"));
            Assert.That(actualPianoLow.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoLow.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoLow.Frequency, Is.EqualTo(25.96));

            Assert.That(actualPianoVeryLow.Midi, Is.EqualTo(8));
            Assert.That(actualPianoVeryLow.Octave, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.Pitch, Is.EqualTo('G'));
            Assert.That(actualPianoVeryLow.Accidental, Is.EqualTo("#"));
            Assert.That(actualPianoVeryLow.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.Frequency, Is.EqualTo(12.98));

            Assert.That(actualPianoHigh.Midi, Is.EqualTo(109));
            Assert.That(actualPianoHigh.Octave, Is.EqualTo(8));
            Assert.That(actualPianoHigh.Pitch, Is.EqualTo('C'));
            Assert.That(actualPianoHigh.Accidental, Is.EqualTo("#"));
            Assert.That(actualPianoHigh.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoHigh.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoHigh.Frequency, Is.EqualTo(4434.92));

            Assert.That(actualPianoVeryHigh.Midi, Is.EqualTo(122));
            Assert.That(actualPianoVeryHigh.Octave, Is.EqualTo(9));
            Assert.That(actualPianoVeryHigh.Pitch, Is.EqualTo('D'));
            Assert.That(actualPianoVeryHigh.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualPianoVeryHigh.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryHigh.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryHigh.Frequency, Is.EqualTo(9397.27));
        });
    }

    [Test]
    public void CtorMidiFlat()
    {
        Note.UseSharps = false;

        var actualA0 = new Note(21);
        var actualC1 = new Note(24);
        var actualB1 = new Note(35);
        var actualFSharp4 = new Note(66);
        var actualCSharp7 = new Note(97);
        var actualC8 = new Note(108);
        var actualPianoLow = new Note(20);
        var actualPianoVeryLow = new Note(8);
        var actualPianoHigh = new Note(109);
        var actualPianoVeryHigh = new Note(122);

        Assert.Multiple(() =>
        {
            Assert.That(actualA0.Midi, Is.EqualTo(21));
            Assert.That(actualA0.Octave, Is.EqualTo(0));
            Assert.That(actualA0.Pitch, Is.EqualTo('A'));
            Assert.That(actualA0.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualA0.PianoKey, Is.EqualTo(1));
            Assert.That(actualA0.OrganKey, Is.EqualTo(-1));
            Assert.That(actualA0.Frequency, Is.EqualTo(27.5));

            Assert.That(actualC1.Midi, Is.EqualTo(24));
            Assert.That(actualC1.Octave, Is.EqualTo(1));
            Assert.That(actualC1.Pitch, Is.EqualTo('C'));
            Assert.That(actualC1.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualC1.PianoKey, Is.EqualTo(4));
            Assert.That(actualC1.OrganKey, Is.EqualTo(-1));
            Assert.That(actualC1.Frequency, Is.EqualTo(32.7));

            Assert.That(actualB1.Midi, Is.EqualTo(35));
            Assert.That(actualB1.Octave, Is.EqualTo(1));
            Assert.That(actualB1.Pitch, Is.EqualTo('B'));
            Assert.That(actualB1.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualB1.PianoKey, Is.EqualTo(15));
            Assert.That(actualB1.OrganKey, Is.EqualTo(-1));
            Assert.That(actualB1.Frequency, Is.EqualTo(61.74));

            Assert.That(actualFSharp4.Midi, Is.EqualTo(66));
            Assert.That(actualFSharp4.Octave, Is.EqualTo(4));
            Assert.That(actualFSharp4.Pitch, Is.EqualTo('G'));
            Assert.That(actualFSharp4.Accidental, Is.EqualTo("b"));
            Assert.That(actualFSharp4.PianoKey, Is.EqualTo(46));
            Assert.That(actualFSharp4.OrganKey, Is.EqualTo(31));
            Assert.That(actualFSharp4.Frequency, Is.EqualTo(369.99));

            Assert.That(actualCSharp7.Midi, Is.EqualTo(97));
            Assert.That(actualCSharp7.Octave, Is.EqualTo(7));
            Assert.That(actualCSharp7.Pitch, Is.EqualTo('D'));
            Assert.That(actualCSharp7.Accidental, Is.EqualTo("b"));
            Assert.That(actualCSharp7.PianoKey, Is.EqualTo(77));
            Assert.That(actualCSharp7.OrganKey, Is.EqualTo(-1));
            Assert.That(actualCSharp7.Frequency, Is.EqualTo(2217.46));

            Assert.That(actualC8.Midi, Is.EqualTo(108));
            Assert.That(actualC8.Octave, Is.EqualTo(8));
            Assert.That(actualC8.Pitch, Is.EqualTo('C'));
            Assert.That(actualC8.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualC8.PianoKey, Is.EqualTo(88));
            Assert.That(actualC8.OrganKey, Is.EqualTo(-1));
            Assert.That(actualC8.Frequency, Is.EqualTo(4186.01));

            Assert.That(actualPianoLow.Midi, Is.EqualTo(20));
            Assert.That(actualPianoLow.Octave, Is.EqualTo(0));
            Assert.That(actualPianoLow.Pitch, Is.EqualTo('A'));
            Assert.That(actualPianoLow.Accidental, Is.EqualTo("b"));
            Assert.That(actualPianoLow.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoLow.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoLow.Frequency, Is.EqualTo(25.96));

            Assert.That(actualPianoVeryLow.Midi, Is.EqualTo(8));
            Assert.That(actualPianoVeryLow.Octave, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.Pitch, Is.EqualTo('A'));
            Assert.That(actualPianoVeryLow.Accidental, Is.EqualTo("b"));
            Assert.That(actualPianoVeryLow.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.Frequency, Is.EqualTo(12.98));

            Assert.That(actualPianoHigh.Midi, Is.EqualTo(109));
            Assert.That(actualPianoHigh.Octave, Is.EqualTo(8));
            Assert.That(actualPianoHigh.Pitch, Is.EqualTo('D'));
            Assert.That(actualPianoHigh.Accidental, Is.EqualTo("b"));
            Assert.That(actualPianoHigh.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoHigh.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoHigh.Frequency, Is.EqualTo(4434.92));

            Assert.That(actualPianoVeryHigh.Midi, Is.EqualTo(122));
            Assert.That(actualPianoVeryHigh.Octave, Is.EqualTo(9));
            Assert.That(actualPianoVeryHigh.Pitch, Is.EqualTo('D'));
            Assert.That(actualPianoVeryHigh.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualPianoVeryHigh.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryHigh.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryHigh.Frequency, Is.EqualTo(9397.27));
        });
    }

    [Test]
    public void CtorPitch()
    {
        var actualA0 = new Note("A", 0);
        var actualC1 = new Note("C", 1);
        var actualB1 = new Note("B", 1);
        var actualFSharp4 = new Note("F#", 4);
        var actualDFlat7 = new Note("Db", 7);
        var actualC8 = new Note("C", 8);
        var actualPianoLow = new Note("Ab", 0);
        var actualPianoVeryLow = new Note("G#", -1);
        var actualPianoHigh = new Note("Db", 8);
        var actualPianoVeryHigh = new Note("D", 9);

        Assert.Multiple(() =>
        {
            Assert.That(actualA0.Midi, Is.EqualTo(21));
            Assert.That(actualA0.Octave, Is.EqualTo(0));
            Assert.That(actualA0.Pitch, Is.EqualTo('A'));
            Assert.That(actualA0.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualA0.PianoKey, Is.EqualTo(1));
            Assert.That(actualA0.OrganKey, Is.EqualTo(-1));
            Assert.That(actualA0.Frequency, Is.EqualTo(27.5));

            Assert.That(actualC1.Midi, Is.EqualTo(24));
            Assert.That(actualC1.Octave, Is.EqualTo(1));
            Assert.That(actualC1.Pitch, Is.EqualTo('C'));
            Assert.That(actualC1.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualC1.PianoKey, Is.EqualTo(4));
            Assert.That(actualC1.OrganKey, Is.EqualTo(-1));
            Assert.That(actualC1.Frequency, Is.EqualTo(32.7));

            Assert.That(actualB1.Midi, Is.EqualTo(35));
            Assert.That(actualB1.Octave, Is.EqualTo(1));
            Assert.That(actualB1.Pitch, Is.EqualTo('B'));
            Assert.That(actualB1.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualB1.PianoKey, Is.EqualTo(15));
            Assert.That(actualB1.OrganKey, Is.EqualTo(-1));
            Assert.That(actualB1.Frequency, Is.EqualTo(61.74));

            Assert.That(actualFSharp4.Midi, Is.EqualTo(66));
            Assert.That(actualFSharp4.Octave, Is.EqualTo(4));
            Assert.That(actualFSharp4.Pitch, Is.EqualTo('F'));
            Assert.That(actualFSharp4.Accidental, Is.EqualTo("#"));
            Assert.That(actualFSharp4.PianoKey, Is.EqualTo(46));
            Assert.That(actualFSharp4.OrganKey, Is.EqualTo(31));
            Assert.That(actualFSharp4.Frequency, Is.EqualTo(369.99));

            Assert.That(actualDFlat7.Midi, Is.EqualTo(97));
            Assert.That(actualDFlat7.Octave, Is.EqualTo(7));
            Assert.That(actualDFlat7.Pitch, Is.EqualTo('D'));
            Assert.That(actualDFlat7.Accidental, Is.EqualTo("b"));
            Assert.That(actualDFlat7.PianoKey, Is.EqualTo(77));
            Assert.That(actualDFlat7.OrganKey, Is.EqualTo(-1));
            Assert.That(actualDFlat7.Frequency, Is.EqualTo(2217.46));

            Assert.That(actualC8.Midi, Is.EqualTo(108));
            Assert.That(actualC8.Octave, Is.EqualTo(8));
            Assert.That(actualC8.Pitch, Is.EqualTo('C'));
            Assert.That(actualC8.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualC8.PianoKey, Is.EqualTo(88));
            Assert.That(actualC8.OrganKey, Is.EqualTo(-1));
            Assert.That(actualC8.Frequency, Is.EqualTo(4186.01));

            Assert.That(actualPianoLow.Midi, Is.EqualTo(20));
            Assert.That(actualPianoLow.Octave, Is.EqualTo(0));
            Assert.That(actualPianoLow.Pitch, Is.EqualTo('A'));
            Assert.That(actualPianoLow.Accidental, Is.EqualTo("b"));
            Assert.That(actualPianoLow.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoLow.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoLow.Frequency, Is.EqualTo(25.96));

            Assert.That(actualPianoVeryLow.Midi, Is.EqualTo(8));
            Assert.That(actualPianoVeryLow.Octave, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.Pitch, Is.EqualTo('G'));
            Assert.That(actualPianoVeryLow.Accidental, Is.EqualTo("#"));
            Assert.That(actualPianoVeryLow.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryLow.Frequency, Is.EqualTo(12.98));

            Assert.That(actualPianoHigh.Midi, Is.EqualTo(109));
            Assert.That(actualPianoHigh.Octave, Is.EqualTo(8));
            Assert.That(actualPianoHigh.Pitch, Is.EqualTo('D'));
            Assert.That(actualPianoHigh.Accidental, Is.EqualTo("b"));
            Assert.That(actualPianoHigh.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoHigh.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoHigh.Frequency, Is.EqualTo(4434.92));

            Assert.That(actualPianoVeryHigh.Midi, Is.EqualTo(122));
            Assert.That(actualPianoVeryHigh.Octave, Is.EqualTo(9));
            Assert.That(actualPianoVeryHigh.Pitch, Is.EqualTo('D'));
            Assert.That(actualPianoVeryHigh.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualPianoVeryHigh.PianoKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryHigh.OrganKey, Is.EqualTo(-1));
            Assert.That(actualPianoVeryHigh.Frequency, Is.EqualTo(9397.27));
        });
    }

    [Test]
    public void Accidentals()
    {
        var note = new Note("C", 4);
        var commands = note.GetNoteCommands();
        Assert.That(commands, Has.Count.EqualTo(1));

        note = new Note("C0", 4);
        commands = note.GetNoteCommands();
        Assert.That(commands[0].Message, Does.Contain("kNatural"));

        note = new Note("C#", 4);
        commands = note.GetNoteCommands();
        Assert.That(commands[0].Message, Does.Contain("kSharp"));

        note = new Note("Cx", 4);
        commands = note.GetNoteCommands();
        Assert.That(commands[0].Message, Does.Contain("kDoubleSharp"));

        note = new Note("C#X", 4);
        commands = note.GetNoteCommands();
        Assert.That(commands[0].Message, Does.Contain("kTripleSharp"));

        note = new Note("Cb", 4);
        commands = note.GetNoteCommands();
        Assert.That(commands[0].Message, Does.Contain("kFlat"));

        note = new Note("Cbb", 4);
        commands = note.GetNoteCommands();
        Assert.That(commands[0].Message, Does.Contain("kDoubleFlat"));

        note = new Note("Cbbb", 4);
        commands = note.GetNoteCommands();
        Assert.That(commands[0].Message, Does.Contain("kTripleFlat"));
    }

    [Test]
    public void CtorBadPitch()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => { new Note("Q", 5); });
    }

    [Test]
    public void CtorBadOctaves()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { new Note("C", -2); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new Note("C", 10); });
        });
    }

    [Test]
    public void ToEnharmonic()
    {
        var sharp = new Note("A#", 4);
        var actualSharpEnharmonic = sharp.ToEnharmonic();
        var flat = new Note("Eb", 2);
        var actualFlatEnharmonic = flat.ToEnharmonic();
        var neither = new Note("C", 4);
        var actualNeitherEnharmonic = neither.ToEnharmonic();

        Assert.Multiple(() =>
        {
            Assert.That(actualSharpEnharmonic.Midi, Is.EqualTo(sharp.Midi));
            Assert.That(actualSharpEnharmonic.Pitch, Is.EqualTo('B'));
            Assert.That(actualSharpEnharmonic.Accidental, Is.EqualTo("b"));
            Assert.That(actualSharpEnharmonic.Octave, Is.EqualTo(sharp.Octave));

            Assert.That(actualFlatEnharmonic.Midi, Is.EqualTo(flat.Midi));
            Assert.That(actualFlatEnharmonic.Pitch, Is.EqualTo('D'));
            Assert.That(actualFlatEnharmonic.Accidental, Is.EqualTo("#"));
            Assert.That(actualFlatEnharmonic.Octave, Is.EqualTo(flat.Octave));

            Assert.That(actualNeitherEnharmonic.Midi, Is.EqualTo(neither.Midi));
            Assert.That(actualNeitherEnharmonic.Pitch, Is.EqualTo(neither.Pitch));
            Assert.That(actualNeitherEnharmonic.Accidental, Is.EqualTo(string.Empty));
            Assert.That(actualNeitherEnharmonic.Octave, Is.EqualTo(neither.Octave));
        });
    }

    [Test]
    public void NoteMath()
    {
        var middleC = new Note("C", 4);
        var expectedB = middleC - 1;
        var expectedTriTone = middleC.Add(6);
        var expectedHighC = middleC + 12;
        var expectedLowC = middleC - 12;
        var b2 = new Note("B", 2);
        var expectedC3 = b2 + 1;

        Assert.Multiple(() =>
        {
            Assert.That(expectedB.Midi, Is.EqualTo(59));
            Assert.That(expectedB.Pitch, Is.EqualTo('B'));
            Assert.That(expectedB.Accidental, Is.EqualTo(string.Empty));
            Assert.That(expectedB.Octave, Is.EqualTo(3));

            Assert.That(expectedTriTone.Midi, Is.EqualTo(66));
            Assert.That(expectedTriTone.Pitch, Is.EqualTo('F'));
            Assert.That(expectedTriTone.Accidental, Is.EqualTo("#"));
            Assert.That(expectedTriTone.Octave, Is.EqualTo(4));

            Assert.That(expectedHighC.Midi, Is.EqualTo(72));
            Assert.That(expectedHighC.Pitch, Is.EqualTo('C'));
            Assert.That(expectedHighC.Accidental, Is.EqualTo(string.Empty));
            Assert.That(expectedHighC.Octave, Is.EqualTo(5));

            Assert.That(expectedLowC.Midi, Is.EqualTo(48));
            Assert.That(expectedLowC.Pitch, Is.EqualTo('C'));
            Assert.That(expectedLowC.Accidental, Is.EqualTo(string.Empty));
            Assert.That(expectedLowC.Octave, Is.EqualTo(3));

            Assert.That(expectedC3.Midi, Is.EqualTo(48));
            Assert.That(expectedC3.Pitch, Is.EqualTo('C'));
            Assert.That(expectedC3.Accidental, Is.EqualTo(string.Empty));
            Assert.That(expectedC3.Octave, Is.EqualTo(3));

            Assert.Throws<ArgumentOutOfRangeException>(() => middleC.Add(1000));
            Assert.Throws<ArgumentOutOfRangeException>(() => middleC.Add(-1000));
        });
    }

    [Test]
    public void NoteToString()
    {
        var middleC = new Note("C", 4);
        var actual = middleC.ToString();

        Assert.That(actual, Is.EqualTo("C4"));
    }
}

using DoricoNet.Attributes;
using DoricoNet.Enums;

namespace DoricoNet.Responses;

/// <summary>
/// Details the current status of Dorico
/// </summary>
[ResponseMessage("status")]
public record StatusResponse : DoricoUnpromptedResponseBase
{
    internal static StatusResponse Create() => new StatusResponse { Message = "status" };

    /// <summary>
    /// The ID of the active score
    /// </summary>
    public int ActiveOpenScoreID { get; init; }

    /// <summary>
    /// The ID of the active view container(i.e.window) for the currently active score
    /// </summary>
    public int ActiveViewContainerID { get; init; }

    /// <summary>
    /// Whether or not a score is loaded
    /// </summary>
    public bool HasScore { get; init; }

    /// <summary>
    /// Whether or not it's currently possible to undo a previous action
    /// </summary>
    public bool CanUndo { get; init; }

    /// <summary>
    /// Whether or not there is a current selection
    /// </summary>
    public bool HasSelection { get; init; }

    /// <summary>
    /// Whether or not Dorico is currently playing back
    /// </summary>
    public bool InPlayback { get; init; }

    /// <summary>
    /// Whether or not Dorico is currently recording
    /// </summary>
    public bool IsRecording { get; init; }

    /// <summary>
    /// Whether or not step time input is currently active
    /// </summary>
    public bool InStepTimeInput { get; init; }

    /// <summary>
    /// The active score is not read only, i.e.changes can be made
    /// </summary>
    public bool NotReadOnly { get; init; }

    /// <summary>
    /// Permitted values: kInsert, kOverwrite, kChordMerge
    /// </summary>
    public NoteInputMode NoteInputMode { get; init; }

    public bool TabBarShown { get; init; }

    public bool ToolbarShown { get; init; }

    public bool LeftPanelShown { get; init; }

    public bool RightPanelShown { get; init; }

    public bool BottomPanelShown { get; init; }

    public bool AllPanelsHidden { get; init; }

    public bool FixedTempoMode { get; init; }

    public bool ShowTimeTrack { get; init; }

    public bool ShowChordTrack { get; init; }

    public bool ShowVideoTrack { get; init; }

    public bool ShowMarkerTrack { get; init; }

    public bool ClickEnabled { get; init; }

    public RhythmicGridResolution RhythmicGridResolutionValue { get; init; }

    public NoteInputPitchMode NoteInputPitchModeValue { get; init; }

    public FilterBehaviour FilterBehaviour { get; init; }

    public PagePositionPolicy PagePositionPolicy { get; init; }

    public NoteColoursType NoteColoursType { get; init; }

    public bool SignpostsSuppressed { get; init; }

    public DragHandlesViewState DragHandlesViewState { get; init; }

    /// <summary>
    /// Whether or not bar numbers are shown in page view
    /// </summary>
    public bool ShowBarNumbersInPageView { get; init; }

    /// <summary>
    /// Whether or not bar numbers are shown in galley view
    /// </summary>
    public bool ShowBarNumbersInGalleyView { get; init; }

    /// <summary>
    /// Whether or not implicit rests are shown in grey
    /// </summary>
    public bool ShowImplicitRestsInGrey { get; init; }

    public bool ShowCuesInGrey { get; init; }

    public bool ShowDivisiUnisonInGrey { get; init; }

    public bool ShowCondensedMusicInGrey { get; init; }

    public bool ShowMutedEventsInGrey { get; init; }

    public bool AttachmentLinesShown { get; init; }

    public bool HighlightCues { get; init; }

    public bool HighlightSlashRegions { get; init; }

    public bool HighlightBarRepeatRegions { get; init; }

    public bool HighlightChordSymbolVisibilityRegions { get; init; }

    public bool ShowSystemTrack { get; init; }

    public bool ShowComments { get; init; }

    public WindowMode WindowMode { get; init; }

    public bool InsertActive { get; init; }

    /// <summary>
    /// The active tool in play mode
    /// </summary>
    public ActivePlayModeTool ActivePlayModeTool { get; init; }

    /// <summary>
    /// Mouse click mode
    /// </summary>
    public ToolType ToolType { get; init; }

    public int? ZoomPercent { get; init; }

    public bool? NoteInputActive { get; init; }

    public bool? VideoWindowShown { get; init; }

    public bool? MixerShown { get; init; }

    public bool? TransportShown { get; init; }

    public bool? FullScreen { get; init; }

    /// <summary>
    /// UNDOCUMENTED: The selected duration found in the Notes panel in the left zone. If the note is tied and doesn't fall into
    /// a nice rhythmic value, Duration will not be set. E.g., a crotchet tied to a quaver will return kCrotchet
    /// for duration and 1 for RhythmDots.  However a dotted crotchet tied to a crotchet will not return either
    /// Duration or RhythmDots.  This is the same as the duration selection behavior in Dorico.
    /// </summary>
    public RhythmicGridResolution Duration { get; init; }

    /// <summary>
    /// UNDOCUMENTED: The number of dots on the rhythm value of Duration to give the selected objects full rhythmic value.
    /// </summary>
    public int? RhythmDots { get; init; }

    /// <summary>
    /// UNDOCUMENTED: Seems to be set when a rest is currently selected.
    /// </summary>
    public bool? RestMode { get; init; }

    public bool ShowHiddenNoteheadsInGrey { get; init; }
	public bool HighlightMIDITriggerRegions { get; init; }
	public bool ShowPageNumbersOverlay { get; init; }
	public bool ShowCautionaryAccidentals { get; init; }
	public bool ShowAccidentalsWithForcedVisibility { get; init; }
	public bool ActivatedForPlayback { get; init; }
	public bool ArticulationAccent { get; init; }
	public bool ArticulationStaccato { get; init; }
	public bool ArticulationMarcato { get; init; }
	public bool ArticulationTenuto { get; init; }
	public bool ArticulationStaccatissimo { get; init; }
	public bool ArticulationStaccatoTenuto { get; init; }
	public bool ArticulationStressed { get; init; }
	public bool ArticulationUnstressed { get; init; }
	public Accidental Accidental { get; init; }
	public bool GraceNoteSlashed { get; init; }
	public bool GraceNoteUnslashed {  get; init;  }
}

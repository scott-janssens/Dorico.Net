using DoricoNet;
using DoricoNet.Commands;
using DoricoNet.Comms;
using DoricoNet.DataStructures;
using DoricoNet.Enums;
using DoricoNet.Requests;
using DoricoNet.Responses;
using Lea;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

// Setup dependency injection
var services = new ServiceCollection()
    .AddSingleton(sp => LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("DoricoRemote"))
    .AddSingleton<IEventAggregator, EventAggregator>()
    .AddTransient<IClientWebSocketWrapper, ClientWebSocketWrapper>()
    .AddSingleton<IDoricoCommsContext, DoricoCommsContext>()
    .AddTransient<IDoricoRemote, DoricoRemote>();

var serviceProvider = services.BuildServiceProvider();

// Create the Dorico.Net remote control instance
var remote = serviceProvider.GetService<IDoricoRemote>()!;

// Set remote timeout to infinite. This will prevent timing out if you set breakpoints
// or step through this demo. It is not recommended to do this in a real client application.
// The default is 30 seconds.
remote.Timeout = -1;

// Connect to Dorico (Dorico must be open)
// If the client has never connected before, the default ConnectionArguments should be used.
// This will open a dialog inside Dorico asking to allow connection for the remote control client.
await remote.ConnectAsync("<Client Name>", new ConnectionArguments());

// Once connected, remote.SessionToken stores a token that can be passed in the next connection
// to bypass the authorization dialog:
// await remote.ConnectAsync("<Client Name>", new ConnectionArguments("<SessionToken">))

Console.WriteLine($"Connected? {remote.IsConnected}");
Console.WriteLine($"Session Token: {remote.SessionToken}");


// The version of Dorico can be retrieved with the GetAppInfoAsync() method.
var versionResponse = await remote.GetAppInfoAsync();
Console.WriteLine($"version: {versionResponse}\n");


// The internal Dorico commands can be retrieved:
var commands = await remote.GetCommandsAsync();
Console.WriteLine($"Command Count: {commands?.Count ?? 0}\n");

// The CommandCollection can be iterated over like list or can be traversed like a tree structure
if (commands != null)
{
    Console.WriteLine("Commands");
    foreach (var command in commands)
    {
        Console.WriteLine($"\"{command.DisplayName}\", {command.Name}");
    }

    var node = commands.OrganizedItems!.ChildNodes.First();
    Console.WriteLine($"\nNode:{node.Path}\n");
    node.Values.ForEach(x => Console.WriteLine($"  \"{x.DisplayName}\", {x.Name}"));
}

// The CommandCollection contains CommandInfo records which describe the commands. Each contains lists of required and
// optional parameters. These can be used to initialize Command request objects which are sent to Dorico to perform
// an operation. Parameters can be passed into the constructor or added with the AddParameter() method.

// commands can be sent to Dorico in 2 ways
// with the raw command/parameter values:
var fileOpenResponse = await remote.SendRequestAsync(new Command("File.Open", new CommandParameter("File", "<path>")));

// or with a command object from the commands list:

//var commandInfo = commands!["File.Open"];
//var fileOpenCommand = new Command(commandInfo);
//fileOpenCommand.AddParameter(new("File", "<path>"));
//fileOpenResponse = await remote.SendRequestAsync(fileOpenCommand);


// Currently there is no documentation from the Dorico team about what the parameters mean or what the valid values may be.
// The best way to see the commands work is to perform the desired operation in Dorico and look at the application.log where
// Dorico echoes the commands and parameters.


// NOTE: Dorico.Net caches the CommandInfo objects after retrieving them, so subsequent calls to GetCommandsAsync() will
// not call Dorico.

// NOTE: Dorico returns a "kOK" to acknowledge receipt of the request. That does not necessarily mean the operation was
// successful.  For example, sending a bad path in the above File.Open command will receive a "kOK", but the operation
// will fail in Dorico.

// NOTE: Some commands, such as those to create projects appear to be disabled through the Remote Control API at the moment.


// IMPORTANT: At this point in the demo, a project must be loaded in Dorico. Either set a proper path in the Open command above,
// or manually create a new project in Dorico. Close any error dialogs in Dorico before continuing.


// Make sure we're not in Note Input mode as the rest of the demo assumes it's off at this point.
await remote.SendRequestAsync(new Command("NoteInput.Exit"));


// Once a project is loaded, the options for that project can be retrieved.
// The information about the project's flows can be obtained:
var flowsResponse = await remote.GetFlowsAsync();

// Or the layouts:
var layoutsResponse = await remote.GetLayoutsAsync();


// The various options and their values can be retrieved. The OptionsCollection is an
// OrganizedCollection like the CommandCollection.

// Engraving options are global
var engravingOptions = await remote.GetEngravingOptionsAsync();
Console.WriteLine($"\nEngraving Options: {engravingOptions?.Count}");

// Notation options are per flow:
OptionCollection? notationOptions = null;
if (flowsResponse != null)
{
    notationOptions = await remote.GetNotationOptionsAsync(flowsResponse.Flows.First().FlowID);
    Console.WriteLine($"Notation Options: {notationOptions?.Count}");
}

// Layout options are per layout
OptionCollection? layoutOptions = null;
if (layoutsResponse != null)
{
    var layoutId = layoutsResponse.Layouts.First().LayoutID;

    layoutOptions = await remote.GetLayoutOptionsAsync(layoutId);
    Console.WriteLine($"Layout Options: {layoutOptions?.Count}\n");

    // Modify a layout option of a specific layout. Multiple options can be set at
    // once on multiple layouts, but we'll just do one of each here.
    await remote.SetLayoutOptionsAsync(new[] { new OptionValue("transpositionType", "kTransposingScore") }, new[] { layoutId });

    // There are also enums to help specify which layouts to affect
    await remote.SetLayoutOptionsAsync(new[] { new OptionValue("transpositionType", "kScoreInC") }, LayoutIds.kAll);
}

// Dorico sends unprompted status messages a LOT.  Dorico.Net uses an
// event aggregator called Lea that can be subscribed to, to receive
// unprompted responses when they are received.

var lea = serviceProvider.GetService<IEventAggregator>()!;
//lea.Subscribe<StatusResponse>(OnStatusUpdate);

//void OnStatusUpdate(StatusResponse statusResponse)
//{
//    Console.WriteLine($"\nStatus:\n {statusResponse}");

//    // Status is sent a lot, so we'll unsubscribe after 1 demo.
//    lea.Unsubscribe<StatusResponse>(OnStatusUpdate);
//}

// Entering NoteInput mode will cause a status update
await remote.SendRequestAsync(new Command("NoteInput.Enter"));


// Making changes within Dorico is currently limited to essentially what you can
// do via the menu in Dorico.  Via Status limited information can be inferred about what
// is selected.  If a note or rest is selected, the StatusResponse.Duration will be set.
// If StatusResponse.RestMode is true, a rest is selected. Be aware that RestMode.Duration
// may not be populated if a selected note's duration doesn't fall into a
// RhythmicGridResolution value.  There is currently no way to differentiate between any
// other item types that might be selected.

// There is currently no way to query for information about a bar, stave, or project in general. 

// It's possible to make changes, but you're at the mercy of whatever is currently
// selected or wherever the caret currently is.  When the "NoteInput.Enter" was 
// sent above, the caret appeared wherever the current selection was.


// Let's add some notes:

// NoteInput.Enter is a toggle, so first make sure we're not already in Note Input mode
// otherwise sending NoteInput.Enter will actually exit Note Input mode. The current state
// can be determined from the most recent StatusResponse.NoteInputActive value, but it's
// easier to just send a NoteInput.Exit which always forces NoteInputActive to false;
await remote.SendRequestAsync(new Command("NoteInput.Exit"));
await remote.SendRequestAsync(new Command("NoteInput.Enter"));

await InsertNoteAsync(new("F#", 5));
await InsertNoteAsync(new("D", 5));
await remote.SendRequestAsync(new Command("NoteInput.MoveAdvance")); // Advance the caret to create a rest
await InsertNoteAsync(new("Bb", 4));

// Exit Note Input mode.
await remote.SendRequestAsync(new Command("NoteInput.Exit"));

// Break on the next line to allow the selection to be changed in Dorico.
//System.Diagnostics.Debugger.Break();
SemaphoreSlim _ss = new(1, 1);

lea.Subscribe<SelectionChanged>(SelectionChangedHandler);
lea.Subscribe<StatusResponse>(StatusChangedHandler);

async void SelectionChangedHandler(SelectionChanged evt)
{
    // When the selection transitions to something selected to no selection, or vice versa, Dorico sends two
    // unprompted status responses, one before and one after the selection changed message.  These contain
    // partial data.  Only the first message will have the hasSelection value present.

    // If a note or rest is selected, check the values of Duration, RhythmDots, and RestMode.
    // If something else is selected, those properties will not be set.

    //Console.WriteLine($"\n\nHasSelection? {remote.CurrentStatus?.HasSelection}");
    //Console.WriteLine($"Duration: {remote.CurrentStatus?.Duration}");
    //Console.WriteLine($"RhythmDots: {remote.CurrentStatus?.RhythmDots}");
    //Console.WriteLine($"RestMode: {remote.CurrentStatus?.RestMode?.ToString() ?? "null"}");

    await WriteStatus(null);
}

async void StatusChangedHandler(StatusResponse evt)
{
    await WriteStatus(evt);
}

async Task WriteStatus(StatusResponse? evt)
{
    await _ss.WaitAsync();
    if (evt == null)
    {
        Console.WriteLine("\n\nSelection Changed");
    }
    else
    {
        //Console.WriteLine($"\n\n{evt.RawJson}");
        Console.WriteLine($"HasSelection: {evt.HasSelection}");
    }
    _ss.Release();
}


// NOTE! When the selection is changed, Dorico sends a SelectionChanged event and then a partial Status
// update.  This partial status update does not include the StatusResponse.HasSelection value.  Be aware
// of this when using the subscribing to the event aggregator for these events.



//CreateMetaFile();

Console.Write("\n\nPress any key to exit.");
Console.ReadKey(true);
Console.WriteLine();

// When done, disconnect nicely
await remote.DisconnectAsync();
Console.WriteLine($"Connected? {remote.IsConnected}");

// Give a moment for the logger to catch up
await Task.Delay(1000);

async Task InsertNoteAsync(Note note)
{
    // Since Dorico doesn't set the accidental as part of the pitch, the Note class has a helper
    // that returns the commands required to set the note and advance the caret.
    foreach (var command in note.GetNoteCommands())
    {
        await remote.SendRequestAsync(command);
    }
}

void CreateMetaFile()
{
    const string metaFolder = @"Dorico.Net\Meta";
    var metaDir = Environment.CurrentDirectory.Split(@"\DoricoNetExample")[0];
    
    var metaFile = Path.Combine(metaDir, metaFolder, "MetaData.json");
    using var commadnsStream = new FileStream(metaFile, FileMode.Create);
    using var streamWriter = new StreamWriter(commadnsStream);
    streamWriter.Write(JsonSerializer.Serialize(new MetaDataObject
    {
        Version = versionResponse?.ToString(),
        Commands = commands,
        EngravingOptions = engravingOptions,
        NotationOptions = notationOptions,
        LayoutOptions = layoutOptions
    },
    new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }));
}

class MetaDataObject
{
    public string? Version { get; init; }

    public int CommandsCount => Commands?.Count ?? 0;
    public CommandCollection? Commands { get; init; }

    public int EngravingOptionsCount => EngravingOptions?.Count ?? 0;
    public OptionCollection? EngravingOptions { get; init; }

    public int NotationOptionsCount => NotationOptions?.Count ?? 0;
    public OptionCollection? NotationOptions { get; init; }
    
    public int LayoutOptionsCount => LayoutOptions?.Count ?? 0;
    public OptionCollection? LayoutOptions { get; init; }
}
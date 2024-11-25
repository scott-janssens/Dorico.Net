using DoricoNet;
using DoricoNet.Commands;
using DoricoNet.Comms;
using DoricoNet.DataStructures;
using DoricoNet.Enums;
using DoricoNet.Exceptions;
using DoricoNet.Requests;
using DoricoNet.Responses;
using Lea;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

// Setup dependency injection
var services = new ServiceCollection()
    .AddSingleton(sp => LoggerFactory.Create(builder => 
        builder.AddFilter("DoricoRemote", LogLevel.Information).AddConsole()).CreateLogger("DoricoRemote"))
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

    // Show the top level tree nodes, but we won't recurse down in this demo.
    var node = commands.OrganizedItems!.ChildNodes.First();
    Console.WriteLine($"\nNode: {node.Path}\n");
    node.Values.ForEach(x => Console.WriteLine($"  \"{x.DisplayName}\", {x.Name}"));
}

// The CommandCollection contains CommandInfo records which describe the various commands. Each CommandInfo object
// contains lists of required and optional parameters.

var commandInfo = commands!["File.Open"];
Console.WriteLine($"\nCommand: {commandInfo}");

Console.WriteLine($"  Required Parameters: ");
foreach (var parameter in commandInfo.RequiredParameters)
{
    Console.WriteLine($"      {parameter}");
}

Console.WriteLine($"  Optional Parameters: ");
foreach (var parameter in commandInfo.OptionalParameters)
{
    Console.WriteLine($"      {parameter}");
}

Console.WriteLine();

// CommandInfo objects can be used to initialize Command request objects which are sent to Dorico to perform
// an operation.  Parameters can be passed into the constructor or added with the AddParameter() method.

// Command objects can be sent to Dorico in 2 ways:

// with the raw command/parameter values:
var fileOpenResponse = await remote.SendRequestAsync(new Command("File.Open", new CommandParameter("File", "<path>")));

// or with a command object from the commands list:
var switchCommandInfo = commands["Window.SwitchMode"];
var switchSetupCommand = new Command(switchCommandInfo, new CommandParameter("WindowMode", "kSetupMode"));
var switchResponse = await remote.SendRequestAsync(switchSetupCommand);

// NOTE!: There is currently no way to automatically distinguish which commands or requests will work with the
// connected Dorico edition.  We can tell if Dorico is Pro, Elements, or SE by the AppInfoResponse, but the get
// commands request doesn't tell us which parameters are invalid for non-Pro editions.  This can be seen by launching
// Dorico with CTRL held down to open as SE, or ALT/Option key to open as Elements.
switchSetupCommand = new Command("Window.SwitchMode", new CommandParameter("WindowMode", "kEngraveMode"));
switchResponse = await remote.SendRequestAsync(switchSetupCommand);

// If we're connected to Dorico SE or Elements, the previous request has no effect as those editions do not have 
// Engrave mode.  However, this doesn't not result in an error response.

// Make sure we return to Write mode.
switchSetupCommand = new Command("Window.SwitchMode", new CommandParameter("WindowMode", "kWriteMode"));
switchResponse = await remote.SendRequestAsync(switchSetupCommand);

// Currently there is no documentation from the Dorico team about what the parameters mean or what the valid values
// may be.  The best way to see the commands work is to perform the desired operation in Dorico and look at the
// application.log where Dorico echoes the commands and parameters.


// NOTE: Dorico.Net caches the CommandInfo objects after retrieving them, so subsequent calls to GetCommandsAsync()
// will not call Dorico.

// NOTE: Dorico returns a "kOK" to acknowledge receipt of the request. That does not necessarily mean the operation
// was successful.  For example, sending a bad path in the above File.Open command will receive a "kOK", but the
// operation will fail in Dorico.

// NOTE: Some commands, such as those to create projects appear to be disabled through the Remote Control API at
// the moment.


// IMPORTANT: At this point in the demo, a project must be loaded in Dorico. Either set a proper path in the Open
// command above, or manually create a new project in Dorico. Close any error dialogs in Dorico before continuing.


// Make sure we're not in Note Input mode as the rest of the demo assumes it's off at this point.
await remote.SendRequestAsync(new Command("NoteInput.Exit"));


// The information about the project's flows can be obtained:
var flowsResponse = await remote.GetFlowsAsync();
if (flowsResponse != null)
{
    foreach (var flow in flowsResponse.Flows)
    {
        Console.WriteLine($"Flow ID: {flow.FlowID}, Name: {flow.FlowName}");
    }
}
Console.WriteLine();


// And the layouts:
var layoutsResponse = await remote.GetLayoutsAsync();
if (layoutsResponse != null)
{
    foreach (var layout in layoutsResponse.Layouts)
    {
        Console.WriteLine($"Layout ID: {layout.LayoutID}, Name: {layout.LayoutName}," +
            $" Number: {layout.LayoutNumber}, Type: {layout.LayoutType}");
    }
}


// The various options and their values can be retrieved.
// The OptionsCollection is an OrganizedCollection like the CommandCollection.

// Engraving options are global
OptionCollection? engravingOptions = null;

try
{
    engravingOptions = await remote.GetEngravingOptionsAsync();
    Console.WriteLine($"\nEngraving Options: {engravingOptions?.Count}");
}
catch (DoricoException<Response> ex)
{
    // If we're connected to Dorico SE or Elements, Dorico responds with an error.  In this case "kUnknownOptionsType"
    // because Engraving options aren't available in those editions.  There is currently no way to automatically tell
    // which requests or commands are supported by the connected Dorico edition.
    Console.WriteLine(ex.Message);
}

// Notation options are per flow:
OptionCollection? notationOptions = null;
if (flowsResponse != null)
{
    try
    {
        notationOptions = await remote.GetNotationOptionsAsync(flowsResponse.Flows.First().FlowID);
        Console.WriteLine($"Notation Options: {notationOptions?.Count}");
    }
    catch (DoricoException<Response> ex)
    {
        Console.WriteLine(ex.Message);
    }
}

// Layout options are per layout
OptionCollection? layoutOptions = null;
if (layoutsResponse != null)
{
    var layoutId = layoutsResponse.Layouts.First().LayoutID;

    layoutOptions = await remote.GetLayoutOptionsAsync(layoutId);
    Console.WriteLine($"Layout Options: {layoutOptions?.Count}\n");

    // Modify a layout option of a specific layout. Multiple options can be set at once on multiple layouts,
    // but we'll just do one of each here.
    await remote.SetLayoutOptionsAsync(
        [new OptionValue("transpositionType", "kTransposingScore")],
        [layoutId]);

    // There are also enums to help specify which layouts to affect
    await remote.SetLayoutOptionsAsync(
        [new OptionValue("transpositionType", "kScoreInC")],
        LayoutId.kAll);
}

// Dorico sends unprompted status messages a LOT.  Dorico.Net uses an event aggregator called Lea that can be
// subscribed to, to receive unprompted responses when they are received.

var resetEvent = new ManualResetEvent(false);
var lea = serviceProvider.GetService<IEventAggregator>()!;
lea.Subscribe<StatusResponse>(StatusChangedHandler);

void StatusChangedHandler(StatusResponse statusResponse)
{
    Console.WriteLine($"\nStatus:\n {statusResponse}");

    // Status is sent a lot, so we'll unsubscribe after 1 demo.
    lea.Unsubscribe<StatusResponse>(StatusChangedHandler);
    resetEvent.Set();
}

// Entering NoteInput mode will cause a status update
await remote.SendRequestAsync(new Command("NoteInput.Enter"));

// Wait until we process and display the Status message
resetEvent.WaitOne();


// Making changes within Dorico is currently limited to essentially what you can do via the menu in Dorico.
// Via Status, limited information can be inferred about what is selected.

// If a note or rest is selected, the StatusResponse.Duration will be set.  If StatusResponse.RestMode is true, a
// rest is selected. There is currently no way to differentiate between any other item types that might be selected.

// There is currently no way to query for information about a bar, stave, or project in general. 

// It's possible to make changes, but you're at the mercy of whatever is currently selected or wherever the caret
// currently is.  When the "NoteInput.Enter" was sent above, the caret appeared wherever the current selection was.


// Let's add some notes:

// NoteInput.Enter is a toggle, so first make sure we're not already in Note Input mode otherwise sending
// NoteInput.Enter will actually exit Note Input mode. The current state can be determined from the most recent
// StatusResponse.NoteInputActive value, but it's easier to just send a NoteInput.Exit which always forces
// NoteInputActive to false;
await remote.SendRequestAsync(new Command("NoteInput.Exit"));
await remote.SendRequestAsync(new Command("NoteInput.Enter"));

await InsertNoteAsync(new("F#", 5));
await InsertNoteAsync(new("D", 5));
await remote.SendRequestAsync(new Command("NoteInput.MoveAdvance")); // Advance the caret to create a rest
await InsertNoteAsync(new("Bb", 4));

// Exit Note Input mode.
await remote.SendRequestAsync(new Command("NoteInput.Exit"));

// Delay to allow the Note insertion to finish so the following code doesn't pollute the Console output with
// Status updates.  You can comment out the next line to see the messages Dorico sends while operations occur.
await Task.Delay(2500);

// When the selection changes, Dorico sends three messages: Status, SelectionChanged, Status.

SemaphoreSlim _ss = new(1, 1);
lea.Subscribe<SelectionChangedResponse>(SelectionChangedHandler);
lea.Subscribe<StatusResponse>(StatusChangedHandler2);

async void SelectionChangedHandler(SelectionChangedResponse evt)
{
    await _ss.WaitAsync();

    Console.WriteLine("\nSelection changed.");

    _ss.Release();
}

async void StatusChangedHandler2(StatusResponse evt)
{
    // If a note or rest is selected, check the values of Duration, RhythmDots, and RestMode.
    // If something else is selected, those properties will not be set.

    await _ss.WaitAsync();

    Console.WriteLine($"\nStatus changed [{DateTime.Now:h:mm:ss.ffff}]");
    Console.WriteLine($"HasSelection? {remote.CurrentStatus?.HasSelection}");
    Console.WriteLine($"Duration: {remote.CurrentStatus?.Duration}");
    Console.WriteLine($"RhythmDots: {remote.CurrentStatus?.RhythmDots}");
    Console.WriteLine($"Accidental: {remote.CurrentStatus?.Accidental.ToString()}");
    Console.WriteLine($"RestMode: {remote.CurrentStatus?.RestMode?.ToString() ?? "null"}");

    _ss.Release();
}

// In Dorico, change the selection to see the messages Dorico sends.  For readability, this demo writes only the
// properties that pertain to the selection changing, rather than the entire Status message.


// This creates the metadata file listing all the commands and options.  The file is not used by Dorico.Net, but used
// to determine if new items have been exposed by the Remote API. tl;dr: Ignore this method call.
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
    // Since Dorico doesn't set the accidental as part of the pitch, the Note class has a helper that returns the
    // commands required to set the note and advance the caret.
    foreach (var command in note.GetNoteCommands())
    {
        await remote.SendRequestAsync(command);
    }
}

// This creates the metadata file listing all the commands and options. The file is not used by Dorico.Net, but used
// to determine if new items have been exposed by the Remote API.
#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
void CreateMetaFile()
{
    if (engravingOptions != null) // Must be Pro edition if not null
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
}
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances
#pragma warning restore CS8321 // Local function is declared but never used

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
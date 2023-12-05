# Dorico.Net
A library for working with Dorico's Remote Control API

Version 0.2.0-beta

# Description

See Example app for usage.

Leave feedback or open a branch if there's a common functionality you think should be added.

Internally, Dorico uses a command system to realize each piece of functionality. These commands are echoed in the application.log file (**On Windows:** C:\Users\\[UserName]\AppData\Roaming\Steinberg\Dorico 5\application.log, **On Mac:** /Library/Users/[UserName]/Application Support/Steinberg/Dorico 5). For example, opening a score generates a command "File.Open?File=[Path]".  Dorico's API allows you to send these commands to be executed.  (Some seem to be disabled, such as "File.New".) Bear in mind that this is not a plugin, so any UI is external to Dorico.

The big limitation at the moment is there is no way to query for contextual information.  The API can return what is currently selected and all the properties for that selection, but there's no good way to get any info about the bar/staff/etc. around that selection.

The Dorico API is not well documented. Examining the application.log is the best way to see which commands and what parameter values are used to accomplish a piece of functionality.

# Notes

The Note class does not currently support microtonality.


# Disclaimer

The Dorico trademark is registered in the US, Europe and other countries by Steinberg Media Technologies GmbH and is used with permission.
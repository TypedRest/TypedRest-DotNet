---
uid: TypedRest.CommandLine.IO
summary: Input and output methods for command-line interfaces.
---
`IConsole` is the seam between the commands and the terminal. Commands read entities, prompts and masked secrets through it and write results and errors back to it, never calling `System.Console` themselves. Swapping in your own implementation lets you unit-test a command by feeding it canned input and asserting on the output, or reuse the commands from a GUI.

The default implementation reads stdin and writes stdout/stderr, using JSON for entities in both directions; unless a type brings its own `ToString()`, which is then preferred for output, so entities can present themselves in a human-readable form without any change to the commands. Errors go to stderr in red, including the messages of all inner exceptions.

`StreamPrinter` adapts an `IObservable<T>` to an `IConsole`: it subscribes, prints each entity as it arrives and completes when the stream ends or is cancelled. This is how the <xref:TypedRest.CommandLine.Commands.Reactive> commands display push streams.

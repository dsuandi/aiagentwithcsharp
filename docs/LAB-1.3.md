# Lab 1.3: C# Function Calling

1. Switch to `lab-1.3-function-calling` and keep your previous project configuration.
2. Run `dotnet build VesselCopilot.Console -c Release` and `dotnet run --project VesselCopilot.Tests -c Release`.
3. Run `dotnet run --project VesselCopilot.Console -c Release -- tools`. Check that the model uses both position and procedure tools before composing an answer.
4. Run `dotnet run --project VesselCopilot.Console -c Release -- tools "Where is VL-118?"`.
5. Test `VL-99X` (invalid format), `VL-305` (unknown vessel), and an unknown procedure. The model must not invent missing facts.
6. Ask `What does ETA mean?` and observe that a tool is not required for general terminology.
7. Compare the model response to the actual synthetic values in `VesselTools.cs`. Function calls are probabilistic; validate the tool functions independently using the local checks.

The invocation middleware is capped at five rounds. Tool arguments use strict ASCII identifiers and exact string boundaries; the guide's `\d` and `$` pattern also admitted non-ASCII digits and a trailing newline. Diagnostic output identifies tool names but omits arguments and result bodies.
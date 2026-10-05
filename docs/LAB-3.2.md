# Lab 3.2: Human Approval And MCP

1. Switch to `lab-3.2-approval-and-mcp`, restore the agent project in locked mode, and build it in Release. The project also builds the MCP server.
2. Run `dotnet run --project VesselCopilot.Tests -c Release`.
3. Run `dotnet run --project VesselCopilot.Agents -c Release -- approve`. Review the proposed function and arguments. Enter `y` to approve the synthetic draft.
4. Run `approve` again and press Enter to reject. Check `Synthetic drafts created this run: 0`. Approval audits contain the decision and tool name, not argument values.
5. Run `dotnet run --project VesselCopilot.Agents -c Release -- mcp-check`. It exercises discovery and valid, invalid, and unknown vessel calls over stdio without model calls.
6. Run `mcp`. The live model should combine the local position tool with the MCP voyage and ETA tool.
7. Review the server source and allowed tool name before exposing any other server to the agent.

The client launches the compiled server DLL using an application-relative path, not `dotnet run`. This avoids build output contaminating stdout's JSON-RPC messages and removes the guide's current-directory requirement. An external installation may override `Mcp:ServerDll` with an absolute trusted path. Server logs go to stderr.

Only y/Y approves; EOF and any other answer deny. Approval is bounded to three rounds, and rejection does not automatically prompt again. Draft IDs use GUIDs rather than a timestamp that can collide within one second. Drafts are synthetic in-memory demonstrations, not durable work orders or scheduled maintenance.
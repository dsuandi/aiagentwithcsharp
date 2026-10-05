# Checkpoint Checkout Validation

Every branch was checked out independently on 2026-10-06. The command below compiled that branch's available source and ran its local checks:

```bash
dotnet run --project VesselCopilot.Tests --configuration Release --verbosity quiet
```

| Branch | Checks Passed |
| --- | ---: |
| lab-1.1-provision-and-connect | 10 |
| lab-1.2-streaming-chat | 13 |
| lab-1.3-function-calling | 20 |
| lab-2.1-ingest-and-index | 28 |
| lab-2.2-grounded-rag | 33 |
| lab-2.3-retrieval-evaluation | 37 |
| lab-3.1-first-agent | 37 |
| lab-3.2-approval-and-mcp | 41 |
| lab-3.3-multi-agent-workflow | 46 |

The final suite additionally tests complete approval payload visibility and cancellation while a prompt is unanswered. The same approval implementation was backported to Lab 3.2. Relevant Day 2 and Day 3 branches contain the complete patched reference snapshot, not an unpublished submodule commit.

This confirms source-checkpoint compilation and local behavior, not all live Azure requirements. Live results and blocked steps are recorded separately in `REVIEW.md`. Repository state was returned to `main` after validation.
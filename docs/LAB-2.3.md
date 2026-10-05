# Lab 2.3: Retrieval Evaluation

Live retrieval scores are unverified until a Search service and populated training index are available. Do not substitute local metric checks for a real measurement.

1. Switch to `lab-2.3-retrieval-evaluation`, build the console project, and run the local checks.
2. Review the ten questions in `RetrievalEval.Questions`: eight answerable and two out of corpus. Edit or extend them for your own synthetic corpus.
3. On your approved training index, run `dotnet run --project VesselCopilot.Console -c Release -- eval`. Record Text, Vector, and Hybrid hit@3 before changing anything.
4. Run the two out-of-corpus questions through `ask` and score abstention separately. An empty expected title is not a retrieval hit.
5. Change exactly one factor. For a chunk-size experiment, choose a fresh index and run `ingest --Chunking:MaxChars=400 --Chunking:OverlapChars=100`, then `eval` against that same index. Preserve the baseline index and question set for comparison.
6. For the injection exercise, copy `docs/fixtures/INJ-01-injection-test.md` into the console data folder, rebuild so it is copied to output, and ingest into a separate test index. Ask what the watch must log during heavy weather. Verify the answer cites HW-07 and does not obey the injected instruction.
7. Remove the injection fixture from both the source data folder and build output, rebuild, and reset the test-index override. Delete only your designated test index after instructor approval. Never ingest the injection fixture into a shared index.
8. Optionally run `quality` to score relevance and groundedness with Microsoft.Extensions.AI.Evaluation.Quality. The judging calls incur model usage; this path requires live retrieval and is not verified here.

With only three documents and k=3, hit@3 can be artificially high because every document may appear. This exercise teaches the measurement workflow, not statistical significance. Report the numerator and denominator, preserve the question set, and use a larger corpus for meaningful comparisons. Model-judge scores are probabilistic and are not a security verdict.
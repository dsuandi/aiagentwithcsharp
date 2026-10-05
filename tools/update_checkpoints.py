from pathlib import Path
import subprocess
from xml.etree import ElementTree as ET


BRANCHES = [
    "lab-1.1-provision-and-connect", "lab-1.2-streaming-chat", "lab-1.3-function-calling",
    "lab-2.1-ingest-and-index", "lab-2.2-grounded-rag", "lab-2.3-retrieval-evaluation",
    "lab-3.1-first-agent", "lab-3.2-approval-and-mcp", "lab-3.3-multi-agent-workflow",
]
SOURCE_FILES = [
    "VesselCopilot.Console/VesselCopilot.Console.csproj",
    "VesselCopilot.Console/appsettings.json", "VesselCopilot.Console/ChatClientFactory.cs",
    "VesselCopilot.Console/EmbeddingFactory.cs", "VesselCopilot.Console/ErrorHints.cs",
    "VesselCopilot.Console/SearchDemo.cs", "VesselCopilot.Console/GroundedAnswer.cs",
    "VesselCopilot.Console/QualityEval.cs", "VesselCopilot.Console/packages.lock.json",
    "VesselCopilot.Agents/AgentBasics.cs", "VesselCopilot.Agents/ApprovalDemo.cs",
    "VesselCopilot.Agents/McpDemo.cs", "VesselCopilot.Agents/VesselWorkflow.cs",
]
DOCUMENTS = [
    "README.md", "LAB.md", "docs/REVIEW.md", "docs/STABLE-PACKAGES.md",
    "docs/STABLE-VALIDATION.md", "docs/STUDENT-LAB-GUIDE-STABLE.docx",
]


def run(*arguments):
    subprocess.run(arguments, check=True)


def exists_at(branch, path):
    return subprocess.run(["git", "cat-file", "-e", f"{branch}:{path}"],
                          stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL).returncode == 0


def main():
    status = subprocess.check_output(["git", "status", "--porcelain"], text=True)
    if status:
        raise RuntimeError("Commit or preserve current changes before updating checkpoints.")
    try:
        for branch in BRANCHES:
            run("git", "switch", branch)
            paths = [path for path in SOURCE_FILES if exists_at(branch, path)] + DOCUMENTS
            if exists_at(branch, "reference/rag-app"):
                paths.append("reference/rag-app")
            for lab in ["2.1", "2.2", "2.3"]:
                path = f"docs/LAB-{lab}.md"
                if exists_at(branch, path):
                    paths.append(path)
            run("git", "restore", "--source", "main", "--", *paths)
            agent_project = Path("VesselCopilot.Agents/VesselCopilot.Agents.csproj")
            if exists_at(branch, str(agent_project)):
                run("git", "restore", "--source", "main", "--", str(agent_project))
                if not exists_at(branch, "VesselMcpServer/VesselMcpServer.csproj"):
                    root = ET.parse(agent_project).getroot()
                    for group in root.findall("ItemGroup"):
                        for reference in list(group):
                            if "VesselMcpServer" in reference.attrib.get("Include", ""):
                                group.remove(reference)
                    ET.indent(root, space="  ")
                    agent_project.write_text(ET.tostring(root, encoding="unicode") + "\n")
                run("dotnet", "build", str(agent_project), "-c", "Release", "--verbosity", "quiet")
            run("dotnet", "run", "--project", "VesselCopilot.Tests", "-c", "Release", "--verbosity", "quiet")
            if exists_at(branch, "reference/rag-app"):
                run("dotnet", "build", "reference/rag-app/app/app.sln", "-c", "Release",
                    "--source", "https://api.nuget.org/v3/index.json", "--verbosity", "quiet")
            run("git", "add", "-A", "--", "VesselCopilot.Console", "VesselCopilot.Tests", "docs", "README.md", "LAB.md")
            if exists_at(branch, str(agent_project)):
                run("git", "add", "VesselCopilot.Agents")
            if exists_at(branch, "reference/rag-app"):
                run("git", "add", "reference/rag-app")
            run("git", "add", "-f", "docs/STUDENT-LAB-GUIDE-STABLE.docx")
            run("git", "commit", "-m", "upgrade: validate stable packages and student material")
    finally:
        run("git", "switch", "main")


if __name__ == "__main__":
    main()
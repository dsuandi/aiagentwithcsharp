import argparse
from copy import deepcopy
from pathlib import Path
import re
from xml.etree import ElementTree as ET
from zipfile import ZipFile


NAMESPACE = "http://schemas.openxmlformats.org/wordprocessingml/2006/main"
NS = {"w": NAMESPACE}


def set_text(paragraph, text):
    properties = paragraph.find("w:pPr", NS)
    template = paragraph.find("w:r/w:rPr", NS)
    for child in list(paragraph):
        if child is not properties:
            paragraph.remove(child)
    run = ET.SubElement(paragraph, f"{{{NAMESPACE}}}r")
    if template is not None:
        run.append(deepcopy(template))
    value = ET.SubElement(run, f"{{{NAMESPACE}}}t")
    value.set("{http://www.w3.org/XML/1998/namespace}space", "preserve")
    value.text = text


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("source", type=Path)
    parser.add_argument("destination", type=Path)
    options = parser.parse_args()
    if options.source.resolve() == options.destination.resolve():
        parser.error("Preserve the original guide; use a different destination.")
    root_folder = Path(__file__).resolve().parents[1]
    versions = {}
    for project in [root_folder / "VesselCopilot.Console/VesselCopilot.Console.csproj",
                    root_folder / "VesselCopilot.Agents/VesselCopilot.Agents.csproj",
                    root_folder / "VesselMcpServer/VesselMcpServer.csproj"]:
        for reference in ET.parse(project).getroot().iter("PackageReference"):
            versions[reference.attrib["Include"]] = reference.attrib["Version"]
    versions["Microsoft.Extensions.AI.Evaluation"] = "10.10.0"
    with ZipFile(options.source) as original:
        document = ET.fromstring(original.read("word/document.xml"))
        paragraphs = document.findall(".//w:p", NS)
        replacements = 0
        for paragraph in paragraphs:
            text = "".join(node.text or "" for node in paragraph.findall(".//w:t", NS))
            changed = text
            match = re.match(r"dotnet add package ([\w.]+)", text)
            if match and match.group(1) in versions:
                package = match.group(1)
                changed = f"dotnet add package {package} --version {versions[package]}"
            elif "--prerelease" in text or "prerelease package" in text.lower() or "SDK is prerelease" in text:
                changed = "Use the pinned stable package versions in the checkpoint project. Restore in locked mode; do not select preview or beta releases. See docs/STABLE-PACKAGES.md."
            changed = changed.replace(".GetProjectOpenAIClient()", ".GetProjectOpenAIClient()")
            if changed != text:
                set_text(paragraph, changed)
                replacements += 1
        body = document.find("w:body", NS)
        if body is None:
            raise ValueError("The guide has no document body.")
        section = body.find("w:sectPr", NS)
        insert_at = list(body).index(section) if section is not None else len(body)
        notes = [
            "Stable SDK Edition - Instructor Corrections",
            "The stable checkpoint source and Markdown lab instructions supersede earlier inline snippets where SDK APIs differ. Every package command uses an explicit stable version; no prerelease package is required.",
            "Configure BOTH Foundry:ProjectEndpoint and Foundry:OpenAIEndpoint before Lab 1.1. The OpenAI resource endpoint is used with /openai/v1/; the project endpoint is not interchangeable with it.",
            "The stable project Responses adapter has a binary incompatibility with OpenAI 2.14.0. Do not use GetProjectResponsesClient or AIProjectClient.AsAIAgent in these checkpoints. Use ChatClientFactory.Create and new ChatClientAgent instead; see the complete tested source in the corresponding lab branch.",
            "Stable packages can still annotate individual APIs as experimental. OPENAI001 is a scoped API opt-in, not a prerelease NuGet dependency.",
            "Use actual deployment names. The tested fallback deployment is workshop-primary (model gpt-5.4-mini), and the embedding deployment is text-embedding-3-large with requested dimensions 1536.",
            "Search keyword mode must not send an empty vector query. Check SearchDemo.Options and run the local regression suite before live retrieval.",
            "The quality evaluator omits temperature and top-p sampling overrides for reasoning models. The tested primary and fallback judges returned relevance 4 and groundedness 5.",
            "Read docs/STABLE-PACKAGES.md and docs/STABLE-VALIDATION.md for commands, package versions, live Lab 2 results, and explicitly blocked reference-app/runtime steps.",
        ]
        for package, version in sorted(versions.items()):
            notes.append(f"dotnet add package {package} --version {version}")
        for text in notes:
            paragraph = ET.Element(f"{{{NAMESPACE}}}p")
            set_text(paragraph, text)
            body.insert(insert_at, paragraph)
            insert_at += 1
        final_text = "\n".join(node.text or "" for node in document.findall(".//w:t", NS))
        if "--prerelease" in final_text:
            raise ValueError("A prerelease installation flag remains in the corrected guide.")
        options.destination.parent.mkdir(parents=True, exist_ok=True)
        with ZipFile(options.destination, "w") as corrected:
            for member in original.infolist():
                data = ET.tostring(document, encoding="utf-8", xml_declaration=True) if member.filename == "word/document.xml" else original.read(member.filename)
                corrected.writestr(member, data)
    print(f"Created {options.destination}; corrected {replacements} existing paragraphs; pinned {len(versions)} packages.")


if __name__ == "__main__":
    main()
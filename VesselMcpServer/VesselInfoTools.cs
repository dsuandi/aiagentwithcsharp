using System.ComponentModel;
using System.Text.RegularExpressions;
using ModelContextProtocol.Server;

[McpServerToolType]
public static partial class VesselInfoTools
{
    [GeneratedRegex(@"\AVL-[0-9]{3}\z")]
    private static partial Regex VesselIdPattern();

    [McpServerTool(Name = "get_vessel_voyage"), Description("Gets the synthetic voyage and ETA for a vessel.")]
    public static string GetVesselVoyage([Description("Identifier such as VL-204.")] string vesselId)
    {
        if (string.IsNullOrEmpty(vesselId) || !VesselIdPattern().IsMatch(vesselId))
            return "Error: vesselId must look like VL-123.";
        return vesselId switch
        {
            "VL-204" => "VL-204: Singapore to Tanjung Priok, ETA 2026-10-03 06:00 local.",
            "VL-118" => "VL-118: at anchor off Singapore, awaiting berth.",
            _ => $"No voyage found for {vesselId}."
        };
    }
}
using System.ComponentModel;
using System.Text.RegularExpressions;

public static partial class VesselTools
{
    [GeneratedRegex(@"\AVL-[0-9]{3}\z")]
    private static partial Regex VesselIdPattern();

    private static readonly Dictionary<string, string> Positions = new()
    {
        ["VL-204"] = "05 12S 106 48E, course 078, 11.2 kn, reported 2026-09-30 12:00 UTC",
        ["VL-118"] = "01 16N 103 50E, at anchor, reported 2026-09-30 11:30 UTC"
    };
    private static readonly Dictionary<string, string> Procedures = new(StringComparer.OrdinalIgnoreCase)
    {
        ["HW-07"] = "HW-07 Heavy-weather passage, revision 4: reduce speed, secure deck cargo, brief the crew, log position and weather every hour.",
        ["MX-02"] = "MX-02 Engine temperature alarm response, revision 2: reduce load, check cooling pump and thermostatic valve, inform the chief engineer."
    };

    public static bool IsValidVesselId(string? vesselId) =>
        vesselId is not null && VesselIdPattern().IsMatch(vesselId);

    [Description("Gets the synthetic last reported position of a vessel.")]
    public static string GetVesselPosition([Description("Identifier such as VL-204.")] string vesselId)
    {
        if (!IsValidVesselId(vesselId)) return "Error: vesselId must look like VL-123.";
        return Positions.TryGetValue(vesselId, out string? position)
            ? $"{vesselId}: {position}" : $"No position found for {vesselId}.";
    }

    [Description("Gets an approved synthetic procedure: HW-07 heavy weather or MX-02 engine temperature alarm.")]
    public static string GetApprovedProcedure([Description("Procedure code such as HW-07.")] string procedureCode) =>
        Procedures.TryGetValue(procedureCode ?? "", out string? procedure)
            ? procedure : "Error: unknown procedure code. Valid codes: HW-07, MX-02.";
}
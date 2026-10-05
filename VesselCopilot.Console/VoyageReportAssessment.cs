using Microsoft.Extensions.AI;

public record VoyageReportAssessment(
    string Summary, string[] OperationalTopics, string[] ReportedRisks,
    string RequiredFollowUp, string RiskLevel);

public static class VoyageAssessment
{
    public const string SampleReport = """
        SYNTHETIC VOYAGE REPORT - VL-204 - noon report, day 6
        Position 05 12S 106 48E, course 078, speed 11.2 knots.
        Wind SW force 7, swell 4 m, rolling heavily. Procedure HW-07 applied at 06:00.
        Port main engine jacket-water temperature varied between 82 and 91 C for two hours.
        One container lashing in bay 14 found loose and re-secured. No injuries.
        ETA Tanjung Priok unchanged.
        """;

    public static void Validate(VoyageReportAssessment assessment)
    {
        if (assessment.RiskLevel is not ("Low" or "Medium" or "High") ||
            string.IsNullOrWhiteSpace(assessment.Summary) ||
            string.IsNullOrWhiteSpace(assessment.RequiredFollowUp) ||
            assessment.OperationalTopics is null || assessment.ReportedRisks is null)
            throw new InvalidOperationException("The assessment contains invalid or missing values.");
    }

    public static async Task RunAsync(IChatClient chatClient, CancellationToken cancellationToken)
    {
        ChatResponse<VoyageReportAssessment> response = await chatClient.GetResponseAsync<VoyageReportAssessment>(
            $"Assess the synthetic voyage report below. RiskLevel must be Low, Medium or High.\n{SampleReport}",
            new ChatOptions { MaxOutputTokens = AppConfig.Model.MaxOutputTokens }, cancellationToken: cancellationToken);
        VoyageReportAssessment assessment = response.Result;
        Validate(assessment);
        Console.WriteLine($"Summary: {assessment.Summary}");
        Console.WriteLine($"Risk: {assessment.RiskLevel}");
        Console.WriteLine($"Topics: {string.Join(", ", assessment.OperationalTopics)}");
        Console.WriteLine($"Risks: {string.Join("; ", assessment.ReportedRisks)}");
        Console.WriteLine($"Follow-up: {assessment.RequiredFollowUp}");
    }
}
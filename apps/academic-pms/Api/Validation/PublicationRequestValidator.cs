namespace AcademicPms.Api.Validation;

public static class PublicationRequestValidator
{
    public static bool ValidExternalPropertyId(string? externalPropertyId) =>
        externalPropertyId is null
        || (!string.IsNullOrWhiteSpace(externalPropertyId)
            && externalPropertyId.Length <= 100);
}

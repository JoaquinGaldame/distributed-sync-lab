namespace AcademicPms.Api.Models;

public sealed record PublicationInput(
    string? ExternalPropertyId,
    bool Published);

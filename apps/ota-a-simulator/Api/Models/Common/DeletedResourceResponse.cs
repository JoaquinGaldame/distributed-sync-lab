namespace OtaASimulator.Models.Common;

public sealed record DeletedResourceResponse(
    bool Deleted,
    string Resource,
    long Id);

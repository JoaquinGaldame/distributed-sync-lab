namespace AcademicPms.Api.Models;

public sealed record ChannelInput(
    string Code,
    string Name,
    bool IsEnabled);

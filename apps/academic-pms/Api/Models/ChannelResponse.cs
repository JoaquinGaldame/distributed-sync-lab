using AcademicPms.Domain;

namespace AcademicPms.Api.Models;

public sealed record ChannelResponse(
    long Id,
    string Code,
    string Name,
    bool IsEnabled,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static ChannelResponse From(Channel channel) => new(
        channel.Id,
        channel.Code,
        channel.Name,
        channel.IsEnabled,
        channel.CreatedAt,
        channel.UpdatedAt);
}

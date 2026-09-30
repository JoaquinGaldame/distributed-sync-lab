using AcademicPms.Domain;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Application;

public sealed class ChannelService(PmsDbContext db)
{
    public async Task<Channel?> CreateAsync(
        string code,
        string name,
        bool isEnabled,
        CancellationToken cancellationToken)
    {
        if (await db.Channels.AnyAsync(
                x => x.Code == code,
                cancellationToken))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var channel = new Channel
        {
            Code = code,
            Name = name,
            IsEnabled = isEnabled,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Channels.Add(channel);
        await db.SaveChangesAsync(cancellationToken);
        return channel;
    }

    public async Task<Channel?> UpdateAsync(
        long id,
        string name,
        bool isEnabled,
        CancellationToken cancellationToken)
    {
        var channel = await db.Channels.FindAsync(
            [id], cancellationToken);

        if (channel is null)
            return null;

        if (channel.Name == name && channel.IsEnabled == isEnabled)
            return channel;

        channel.Name = name;
        channel.IsEnabled = isEnabled;
        channel.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return channel;
    }
}

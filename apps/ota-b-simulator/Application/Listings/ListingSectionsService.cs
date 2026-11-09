using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Api.Mappers;
using OtaBSimulator.Api.Models.Listings;
using OtaBSimulator.Application.Common;
using OtaBSimulator.Domain;
using OtaBSimulator.Infrastructure.Persistence;

namespace OtaBSimulator.Application.Listings;

public sealed class ListingSectionsService(OtaBDbContext db, ResourceGuard guard)
{
    public async Task<LocationResponse> PutLocation(Guid id, LocationRequest r, CancellationToken ct)
    {
        RequestRules.Text(r.City, "city");
        if (r.Latitude.HasValue != r.Longitude.HasValue)
            throw ApiException.Invalid("latitude y longitude deben informarse juntas o ser ambas null.");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockedListingAsync(id, ct);
        var x = await db.Locations.FindAsync([id], ct);
        if (x is null) { x = new Location { ListingId = id }; db.Locations.Add(x); }
        x.CountryCode = r.CountryCode; x.Region = r.Region; x.City = r.City.Trim(); x.Neighbourhood = r.Neighbourhood;
        x.AddressLine = r.AddressLine; x.PostalCode = r.PostalCode; x.Latitude = r.Latitude; x.Longitude = r.Longitude;
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return ListingMapper.ToResponse(x);
    }

    public async Task<IReadOnlyList<ContactResponse>> PutContacts(Guid id, IReadOnlyList<ContactRequest>? requests, CancellationToken ct)
    {
        if (requests is null || requests.Any(x => x is null)) throw ApiException.Invalid("contacts debe ser una colección y no admite elementos null.");
        RequestRules.NoDuplicates(requests.Select(x => x.RoleCode), "roleCode");
        foreach (var x in requests)
            if (string.IsNullOrWhiteSpace(x.Email) && string.IsNullOrWhiteSpace(x.Phone))
                throw ApiException.Invalid("Cada contacto debe incluir email o phone.");
        var roles = requests.Select(x => x.RoleCode).Distinct().ToArray();
        var languages = requests.Select(x => x.LanguageCode).Distinct().ToArray();
        if (await db.ContactRoles.CountAsync(x => roles.Contains(x.Code), ct) != roles.Length)
            throw ApiException.Invalid("Uno o más roleCode no existen en el catálogo.");
        if (await db.Languages.CountAsync(x => languages.Contains(x.Code), ct) != languages.Length)
            throw ApiException.Invalid("Uno o más languageCode no existen en el catálogo.");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockedListingAsync(id, ct);
        db.Contacts.RemoveRange(await db.Contacts.Where(x => x.ListingId == id).ToListAsync(ct));
        var entities = requests.Select(x => new Contact { ListingId = id, RoleCode = x.RoleCode, DisplayName = x.DisplayName, Email = x.Email, Phone = x.Phone, LanguageCode = x.LanguageCode }).ToArray();
        db.Contacts.AddRange(entities); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return entities.OrderBy(x => x.RoleCode).Select(ListingMapper.ToResponse).ToArray();
    }

    public async Task<IReadOnlyList<TextResponse>> PutTexts(Guid id, IReadOnlyList<TextRequest>? requests, CancellationToken ct)
    {
        if (requests is null || requests.Any(x => x is null)) throw ApiException.Invalid("texts debe ser una colección y no admite elementos null.");
        RequestRules.NoDuplicates(requests.Select(x => x.LanguageCode), "languageCode");
        foreach (var x in requests) RequestRules.Text(x.Description, "description");
        var codes = requests.Select(x => x.LanguageCode).Distinct().ToArray();
        var languages = await db.Languages.Where(x => codes.Contains(x.Code)).ToDictionaryAsync(x => x.Code, ct);
        if (languages.Count != codes.Length) throw ApiException.Invalid("Uno o más languageCode no existen en el catálogo.");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockedListingAsync(id, ct);
        db.ListingTexts.RemoveRange(await db.ListingTexts.Where(x => x.ListingId == id).ToListAsync(ct));
        var entities = requests.Select(x => new ListingText { ListingId = id, LanguageId = languages[x.LanguageCode].Id, Language = languages[x.LanguageCode], Summary = x.Summary, Description = x.Description.Trim() }).ToArray();
        db.ListingTexts.AddRange(entities); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return entities.OrderBy(x => x.Language.Code).Select(ListingMapper.ToResponse).ToArray();
    }

    public async Task<IReadOnlyList<string>> PutAmenities(Guid id, IReadOnlyList<string>? codes, CancellationToken ct)
    {
        if (codes is null || codes.Any(x => x is null)) throw ApiException.Invalid("amenities debe ser una colección y no admite elementos null.");
        RequestRules.NoDuplicates(codes, "amenityCode");
        var catalog = await db.Amenities.Where(x => codes.Contains(x.Code)).ToDictionaryAsync(x => x.Code, ct);
        if (catalog.Count != codes.Count) throw ApiException.Invalid("Uno o más códigos de amenities no existen en el catálogo.");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockedListingAsync(id, ct);
        db.ListingAmenities.RemoveRange(await db.ListingAmenities.Where(x => x.ListingId == id).ToListAsync(ct));
        db.ListingAmenities.AddRange(codes.Select(code => new ListingAmenity { ListingId = id, AmenityId = catalog[code].Id }));
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return codes.OrderBy(x => x).ToArray();
    }

    public async Task<HouseRulesResponse> PutHouseRules(Guid id, HouseRulesRequest r, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(id, ct);
        var x = await db.HouseRules.FindAsync([id], ct);
        if (x is null) { x = new HouseRules { ListingId = id }; db.HouseRules.Add(x); }
        x.CheckInFrom = r.CheckInFrom; x.CheckInUntil = r.CheckInUntil; x.CheckOutFrom = r.CheckOutFrom; x.CheckOutUntil = r.CheckOutUntil;
        x.MinimumGuestAge = r.MinimumGuestAge; x.AllowsChildren = r.AllowsChildren; x.AllowsPets = r.AllowsPets;
        x.AllowsSmoking = r.AllowsSmoking; x.AllowsParties = r.AllowsParties; x.AdditionalRules = r.AdditionalRules;
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return ListingMapper.ToResponse(x);
    }

    public async Task<BookingRulesResponse> PutBookingRules(Guid id, BookingRulesRequest r, CancellationToken ct)
    {
        if (r.MaxNights is not null && r.MaxNights < r.MinNights) throw ApiException.Invalid("maxNights debe ser mayor o igual que minNights.");
        if (!await db.CancellationTerms.AnyAsync(x => x.Code == r.CancellationTermCode, ct)) throw ApiException.Invalid("cancellationTermCode no existe en el catálogo.");
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(id, ct);
        var x = await db.BookingRules.FindAsync([id], ct);
        if (x is null) { x = new BookingRules { ListingId = id }; db.BookingRules.Add(x); }
        x.CancellationTermCode = r.CancellationTermCode; x.MinNights = r.MinNights; x.MaxNights = r.MaxNights;
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return ListingMapper.ToResponse(x);
    }

    public async Task<PricingResponse> PutPricing(Guid id, PricingRequest r, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(id, ct);
        var x = await db.ListingPricing.FindAsync([id], ct);
        if (x is null) { x = new ListingPricing { ListingId = id }; db.ListingPricing.Add(x); }
        x.BaseNightlyAmountMinor = r.BaseNightlyAmountMinor;
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return ListingMapper.ToResponse(x);
    }

    public async Task<IReadOnlyList<FeeResponse>> PutFees(Guid id, IReadOnlyList<FeeRequest>? requests, CancellationToken ct)
    {
        if (requests is null || requests.Any(x => x is null)) throw ApiException.Invalid("fees debe ser una colección y no admite elementos null.");
        RequestRules.NoDuplicates(requests.Select(x => x.Code), "code");
        var bases = requests.Select(x => x.BasisCode).Distinct().ToArray();
        if (await db.FeeBases.CountAsync(x => bases.Contains(x.Code), ct) != bases.Length) throw ApiException.Invalid("Uno o más basisCode no existen en el catálogo.");
        foreach (var x in requests)
        {
            RequestRules.Text(x.Code, "code"); RequestRules.Text(x.Name, "name");
            var percent = x.BasisCode == "LODGING_PERCENT";
            if (percent && (x.Percent is null || x.AmountMinor is not null)) throw ApiException.Invalid("LODGING_PERCENT exige percent y no admite amountMinor.");
            if (!percent && (x.AmountMinor is null || x.Percent is not null)) throw ApiException.Invalid("La base indicada exige amountMinor y no admite percent.");
        }
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(id, ct);
        db.ListingFees.RemoveRange(await db.ListingFees.Where(x => x.ListingId == id).ToListAsync(ct));
        var entities = requests.Select(x => new ListingFee { ListingId = id, Code = x.Code, Name = x.Name.Trim(), BasisCode = x.BasisCode, AmountMinor = x.AmountMinor, Percent = x.Percent, IsIncluded = x.IsIncluded }).ToArray();
        db.ListingFees.AddRange(entities); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return entities.OrderBy(x => x.Code).Select(ListingMapper.ToResponse).ToArray();
    }
}

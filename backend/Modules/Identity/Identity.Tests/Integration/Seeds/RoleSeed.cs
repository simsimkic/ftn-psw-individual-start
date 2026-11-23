using Microsoft.AspNetCore.Identity;

namespace Identity.Tests.Integration.Seeds;

internal static class RoleSeed
{
    public static readonly IdentityRole<Guid> Guide = new() { Id = Guid.Parse("00000000-0000-0000-0001-000000000002"), Name = "guide", NormalizedName = "GUIDE" };
    public static readonly IdentityRole<Guid> Tourist = new() { Id = Guid.Parse("00000000-0000-0000-0001-000000000003"), Name = "tourist", NormalizedName = "TOURIST" };

    public static object[] All => [Guide, Tourist];
}

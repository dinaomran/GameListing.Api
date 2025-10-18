using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GameListing.Api.Common.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameListing.Api.Domain.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "eb607417-01cb-49aa-8bd0-c571e7a79190", // Id is by default a GUID string
                Name = RoleNames.Administrator,
                NormalizedName = RoleNames.Administrator.ToUpper()
            },
            new IdentityRole
            {
                Id = "27610462-eced-481a-8cd2-04193f641b7b",
                Name = RoleNames.User,
                NormalizedName = RoleNames.User.ToUpper()
            }
        );
    }
}
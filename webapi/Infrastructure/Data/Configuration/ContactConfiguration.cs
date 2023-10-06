using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webapi.Domain.Entities;

namespace webapi.Infrastructure.Data.Configuration;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.EmailAddress).IsRequired();
    }
}
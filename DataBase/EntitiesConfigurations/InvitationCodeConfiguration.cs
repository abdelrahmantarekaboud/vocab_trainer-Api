using System.Text.Json;
using VocabTrainer.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VocabTrainer.Api.DataBase.EntitiesConfigurations
{
    public class InvitationCodeConfiguration : IEntityTypeConfiguration<InvitationCode>
    {
        public void Configure(EntityTypeBuilder<InvitationCode> b)
        {
            b.HasIndex(x => x.Code).IsUnique();

            var comparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            b.Property(x => x.Languages)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new()
                )
                .Metadata.SetValueComparer(comparer);
        }
    }
}
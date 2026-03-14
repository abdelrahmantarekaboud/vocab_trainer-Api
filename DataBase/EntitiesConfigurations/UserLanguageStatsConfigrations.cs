using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VocabTrainer.Api.Entities;

namespace VocabTrainer.Api.DataBase.EntitiesConfigurations;

public class UserLanguageStatsConfigrations : IEntityTypeConfiguration<UserLanguageStats>
{
    public void Configure(EntityTypeBuilder<UserLanguageStats> b)
    {
        b.HasIndex(x => new { x.UserId, x.LanguageId })
        .IsUnique();
    }
}
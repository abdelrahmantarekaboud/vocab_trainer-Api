using VocabTrainer.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VocabTrainer.Api.DataBase.EntitiesConfigurations
{
    public class WordConfiguration : IEntityTypeConfiguration<Word>
    {
        public void Configure(EntityTypeBuilder<Word> b)
        {
            b.HasIndex(w => new { w.OwnerId, w.LanguageId, w.Text }).IsUnique();
        }
    }
}
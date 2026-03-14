using VocabTrainer.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VocabTrainer.Api.DataBase.EntitiesConfigurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> b)
        {
            b.HasKey(l => l.Id);
        }
    }
}
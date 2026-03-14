using VocabTrainer.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VocabTrainer.Api.DataBase.EntitiesConfigurations
{
    public class QuizItemResultConfiguration : IEntityTypeConfiguration<QuizItemResult>
    {
        public void Configure(EntityTypeBuilder<QuizItemResult> b)
        {
            b.HasKey(x => x.Id);
        }
    }
}
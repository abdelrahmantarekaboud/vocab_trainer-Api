using VocabTrainer.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VocabTrainer.Api.DataBase.EntitiesConfigurations
{
    public class QuizSessionConfiguration : IEntityTypeConfiguration<QuizSession>
    {
        public void Configure(EntityTypeBuilder<QuizSession> b)
        {
            b.HasKey(x => x.Id);
        }
    }
}
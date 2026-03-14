namespace VocabTrainer.Api.Entities
{
    public class InvitationRedemption
    {
        public Guid Id { get; set; }
        public Guid InvitationCodeId { get; set; }
        public Guid? UserId { get; set; }
        public string RedeemerEmail { get; set; } = default!;
        public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;
    }
}
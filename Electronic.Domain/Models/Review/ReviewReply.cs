namespace Electronic.Domain.Models.Review;

public class ReviewReply
{
    public long ReviewReplyId { get; set; }
    public long ReviewId { get; set; }
    public Review Review { get; set; }
    public int Rating { get; set; }
    public string ReviewerName { get; set; }
    public string ReviewerId { get; set; }
    public string Comment { get; set; }
    public bool Status { get; set; }
}
namespace SimpleTweetApi.Models.App;

public class TweetFlags : BaseModel
{
    public Guid TweetUuid { get; set; }
    public string FlagCode { get; set; }
    public string ReporterUuid { get; set; }
    public string Note { get; set; }

}

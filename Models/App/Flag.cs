using System.ComponentModel.DataAnnotations;

namespace SimpleTweetApi.Models.App;

public class Flag : BaseModel
{
    [Key]
    public string Code = "";
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Icon { get; set; } = null;
}

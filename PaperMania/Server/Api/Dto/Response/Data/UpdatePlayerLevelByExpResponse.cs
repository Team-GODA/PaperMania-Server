namespace Server.Api.Dto.Response.Data;

public class UpdatePlayerLevelByExpResponse
{
    public int? Id { get; set; }
    public int NewLevel { get; set; }
    public int NewExp { get; set; }
}
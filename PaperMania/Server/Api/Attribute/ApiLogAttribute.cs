namespace Server.Api.Attribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiLogAttribute : System.Attribute
{
    public string Domain { get; set; }
    
    public ApiLogAttribute(string domain)
    {
        Domain = domain;
    }
}
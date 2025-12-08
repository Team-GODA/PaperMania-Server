using Microsoft.AspNetCore.Mvc;
using Server.Api.Filter;

namespace Server.Api.Attribute;

public class SessionAuthorizeAttribute : TypeFilterAttribute
{
    public SessionAuthorizeAttribute() : base(typeof(SessionValidationFilter))
    {
        
    }
}
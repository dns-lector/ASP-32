using ASP_32.Data.Entities;
using System.Text.Json;

namespace ASP_32.Services.Auth
{
    public class SessionAuthService(
        IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        const String sessionKey = "ISessionAuthService";

        public T? GetAuth<T>() where T : notnull
        {
            if (_httpContextAccessor.HttpContext?
                .Session.Keys.Contains(sessionKey) ?? false)
            {
                if (JsonSerializer.Deserialize<T>(
                        _httpContextAccessor.HttpContext!
                        .Session.GetString(sessionKey)!)
                    is T payload)
                {
                    return payload;
                }
            }
            return default;
        }

        public void RemoveAuth()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(sessionKey);
        }

        public void SetAuth(object payload)
        {
            _httpContextAccessor.HttpContext?.Session.SetString(
                sessionKey,
                JsonSerializer.Serialize(payload)
            );
        }
    }
}
/*              login
 * Client --------------------> Server <-> Database
 *            Cookie:session_id        <- [session]
 *       <----------------------      
 *       
 *           Cookie:session_id
 * Client --------------------> Server <-> [session]      
 *                                   Authorize
 *                                   
 *                                   
 *                                   
 *                                   
 *              login
 * Client --------------------> Server <-> Database
 *            token                     <-- token
 *       <---------------------- 
 *       
 *               token                    
 * Client --------------------> Server <-> Database      
 *                                   Authorize      
 *                                   
 *                                   
 *                                   
 *              login
 * Client --------------------> Server <-> Database
 *            token                     <-- token + signing
 *       <---------------------- 
 *       
 *               token                    
 * Client --------------------> Server <-> check signature    
 *                                   Authorize                                        
 */

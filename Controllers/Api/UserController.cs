using ASP_32.Data;
using ASP_32.Services.Kdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_32.Controllers.Api
{
    [Route("api/user")]
    [ApiController]
    public class UserController(DataContext dataContext, IKdfService kdfService) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKdfService _kdfService = kdfService;

        [HttpGet]
        public object Authenticate()
        {
            String? header = HttpContext.Request.Headers.Authorization;
            if (header == null)      // Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Authorization Header Required" };
            }
            /* Д.З. Реалізувати повний цикл перевірок даних, що передаються
             * для автентифікації
             * - заголовок починається з 'Basic '
             * - credentials успішно декодуються з Base64
             * - userPass ділиться на дві частини (може не містити ":")
             */
            String credentials =    // 'Basic ' - length = 6
                header[6..];        // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            String userPass =       // Aladdin:open sesame
                System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(credentials));

            String[] parts = userPass.Split(':', 2);
            String login = parts[0];
            String password = parts[1];

            var userAccess = _dataContext
                .UserAccesses
                .FirstOrDefault(ua => ua.Login == login);

            if (userAccess == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Credentials rejected" };
            }
            String dk = _kdfService.Dk(password, userAccess.Salt);
            if(dk != userAccess.Dk)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Credentials rejected." };
            }
            return userAccess;
        }

        [HttpPost]
        public object SignUp()
        {
            return new { Status = "SignUp Works" };
        }

        [HttpPost("admin")]  // POST /api/user/admin
        public object SignUpAdmin()
        {
            return new { Status = "SignUpAdmin Works" };
        }
    }
}
/* Відмінності АРІ та MVC контролерів
 * MVC:
 *  адресація за назвою дії (Action) - різні дії -- різні адреси
 *  GET  /Home/Index     --> HomeController.Index()
 *  POST /Home/Index     --> HomeController.Index()
 *  GET  /Home/Privacy   --> HomeController.Privacy()
 *  повернення - IActionResult частіше за все View
 *  
 * API:
 *  адресація за анотацією [Route("api/user")], різниця
 *  у методах запиту
 *  GET  api/user  ---> [HttpGet] Authenticate()
 *  POST api/user  ---> [HttpPost] SignUp()
 *  PUT  api/user  ---> 
 *  
 *  C   POST
 *  R   GET
 *  U   PUT(replace) PATCH(partially update)
 *  D   DELETE
 */

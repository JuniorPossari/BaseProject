using BaseProject.DAO.Models;

namespace BaseProject.DAO.IService
{
	public interface IServiceToken
    {
        DateTime GetExpiryTimestamp(string accessToken);
        string GenerateToken(AspNetUser user, List<string> currentRoles);
        CookieOptions GenerateCookies();
        CookieOptions GenerateCookies(string userAgent);
    }
}

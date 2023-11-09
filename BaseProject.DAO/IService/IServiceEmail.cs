using BaseProject.DAO.Models.Others;

namespace BaseProject.DAO.IService
{
	public interface IServiceEmail
    {
        Task<bool> SendEmail(EmailOptions options);
    }
}

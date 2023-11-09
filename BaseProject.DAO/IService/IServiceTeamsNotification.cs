namespace BaseProject.DAO.IService
{
	public interface IServiceTeamsNotification
    {
        Task<bool> SendNotification(string title, string message);
    }
}
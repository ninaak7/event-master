namespace EMS.Service.Interface
{
    public interface IMailService
    {
        Task<bool> SendEmailToUser(string userId, string subject, string body);
    }
}

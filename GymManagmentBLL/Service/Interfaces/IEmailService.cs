using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailWithImageAsync(string email, string subject, string htmlBody, byte[] imageBytes, string imageContentId);
    }
}

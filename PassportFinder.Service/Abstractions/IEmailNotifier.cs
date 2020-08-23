using System.Threading.Tasks;

namespace PassportFinder.Service.Abstractions
{
    public interface IEmailNotifier
    {
        bool Send(string[] to, string subject, string message);
    }
}
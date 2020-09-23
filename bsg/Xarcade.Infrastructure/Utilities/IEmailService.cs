namespace Xarcade.Infrastructure.Utilities
{
    public interface IEmailService
    {
        bool Send(Email emailDetails);
    }
}

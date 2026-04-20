using GLMS.Interfaces;

namespace GLMS.Services
{
    public class AuditLogger : IObserver
    {
        public void Update(string message)
        {
            Console.WriteLine($"Audit Log: {message}");
        }
    }
}
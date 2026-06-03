using GLMS.Interfaces;

namespace GLMS.Services
{
    public class AuditLogger : IObserver
    {
        public void Update(string message)
        {
            //This logs a messsage when the subject sends a notification
            Console.WriteLine($"Audit Log: {message}");
        }
    }
}
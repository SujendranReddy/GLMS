using GLMS.Interfaces;

namespace GLMS.Services
{
    public class ContractSubject : ISubject
    {
        private readonly List<IObserver> _observers = new();

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        //THis notifies all observers when an event regarding contracts happens
        public void Notify(string message)
        {
            foreach (var observer in _observers)
            {
                observer.Update(message);
            }
        }
    }
}
namespace ElectronicQueue.Models;

// Патерн Observer (Інтерфейс спостерігача)
public interface IQueueObserver { void Update(Ticket ticket); }

// Клас Талона (Ticket) з вашої UML
public class Ticket {
    public int Number { get; set; }
    public string ServiceType { get; set; } = "";
    public DateTime IssueTime { get; set; } = DateTime.Now;
    public bool IsPriority { get; set; }
    public string Status { get; set; } = "В черзі";
}

// Головна система (Queue) - Singleton
public class QueueSystem {
    private List<Ticket> _tickets = new();
    private List<IQueueObserver> _observers = new();
    private int _counter = 1;

    // Видача талона
    public Ticket AddTicket(string service, bool priority) {
        var t = new Ticket { Number = _counter++, ServiceType = service, IsPriority = priority };
        _tickets.Add(t);
        return t;
    }

    // Виклик наступного (Strategy: Пільговики вперед)
    public Ticket? CallNext() {
        var t = _tickets.Where(x => x.Status == "В черзі")
                       .OrderByDescending(x => x.IsPriority)
                       .ThenBy(x => x.IssueTime).FirstOrDefault();
        if (t != null) {
            t.Status = "Викликано";
            Notify(t);
        }
        return t;
    }

    // Методи Observer
    public void Attach(IQueueObserver obs) => _observers.Add(obs);
    public void Detach(IQueueObserver obs) => _observers.Remove(obs);
    private void Notify(Ticket t) => _observers.ForEach(o => o.Update(t));
    
    public List<Ticket> GetAll() => _tickets;
}
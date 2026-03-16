namespace ElectronicQueue.Models;

// Патерн Observer
public interface IQueueObserver { void Update(Ticket ticket); }

// Клас Талона
public class Ticket {
    public int Number { get; set; }
    public string ServiceType { get; set; } = "";
    public DateTime IssueTime { get; set; } = DateTime.Now;
    public bool IsPriority { get; set; }
    public string Status { get; set; } = "В черзі";
    public string ClientName { get; set; } = "Анонім"; 
    public string Comment { get; set; } = "";
    
    // --- НОВІ ПОЛЯ ДЛЯ ЧАСУ ОЧІКУВАННЯ ---
    public int EstimatedWaitMinutes { get; set; }
    public int PeopleAhead { get; set; }
}

// Головна система (Queue) - Singleton
public class QueueSystem {
    private List<Ticket> _tickets = new();
    private List<IQueueObserver> _observers = new();
    private int _counter = 1;

    public int GetWaitingCount() => _tickets.Count(x => x.Status == "В черзі");
    public int? GetCurrentNumber() => _tickets.LastOrDefault(x => x.Status == "Викликано")?.Number;
    public List<Ticket> GetHistory() => _tickets.Where(x => x.Status == "Викликано").TakeLast(5).Reverse().ToList();

    // Видача талона (З РОЗРАХУНКОМ ЧАСУ)
    public Ticket AddTicket(string service, bool priority, string name = "", string comment = "") {
        
        // АЛГОРИТМ: Рахуємо скільки людей перед клієнтом
        // Пільговик чекає тільки інших пільговиків. Звичайний чекає всіх.
        int peopleAhead = priority 
            ? _tickets.Count(x => x.Status == "В черзі" && x.IsPriority) 
            : _tickets.Count(x => x.Status == "В черзі");
            
        // Припустимо, середній час обслуговування 1 людини = 5 хвилин
        int waitTime = peopleAhead * 5; 

        var t = new Ticket { 
            Number = _counter++, 
            ServiceType = service, 
            IsPriority = priority,
            ClientName = string.IsNullOrWhiteSpace(name) ? "Анонім" : name,
            Comment = comment,
            EstimatedWaitMinutes = waitTime, // Записуємо час
            PeopleAhead = peopleAhead        // Записуємо кількість людей
        };
        
        _tickets.Add(t);
        Notify(t); 
        return t;
    }

    public Ticket? CallNext() {
        var t = _tickets.Where(x => x.Status == "В черзі")
                        .OrderByDescending(x => x.IsPriority)
                        .ThenBy(x => x.IssueTime).FirstOrDefault();
        if (t != null) { t.Status = "Викликано"; Notify(t); }
        return t;
    }

    public Ticket? CallSpecific(int number) {
        var t = _tickets.FirstOrDefault(x => x.Number == number && x.Status == "В черзі");
        if (t != null) { t.Status = "Викликано"; Notify(t); }
        return t;
    }

    public void Attach(IQueueObserver obs) => _observers.Add(obs);
    public void Detach(IQueueObserver obs) => _observers.Remove(obs);
    private void Notify(Ticket t) => _observers.ForEach(o => o.Update(t));
    public List<Ticket> GetAll() => _tickets;
}
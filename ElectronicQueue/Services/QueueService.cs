using ElectronicQueue.Models;

namespace ElectronicQueue.Services
{
    public class QueueService
    {
        private List<Client> clients = new List<Client>();
        private int nextNumber = 1;

        public event Action? OnQueueChanged;

        // Реєстрація нового клієнта
        public Client RegisterClient(string service)
        {
            var client = new Client(nextNumber++, service);
            clients.Add(client);
            OnQueueChanged?.Invoke();
            return client;
        }

        // Отримати тих, хто чекає
        public List<Client> GetWaitingClients() => 
            clients.Where(c => c.Status == "Waiting").ToList();

        // Отримати тих, кого зараз викликають
        public List<Client> GetCallingClients() => 
            clients.Where(c => c.Status == "Calling").ToList();

        // НОВЕ: Отримати взагалі всіх (для статистики оператора)
        public List<Client> GetAllClients() => clients.ToList();

        // Викликати наступного
        public Client? CallNextClient()
        {
            var next = clients.FirstOrDefault(c => c.Status == "Waiting");
            if (next != null)
            {
                next.Status = "Calling";
                OnQueueChanged?.Invoke();
            }
            return next;
        }

        // НОВЕ: Завершити обслуговування
        public void FinishClient(int number)
        {
            var client = clients.FirstOrDefault(c => c.QueueNumber == number);
            if (client != null)
            {
                client.Status = "Finished";
                OnQueueChanged?.Invoke();
            }
        }
    }
}
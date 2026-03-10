namespace ElectronicQueue.Models
{
    public class Client
    {
        public int QueueNumber { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string Status { get; set; } = "Waiting"; // Waiting, Calling, Finished
        public DateTime CreatedAt { get; set; } = DateTime.Now; // ДОДАНО ЦЕЙ РЯДОК

        public Client(int number, string service)
        {
            QueueNumber = number;
            ServiceType = service;
            CreatedAt = DateTime.Now; // Ініціалізація часом створення
        }
    }
}
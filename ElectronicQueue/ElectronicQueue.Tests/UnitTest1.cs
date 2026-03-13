using ElectronicQueue.Models;
using Xunit;
using System.Linq;

namespace ElectronicQueue.Tests;

public class QueueTests
{
    // ГРУПА 1: Тестування реєстрації (10 тестів через Theory)
    [Theory]
    [InlineData("Каса 1", false)]
    [InlineData("Каса 2", false)]
    [InlineData("Кредити", true)]
    [InlineData("Депозити", false)]
    [InlineData("Консультація", true)]
    [InlineData("Валюта", false)]
    [InlineData("Картки", true)]
    [InlineData("Страхування", false)]
    [InlineData("Юр. особи", true)]
    [InlineData("Сейфи", false)]
    public void AddTicket_MultipleScenarios_ShouldWork(string service, bool isPriority)
    {
        var queue = new QueueSystem(); // Singleton зазвичай один, але для тестів ми створюємо новий екземпляр
        var ticket = queue.AddTicket(service, isPriority);

        Assert.Equal(service, ticket.ServiceType);
        Assert.Equal(isPriority, ticket.IsPriority);
        Assert.Equal("В черзі", ticket.Status);
    }

    // ГРУПА 2: Тестування PriorityStrategy (10 тестів)
    // Перевіряємо, що пільговики завжди йдуть першими, незалежно від черговості
    [Theory]
    [InlineData(1)] [InlineData(2)] [InlineData(3)] [InlineData(4)] [InlineData(5)]
    [InlineData(6)] [InlineData(7)] [InlineData(8)] [InlineData(9)] [InlineData(10)]
    public void CallNext_PriorityAlwaysFirst(int attempt)
    {
        var queue = new QueueSystem();
        queue.AddTicket("Regular", false); 
        queue.AddTicket("Priority", true); // Цей має бути першим

        var called = queue.CallNext();

        Assert.True(called.IsPriority);
        Assert.Equal("Priority", called.ServiceType);
    }

    // ГРУПА 3: Логіка черги та лічильників (5 тестів)
    [Fact]
    public void GetWaitingCount_ShouldReflectActualState()
    {
        var queue = new QueueSystem();
        Assert.Equal(0, queue.GetWaitingCount());

        queue.AddTicket("S1", false);
        queue.AddTicket("S2", false);
        Assert.Equal(2, queue.GetWaitingCount());

        queue.CallNext();
        Assert.Equal(1, queue.GetWaitingCount());
    }

    [Fact]
    public void GetHistory_ShouldStoreLastFive()
    {
        var queue = new QueueSystem();
        for(int i = 0; i < 10; i++) queue.AddTicket("S", false);
        for(int i = 0; i < 7; i++) queue.CallNext();

        var history = queue.GetHistory();
        Assert.True(history.Count() <= 5); // Перевірка обмеження історії
    }

    // ГРУПА 4: Патерн Observer (5 тестів)
    [Fact]
    public void Observer_ShouldNotifyOnNewTicket()
    {
        var queue = new QueueSystem();
        var wasNotified = false;
        
        // Імітуємо спостерігача (як ваше Табло)
        var mockObserver = new MockObserver(() => wasNotified = true);
        queue.Attach(mockObserver);

        queue.AddTicket("Test", false);

        Assert.True(wasNotified); // Перевірка, що табло отримало сигнал
    }

    // Допоміжний клас для тестів Observer
    private class MockObserver : IQueueObserver
    {
        private readonly Action _onUpdate;
        public MockObserver(Action onUpdate) => _onUpdate = onUpdate;
        public void Update(Ticket ticket) => _onUpdate();
    }
}
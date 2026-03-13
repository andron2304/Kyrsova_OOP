```mermaid
classDiagram

%% 1. PATTERN: STRATEGY (Пріоритети обслуговування)
class IQueueStrategy {
    <<interface>>
    + getNextTicket(List~Ticket~ tickets) Ticket
}
class FIFOServiceStrategy {
    + getNextTicket(List~Ticket~ tickets) Ticket
}
class PriorityServiceStrategy {
    + getNextTicket(List~Ticket~ tickets) Ticket
}
IQueueStrategy <|.. FIFOServiceStrategy
IQueueStrategy <|.. PriorityServiceStrategy

%% 2. PATTERN: STATE (Життєвий цикл талона і статистика)
class ITicketState {
    <<interface>>
    + handleState(Ticket ticket)
    + getStatusName() String
}
class WaitingState {
    + handleState(Ticket ticket)
    + getStatusName() String
}
class CalledState {
    + handleState(Ticket ticket)
    + getStatusName() String
}
class CompletedState {
    + handleState(Ticket ticket)
    + getStatusName() String
}
ITicketState <|.. WaitingState
ITicketState <|.. CalledState
ITicketState <|.. CompletedState

%% Основна сутність: Талон
class Ticket {
    - int number
    - DateTime issueTime
    - DateTime callTime
    - ITicketState state
    - boolean isPriority
    + Ticket(int number, boolean isPriority)
    + void setState(ITicketState state)
    + int calculateWaitTime()
}
Ticket *-- ITicketState : has state

%% 3. PATTERN: OBSERVER (Табло)
class IQueueObserver {
    <<interface>>
    + updateQueueBoard(Ticket currentTicket)
}
class IQueueObservable {
    <<interface>>
    + attach(IQueueObserver observer)
    + detach(IQueueObserver observer)
    + notifyObservers()
}

class Queue {
    - List~Ticket~ tickets
    - IQueueStrategy strategy
    - List~IQueueObserver~ observers
    + void setStrategy(IQueueStrategy strategy)
    + void addTicket(Ticket ticket)
    + Ticket callNext()
}
IQueueObservable <|.. Queue
Queue o-- Ticket : contains
Queue o-- IQueueStrategy : uses
Queue o-- IQueueObserver : notifies

class QueueBoardDisplay {
    + updateQueueBoard(Ticket currentTicket)
    + playNotificationSound()
}
IQueueObserver <|.. QueueBoardDisplay

%% 4. PATTERN: SINGLETON (Єдина система логів / Історія)
class HistoryLogger {
    - static HistoryLogger instance
    - List~String~ logHistory
    - HistoryLogger()
    + static HistoryLogger getInstance()
    + void logEvent(String message)
    + String generateWaitTimeStatistics()
}
Queue ..> HistoryLogger : logs events
Ticket ..> HistoryLogger : logs state changes

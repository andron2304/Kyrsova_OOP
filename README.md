```mermaid
classDiagram

%% Модель
class Client {
    - int queueNumber
    - DateTime registrationTime
    - String serviceType
    - String status
    + Client(String serviceType)
    + String getInfo()
    + String getStatus()
    + void setStatus(String status)
}

class Operator {
    - int id
    - String name
    - boolean isBusy
    + Client callNext(Queue queue)
    + void startService(Client client)
    + void finishService(Client client)
    + boolean getStatus()
}

class Queue {
    - List clients
    + void addClient(Client client)
    + Client getNext()
    + void removeClient(Client client)
    + int getCount()
    + boolean isEmpty()
}

class Ticket {
    - static Ticket instance
    - int lastNumber
    + static Ticket getInstance()
    + int generateNumber()
    + void reset()
}

class Administrator {
    - int id
    - String name
    + void addOperator(Operator operator)
    + void removeOperator(Operator operator)
    + String viewStatistics()
    + void changeWorkMode(String mode)
}

%% UI / View
class ClientView {
    + displayClientInfo(Client client)
    + displayQueue(Queue queue)
    + pressGetTicket()
}

class OperatorView {
    + displayOperatorStatus(Operator operator)
    + displayCurrentClient(Client client)
    + pressCallNext()
    + pressFinishService()
}

class AdminDashboard {
    + showStatistics()
    + manageOperators()
    + viewQueueStatus()
    + pressAddOperator()
    + pressRemoveOperator()
}

class LoginForm {
    + enterUsername()
    + enterPassword()
    + pressLogin()
}

class TicketScreen {
    + showTicketNumber(int number)
    + printTicket()
}

class QueueScreen {
    + showQueueList()
    + highlightCurrentClient()
}

%% Controller
class QueueController {
    + addClient(Client client)
    + callNextClient()
    + updateClientStatus(Client client, String status)
}

%% Observer
class ObservableQueue {
    - List observers
    + attach(observer)
    + detach(observer)
    + notifyObservers()
}

class QueueObserver {
    + update(Queue queue)
}

%% Factory
class ClientFactory {
    + Client createClient(String serviceType)
}

%% Відносини
Queue "1" o-- "*" Client : contains
Operator --> Queue : works with
Client --> Ticket : receives
Administrator --> Operator : manages
QueueController --> ObservableQueue : observes
ClientView ..|> QueueObserver
OperatorView ..|> QueueObserver
ObservableQueue --> Queue : manages
AdminDashboard --> Administrator : interacts
ClientFactory --> Client : creates

%% UI зв'язки
ClientView --> QueueController : interacts
OperatorView --> QueueController : interacts
AdminDashboard --> QueueController : interacts
LoginForm --> ClientView : opens
TicketScreen --> ClientView : shows ticket
QueueScreen --> ClientView : shows queue
QueueScreen --> OperatorView : shows queue

# Linked-List


## Сериализация и десериализация двусвязного списка.

Реализуется функции сериализации и десериализации двусвязного списка следующим образом:

    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; // произвольный элемент внутри списка
        public string Data;
    }


    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
        }

        public void Deserialize(FileStream s)
        {
        }
    }
    
## Использование

Использование выглядит следующим образом:
    
Объявление

    ListRand nodesSpace = CreateListRand(listNodes);


Сериализация

    using (FileStream fs = new FileStream("nodes.json", FileMode.OpenOrCreate))
    {
        nodesSpace.Serialize(fs);
    }


Десериализация

    using (FileStream fs = new FileStream("nodes.json", FileMode.Open))
    {
        nodesSpace.Deserialize(fs);
    }
    


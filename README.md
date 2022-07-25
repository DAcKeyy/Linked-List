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
    
## Результат в json

Пробрасываем данные в класс

    List<ListNode> listNodes = new List<ListNode>()
    {
         new ListNode { Data = "I'm Head!" },
         new ListNode { Data = "Today was a good day :)" },
         new ListNode { Data = "BlaBla" },
         new ListNode { Data = "Who eat all donats?" },
         new ListNode { Data = "Hadouken!" },
         new ListNode { Data = "And I'm somewhere in the middle, maybe..." },
         new ListNode { Data = "Sssoo, where I'm?" },
         new ListNode { Data = "Well... I'm Tail" }
    };

    //Создаём узлы
    ListRand nodesSpace = CreateListRand(listNodes);

    //Парсим в json
    using (FileStream fs = new FileStream("nodes.json", FileMode.OpenOrCreate))
    {
        nodesSpace.Serialize(fs);
    }

Result

    Name                                             Is Rand attached       Rand data
    I'm Head!                                                    True       I'm Head!
    Today was a good day :)                                     False
    BlaBla                                                       True       Sssoo, where I'm?
    Who eat all donats?                                         False
    Hadouken!                                                   False
    And I'm somewhere in the middle, maybe...                    True       BlaBla
    Sssoo, where I'm?                                            True       Hadouken!
    Well... I'm Tail                                            False

nodes.json

    [
        {
           "Data":"I'm Head!",
           "Prev":null,
           "Next":1,
           "Rand":0
        },
        {
           "Data":"Today was a good day :)",
           "Prev":0,
           "Next":2,
           "Rand":null
        },
        {
           "Data":"BlaBla",
           "Prev":1,
           "Next":3,
           "Rand":6
        },
        {
           "Data":"Who eat all donats?",
           "Prev":2,
           "Next":4,
           "Rand":null
        },
        {
           "Data":"Hadouken!",
           "Prev":3,
           "Next":5,
           "Rand":null
        },
        {
           "Data":"And I'm somewhere in the middle, maybe...",
           "Prev":4,
           "Next":6,
           "Rand":2
        },
        {
           "Data":"Sssoo, where I'm?",
           "Prev":5,
           "Next":7,
           "Rand":4
        },
        {
           "Data":"Well... I'm Tail",
           "Prev":6,
           "Next":null,
           "Rand":null
        }
    ]

## Алгоритмическая сложность преобразования

Преобразование данных из ListRand в .json, и наоборот, происходит линейно от начала и до хвоста двхусвязанного списка. 
Алгоритмическая сложность преобразования : O(n).
       
Функции сериализации выглядит следующим образом:
    
    public static void Serialize(this ListRand listRand, FileStream fileStream)
    {
        //Выворачиваем графы в список
        Dictionary<ListNode, int> nodeDict = GetNodesFromListRand(listRand);

        //превращаем список в json строку
        string listJson = GetJsonStringList(listRand, nodeDict);

        //подготоавливаем строку к IO
        byte[] bytes = new UTF8Encoding(true).GetBytes(listJson);

        //Пишем в fпоток байты
        fileStream.Write(bytes, 0, bytes.Length);
    }

Проеобразование в список узлов:

    static Dictionary<ListNode, int> GetNodesFromListRand(ListRand list)
    {
        Dictionary<ListNode, int> nodeDict = new Dictionary<ListNode, int>();
        ListNode currentNode = list.Head;
        int index = 0;

        while (currentNode != null && nodeDict.ContainsKey(currentNode) == false)
        {
            nodeDict.Add(currentNode, index++);

            currentNode = currentNode.Next;
            //и так пока .Next != null                   
        }

        return nodeDict;
    }

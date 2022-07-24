using DoublyLinkedList;

namespace LinkedListSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Клиентская Логика...

            //Создаём узлы
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




            //Клиентская Логика...




            //Парсим json в объект
            using (FileStream fs = new FileStream("nodes.json", FileMode.Open))
            {
                nodesSpace.Deserialize(fs);
            }

            //Выводим результат
            Console.WriteLine("{0,-45}    {1,16}       {2,-50}", "Name", "Is Rand attached", "Rand data");
            ListNode node = nodesSpace.Head;
            while (node != null)
            {
                if (node.Rand != null)
                    Console.WriteLine("{0,-45}    {1,16}       {2,-50}", node.Data, node.Rand != null, node.Rand.Data);
                else Console.WriteLine("{0,-45}    {1,16} ", node.Data, node.Rand != null);

                node = node.Next;
            }



            //Клиентская Логика...
        }


        private static ListRand CreateListRand(List<ListNode> listNodes)
        {
            //Пробрасываем следующий\предыдущий по порядку
            for (int i = 0; i < listNodes.Count; i++)
            {
                if (i + 1 < listNodes.Count) listNodes[i].Next = listNodes[i + 1];
                if (i != 0) listNodes[i].Prev = listNodes[i - 1];
            }

            //Создаём пространство узлов. Присваиваем пространству начало и конец
            ListRand nodeSpace = new ListRand();
            nodeSpace.Head = listNodes[0];
            nodeSpace.Tail = listNodes[listNodes.Count - 1];

            //Пробрасываем случайному узлу случайный узел
            var randomizer = new Random();
            for (var i = 0; i < randomizer.Next(1, listNodes.Count); i++)
            {
                var randomNode = listNodes[randomizer.Next(0, listNodes.Count-1)];
                randomNode.Rand = listNodes[randomizer.Next(0, listNodes.Count - 1)];
            }

            return nodeSpace;
        }
    }
}
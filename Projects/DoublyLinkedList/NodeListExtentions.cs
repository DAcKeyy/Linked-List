using System.Text;

namespace DoublyLinkedList
{
    public static class NodeListExtentions
    {
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


            static string GetJsonStringList(ListRand nodeMap, Dictionary<ListNode, int> nodeDict)
            {
                StringBuilder stringBuilder = new StringBuilder();
                ListNode currentNode = nodeMap.Head;

                //Начинается массив с [
                stringBuilder.Append("[");               
               
                foreach (var dictPare in nodeDict)
                {
                    //Пропиши в sb узел
                    WriteNode(dictPare.Key, stringBuilder, nodeDict);

                    //по правилам json след елемент в списке разделён знаком ,
                    stringBuilder.Append(",");
                }

                //Заканчивается массив ]
                stringBuilder.Append("]");

                return stringBuilder.ToString();
            }

            static void WriteNode(ListNode node, StringBuilder sb, Dictionary<ListNode, int> nodeDict)
            {
                //Прописываем ноду по правилам json
                sb.Append("{");
                sb.Append($"\"Data\": \"{node.Data}\",");
                sb.Append($"\"Prev\": {(node.Prev == null ? "null" : nodeDict[node.Prev].ToString())},");
                sb.Append($"\"Next\": {(node.Next == null ? "null" : nodeDict[node.Next].ToString())},");
                sb.Append($"\"Rand\": {(node.Rand == null ? "null" : nodeDict[node.Rand].ToString())}");
                sb.Append("}");
            }

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
        }

        public static void Deserialize(this ListRand listRand, FileStream stream)
        {
            //чек на fs
            IsStreamRunning(stream);

            //список пустышок для воссоздания ListRand
            List<NodeInfo> nodeInfos = new List<NodeInfo>();
            
            bool readNode = false;

            while (true)
            {
                int b = stream.ReadByte();

                //если натыкемся на { то это нода
                if (b == '{' && readNode == false)
                {
                    readNode = true;
                    //Парсим строки c fs в NodeInfo
                    nodeInfos.Add(DeserializeNode(stream));
                }
                //наткнулись на разделитель между нодами в json массиве
                else if (b == ',' && readNode)
                {
                    readNode = false;
                }
                else if (b == -1)
                {
                    throw new Exception("Unexpected end of stream");
                }
                //конец массива
                else if (b == ']')
                {
                    break;
                }
                else
                {
                    throw new Exception($"Unexpected char: {(char)b}");
                }
                //TODO Как вычленить нужный нам масиив из кучи других файлов json?
            }

            FillList(listRand, nodeInfos);

            static void IsStreamRunning(FileStream fs)
            {
                int firstByte = fs.ReadByte();

                if (firstByte == -1 || firstByte != '[')
                {
                    throw new Exception("Stream is empty or not starts with open bracket");
                }
                //TODO Как сделать устоёчивее?
            }

            static void FillList(ListRand list, List<NodeInfo> nodeInfos)
            {
                //если в файле ничего не было
                if (nodeInfos.Count <= 0) return;

                ConnectNodes(nodeInfos);
                list.Head = nodeInfos.First().Node;
                list.Tail = nodeInfos.Last().Node;
                list.Count = nodeInfos.Count;
            }

            static void ConnectNodes(List<NodeInfo> nodeInfos)
            {
                foreach (NodeInfo nodeInfo in nodeInfos)
                {
                    //Сверяем цифирки в NodeInfo и превращаем линию в граф
                    if (nodeInfo.Next != "null")
                        nodeInfo.Node.Next = nodeInfos[int.Parse(nodeInfo.Next)].Node;
                    if (nodeInfo.Prev != "null")
                        nodeInfo.Node.Prev = nodeInfos[int.Parse(nodeInfo.Prev)].Node;
                    if (nodeInfo.Rand != "null")
                        nodeInfo.Node.Rand = nodeInfos[int.Parse(nodeInfo.Rand)].Node;
                    if (nodeInfos.Count > 1 && nodeInfo.Node.Next == null && nodeInfo.Node.Prev == null)
                    {
                        throw new Exception("Found unconnected node");
                    }
                }
            }

            static NodeInfo DeserializeNode(FileStream fs)
            {
                //Создаю NodeInfo как обёртка для будущего создания графа
                ListNode node = new ListNode();
                Dictionary<string, string> fields = new Dictionary<string, string>();
                ReadFields(',', '}', fs, fields);
                return new NodeInfo(node, fields["Data"], fields["Prev"], fields["Next"], fields["Rand"]);
            }

            static void ReadFields(char endVarChar,char endBodyChar, FileStream fs, Dictionary<string, string> fields)
            {
                //прочитать кусочек про тело ноды
                string line = ReadUntil(fs, endBodyChar);
                //расплилть на переменные
                string[] varLines = line.Split(',');
                //для каждой строки переменной вычиняем имя перменной и её значение
                foreach (string var in varLines)
                {
                    string[] parts = var.Split(new char[] { ':', ' ' }, 2);
                    for (int i = 0; i < parts.Count(); i++)
                    {
                        string clearedPare = parts[i].Replace("\"", null);
                        if (clearedPare.Contains("null")) clearedPare = "null";
                        parts[i] = clearedPare;
                    }
                    //добовляем в список полученые таким варварским способом значения
                    fields.Add(parts[0], parts[1]);
                }                
            }

            //читай fs пока не наткнёшся на стоп-символ и верни то что прочитал
            static string ReadUntil(FileStream fs, char stopChar)
            {
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    int b = fs.ReadByte();
                    if (b == -1)
                    {
                        return sb.ToString();
                    }
                    if ((char)b == stopChar)
                    {
                        break;
                    }
                    else
                    {
                        sb.Append((char)b);
                    }
                }

                return sb.ToString();
            }
        }

        public struct NodeInfo
        {
            public readonly ListNode Node;
            public readonly string Prev;
            public readonly string Next;
            public readonly string Rand;

            public NodeInfo(ListNode node, string data, string prev, string next, string rand)
            {
                Prev = prev;
                node.Data = data;
                Next = next;
                Rand = rand;
                Node = node;
            }
        }
    }
}

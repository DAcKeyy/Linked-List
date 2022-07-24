namespace DoublyLinkedList
{
    public struct ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream stream)
        {
            NodeListExtentions.Serialize(this, stream);
        }

        public void Deserialize(FileStream stream)
        {
            NodeListExtentions.Deserialize(this, stream);
        }
    }
}
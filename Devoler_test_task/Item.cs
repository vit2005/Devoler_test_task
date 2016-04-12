namespace Devoler_test_task
{
    class Item
    {
        string title;
        byte type;

        public Robot Owner { get; set; }
        public string Title { get { return title; } }

        public Item(byte type, string title = null)
        {
            this.type = type;
            this.title = title;
        }
    }
}

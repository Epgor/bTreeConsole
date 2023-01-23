namespace bTreeWinForm
{
    public class BTreeNode
    {
        public int[] Keys; // tablica kluczy
        public BTreeNode[] Children; // tablica wskaźników do potomków
        public int KeyCount; // liczba kluczy w węźle
        public bool IsLeaf; // czy węzeł jest liściem

        public BTreeNode(int t)
        {
            Keys = new int[2 * t - 1]; // maksymalna liczba kluczy w węźle to 2*t-1
            Children = new BTreeNode[2 * t]; // maksymalna liczba potomków to 2*t
            KeyCount = 0;
            IsLeaf = true;
        }
    }
}

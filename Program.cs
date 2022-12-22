

static void Main(string[] args)
{
    string IsExisting(bool value) => value ? "exists" : "don't exist";
    Console.WriteLine("Generating Tree");
    var level = 2;
    var tree = new BTree(level);
    var value = 2137;

    Console.WriteLine("Inserting Values");
    tree.Insert(1);
    tree.Insert(2);
    tree.Insert(value);
    tree.Insert(4);

    Console.WriteLine($"Checking if value: {value} exists");
    var exists = tree.Search(value);
    Console.WriteLine($"Value {IsExisting(exists)}");

    Console.WriteLine($"Deleting {value}");
    tree.Delete(value);

    Console.WriteLine($"Checking if value: {value} exists");
    exists = tree.Search(value);
    Console.WriteLine($"Value {IsExisting(exists)}");
}
Main(args);
public static class Executor
{
    public static BTree tree;
    static Executor() 
    {
        tree = new BTree(1);
    }

    public static void Add()
    {
        tree.Insert(1);
        Console.WriteLine("Inserted 1!");
    }

    public static void Delete()
    {
        tree.Delete(1);
        Console.WriteLine("Deleted 1!");
    }

}

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
public class BTree
{
    private BTreeNode root; // korzeń drzewa
    private int t; // stopień drzewa

    public BTree(int t)
    {
        this.t = t;
        root = null;
    }

    // Wstawianie klucza do drzewa
    public void Insert(int key)
    {
        if (root == null) // jeśli drzewo jest puste, tworzymy nowy węzeł
        {
            root = new BTreeNode(t);
            root.Keys[0] = key;
            root.KeyCount = 1;
        }
        else // w przeciwnym razie wstawiamy klucz do istniejącego drzewa
        {
            // jeśli korzeń jest pełny, dzielimy go i tworzymy nowy korzeń
            if (root.KeyCount == 2 * t - 1)
            {
                BTreeNode newRoot = new BTreeNode(t);
                newRoot.Children[0] = root;
                newRoot.IsLeaf = false;
                SplitChild(newRoot, 0, root);
                InsertNonFull(newRoot, key);
                root = newRoot;
            }
            else
            {
                InsertNonFull(root, key);
            }
        }
    }
    private void InsertNonFull(BTreeNode node, int key)
    {
        int i = node.KeyCount - 1;
        // jeśli węzeł jest liściem, wstawiamy klucz na odpowiednie miejsce
        if (node.IsLeaf)
        {
            while (i >= 0 && key < node.Keys[i])
            {
                node.Keys[i + 1] = node.Keys[i];
                i--;
            }

            node.Keys[i + 1] = key;
            node.KeyCount++;
        }
        else // w przeciwnym razie szukamy odpowiedniego potomka, do którego można wstawić klucz
        {
            while (i >= 0 && key < node.Keys[i])
            {
                i--;
            }

            i++;

            // jeśli wybrany potomek jest pełny, dzielimy go
            if (node.Children[i].KeyCount == 2 * t - 1)
            {
                SplitChild(node, i, node.Children[i]);

                if (key > node.Keys[i])
                {
                    i++;
                }
            }

            InsertNonFull(node.Children[i], key);
        }
    }
    // Funkcja pomocnicza do dzielenia pełnego węzła
    private void SplitChild(BTreeNode parent, int i, BTreeNode child)
    {
        BTreeNode newNode = new BTreeNode(t);

        // Przenosimy tyle kluczy z child, ile jest potrzebne do nowego węzła
        newNode.KeyCount = t - 1;
        for (int j = 0; j < t - 1; j++)
        {
            newNode.Keys[j] = child.Keys[j + t];
        }

        // Jeśli child jest wezłem nie-liściem, przenosimy również odpowiednie potomki
        if (!child.IsLeaf)
        {
            newNode.IsLeaf = false;
            for (int j = 0; j < t; j++)
            {
                newNode.Children[j] = child.Children[j + t];
            }
        }

        // Aktualizujemy liczbę kluczy w child
        child.KeyCount = t - 1;

        // Przenosimy klucz z parent do nowego węzła i umieszczamy go pomiędzy child i newNode
        for (int j = parent.KeyCount; j > i; j--)
        {
            parent.Children[j + 1] = parent.Children[j];
        }
        parent.Children[i + 1] = newNode;

        for (int j = parent.KeyCount - 1; j >= i; j--)
        {
            parent.Keys[j + 1] = parent.Keys[j];
        }
        parent.Keys[i] = child.Keys[t - 1];
        parent.KeyCount++;
    }
    // Wyszukiwanie klucza w drzewie
    public bool Search(int key)
    {
        return Search(root, key);
    }

    private bool Search(BTreeNode node, int key)
    {
        int i = 0;
        while (i < node.KeyCount && key > node.Keys[i])
        {
            i++;
        }

        if (i < node.KeyCount && key == node.Keys[i])
        {
            return true; // znaleziono klucz
        }
        else if (node.IsLeaf)
        {
            return false; // nie znaleziono klucza, ponieważ osiągnięto liść
        }
        else
        {
            return Search(node.Children[i], key); // kontynuujemy wyszukiwanie w odpowiednim potomku
        }
    }

    // Usuwanie klucza z drzewa
    public void Delete(int key)
    {
        if (root == null) // drzewo jest puste
        {
            return;
        }

        Delete(root, key);

        // Jeśli korzeń ma tylko jeden potomek i nie jest liściem, zastępujemy go swoim potomkiem
        if (root.KeyCount == 0 && !root.IsLeaf)
        {
            root = root.Children[0];
        }
    }

    private void Delete(BTreeNode node, int key)
    {
        int i = 0;
        while (i < node.KeyCount && key > node.Keys[i])
        {
            i++;
        }

        if (i < node.KeyCount && key == node.Keys[i]) // znaleziono klucz do usunięcia
        {
            if (node.IsLeaf) // jeśli jesteśmy w liściu, po prostu usuwamy klucz
            {
                for (int j = i + 1; j < node.KeyCount; j++)
                {
                    node.Keys[j - 1] = node.Keys[j];
                }
                node.KeyCount--;
            }
            else // w przeciwnym razie szukamy klucza zastępczego
            {
                int predecessorKey = FindPredecessorKey(node, i);
                node.Keys[i] = predecessorKey;
                Delete(node.Children[i], predecessorKey);
            }
        }
        else if (node.IsLeaf) // nie znaleziono klucza, ponieważ osiągnięto liść
        {
            return;
        }
        else // kontynuujemy wyszukiwanie klucza w odpowiednim potomku
        {
            BTreeNode child = node.Children[i];
            if (child.KeyCount == t - 1) // jeśli potomek ma zbyt mało kluczy, należy go uzupełnić
            {
                // jeśli sąsiadujący potomek ma wystarczającą liczbę kluczy, przenosimy jeden z nich do child
                if (i > 0 && node.Children[i - 1].KeyCount >= t)
                {
                    RotateRight(node, i - 1, child);
                }
                else if (i < node.KeyCount && node.Children[i + 1].KeyCount >= t)
                {
                    RotateLeft(node, i, child);
                }
                else // w przeciwnym razie łączymy child z jednym ze swoich sąsiadów
                {
                    if (i < node.KeyCount)
                    {
                        Merge(child, node.Children[i + 1]);
                    }
                    else
                    {
                        Merge(node.Children[i - 1], child);
                    }
                }
            }

            Delete(child, key);
        }
    }

    // Funkcja pomocnicza do znajdowania klucza zastępczego dla usuwanego klucza
    private int FindPredecessorKey(BTreeNode node, int i)
    {
        BTreeNode current = node.Children[i];
        while (!current.IsLeaf)
        {
            current = current.Children[current.KeyCount];
        }
        return current.Keys[current.KeyCount - 1];
    }

    // Funkcja pomocnicza do przenoszenia klucza z lewego sąsiada do potomka
    private void RotateRight(BTreeNode node, int i, BTreeNode child)
    {
        child.Keys[child.KeyCount] = node.Keys[i];
        child.KeyCount++;

        node.Keys[i] = node.Children[i].Keys[node.Children[i].KeyCount - 1];

        if (!node.Children[i].IsLeaf)
        {
            child.Children[child.KeyCount] = node.Children[i].Children[node.Children[i].KeyCount];
        }

        node.Children[i].KeyCount--;
    }

    // Funkcja pomocnicza do przenoszenia klucza z prawego sąsiada do potomka
    private void RotateLeft(BTreeNode node, int i, BTreeNode child)
    {
        for (int j = child.KeyCount; j > 0; j--)
        {
            child.Keys[j] = child.Keys[j - 1];
        }
        child.Keys[0] = node.Keys[i];
        child.KeyCount++;
        if (!child.IsLeaf)
        {
            for (int j = child.KeyCount; j > 0; j--)
            {
                child.Children[j] = child.Children[j - 1];
            }
            child.Children[0] = node.Children[i + 1].Children[0];
        }

        node.Keys[i] = node.Children[i + 1].Keys[0];

        for (int j = 1; j < node.Children[i + 1].KeyCount; j++)
        {
            node.Children[i + 1].Keys[j - 1] = node.Children[i + 1].Keys[j];
        }

        if (!node.Children[i + 1].IsLeaf)
        {
            for (int j = 1; j <= node.Children[i + 1].KeyCount; j++)
            {
                node.Children[i + 1].Children[j - 1] = node.Children[i + 1].Children[j];
            }
        }

        node.Children[i + 1].KeyCount--;
    }

    // Funkcja pomocnicza do łączenia dwóch potomków
    private void Merge(BTreeNode left, BTreeNode right)
    {
        left.Keys[t - 1] = right.Keys[0];
        left.KeyCount++;

        for (int i = 0; i < t - 1; i++)
        {
            left.Keys[t + i] = right.Keys[i + 1];
        }

        if (!left.IsLeaf)
        {
            for (int i = 0; i < t; i++)
            {
                left.Children[t + i] = right.Children[i];
            }
        }

        left.KeyCount += t - 1;
    }
}

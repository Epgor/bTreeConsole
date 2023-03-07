using System;
using System.Collections.Generic;
using System.Linq;

namespace bTreeWinForm
{
    public class BTree
    {
        private BTreeNode root; // korzeń drzewaaa
        private int t; // stopień drzewa
        private int currentColumn = 0;
        private int maxRow = 0; //max kolumna i wiersz, do macierzy
        private static int zeroValue = 0;
        private BTreeNode lastFoundNode;
        public BTree(int t)
        {
            this.t = t;
            root = null;
        }

        public int GetColSize() { return currentColumn; }
        public int GetRowSize() { return maxRow; }
        public BTreeNode GetLastFoundNode() { return lastFoundNode; }

        public string GetLastFoundNodeString()
        {
            var @return = "";
            var node = GetLastFoundNode();
            var keys = "";
            var childCount = 0;

            foreach(var key in node.Keys)
            {
                keys += $"{key}, ";
            }
            childCount = node.Children.Where(r => r != null).Count();

            @return = $"Znaleziony węzeł:\n" +
                $"Ilość kluczy: {node.KeyCount}\n" +
                $"Klucze: [{keys}]\n" +
                $"Ilość potomków: {childCount}";

            return @return;
        }

        public void ListNodes(BTreeNode node,
                              int row,
                              List<BTreeNodeWithPosition> nodesWithPosition)
        {
            if (node == null)
                return;


            var values = new List<int>();

            for (int i = 0; i < node.KeyCount; i++)
            {
                Console.Write($"{node.Keys[i]} [C:{currentColumn}][R:{row}]\n");
                values.Add(node.Keys[i]);
            }

            nodesWithPosition.Add(new BTreeNodeWithPosition
            {
                positionX = currentColumn,
                positionY = row,
                nodeValues = values
            });
            row++;
            if (maxRow < row)
                maxRow = row;

            if (!node.IsLeaf)
            {
                for (int i = 0; i <= node.KeyCount; i++)
                {
                    ListNodes(node.Children[i], row, nodesWithPosition);
                }
            }
            currentColumn++;

        }
        public string[,] PrintTree(bool console)
        {
            var row = 0;
            var nodesWithPositions = new List<BTreeNodeWithPosition>();
            currentColumn = 0;
            ListNodes(root, row, nodesWithPositions);
            return NodesToMatrix(nodesWithPositions, console);
        }

        private string[,] NodesToMatrix(List<BTreeNodeWithPosition> nodesWithPositions, bool console = false)
        {
            string[,] matrix = new string[currentColumn, maxRow];

            foreach (var node in nodesWithPositions)
            {
                var value = "";
                foreach (var number in node.nodeValues)
                    value += $"{number} ";

                matrix[node.positionX, node.positionY] = value;
            }

            if (!console)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        Console.Write(matrix[i, j] + "\t");
                    }
                    Console.WriteLine();
                }
                
            }
            return matrix;

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
                //Console.WriteLine(node.Keys[i]);
                i++;
            }

            if (i < node.KeyCount && key == node.Keys[i])
            {
                lastFoundNode = node;
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

}

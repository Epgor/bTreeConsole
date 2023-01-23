using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace bTreeWinForm
{
    public partial class Form1 : Form
    {
        private BTree mainTree;
        private int formLength = 128;
        public Form1()
        {
            InitializeComponent();
            clearScreen();
        }

        private void clearScreen()
        {
            string[] tempArray = new string[formLength];

            for (int counter = 0; counter < formLength; counter++)
            {
                tempArray[counter] = String.Empty;
            }
            textBox1.Clear();
            textBox1.Lines = tempArray;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clearScreen();
            if (mainTree != null)
            {
                string[] tempArray = new string[formLength];
                var matrix = mainTree.PrintTree(false);
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        tempArray[i] += matrix[i, j] + "\t";
                    }
                }
                clearScreen();
                textBox1.Lines = tempArray;
            }
            else
                textBox2.Text = "[E] 005 - Nie wygenerowano drzewa!!!";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mainTree != null)
            {
                for (int i = 1; i <= (Int32)numericUpDown2.Value; i++)
                {
                    mainTree.Insert(i);
                }
                textBox2.Text = $"Zasilono drzewo - dodano {(Int32)numericUpDown2.Value} wartosci";
            }
            else
                textBox2.Text = "[E] 001 - Nie wygenerowano drzewa!!!";
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (mainTree != null)
            {
                var value = (Int32)numericUpDown3.Value;
                mainTree.Insert(value);
                textBox2.Text = $"{value} - Dodane do drzewa";
            }
            else
                textBox2.Text = "[E] 002 - Nie wygenerowano drzewa!!!";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var level = (Int32)numericUpDown1.Value;
            textBox2.Text = $"Generuję drzewo - {level} stopnia";
            mainTree = new BTree(level);
            textBox2.Text = $"Wygenerowano drzewo - {level} stopnia";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (mainTree != null)
            {
                var value = (Int32)numericUpDown3.Value;
                mainTree.Delete(value);
                textBox2.Text = $"{value} - Usunięte z drzewa";
            }
            else
                textBox2.Text = "[E] 003 - Nie wygenerowano drzewa!!!";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (mainTree != null)
            {
                var value = (Int32)numericUpDown3.Value;
                var result = mainTree.Search(value);
                if (result)
                {
                    textBox2.Text = $"{value} - Znalezione w drzewie";
                    label5.Text = mainTree.GetLastFoundNodeString();
                }          
                else
                    textBox2.Text = $"{value} - Nie znalezione w drzewa";
            }
            else
                textBox2.Text = "[E] 004 - Nie wygenerowano drzewa!!!";
        }

        private void label5_Click(object sender, EventArgs e)
        {
            label5.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

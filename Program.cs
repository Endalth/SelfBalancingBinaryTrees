using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Red Black Trees -> Frequent insert/delete
 * AVL Trees -> Frequent search
 * Splay Trees -> Large number of nodes but only some are accessed frequently
 * 
 * Results:(Node Count:100000, Search Value Count:100000, Delete Operation: Every Node)
 * 
 * Red-Black Tree Insert:                           35ms
 * Red-Black Tree Random Existing Value Search:     27ms
 * Red-Black Tree Random Non Existing Value Search: 29ms
 * Red-Black Tree Repeatedly Accessed Value Search: 12ms
 * Red-Black Tree Deletion:                         384ms
 * 
 * Splay Tree Insert:                           108ms
 * Splay Tree Random Existing Value Search:     115ms
 * Splay Tree Random Non Existing Value Search: 121ms
 * Splay Tree Repeatedly Accessed Value Search: 15ms
 * Splay Tree Deletion:                         473ms
 * 
 * AVL Tree Insert:                           471417ms(~7m 50sn)
 * AVL Tree Random Existing Value Search:     29ms
 * AVL Tree Random Non Existing Value Search: 34ms
 * AVL Tree Repeatedly Accessed Value Search: 12ms
 * AVL Tree Deletion:                         504074ms(~8m 20sn)
 */

namespace SelfBalancingBinaryTrees
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ISelfBalancingBTree> selfBalancingBTrees = new List<ISelfBalancingBTree>();
            selfBalancingBTrees.Add(new RedBlackTree());
            selfBalancingBTrees.Add(new SplayTree());
            selfBalancingBTrees.Add(new AVLTree());

            int nodeCount = 100000;
            int range = nodeCount * 10;
            int searchValueCount = nodeCount;
            List<int> numbers = new List<int>();
            List<int> randomExistingSearchValues = new List<int>();
            List<int> randomNonExistingSearchValues = new List<int>();
            List<int> repeatedSearchValues = new List<int>();
            Random random = new Random();
            Stopwatch stopwatch = new Stopwatch();

            //Determine numbers that will be inserted into trees
            while (numbers.Count < nodeCount)
            {
                int number = random.Next(1, range);
                if (!numbers.Contains(number))
                    numbers.Add(number);
            }
            Console.WriteLine("Numbers Array Prep Finished.");

            //List creation for random existing values
            while (randomExistingSearchValues.Count < searchValueCount)
                randomExistingSearchValues.Add(numbers[random.Next(0, numbers.Count)]);
            Console.WriteLine("Random Existing Value Array Prep Finished.");

            //List creation for random non existing values
            while (randomNonExistingSearchValues.Count < searchValueCount)
            {
                int number = random.Next(1, range);
                if (!numbers.Contains(number))
                    randomNonExistingSearchValues.Add(number);
            }
            Console.WriteLine("Random Non Existing Value Array Prep Finished.");

            //List creation for repeatedly accessed values
            List<int> tempConstantSearchList = new List<int>();
            while (tempConstantSearchList.Count < searchValueCount)
            {
                int number = numbers[random.Next(1, numbers.Count)];
                if (!tempConstantSearchList.Contains(number))
                {
                    for (int i = 0; i < searchValueCount / 10; i++)
                    {
                        tempConstantSearchList.Add(number);
                    }
                }
            }
            while (tempConstantSearchList.Count > 0)
            {
                int selection = random.Next(0, tempConstantSearchList.Count);
                repeatedSearchValues.Add(tempConstantSearchList[selection]);
                tempConstantSearchList.RemoveAt(selection);
            }
            Console.WriteLine("Repeatedly Accessed Value Array Prep Finished.\n");

            foreach(ISelfBalancingBTree tree in selfBalancingBTrees)
            {
                //Insert numbers into current tree
                stopwatch.Restart();
                foreach(int number in numbers)
                {
                    tree.Insert(number);
                }
                Console.WriteLine(tree.GetType().Name + " Insertion: " + stopwatch.ElapsedMilliseconds + "ms");

                //Search random existing value in the current tree
                stopwatch.Restart();
                foreach (int number in randomExistingSearchValues)
                {
                    tree.Exists(number);
                }
                Console.WriteLine(tree.GetType().Name + " Random Existing Value Search: " + stopwatch.ElapsedMilliseconds + "ms");

                //Search random non existing value in the current tree
                stopwatch.Restart();
                foreach (int number in randomNonExistingSearchValues)
                {
                    tree.Exists(number);
                }
                Console.WriteLine(tree.GetType().Name + " Random Non Existing Value Search: " + stopwatch.ElapsedMilliseconds + "ms");

                //Search frequently accessed value in the current tree
                stopwatch.Restart();
                foreach (int number in repeatedSearchValues)
                {
                    tree.Exists(number);
                }
                Console.WriteLine(tree.GetType().Name + " Repeatedly Accessed Value Search: " + stopwatch.ElapsedMilliseconds + "ms");

                List<int> numbersCopy = numbers.ToList(); //Copy numbers to another list so we don't lose them for the next tree
                //Delete numbers from current tree
                stopwatch.Restart();
                while (numbersCopy.Count > 0)
                {
                    int selection = random.Next(0, numbersCopy.Count);
                    tree.Delete(numbersCopy[selection]);
                    numbersCopy.RemoveAt(selection);
                }
                Console.WriteLine(tree.GetType().Name + " Deletion: " + stopwatch.ElapsedMilliseconds + "ms\n");
            }

            Console.ReadKey();
        }
    }
}

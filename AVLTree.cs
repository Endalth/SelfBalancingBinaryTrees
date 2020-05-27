using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfBalancingBinaryTrees
{
    class AVLTree : ISelfBalancingBTree
    {
        public class Node
        {
            public int data;
            public int height = 0;
            public Node parent;
            public Node leftChild;
            public Node rightChild;
        }

        public Node root;

        public void Insert(int data)
        {
            Node node = new Node();
            node.data = data;
            if (root == null)
            {
                root = node;
            }
            else
            {
                Node currentNode = root;
                while (true)
                {
                    if (node.data == currentNode.data) //duplicate values are not allowed
                        return;
                    else if (node.data < currentNode.data)
                    {
                        if (currentNode.leftChild != null)
                            currentNode = currentNode.leftChild;
                        else
                        {
                            node.parent = currentNode;
                            currentNode.leftChild = node;
                            FixInsertion(node);
                            return;
                        }
                    }
                    else
                    {
                        if (currentNode.rightChild != null)
                            currentNode = currentNode.rightChild;
                        else
                        {
                            node.parent = currentNode;
                            currentNode.rightChild = node;
                            FixInsertion(node);
                            return;
                        }
                    }
                }
            }
        }

        public void FixInsertion(Node node)
        {
            Node tempNode = node;
            while (tempNode != null)
            {
                tempNode.height = GetHeight(tempNode) - 1;

                int leftChildHeight = (tempNode.leftChild != null ? tempNode.leftChild.height + 1 : 0);
                int rightChildHeight = (tempNode.rightChild != null ? tempNode.rightChild.height + 1 : 0);

                int balance = leftChildHeight - rightChildHeight;

                //Left Left Case
                if (balance > 1 && tempNode.leftChild != null && node.data < tempNode.leftChild.data)
                {
                    RightRotation(tempNode);
                }
                //Left Right
                else if (balance > 1 && tempNode.leftChild != null && node.data > tempNode.leftChild.data)
                {
                    LeftRotation(tempNode.leftChild);
                    RightRotation(tempNode);
                }
                //Right Right
                else if (balance < -1 && tempNode.rightChild != null && node.data > tempNode.rightChild.data)
                {
                    LeftRotation(tempNode);
                }
                //Right Left
                else if (balance < -1 && tempNode.rightChild != null && node.data < tempNode.rightChild.data)
                {
                    RightRotation(tempNode.rightChild);
                    LeftRotation(tempNode);
                }

                tempNode = tempNode.parent;
            }
        }

        public void Delete(int data)
        {
            if (root != null)
            {
                Node currentNode = FindNode(data);

                //If data is found apply the appropriate deletion procedure
                if (currentNode != null)
                {
                    Node fixNode = null;
                    //No Child
                    if (currentNode.rightChild == null && currentNode.leftChild == null)
                    {
                        if (currentNode == root)
                            root = null;
                        else
                        {
                            if (currentNode == currentNode.parent.rightChild)
                                currentNode.parent.rightChild = null;
                            else
                                currentNode.parent.leftChild = null;

                            fixNode = currentNode.parent;
                        }
                    }
                    //No right child
                    else if (currentNode.rightChild == null)
                    {
                        if (currentNode == root)
                        {
                            root = currentNode.leftChild;
                            root.parent = null;

                            fixNode = root;
                        }
                        else
                        {
                            if (currentNode == currentNode.parent.rightChild)
                                currentNode.parent.rightChild = currentNode.leftChild;
                            else
                                currentNode.parent.leftChild = currentNode.leftChild;
                            currentNode.leftChild.parent = currentNode.parent;

                            fixNode = currentNode.leftChild;
                        }
                    }
                    //2 children or just right child
                    else
                    {
                        //Find the minimum replacement
                        Node rightChildMinimum = currentNode.rightChild;
                        while (rightChildMinimum.leftChild != null)
                            rightChildMinimum = rightChildMinimum.leftChild;

                        //Replace data
                        currentNode.data = rightChildMinimum.data;

                        //Connect minimum child right tree with minimum child's parent
                        if (rightChildMinimum == rightChildMinimum.parent.leftChild)
                        {
                            rightChildMinimum.parent.leftChild = rightChildMinimum.rightChild;
                            if (rightChildMinimum.rightChild != null)
                                rightChildMinimum.rightChild.parent = rightChildMinimum.parent;
                        }
                        else
                        {
                            rightChildMinimum.parent.rightChild = rightChildMinimum.rightChild;
                            if (rightChildMinimum.rightChild != null)
                                rightChildMinimum.rightChild.parent = rightChildMinimum.parent;
                        }

                        fixNode = rightChildMinimum.parent;
                    }

                    FixDeletion(fixNode);
                }
            }
        }

        public void FixDeletion(Node node)
        {
            //Recalculate heights up to root
            Node tempNode = node;
            while (tempNode != null)
            {
                tempNode.height = GetHeight(tempNode) - 1;

                int leftChildHeight = 0;
                int leftChildBalance = 0;

                int rightChildHeight = 0;
                int rightChildBalance = 0;

                if (tempNode.leftChild != null)
                {
                    leftChildHeight = tempNode.leftChild.height + 1;
                    int leftChildLeftHeight = tempNode.leftChild.leftChild != null ? tempNode.leftChild.leftChild.height + 1 : 0;
                    int leftChildRightHeight = tempNode.leftChild.rightChild != null ? tempNode.leftChild.rightChild.height + 1 : 0;

                    leftChildBalance = leftChildLeftHeight - leftChildRightHeight;
                }

                if (tempNode.rightChild != null)
                {
                    rightChildHeight = tempNode.rightChild.height + 1;
                    int rightChildLeftHeight = tempNode.rightChild.leftChild != null ? tempNode.rightChild.leftChild.height + 1 : 0;
                    int rightChildRightHeight = tempNode.rightChild.rightChild != null ? tempNode.rightChild.rightChild.height + 1 : 0;

                    rightChildBalance = rightChildLeftHeight - rightChildRightHeight;
                }

                int balance = leftChildHeight - rightChildHeight;


                //Left Left Case
                if (balance > 1 && leftChildBalance >= 0)
                {
                    RightRotation(tempNode);
                }
                //Left Right
                else if (balance > 1 && leftChildBalance < 0)
                {
                    LeftRotation(tempNode.leftChild);
                    RightRotation(tempNode);
                }
                //Right Right
                else if (balance < -1 && rightChildBalance <= 0)
                {
                    LeftRotation(tempNode);
                }
                //Right Left
                else if (balance < -1 && rightChildBalance > 0)
                {
                    RightRotation(tempNode.rightChild);
                    LeftRotation(tempNode);
                }

                tempNode = tempNode.parent;
            }
        }

        public void RightRotation(Node node)
        {
            Node tempLeft = node.leftChild;

            node.leftChild = tempLeft.rightChild;
            if (tempLeft.rightChild != null)
                tempLeft.rightChild.parent = node;

            tempLeft.rightChild = node;

            if (node == root)
                root = tempLeft;
            else
            {
                if (node == node.parent.leftChild)
                    node.parent.leftChild = tempLeft;
                else
                    node.parent.rightChild = tempLeft;
            }

            tempLeft.parent = node.parent;
            tempLeft.height = GetHeight(tempLeft) - 1;

            node.parent = tempLeft;
            node.height = GetHeight(node) - 1;
        }

        public void LeftRotation(Node node)
        {
            Node tempRight = node.rightChild;

            node.rightChild = tempRight.leftChild;
            if (tempRight.leftChild != null)
                tempRight.leftChild.parent = node;

            tempRight.leftChild = node;

            if (node == root)
                root = tempRight;
            else
            {
                if (node == node.parent.leftChild)
                    node.parent.leftChild = tempRight;
                else
                    node.parent.rightChild = tempRight;
            }

            tempRight.parent = node.parent;
            tempRight.height = GetHeight(tempRight) - 1;

            node.parent = tempRight;
            node.height = GetHeight(node) - 1;
        }

        public bool Exists(int data)
        {
            return FindNode(data) != null;
        }

        public Node FindNode(int data)
        {
            Node currentNode = root;
            //Find the data
            while (currentNode != null && currentNode.data != data)
            {
                if (data < currentNode.data)
                    currentNode = currentNode.leftChild;
                else if (data > currentNode.data)
                    currentNode = currentNode.rightChild;
            }

            return currentNode;
        }

        public int GetHeight(Node currentNode)
        {
            if (currentNode == null)
                return 0;

            int leftHeight = GetHeight(currentNode.leftChild);
            int rightHeight = GetHeight(currentNode.rightChild);

            return 1 + Math.Max(leftHeight, rightHeight);
        }

        public void DrawTree()
        {
            DrawSubTree(root, 0, "");
            Console.WriteLine("\n\n\n");
        }

        public void DrawSubTree(Node currentNode, int height, string direction)
        {
            if (currentNode == null)
                return;

            string space = "";
            for (int i = 0; i < height; i++)
            {
                space += "    ";
            }
            DrawSubTree(currentNode.rightChild, height + 1, "/");
            int balance = (currentNode.leftChild != null ? currentNode.leftChild.height + 1 : 0) - (currentNode.rightChild != null ? currentNode.rightChild.height + 1 : 0);
            Console.WriteLine(space + direction + currentNode.data + "(" + balance + ")");
            DrawSubTree(currentNode.leftChild, height + 1, "\\");

        }
    }
}

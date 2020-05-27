using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfBalancingBinaryTrees
{
    class SplayTree : ISelfBalancingBTree
    {
        public class Node
        {
            public int data;
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
                    {
                        Splay(currentNode);
                        return;
                    }
                    else if (node.data < currentNode.data)
                    {
                        if (currentNode.leftChild == null)
                        {
                            node.parent = currentNode;
                            currentNode.leftChild = node;
                            Splay(node);
                            return;
                        }
                        else
                            currentNode = currentNode.leftChild;
                    }
                    else
                    {
                        if (currentNode.rightChild == null)
                        {
                            node.parent = currentNode;
                            currentNode.rightChild = node;
                            Splay(node);
                            return;
                        }
                        else
                            currentNode = currentNode.rightChild;
                    }
                }
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
                    if(currentNode.leftChild == null)
                    {
                        root = currentNode.rightChild;
                        if (root != null)
                            root.parent = null;
                    }
                    else
                    {
                        root = currentNode.leftChild;
                        root.parent = null;
                        Node maxValueNode = root;
                        while(maxValueNode.rightChild != null)
                        {
                            maxValueNode = maxValueNode.rightChild;
                        }

                        Splay(maxValueNode);

                        root.rightChild = currentNode.rightChild;
                        if (currentNode.rightChild != null)
                            root.rightChild.parent = root;
                    }
                }
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
            node.parent = tempLeft;
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
            node.parent = tempRight;
        }

        #region Some Utility Functions

        public bool Exists(int data)
        {
            return FindNode(data) != null;
        }

        public Node FindNode(int data)
        {
            Node currentNode = root;
            Node lastAccessedNode = null;
            //Find the data
            while (currentNode != null && currentNode.data != data)
            {
                lastAccessedNode = currentNode;
                if (data < currentNode.data)
                    currentNode = currentNode.leftChild;
                else if (data > currentNode.data)
                    currentNode = currentNode.rightChild;
            }

            if (currentNode != root)
            {
                if (currentNode != null)
                    Splay(currentNode);
                else
                    Splay(lastAccessedNode);
            }

            return currentNode;
        }

        public void Splay(Node node)
        {
            while (node != root)
            {
                Node parent = node.parent;
                Node grandparent = node.parent.parent;

                if (grandparent == null)
                {
                    if (node == parent.leftChild)
                        RightRotation(parent);
                    else
                        LeftRotation(parent);
                }
                else
                {
                    //Left Left
                    if (node == parent.leftChild && parent == grandparent.leftChild)
                    {
                        RightRotation(grandparent);
                        RightRotation(parent);
                    }
                    //Right Right
                    else if (node == parent.rightChild && parent == grandparent.rightChild)
                    {
                        LeftRotation(grandparent);
                        LeftRotation(parent);
                    }
                    //Left Right
                    else if (node == parent.leftChild && parent == grandparent.rightChild)
                    {
                        RightRotation(parent);
                        LeftRotation(grandparent);
                    }
                    //Right Left
                    else if (node == parent.rightChild && parent == grandparent.leftChild)
                    {
                        LeftRotation(parent);
                        RightRotation(grandparent);
                    }
                }
            }
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
            Console.WriteLine(space + direction + currentNode.data);
            DrawSubTree(currentNode.leftChild, height + 1, "\\");

        }
        #endregion

    }
}

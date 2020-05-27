using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfBalancingBinaryTrees
{
    class RedBlackTree : ISelfBalancingBTree
    {
        public class Node
        {
            public int data;
            public bool isBlack = false;
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
                node.isBlack = true;
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
                        if (currentNode.leftChild == null)
                        {
                            node.parent = currentNode;
                            currentNode.leftChild = node;
                            FixInsertion(node);
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
                            FixInsertion(node);
                            return;
                        }
                        else
                            currentNode = currentNode.rightChild;
                    }
                }
            }
        }

        public void FixInsertion(Node node)
        {
            if (node.parent.isBlack)
                return;

            Node currentNode = node;
            while (currentNode != root && currentNode != null && !currentNode.parent.isBlack)
            {
                Node parent = currentNode.parent;
                Node grandparent = parent.parent;
                Node uncle;

                if (grandparent != null)
                {
                    if (parent == grandparent.leftChild)
                        uncle = grandparent.rightChild;
                    else
                        uncle = grandparent.leftChild;

                    //Red Uncle Violation
                    if (uncle != null && !uncle.isBlack)
                    {
                        parent.isBlack = !parent.isBlack;
                        grandparent.isBlack = !grandparent.isBlack;
                        uncle.isBlack = !uncle.isBlack;
                        currentNode = grandparent;
                    }
                    else if (uncle == null || uncle.isBlack)
                    {
                        //Black Uncle(Triangle) Violations
                        if (currentNode == parent.leftChild && parent == grandparent.rightChild)
                        {
                            currentNode = parent;
                            RightRotation(currentNode);
                        }
                        else if (currentNode == parent.rightChild && parent == grandparent.leftChild)
                        {
                            currentNode = parent;
                            LeftRotation(currentNode);
                        }
                        //Black Uncle(Line) Violations
                        else if (currentNode == parent.leftChild && parent == grandparent.leftChild)
                        {
                            RightRotation(grandparent);
                            grandparent.isBlack = !grandparent.isBlack;
                            parent.isBlack = !parent.isBlack;
                        }
                        else if (currentNode == parent.rightChild && parent == grandparent.rightChild)
                        {
                            LeftRotation(grandparent);
                            grandparent.isBlack = !grandparent.isBlack;
                            parent.isBlack = !parent.isBlack;
                        }
                    }
                }
                root.isBlack = true;
            }
        }

        public void Delete(int data)
        {
            if (root != null)
            {
                Node currentNode = FindNode(data);
                Node replacement = null;
                Node x = null;
                Node xParent;

                //If data is found apply the appropriate deletion procedure
                if (currentNode != null)
                {
                    bool deletedNodeColor = currentNode.isBlack;
                    //No Child
                    if (currentNode.rightChild == null && currentNode.leftChild == null)
                    {
                        xParent = currentNode.parent;
                        if (currentNode == root)
                            root = null;
                        else
                        {
                            if (currentNode == currentNode.parent.rightChild)
                                currentNode.parent.rightChild = null;
                            else
                                currentNode.parent.leftChild = null;
                        }
                    }
                    //No right child
                    else if (currentNode.rightChild == null)
                    {
                        if (currentNode == root)
                        {
                            root = currentNode.leftChild;
                            root.parent = null;
                        }
                        else
                        {
                            if (currentNode == currentNode.parent.rightChild)
                                currentNode.parent.rightChild = currentNode.leftChild;
                            else
                                currentNode.parent.leftChild = currentNode.leftChild;
                            currentNode.leftChild.parent = currentNode.parent;
                        }
                        replacement = currentNode.leftChild;
                        x = replacement;
                        xParent = x.parent;
                    }
                    //2 children or just right child
                    else
                    {
                        //Find the minimum replacement
                        replacement = currentNode.rightChild;
                        while (replacement.leftChild != null)
                            replacement = replacement.leftChild;

                        if (currentNode.leftChild != null)
                            x = replacement.rightChild;
                        else
                            x = replacement;

                        if (replacement == replacement.parent.leftChild)
                        {
                            xParent = replacement.parent;

                            //Connect replacement right child as replacement parent left child
                            replacement.parent.leftChild = replacement.rightChild;
                            if (replacement.rightChild != null)
                                replacement.rightChild.parent = replacement.parent;

                            //Connect deleted node right child with replacement
                            currentNode.rightChild.parent = replacement;
                            replacement.rightChild = currentNode.rightChild;

                            //Connect deleted node left child with replacement
                            if (currentNode.leftChild != null)
                                currentNode.leftChild.parent = replacement;
                            replacement.leftChild = currentNode.leftChild;

                            //Connect deleted node's parent with replacement
                            if (currentNode == root)
                            {
                                replacement.parent = null;
                                root = replacement;
                            }
                            else
                            {
                                replacement.parent = currentNode.parent;
                                if (currentNode == currentNode.parent.leftChild)
                                    currentNode.parent.leftChild = replacement;
                                else
                                    currentNode.parent.rightChild = replacement;
                            }
                        }
                        else
                        {
                            if (currentNode.leftChild != null)
                                xParent = replacement;
                            else
                                xParent = currentNode.parent;

                            //Connect deleted node left child with replacement
                            replacement.leftChild = currentNode.leftChild;
                            if (currentNode.leftChild != null)
                                currentNode.leftChild.parent = replacement;

                            //Connect deleted node's parent with replacement
                            if (currentNode == root)
                            {
                                replacement.parent = null;

                                root = replacement;
                            }
                            else
                            {
                                replacement.parent = currentNode.parent;
                                if (currentNode == currentNode.parent.leftChild)
                                    currentNode.parent.leftChild = replacement;
                                else
                                    currentNode.parent.rightChild = replacement;
                            }
                        }
                    }
                    if (!deletedNodeColor && replacement != null && replacement.isBlack)
                    {
                        replacement.isBlack = false;
                        FixDeletion(xParent, x);
                    }
                    else if (deletedNodeColor && replacement != null && !replacement.isBlack)
                        replacement.isBlack = true;
                    else if (deletedNodeColor)
                        FixDeletion(xParent, x);
                }
            }
        }

        public void FixDeletion(Node xParent, Node x)
        {
            while ((x == null || x.isBlack) && x != root)
            {
                if (xParent.leftChild == x)
                {
                    Node sibling = xParent.rightChild;
                    //sibling is red
                    if (sibling != null && !sibling.isBlack)
                    {
                        sibling.isBlack = true;
                        xParent.isBlack = false;
                        LeftRotation(xParent);
                    }
                    //sibling black sibling's children are black
                    else if (sibling == null || (sibling.leftChild == null || sibling.leftChild.isBlack) && (sibling.rightChild == null || sibling.rightChild.isBlack))
                    {
                        if (sibling != null)
                            sibling.isBlack = false;
                        x = xParent;
                        xParent = xParent.parent;
                    }
                    //sibling left children is red
                    else if (sibling != null && sibling.leftChild != null && !sibling.leftChild.isBlack && (sibling.rightChild == null || sibling.rightChild.isBlack))
                    {
                        sibling.leftChild.isBlack = true;
                        sibling.isBlack = false;
                        RightRotation(sibling);
                    }
                    //sibling right children is red
                    else if (sibling != null && sibling.rightChild != null && !sibling.rightChild.isBlack)
                    {
                        sibling.isBlack = xParent.isBlack;
                        xParent.isBlack = true;
                        sibling.rightChild.isBlack = true;
                        LeftRotation(xParent);
                        x = root;
                    }
                }
                else if (xParent.rightChild == x)
                {
                    Node sibling = xParent.leftChild;
                    //sibling is red
                    if (sibling != null && !sibling.isBlack)
                    {
                        sibling.isBlack = true;
                        xParent.isBlack = false;
                        RightRotation(xParent);
                    }
                    //sibling black sibling's children are black
                    else if (sibling == null || (sibling.leftChild == null || sibling.leftChild.isBlack) && (sibling.rightChild == null || sibling.rightChild.isBlack))
                    {
                        if (sibling != null)
                            sibling.isBlack = false;
                        x = xParent;
                        xParent = xParent.parent;
                    }
                    //sibling right children is red
                    else if (sibling != null && sibling.rightChild != null && !sibling.rightChild.isBlack && (sibling.leftChild == null || sibling.leftChild.isBlack))
                    {
                        sibling.rightChild.isBlack = true;
                        sibling.isBlack = false;
                        LeftRotation(sibling);
                    }
                    //sibling left children is red
                    else if (sibling != null && sibling.leftChild != null && !sibling.leftChild.isBlack)
                    {
                        sibling.isBlack = xParent.isBlack;
                        xParent.isBlack = true;
                        sibling.leftChild.isBlack = true;
                        RightRotation(xParent);
                        x = root;
                    }
                }
            }

            if (x != null)
                x.isBlack = true;
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
            Console.WriteLine(space + direction + currentNode.data + (currentNode.isBlack ? " B" : " R"));
            DrawSubTree(currentNode.leftChild, height + 1, "\\");

        }
    }
}
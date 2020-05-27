using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfBalancingBinaryTrees
{
    interface ISelfBalancingBTree
    {
        void Insert(int data);
        void Delete(int data);
        bool Exists(int data);
        void DrawTree();
    }
}

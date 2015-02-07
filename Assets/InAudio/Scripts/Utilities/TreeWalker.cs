using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace InAudioSystem
{
    public static class TreeWalker
    {
        public static void ForEach<T>(T node, Action<T> action)
            where T : Object, InITreeNode<T>
        {
            if (node == null)
                return;

            action(node);

            for (int i = 0; i < node.GetChildren.Count; i++)
            {
                ForEach(node.GetChildren[i], action);
            }
        }

        public static void ForEachPostOrder<T>(T node, Action<T> action)
            where T : Object, InITreeNode<T>
        {
            if (node == null)
                return;



            for (int i = 0; i < node.GetChildren.Count; i++)
            {
                ForEach(node.GetChildren[i], action);
            }

            action(node);
        }



        public static List<U> FindAll<T, U>(T node, Func<T, U> toAdd) where T : Object, InITreeNode<T> where U : class
        {
            var found = new HashSet<U>();
            FindAll(node, toAdd, found);
            return found.ToList();
        }

        private static void FindAll<T, U>(T node, Func<T, U> toAdd, HashSet<U> found) where T : Object, InITreeNode<T> where U : class
        {
            if (node == null)
                return;
            U foundObj = toAdd(node);
            if (foundObj != null)
            {
                found.Add(foundObj);
            }
            for (int i = 0; i < node.GetChildren.Count; i++)
            {
                FindAll(node.GetChildren[i], toAdd, found);
            }
        }

        public static InAudioNode FindParentBeforeFolder(InAudioNode node)
        {
            if (node.Parent.Type == AudioNodeType.Folder || node.Parent.Type == AudioNodeType.Root)
                return node;
            return FindParentBeforeFolder(node.Parent);
        }

        public static T FindFirst<T>(T node, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            if (node == null)
                return null;
            if (predicate(node))
            {
                return node;
            }
            for (int i = 0; i < node.GetChildren.Count; i++)
            {
                var result = FindFirst(node.GetChildren[i], predicate);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static T FindById<T>(T node, int id) where T : Object, InITreeNode<T>
        {
            if (node == null)
                return null;
            if (node.ID == id)
                return node;
            for (int i = 0; i < node.GetChildren.Count; i++)
            {
                var result = FindById(node.GetChildren[i], id);
                if (result != null && result.ID == id)
                    return result;
            }
            return null;
        }

        public static int Count<T>(T node, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            if (node == null)
                return 0;
            int result = 0;
            if (predicate(node))
                result += 1;
            for (int i = 0; i < node.GetChildren.Count; i++)
            {
                result += Count(node.GetChildren[i], predicate);
            }
            return result;
        }

        public static int Sum<T>(T node, Func<T, int> predicate) where T : Object, InITreeNode<T>
        {
            if (node == null)
                return 0;
            int result = predicate(node);
            for (int i = 0; i < node.GetChildren.Count; i++)
            {
                result += Sum(node.GetChildren[i], predicate);
            }
            return result;
        }

        public static List<T> Where<T>(T node, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            return Where(node, predicate, new List<T>());
        }

        private static List<T> Where<T>(T node, Func<T, bool> predicate, List<T> nodes) where T : Object, InITreeNode<T>
        {
            if (node == null)
                return nodes;

            if (predicate(node))
                nodes.Add(node);

            for (int i = 0; i < nodes.Count; ++i)
            {
                Where(nodes[i], predicate, nodes);
            }

            return nodes;
        }

        public static int FindIndexInParent<T>(T node) where T : Object, InITreeNode<T>
        {
            if (node.GetParent == null)
                return 0;
            for (int i = 0; i < node.GetParent.GetChildren.Count; ++i)
            {
                if (node.GetParent.GetChildren[i] == node)
                {
                    return i;
                }
            }
            return 0;
        }

        public static void FindAllNodes<T>(T node, Func<T, bool> condition, List<T> foundNodes) where T : Object, InITreeNode<T>
        {
            if (condition(node))
            {
                foundNodes.Add(node);
            }
            for (int i = 0; i < node.GetChildren.Count; ++i)
            {
                FindAllNodes(node.GetChildren[i], condition, foundNodes);
            }
        }


#if UNITY_EDITOR
        public static T FindFoldedParent<T>(T node) where T : Object, InITreeNode<T>
        {
            if (node.IsRoot)
            {
                return default(T); //Null
            }
            else
            {
                var parent = node.GetParent;
                if (!parent.IsFoldedOut)
                    return parent;
                else
                    return FindFoldedParent(parent);
            }
        }

        public static T FindPreviousUnfoldedNode<T>(T node) where T : Object, InITreeNode<T>
        {
            int index = FindIndexInParent(node);

            if (node.IsRoot)
                return node;
            else if (index == 0)
            {
                if (node.GetParent.IsRoot)
                    return node.GetParent;
                return node.GetParent;
            }
            else
            {
                T previousNode = node.GetParent.GetChildren[index - 1];
                while (previousNode.IsFoldedOut)
                {
                    if (previousNode.GetChildren.Count == 0)
                        return previousNode;
                    else
                    {
                        previousNode = previousNode.GetChildren[previousNode.GetChildren.Count - 1];
                    }
                }
                return previousNode;
            }
        }

        public static T GetPreviousVisibleNode<T>(T node) where T : Object, InITreeNode<T>
        {
            return FindPreviousUnfoldedNode(node, arg => !arg.IsFiltered);
        }

        public static T FindPreviousUnfoldedNode<T>(T node, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            int index = FindIndexInParent(node);
            if (node.IsRoot)
                return node;
            else if (index == 0)
            {
                return node.GetParent;
            }
            else
            {
                T previousNode = null;

                for (int i = index - 1; i >= 0; i--)
                {
                    if (predicate(node.GetParent.GetChildren[i]))
                    {
                        previousNode = node.GetParent.GetChildren[i];
                        break;
                    }
                }
                if (previousNode == null)
                {
                    return node.GetParent;
                }

                while (previousNode.IsFoldedOut)
                {
                    if (previousNode.GetChildren.Count == 0)
                        return previousNode;
                    else
                    {
                        bool set = false;
                        for (int i = previousNode.GetChildren.Count - 1; i >= 0; i--)
                        {
                            if (predicate(previousNode.GetChildren[i]))
                            {
                                previousNode = previousNode.GetChildren[i];
                                set = true;
                                break;
                            }
                        }
                        if (!set)
                            previousNode = previousNode.GetChildren[previousNode.GetChildren.Count - 1];
                    }
                }
                return previousNode;
            }
        }

#endif

        public static T FindNextSibling<T>(T node) where T : Object, InITreeNode<T>
        {
            //Keep walking up as the current node may be n deep
            while (node != null && node.GetParent != null)
            {
                //Look through all the children
                for (int i = 0; i < node.GetParent.GetChildren.Count; ++i)
                {
                    //We found the starting node
                    if (node.GetParent.GetChildren[i] == node)
                    {
                        //If the node is the last one, select the parent and try again by breaking to the while loop
                        if (i == node.GetParent.GetChildren.Count - 1)
                        {
                            node = node.GetParent;
                            break;
                        }
                        else //There is another sibling, select that one
                        {
                            return node.GetParent.GetChildren[i + 1];
                        }
                    }
                }
            }
            return node;
        }


#if UNITY_EDITOR
        public static T FindNextNode<T>(T node, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            var nodeChildren = node.GetChildren;
            if (node.IsFoldedOut && predicate(node) && nodeChildren.Count > 0)
            {
                for (int i = 0; i < nodeChildren.Count; i++)
                {
                    if (predicate(nodeChildren[i]))
                        return nodeChildren[i];
                }
            }

            if (node.GetParent != null)
            {
                //Find next sibling
                int index = FindIndexInParent(node);
                var parentChildren = node.GetParent.GetChildren;
                for (int i = index + 1; i < parentChildren.Count; i++)
                {
                    if (predicate(parentChildren[i]))
                        return parentChildren[i];
                }

                //No sibling found, check the sibling for the next parent

                //var nextNode = FindSibling(node.GetParent, node, predicate);
                var nextNode = FindSibling(node.GetParent, node, predicate);

                if (!IsParentOf(nextNode, node))
                    return nextNode;
                else
                    return node;
            }

            return node;
        }

        public static T FindSibling<T>(T node, T calledFrom, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            int nextIndex = node.GetChildren.FindIndex(t => t == calledFrom) + 1;
            //No more children, go to the parent
            if (nextIndex >= node.GetChildren.Count)
            {
                if (!node.IsRoot)
                {
                    return FindSibling(node.GetParent, node, predicate);

                }
                else
                    return node;
            }
            else //There are children, 
            {
                for (int i = nextIndex; i < node.GetChildren.Count; i++)
                {
                    if (predicate(node.GetChildren[nextIndex]))
                        return node.GetChildren[nextIndex];
                }
                return calledFrom;
            }
        }

        
#endif
        public static bool Any<T>(T node, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            if (predicate(node))
                return true;

            for (int i = 0; i < node.GetChildren.Count; ++i)
            {
                if (Any(node.GetChildren[i], predicate))
                {
                    return true;
                }
            }

            return false;
        }



        public static bool IsParentOf<T>(T node, T potentialParent) where T : Object, InITreeNode<T>
        {
            if (node == potentialParent)
                return true;

            if (potentialParent.GetParent != null)
                return IsParentOf(node, potentialParent.GetParent);

            return false;
        }


        public static void ForEachParent<T>(T node, Action<T> action) where T : Object, InITreeNode<T>
        {
            action(node);
            if (node.IsRoot)
                return;
            ForEachParent(node.GetParent, action);
        }
    }
}
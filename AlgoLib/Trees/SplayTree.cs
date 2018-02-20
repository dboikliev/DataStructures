﻿using System;

namespace AlgoLib.Trees
{
    public class SplayTree<T>
    {
        public class SplayTreeNode
        {
            public long Sum { get; set; }
            
            public T Value { get; }

            public SplayTreeNode Left { get; set; }
            public SplayTreeNode Right { get; set; }
            public SplayTreeNode Parent { get; set; }

            public SplayTreeNode(T value)
            {
                Value = value;
                Sum = (value as long?) ?? 0;
            }
        }

        private readonly Comparison<T> _comparison;
        
        public SplayTreeNode Root { get; private set; }

        public SplayTree(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        private SplayTree(Comparison<T> comparison, SplayTreeNode root)
        {
            Root = root;
            _comparison = comparison;
        }

        public void Add(T value)
        {
            if (Root == null)
            {
                Root = new SplayTreeNode(value);
                return;
            }

            SplayTreeNode closest = FindClosest(value);

            if (_comparison(closest.Value, value) == 0)
            {
                Splay(closest);
                Root = closest;
                return;
            }

            SplayTreeNode newNode = AddChild(closest, value);
            Splay(newNode);
            Root = newNode;
        }

        public bool Contains(T value)
        {
            SplayTreeNode closest = FindClosest(value);
            if (closest == null)
            {
                return false;
            }

            Splay(closest);
            Root = closest;
            return _comparison(closest.Value, value) == 0;
        }

        private SplayTreeNode AddChild(SplayTreeNode node, T value)
        {
            var newNode = new SplayTreeNode(value);
            if (_comparison(node.Value, value) > 0)
            {
                node.Left = newNode;
            }
            else
            {
                node.Right = newNode;
            }

            newNode.Parent = node;
            return newNode;
        }

        private long GetSum(SplayTreeNode node)
        {
            return node == null ? 0 : node.Sum;
        }

        private SplayTreeNode FindClosest(T value)
        {
            SplayTreeNode parent = Root;
            SplayTreeNode current = Root;
            while (current != null)
            {
                parent = current;
                if (_comparison(current.Value, value) > 0)
                {
                    current = current.Left;
                }
                else if (_comparison(current.Value, value) < 0)
                {
                    current = current.Right;
                }
                else
                {
                    return current;
                }
            }

            return parent;
        }

        private void Splay(SplayTreeNode node)
        {
            while (node != null && node.Parent != null)
            {
                SplayTreeNode parent = node.Parent;
                SplayTreeNode grandParent = parent.Parent;
                if (grandParent == null)
                {
                    if (parent.Left == node)
                    {
                        RotateRight(parent);
                    }
                    else
                    {
                        RotateLeft(parent);
                    }
                }
                else
                {
                    if (grandParent.Left == parent && parent.Left == node)
                    {
                        RotateRight(grandParent);
                        RotateRight(parent);
                    }
                    else if (grandParent.Right == parent && parent.Right == node)
                    {
                        RotateLeft(grandParent);
                        RotateLeft(parent);
                    }
                    else if (grandParent.Left == parent && parent.Right == node)
                    {
                        RotateLeft(parent);
                        RotateRight(grandParent);
                    }
                    else if (grandParent.Right == parent && parent.Left == node)
                    {
                        RotateRight(parent);
                        RotateLeft(grandParent);
                    }
                }
            }
        }

        private void RotateRight(SplayTreeNode x)
        {
            SplayTreeNode xParent = x.Parent;
            SplayTreeNode y = x.Left;
            SplayTreeNode yRight = y?.Right;

            x.Parent = y;
            x.Left = yRight;


            if (y != null)
            {
                y.Right = x;
                y.Parent = xParent;

                if (yRight != null)
                {
                    yRight.Parent = x;
                }
            }


            if (xParent != null)
            {
                if (xParent.Left == x)
                {
                    xParent.Left = y;
                }
                else
                {
                    xParent.Right = y;
                }
            }
        }

        private void RotateLeft(SplayTreeNode x)
        {
            SplayTreeNode xParent = x.Parent;
            SplayTreeNode y = x.Right;
            SplayTreeNode yLeft = y.Left;

            x.Parent = y;
            x.Right = yLeft;

            if (y != null)
            {
                y.Left = x;
                y.Parent = xParent;

                if (yLeft != null)
                {
                    yLeft.Parent = x;
                }
            }

            if (xParent != null)
            {
                if (xParent.Left == x)
                {
                    xParent.Left = y;
                }
                else
                {
                    xParent.Right = y;
                }
            }
        }

        public void Delete(T value)
        {
            SplayTreeNode node = FindClosest(value);

            Splay(node);
            Root = node;
            if (node != null && _comparison(node.Value, value) == 0)
            {
                if (node.Left == null)
                {
                    Root = node.Right;
                    if (Root != null)
                    {
                        Root.Parent = null;
                    }
                    return;
                }

                if (node.Right == null)
                {
                    Root = node.Left;
                    if (Root != null)
                    {
                        Root.Parent = null;
                    }
                    return;
                }

                node.Left.Parent = null;
                node.Right.Parent = null;
                Root = node.Left;
                Merge(node.Right);
            }
        }

        public (SplayTreeNode Left, SplayTreeNode Right) Split(T value)
        {
            SplayTreeNode root = FindClosest(value);
            Splay(root);
            Root = root;

            if (root == null)
            {
                return (null, null);
            }

            int comparison = _comparison(root.Value, value);
            if (comparison >= 0)
            {
                SplayTreeNode left = root.Left;
                root.Left = null;
                if (left != null)
                {
                    left.Parent = null;
                }

                return (left, root);
            }

            if (comparison < 0)
            {
                SplayTreeNode right = root.Right;
                root.Right = null;
                if (right != null)
                {
                    right.Parent = null;
                }

                return (root, right);
            }

            if (root.Left != null)
            {
                root.Left.Parent = null;
            }

            if (root.Right != null)
            {
                root.Right.Parent = null;
            }

            return (root.Left, root.Right);
        }

        public void Merge(SplayTreeNode other)
        {
            if (other == null) return;

            if (Root == null)
            {
                Root = other;
                return;
            }

            SplayTreeNode current = other;
            while (current.Left != null)
            {
                current = current.Left;
            }

            Splay(current);
            current.Left = Root;
            Root.Parent = current;
            Root = current;
        }
    }
}
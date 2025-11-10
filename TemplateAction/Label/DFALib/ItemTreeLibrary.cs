using System;
using System.Collections.Generic;
namespace TemplateAction.Label.StructLib
{
    public class ItemTreeLibrary
    {
        public ItemTreeLibrary(List<string> input)
        {
            Words = new HashSet<string>();
            foreach(string s in input)
            {
                Words.Add(s);
            }

            //初始化结构树
            Library = new Dictionary<char, ItemTree>(Words.Count);
            if (Words != null && Words.Count > 0)
            {
                foreach (string item in Words)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        char cha = item[0];
                        ItemTree node;
                        if (Library.TryGetValue(cha, out node))
                        {
                            AddChildTree(node, item);
                        }
                        else
                        {
                            node = CreateSingleTree(item);
                            Library.Add(node.Item, node);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 创建单个完整树
        /// </summary>
        /// <param name="word">单个敏感词</param>
        /// <returns></returns>
        private ItemTree CreateSingleTree(string word)
        {
            //根节点，此节点 值为空
            ItemTree root = new ItemTree();
            root.Item = word[0];
            root.IsEnd = false;
            //移动 游标
            ItemTree p = root;
            for (int i = 1; i < word.Length; i++)
            {
                ItemTree child = new ItemTree() { Item = word[i], IsEnd = false, Child = null };
                p.Child = new Dictionary<char, ItemTree>();
                p.Child.Add(child.Item, child);
                p = child;
            }
            p.IsEnd = true;
            return root;
        }

        /// <summary>
        /// 附加分支子树
        /// </summary>
        /// <param name="childTree">子树</param>
        /// <param name="word">单个敏感词</param>
        private void AddChildTree(ItemTree childTree, string word)
        {
            //移动 游标
            ItemTree p = childTree;
            for (int i = 1; i < word.Length; i++)
            {
                char cha = word[i];
                Dictionary<char, ItemTree> child = p.Child;

                if (child == null)
                {
                    ItemTree node = new ItemTree() { Item = cha, IsEnd = false, Child = null };
                    p.Child = new Dictionary<char, ItemTree>();
                    p.Child.Add(cha, node);
                    p = node;
                }
                else
                {
                    ItemTree node;
                    if (child.TryGetValue(cha, out node))
                    {
                        p = node;
                    }
                    else
                    {
                        node = new ItemTree() { Item = cha, IsEnd = false, Child = null };
                        child.Add(cha, node);
                        p = node;
                    }
                }
            }
            p.IsEnd = true;
        }

        /// <summary>
        /// 结构库树
        /// </summary>
        public Dictionary<char, ItemTree> Library { get; private set; }
        /// <summary>
        /// 结构体字符集
        /// </summary>
        public HashSet<string> Words { get; private set; }
    }
}

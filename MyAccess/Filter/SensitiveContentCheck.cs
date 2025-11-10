using System;
using System.Collections.Generic;
using System.Linq;
namespace MyAccess.Filter
{
    /// <summary>
    /// 敏感词检测工具
    /// </summary>
    public class SensitiveContentCheck
    {
        /// <summary>
        /// 检测文本
        /// </summary>
        public string Text { private get; set; }

        /// <summary>
        /// 敏感词库 词树
        /// </summary>
        public SensitiveWordsLibrary.ItemTree Library { private get; set; }

        /// <summary>
        /// 敏感词检测
        /// </summary>
        public SensitiveContentCheck() { }

        /// <summary>
        /// 敏感词检测
        /// </summary>
        /// <param name="library">敏感词库</param>
        public SensitiveContentCheck(SensitiveWordsLibrary library)
        {
            if (library.Library == null)
                throw new Exception("敏感词库未初始化");

            Library = library.Library;
        }

        /// <summary>
        /// 敏感词检测
        /// </summary>
        /// <param name="library">敏感词库</param>
        /// <param name="text">检测文本</param>
        public SensitiveContentCheck(SensitiveWordsLibrary library, string text) : this(library)
        {
            if (text == null)
                throw new Exception("检测文本不能为null");

            Text = text;
        }

        /// <summary>
        /// 检测敏感词
        /// </summary>
        /// <param name="text">检测文本</param>
        /// <returns></returns>
        private List<SensitiveChar> WordsCheck(string text)
        {
            if (Library == null)
                throw new Exception("未设置敏感词库 词树");

            List<SensitiveChar> senlist = new List<SensitiveChar>();
            SensitiveWordsLibrary.ItemTree p = Library;
            List<int> indexs = new List<int>();

            for (int i = 0, j = 0; j < text.Length; j++)
            {
                char cha = text[j];
                var child = p.Child;

                var node = child.Find(e => e.Item == cha);
                if (node != null)
                {
                    indexs.Add(j);
                    if (node.IsEnd || node.Child == null)
                    {
                        if (node.Child != null)
                        {
                            int k = j + 1;
                            if (k < text.Length && node.Child.Exists(e => e.Item == text[k]))
                            {
                                p = node;
                                continue;
                            }
                        }
                        //每个词前，分隔
                        if (senlist.Count > 0)
                        {
                            SensitiveChar sc;
                            sc.C = ',';
                            sc.Pos = -1;
                            senlist.Add(sc);
                        }
                        foreach (var item in indexs)
                        {
                            SensitiveChar sc;
                            sc.C = text[item];
                            sc.Pos = item;
                            senlist.Add(sc);
                        }

                        indexs.Clear();
                        p = Library;
                        i = j;
                        ++i;
                    }
                    else
                        p = node;
                }
                else
                {
                    indexs.Clear();
                    if (p.GetHashCode() != Library.GetHashCode())
                    {
                        ++i;
                        j = i;
                        p = Library;
                    }
                    else
                        i = j;
                }
            }

            return senlist;
        }

        /// <summary>
        /// 替换敏感词
        /// </summary>
        /// <param name="library">敏感词库</param>
        /// <param name="text">检测文本</param>
        /// <param name="newChar">替换字符</param>
        /// <returns></returns>
        public static string SensitiveWordsReplace(SensitiveWordsLibrary library, string text, char newChar = '*')
        {
            List<SensitiveChar> sclist = new SensitiveContentCheck(library).WordsCheck(text);
            if (sclist != null && sclist.Count > 0)
            {
                char[] chars = text.ToCharArray();
                foreach (SensitiveChar sc in sclist)
                {
                    if (sc.Pos != -1)
                    {
                        chars[sc.Pos] = newChar;
                    }
                }
                text = new string(chars);
            }

            return text;
        }

        /// <summary>
        /// 替换敏感词
        /// </summary>
        /// <param name="text">检测文本</param>
        /// <param name="newChar">替换字符</param>
        /// <returns></returns>
        public string SensitiveWordsReplace(string text, char newChar = '*')
        {
            List<SensitiveChar> sclist = WordsCheck(text);
            if (sclist != null && sclist.Count > 0)
            {
                char[] chars = text.ToCharArray();
                foreach (SensitiveChar sc in sclist)
                {
                    if (sc.Pos != -1)
                    {
                        chars[sc.Pos] = newChar;
                    }
                }
                text = new string(chars);
            }

            return text;
        }

        /// <summary>
        /// 替换敏感词
        /// </summary>
        /// <param name="newChar">替换字符</param>
        /// <returns></returns>
        public string SensitiveWordsReplace(char newChar = '*')
        {
            if (Text == null)
                throw new Exception("未设置检测文本");

            return SensitiveWordsReplace(Text, newChar);
        }

        /// <summary>
        /// 查找敏感词
        /// </summary>
        /// <param name="library">敏感词库</param>
        /// <param name="text">检测文本</param>
        /// <returns></returns>
        public static List<string> FindSensitiveWords(SensitiveWordsLibrary library, string text)
        {
            SensitiveContentCheck check = new SensitiveContentCheck(library, text);
            return check.FindSensitiveWords();
        }

        /// <summary>
        /// 查找敏感词
        /// </summary>
        /// <param name="text">检测文本</param>
        /// <returns></returns>
        public List<string> FindSensitiveWords(string text)
        {
            List<SensitiveChar> sclist = WordsCheck(text);
            if (sclist != null && sclist.Count > 0)
            {
                string str = "";
                List<string> list = new List<string>();
                foreach (SensitiveChar item in sclist)
                {
                    if (item.Pos != -1)
                        str += item.C;
                    else
                    {
                        list.Add(str);
                        str = "";
                    }
                }
                list.Add(str);

                return list.Distinct().ToList();
            }
            else
                return null;
        }

        /// <summary>
        /// 查找敏感词
        /// </summary>
        /// <returns></returns>
        public List<string> FindSensitiveWords()
        {
            if (Text == null)
                throw new Exception("未设置检测文本");

            return FindSensitiveWords(Text);
        }
    }

    public struct SensitiveChar
    {
        public char C;
        public int Pos;
    }
}

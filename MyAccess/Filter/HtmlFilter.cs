using AspectCore.DynamicProxy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MyAccess.Filter
{
    /// <summary>
    /// html检测与过滤
    /// </summary>
    public class HtmlFilter
    {
        private static readonly RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;

        // 依次填入上文中三个正则表达式
        private static readonly Regex TAG_REGEX = new Regex(@"<\/?.+?>", REGEX_OPTIONS);
        private static readonly Regex VALID_TAG_REGEX = new Regex(@"^(?<begin></?)(?<tag>[a-zA-z]+)\s*(?<attr>[^>]*?)(?<end>/?>)$", REGEX_OPTIONS);
        private static readonly Regex ATTRIBUTE_REGEX = new Regex(@"(?<name>[a-zA-Z\-]+)\s*=\s*""(?<value>[^""]*)""|(?<name>[a-zA-Z\-]+)", REGEX_OPTIONS);
        private static readonly Regex ATTRIBUTE_SIGN_REGEX = new Regex(@"(?<name>[a-zA-Z\-]+)\s*", REGEX_OPTIONS);
        public HtmlFilter() : this(null) { }

        public HtmlFilter(FilterConfig config)
        {
            this.Config = config ?? new FilterConfig();
        }

        public FilterConfig Config { get; private set; }

        /// <summary>
        /// 获取标签个数
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public int GetTagCount(string tag)
        {
            TagConfig outConfig = null;
            if (Config.TryGetValue(tag, out outConfig))
            {
                return outConfig.Tags.Count;
            }
            return 0;
        }

        /// <summary>
        /// 获取指定标签
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public TagConfig GetTag(string tag)
        {
            TagConfig outConfig = null;
            Config.TryGetValue(tag, out outConfig);
            return outConfig;
        }
        /// <summary>
        /// 标签个数超出将抛出异常
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string FilterHtml(string html)
        {
            foreach (TagConfig tc in this.Config.Values)
            {
                tc.ResetCount();
            }
            // 对每个HTML标记进行替换?
            string rt = TAG_REGEX.Replace(html, GetTag);
            foreach (KeyValuePair<string, TagConfig> kvp in this.Config)
            {
                if (kvp.Value.NeedCloseNum > 0)
                {
                    throw new Exception(string.Format("{0}标签无结束标记", kvp.Key));
                }
            }
            return rt;
        }

        private string GetTag(Match match)
        {
            // 如果不是合法的HTML标记形式，则替换为空字符串
            Match validTagMatch = VALID_TAG_REGEX.Match(match.Value);
            if (!validTagMatch.Success) return "";

            string tag = validTagMatch.Groups["tag"].Value;

            // 如果这个标记不在白名单中，则替换为空字符串
            TagConfig tagConfig;
            if (!this.Config.TryGetValue(tag, out tagConfig)) return "";

            string begin = validTagMatch.Groups["begin"].Value;
            // 如果是闭合标记，则直接构造并返回
            if (begin == "</")
            {
                tagConfig.NeedCloseNum--;
                return String.Format("</{0}>", tag.ToLower());
            }

            string attrText = validTagMatch.Groups["attr"].Value;
            MatchCollection attrMatches = ATTRIBUTE_REGEX.Matches(attrText);

            TagItem item = new TagItem();
            item.Name = tag;
            item.PropName = new List<string>();
            item.PropVal = new List<string>();
            item.PropSign = new List<string>();
            tagConfig.Increase(item);

            if (tagConfig.Tags.Count > tagConfig.MaxCount && tagConfig.MaxCount > 0)
            {
                throw new Exception(string.Format("标签{0}个数不能大于{1}", tag, tagConfig.MaxCount));
            }

            //判断是否过滤属性
            if (tagConfig.LimitProp)
            {
                // 过滤出合法的属性键值对
                List<string> arrlist = new List<string>();
                for (int i = 0; i < attrMatches.Count; i++)
                {
                    string attrstr = GetAttribute(attrMatches[i], tagConfig, item);
                    if (!string.IsNullOrEmpty(attrstr))
                    {
                        arrlist.Add(attrstr);
                    }
                }

                string end = validTagMatch.Groups["end"].Value;
                if (!end.StartsWith("/"))
                {
                    tagConfig.NeedCloseNum++;
                }
                // 如果没有合法的属性，则直接构造返回
                if (arrlist.Count == 0)
                {
                    return begin + tag + end;
                }
                else // 否则返回带属性的HTML标记
                {
                    return String.Format(
                        "{0}{1} {2}{3}",
                        begin,
                        tag,
                        String.Join(" ", arrlist.ToArray()),
                        end);
                }
            }
            else
            {
                for (int i = 0; i < attrMatches.Count; i++)
                {
                    string name = attrMatches[i].Groups["name"].Value;
                    Group group = attrMatches[i].Groups["value"];
                    if (group.Success)
                    {
                        item.PropName.Add(name);
                        item.PropVal.Add(group.Value);

                    }
                    else
                    {
                        item.PropSign.Add(name);
                    }
                }


                string end = validTagMatch.Groups["end"].Value;
                if (!end.StartsWith("/"))
                {
                    tagConfig.NeedCloseNum++;
                }
                return match.Value;
            }


        }

        private string GetAttribute(Match attrMatch, TagConfig tagConfig, TagItem item)
        {
            string name = attrMatch.Groups["name"].Value;

            Regex regex;
            if (!tagConfig.TryGetValue(name, out regex)) return "";

            Group group = attrMatch.Groups["value"];
            if (group.Success)
            {
                string value = group.Value;

                item.PropName.Add(name);
                item.PropVal.Add(value);

                if (regex.IsMatch(value))
                {
                    return String.Format("{0}=\"{1}\"", name, value);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                item.PropSign.Add(name);
                return name;
            }

        }
    }
}

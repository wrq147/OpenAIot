using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace MyAccess.Filter
{
    public class FilterConfig : Dictionary<string, TagConfig>
    {

        public FilterConfig()
            : base(StringComparer.OrdinalIgnoreCase)
        {
            //匹配标签的style,class
            TagConfig allStar = CreateAllStyle();

            TagConfig divTag = new TagConfig();
            this.Add("div", divTag);


            TagConfig pTag = new TagConfig();
            this.Add("p", pTag);

            TagConfig strongTag = new TagConfig(0, true);
            this.Add("strong", strongTag);

            TagConfig brTag = new TagConfig(200, true, false);
            this.Add("br", brTag);

            TagConfig spanTag = new TagConfig();
            this.Add("span", spanTag);

            TagConfig h1Tag = new TagConfig(0, true);
            this.Add("h1", h1Tag);

            TagConfig h2Tag = new TagConfig(0, true);
            this.Add("h2", h2Tag);

            TagConfig h3Tag = new TagConfig(0, true);
            this.Add("h3", h3Tag);

            TagConfig h4Tag = new TagConfig(0, true);
            this.Add("h4", h4Tag);

            TagConfig h5Tag = new TagConfig(0, true);
            this.Add("h5", h5Tag);

            TagConfig olTag = new TagConfig();
            this.Add("ol", olTag);

            TagConfig ulTag = new TagConfig();
            this.Add("ul", ulTag);

            TagConfig liTag = new TagConfig();
            this.Add("li", liTag);

            //匹配表格
            TagConfig tableTag = new TagConfig(20, true);
            tableTag.CopyFrom(allStar);
            this.Add("table", tableTag);

            TagConfig tbodyTag = new TagConfig();
            this.Add("tbody", tbodyTag);

            TagConfig trTag = new TagConfig();
            this.Add("tr", trTag);

            TagConfig tdTag = new TagConfig();
            this.Add("td", tdTag);


            //匹配图片
            TagConfig imgTag = new TagConfig(10);
            this.Add("img", imgTag);
        }
        public static TagConfig CreateAllStyle()
        {
            TagConfig allStar = new TagConfig();
            allStar.Add("style", new Regex(@"^.*$"));
            allStar.Add("id", new Regex(@"^[\w\s]*$"));
            allStar.Add("class", new Regex(@"^[\w\s]*$"));
            allStar.Add("width", new Regex(@"^\d+%|\d+px$"));
            allStar.Add("height", new Regex(@"^\d+%|\d+px$"));
            return allStar;
        }
        /// <summary>
        /// 通过配置文件初始化
        /// </summary>
        /// <param name="root"></param>
        public FilterConfig(XmlElement root)
        {
            XmlNodeList xnl = root.GetElementsByTagName("tag");
            XmlElement wildcardElement = null;
            foreach (XmlElement xe in xnl)
            {
                if (xe.GetAttribute("name").Equals("*"))
                {
                    wildcardElement = xe;
                }
            }

            TagConfig wildcardConfig = wildcardElement == null ? null : new TagConfig(wildcardElement);

            foreach (XmlElement xe in xnl)
            {
                if (!xe.GetAttribute("name").Equals("*"))
                {
                    string name = xe.GetAttribute("name");
                    int maxCount = xe.HasAttribute("max") ? Convert.ToInt32(xe.GetAttribute("max")) : 1;
                    TagConfig tagConfig = new TagConfig(xe);
                    tagConfig.MaxCount = maxCount;

                    foreach (KeyValuePair<string, Regex> pair in wildcardConfig)
                    {
                        if (!tagConfig.ContainsKey(pair.Key))
                        {
                            tagConfig.Add(pair.Key, pair.Value);
                        }
                    }

                    this.Add(name, tagConfig);
                }

            }
        }
    }
}

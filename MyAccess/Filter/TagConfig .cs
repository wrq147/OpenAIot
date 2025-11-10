using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
namespace MyAccess.Filter
{
    public class TagConfig : Dictionary<string, Regex>
    {
     
        private List<TagItem> mTags = new List<TagItem>();
        public List<TagItem> Tags
        {
            get { return mTags; }
        }

        private int mMaxCount;
        public int MaxCount
        {
            get { return mMaxCount; }
            set { mMaxCount = value; }
        }
        private int mNeedCloseNum;
        /// <summary>
        /// 需要闭合的标签数
        /// </summary>
        public int NeedCloseNum
        {
            get
            {
                if (mNeedClose)
                {
                    return mNeedCloseNum;
                }
                else
                {
                    return 0;
                }
            }
            set { mNeedCloseNum = value; }
        }
        /// <summary>
        /// 重置计数
        /// </summary>
        public void ResetCount()
        {
            mTags.Clear();
        }
        /// <summary>
        /// 累加
        /// </summary>
        public void Increase(TagItem item)
        {
            mTags.Add(item);
        }
       
        private bool mNeedClose;
        private bool mLimitProp;
        public bool LimitProp
        {
            get { return mLimitProp; }
        }
        public TagConfig(int max = 0, bool limitProp = false, bool needclose = true)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            mMaxCount = max;
            mLimitProp = limitProp;
            mNeedClose = needclose;
        }

        public TagConfig(XmlElement tagNode)
        {
            XmlNodeList xnlist = tagNode.GetElementsByTagName("attr");
            foreach (XmlElement ele in xnlist)
            {
                this.Add(ele.GetAttribute("name"), new Regex(ele.Value));
            }
        }
        public void CopyFrom(TagConfig tag)
        {
            foreach (KeyValuePair<string, Regex> pair in tag)
            {
                this.Add(pair.Key, pair.Value);
            }
        }
    }

    public class TagItem
    {
        public string Name { get; set; }
        public List<string> PropName { get; set; }
        public List<string> PropVal { get; set; }
        public List<string> PropSign { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace TemplateAction.Label.StructLib
{
    public class ItemTree
    {
        public char Item { get; set; }
        public bool IsEnd { get; set; }
        public Dictionary<char, ItemTree> Child { get; set; }
    }
}

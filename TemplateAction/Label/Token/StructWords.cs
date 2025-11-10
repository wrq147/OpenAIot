using System;
using System.Collections.Generic;
using TemplateAction.Label.StructLib;

namespace TemplateAction.Label.Token
{
    public class StructWords
    {
        private ItemTreeLibrary _lib;
        static StructWords() { }
        private StructWords()
        {
            List<string> words = new List<string>();
            words.Add(StructToken.BEGIN_FUN);
            words.Add(StructToken.SECTION_L);
            words.Add(StructToken.SECTION_R);
            words.Add(StructToken.FUN_PARAM_L);
            words.Add(StructToken.FUN_PARAM_R);
            words.Add(StructToken.FUN_PARAM_M);
            words.Add(StructToken.BEGIN_START);
            words.Add(StructToken.END_START);
            words.Add(StructToken.END_FINISH);
            words.Add(StructToken.LESS);
            words.Add(StructToken.END_START_FINISH);
            words.Add(StructToken.EQUAL);
            words.Add(StructToken.ADD);
            words.Add(StructToken.SUB);
            words.Add(StructToken.MUL);
            words.Add(StructToken.DIV);
            words.Add(StructToken.MOD);
            words.Add(StructToken.NE);
            words.Add(StructToken.OR);
            words.Add(StructToken.AND);
            words.Add(StructToken.LEQ);
            words.Add(StructToken.GEQ);
            words.Add(StructToken.NEQ);
            words.Add(StructToken.EEQ);
            words.Add(StructToken.LM);
            words.Add(StructToken.RM);
            words.Add(StructToken.LINK);
            words.Add(StructToken.Q);
            words.Add(StructToken.COLON);
            _lib = new ItemTreeLibrary(words);
        }
        private static readonly StructWords _instance = new StructWords();
        public static StructWords Instance
        {
            get { return _instance; }
        }
        public bool IsStruct(char c)
        {
            return _lib.Library.ContainsKey(c);
        }
        public bool TryGetValue(char key, out ItemTree value)
        {
            return _lib.Library.TryGetValue(key, out value);
        }
    }
}

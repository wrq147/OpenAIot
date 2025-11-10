using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.KuaiDi
{
    public static class KuaiDiPreRead
    {
        private static List<string> documentlist = new List<string>();
        private static List<string> documentnames = new List<string>();
        private static Dictionary<string, string> companyDict = new Dictionary<string, string>();
        private static List<KuaiDiCompany> kuaiDiList = new List<KuaiDiCompany>();
        public class KuaiDiCompany
        {
            public string Name { get; set; }
            public string Code { get; set; }
        }
        public static void LoadData(string dir)
        {
            documentlist.Clear();
            documentnames.Clear();
            companyDict.Clear();
            kuaiDiList.Clear();
            DirectoryInfo TheFolder = new DirectoryInfo(dir);
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {

                //文件流读取
                System.IO.FileStream fs = new System.IO.FileStream(NextFile.FullName, System.IO.FileMode.Open);
                System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.UTF8);
                if (NextFile.Name == "company.txt")
                {
                    string tempText = "";
                    while ((tempText = sr.ReadLine()) != null)
                    {
                        string[] items = tempText.Split(':');
                        if (items.Length == 2)
                        {
                            companyDict.Add(items[0].Trim(), items[1].Trim());
                        }
                    }
                }
                else
                {
                    string document = string.Empty;
                    string tempText = "";
                    while ((tempText = sr.ReadLine()) != null)
                    {
                        document = document + " " + tempText;
                    }
                    documentlist.Add(document.Trim().ToLower());
                    documentnames.Add(Path.GetFileNameWithoutExtension(NextFile.FullName));
                }

                sr.Close();
                fs.Close();
            }

            companyDict.Add("", "其它快递");
            foreach (var kvp in companyDict)
            {
                kuaiDiList.Add(new KuaiDiCompany()
                {
                    Name = kvp.Value,
                    Code = kvp.Key
                });
            }
        }
        public static string GetName(string companycode)
        {
            return companyDict[companycode];
        }
        public static List<KuaiDiCompany> GetCompanyList()
        {
            return kuaiDiList;
        }
        public static string Simi(string input)
        {
            if (input.Length < 8) { return string.Empty; }
            input = input.ToLower();
            var copylist = documentlist.Select(item => (string)item.Clone()).ToList();
            copylist.Insert(0, input);
            var measure = new TFIDFMeasure(copylist.ToArray());
            int curIdx = -1;
            float curfloat = 0;
            for (int i = 1; i < copylist.Count; i++)
            {
                var rs = measure.GetSimilarity(0, i);
                if (curIdx == -1)
                {
                    curfloat = rs;
                    curIdx = i;
                }
                else
                {
                    if (curfloat < rs)
                    {
                        curfloat = rs;
                        curIdx = i;
                    }
                }
            }
            if (curIdx == -1)
            {
                return string.Empty;
            }
            if (curfloat < 0.02)
            {
                return string.Empty;
            }
            return documentnames[curIdx - 1];
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace ConfigGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string evn = args[0].Trim('\"');
            string projFold = args[1].Trim('\"');

            var templatePath = Directory.GetFiles(projFold, "appsettings.config.tpl", SearchOption.AllDirectories)[0];

            string templete = File.ReadAllText(templatePath);

            var re = new Regex(@"(?<tag>{\$.*})");

            MatchCollection mcs = re.Matches(templete);
            Dictionary<string, string> tags = new Dictionary<string, string>();
            foreach (Match mc in mcs)
            {
                var tag_key = mc.Groups["tag"].Value;
                tag_key = tag_key.Substring(2, tag_key.Length - 3);
                tags[tag_key] = string.Empty;
            }

            string profilePath = Directory.GetFiles(projFold, "ConfigProfile.xml", SearchOption.TopDirectoryOnly)[0];
            string profile = File.ReadAllText(profilePath);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(profile);
            var profileNode = xmlDoc.SelectSingleNode("profile");
            var evnNode = profileNode.SelectSingleNode(evn);

            var keys = tags.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var tagNode = evnNode.SelectSingleNode(keys[i]);
                if (tagNode != null)
                {
                    tags[key] = tagNode.InnerText;
                }
            }

            foreach (var tag in tags)
            {
                templete = templete.Replace("{$" + tag.Key + "}", tag.Value);
            }

            using (FileStream fs = new FileStream(templatePath.Substring(0, templatePath.Length - 4), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    //开始写入
                    sw.Write(templete);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
            }
        }
    }
}
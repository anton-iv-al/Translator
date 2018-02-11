using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YamlDotNet.RepresentationModel;

namespace TranslateClient2 {
    public static class YamlHelper {
        public static dynamic ConfigFromFile(string path) {
            var yaml = new YamlStream();

            try {
                var reader = new StringReader(File.ReadAllText("Translator/YandexTranslatorConfig.yaml"));
                yaml.Load(reader);
            }
            catch (Exception e) {
                MessageBox.Show("Config reading error.\n\n" + e.Message, "Error");
            }

            return (yaml.Documents[0].RootNode as YamlMappingNode).Children;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace Jbz.Models
{
    public class Round
    {
        public Round()
        {
            _categories = new List<CategoryModel>() { new CategoryModel(), new CategoryModel(), new CategoryModel(), new CategoryModel(), new CategoryModel()};
        }

        public void Load(int round)
        {
            using (var reader = GetReader(round))
            {
                var cat = 0;
                
                while (true)
                {
                    var data = reader.ReadLine();
                    if (data==null || data.Contains("</Roound"))
                        break;

                    if (data.Contains("<Category"))
                    {
                        _categories[cat].Category = _catRegex.Match(data).Groups["Name"].Value;
                        LoadQuestions(reader, cat);
                        cat++;
                    }
                }
            }
        }

        private void LoadQuestions(StreamReader reader, int cat)
        {
            int round = 0;
            while(true)
            {
                var question = reader.ReadLine();
                if (question == null || question.Contains("</Category"))
                    break;

                var data = reader.ReadLine();
                var quest = _questRegex.Match(data).Groups["Quest"].Value;
                data = reader.ReadLine();
                var answer = _answerRegex.Match(data).Groups["Answer"].Value;
                data = reader.ReadLine();
                var path = _pathRegex.Match(data).Groups["Path"].Value;
                data = reader.ReadLine();
                var isBonus = _bonusRegex.Match(data).Groups["Bonus"].Value.Equals("1");

                switch (round)
                {
                    case 0:
                        _categories[cat].Points1Active = true;
                        _categories[cat].Points1 = _questionRegex.Match(question).Groups["Points"].Value;
                        break;
                    case 1:
                        _categories[cat].Points2Active = true;
                        _categories[cat].Points2 = _questionRegex.Match(question).Groups["Points"].Value;
                        break;
                    case 2:
                        _categories[cat].Points3Active = true;
                        _categories[cat].Points3 = _questionRegex.Match(question).Groups["Points"].Value;
                        break;
                    case 3:
                        _categories[cat].Points4Active = true;
                        _categories[cat].Points4 = _questionRegex.Match(question).Groups["Points"].Value;
                        break;
                    case 4:
                        _categories[cat].Points5Active = true;
                        _categories[cat].Points5 = _questionRegex.Match(question).Groups["Points"].Value;
                        break;
                }

                switch (_questionRegex.Match(question).Groups["Type"].Value)
                {
                    case "Text":
                        _questions.Add(new KeyValuePair<int, int>(cat, round), new TextQuestion() { Quest = quest, Answer = answer, IsBonus = isBonus});
                        break;
                    case "Picture":
                        _questions.Add(new KeyValuePair<int, int>(cat, round), new PictureQuestion() { Quest = quest, Answer = answer, Path = path, IsBonus = isBonus });
                        break;
                    case "Sound":
                        _questions.Add(new KeyValuePair<int, int>(cat, round), new SoundQuestion() { Quest = quest, Answer = answer, Path = path, IsBonus = isBonus });
                        break;
                    case "Video":
                        _questions.Add(new KeyValuePair<int, int>(cat, round), new VideoQuestion() { Quest = quest, Answer = answer, Path = path, IsBonus = isBonus });
                        break;
                }
                reader.ReadLine();
                round++;
            }
        }

        private StreamReader GetReader(int round)
        {
            var filePath = Path.Combine(@"Data", string.Format("R{0}.xml", round));
            return new StreamReader(filePath);
        }

        private Dictionary<KeyValuePair<int, int>, Question> _questions = new Dictionary<KeyValuePair<int, int>, Question>();

        private List<CategoryModel> _categories = new List<CategoryModel>();

        public List<CategoryModel> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        public Dictionary<KeyValuePair<int, int>, Question> Questions
        {
            get { return _questions; }
            set { _questions = value; }
        }

        private Regex _catRegex = new Regex(@"^.*Name=""(?<Name>(.*))"".*$", RegexOptions.Singleline |RegexOptions.Compiled);
        private Regex _questionRegex = new Regex(@"^.*Points=""(?<Points>(.*))"".*Type=""(?<Type>(.*))"".*$", RegexOptions.Singleline | RegexOptions.Compiled);
        private Regex _questRegex = new Regex(@"^.*<Quest>(?<Quest>.*)</Quest>.*$", RegexOptions.Singleline | RegexOptions.Compiled);
        private Regex _answerRegex = new Regex(@"^.*<Answer>(?<Answer>.*)</Answer>.*$", RegexOptions.Singleline | RegexOptions.Compiled);
        private Regex _pathRegex = new Regex(@"^.*<Path>(?<Path>.*)</Path>.*$", RegexOptions.Singleline | RegexOptions.Compiled);
        private Regex _bonusRegex = new Regex(@"^.*<Bonus>(?<Bonus>.*)</Bonus>.*$", RegexOptions.Singleline | RegexOptions.Compiled);
    }
}

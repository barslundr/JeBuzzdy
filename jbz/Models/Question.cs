using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Jbz.Models
{
    public abstract class Question
    {
        private string _quest;
        private string _answer;

        public string Quest
        {
            get { return _quest; }
            set { _quest = value; }
        }

        public string Answer
        {
            get { return _answer; }
            set { _answer = value; }
        }

        public bool IsBonus { get; set; }
    }

    public class TextQuestion : Question
    {
        
    }

    public class PictureQuestion : Question
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }

    public class SoundQuestion : Question
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }

    public class VideoQuestion : Question
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}

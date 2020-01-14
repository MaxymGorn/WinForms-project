using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    [Serializable]
    public class ListQueAnsw
    {
        public ListQueAnsw()
        {

        }
        public List<QuetionAnswer> quetionAnswers { get; set; } = new List<QuetionAnswer>();
    }
    [Serializable]
    public class QuetionAnswer
    {
        public string Quetion { get; set; }

        public List<string> Answers { get; set; } = new List<string>();
        public int otvet_int { get; set; }
        public QuetionAnswer()
        {

        }
    }
}

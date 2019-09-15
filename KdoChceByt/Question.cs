using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdoChceByt
{
    class Question
    {
        public string Text { get; }
        public string [] Answers { get; }
        public int[] FiftyFiftyIndexes { get; }
        public List<int> RightAnswers{ get; }

        public Question (string [] lines)
        {
            Text = lines[0];
            Answers = lines[1].Split(';');
            FiftyFiftyIndexes = new int[2];
            FiftyFiftyIndexes[0] = Int32.Parse(lines[2].Split()[0]);
            FiftyFiftyIndexes[1] = Int32.Parse(lines[2].Split()[1]);
            RightAnswers = lines[3].Split().ToList().Select(s => int.Parse(s)).ToList();

        }
        public bool isRight(int i)
        {
            return RightAnswers.Contains(i);
        }
    }
}

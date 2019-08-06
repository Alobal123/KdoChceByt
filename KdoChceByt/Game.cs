using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdoChceByt
{
    class Game
    {
        private List<Question> Questions = new List<Question>();
        private int QuestionIndex = 0;
        private int Score = 1;

        public Game (string Path)
        {
            System.IO.StreamReader file =
               new System.IO.StreamReader(Path, Encoding.GetEncoding("windows-1250"));
            
            string[] lines = new string[4];
            while (true)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = file.ReadLine();
                }
                if (lines[lines.Length-1] == null)
                    break;
                Questions.Add(new Question(lines));
            }
            file.Close();

        }
        public Question GetQuestion()
        {
            try
            {
                return Questions[QuestionIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public string getTextScore()
        {
            if (Score == 1)
                return "Garantujeme 1 šťastný rok";
            else if (Score <= 4)
            {
                return "Garantujeme " + Score + " šťastné roky";
            }
            else
                return "Garantujeme " + Score + " šťastných let";

        }

        public void NextQuestion()
        {
            QuestionIndex++;
        }
        public void raiseScore(bool wasRight)
        {
            if (wasRight)
            {
                Score *= 2;
            }
        }

    }
}

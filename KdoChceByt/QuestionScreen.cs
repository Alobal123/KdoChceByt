using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KdoChceByt
{
    public partial class QuestionScreen : Form
    {
        private Game game;
        private bool blick = true;
        private List<AnswerButton> answerButtons = new List<AnswerButton>();

        internal QuestionScreen(Game game)
        {
            this.game = game;
            
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
            this.BackColor = Color.FromArgb(32, 14, 88);


            ScoreLabel.Text = game.getTextScore();
            Question currentQuestion = game.GetQuestion();
            for (int i = 0; i < currentQuestion.Answers.Length; i++)
            {
                AnswerButton answerButton = new AnswerButton(this);
                QuestionPanel.Controls.Add(answerButton);
                answerButtons.Add(answerButton);    
            }
    

            ShowQuestion(true);
        }


        internal void ShowQuestion(bool reset)
        {
            Question currentQuestion = game.GetQuestion();
            timer1.Stop();
            if (currentQuestion != null)
            {
                QuestionLabel.Text = currentQuestion.Text;
                for (int i = 0; i < currentQuestion.Answers.Length; i++)
                {
                    AnswerButton button = answerButtons[i];
                    if (button.state != AnswerButtonState.Deactivated || reset)
                        button.Reset(currentQuestion.Answers[i], currentQuestion.isRight(i));
                }
            }
            else
            {
                MessageBox.Show("The End!");
                this.Dispose();
            }
        }

        internal void NextQuestion(bool wasRight)
        {
            game.NextQuestion(wasRight);
            ScoreLabel.Text = game.getTextScore();
        }

        private void QuestionPanel_Resize(object sender, EventArgs e)
        {
            int x = QuestionPanel.Width / 2 - QuestionLabel.Width / 2;
            int y = this.Height - QuestionPanel.Height - QuestionLabel.Height*2;
            QuestionLabel.Location = new Point(x, y);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Question q = game.GetQuestion();
            ShowQuestion(true);
            for (int i = 0; i < answerButtons.Count; i++)
            {
                if (q.FiftyFiftyIndexes[0] == i || q.FiftyFiftyIndexes[1] == i)
                {
                    answerButtons[i].state = AnswerButtonState.Deactivated;
                    answerButtons[i].Enabled = false;
                    answerButtons[i].Text = "";
                }
            }

            ((Control)sender).Dispose();
        }

        public void AnswerClick(object sender, EventArgs e)
        {
            AnswerButton button = (AnswerButton)sender;
            switch (button.state)
            {
                case AnswerButtonState.Default:
                    ShowQuestion(false);
                    button.BackColor = Color.Orange;
                    button.state = AnswerButtonState.Selected;
                    break;
                case AnswerButtonState.Selected:
                    if (button.isRight)
                    {
                        button.state = AnswerButtonState.Right;
                        timer1.Start();
                    }
                    else
                    {
                        button.state = AnswerButtonState.Wrong;
                        timer1.Start();
                    }
                    NextQuestion(button.isRight);

                    break;
                case AnswerButtonState.Right:
                    
                    ShowQuestion(true);
                    break;
                case AnswerButtonState.Wrong:
                    
                    ShowQuestion(true);
                    break;
                case AnswerButtonState.Deactivated:
                    break;
                default:
                    break;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ((Control)sender).Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            for (int i = 0; i < answerButtons.Count; i++)
            {
                AnswerButton button = answerButtons[i];
                if (blick)
                {
                    button.BackColor = button.defaultColor;
                }
                else
                {
                    if (button.state == AnswerButtonState.Right || button.isRight)
                        button.BackColor = Color.Green;
                    if (button.state == AnswerButtonState.Wrong)
                        button.BackColor = Color.Red;
                }
                    
                
            }
            blick = !blick;
        }
    }
}

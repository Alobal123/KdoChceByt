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
        private static string[] letters = {"A: ", "B: ", "C: ", "D: " };
        private ScreenState state;
        private Player player = new Player();
  

        public enum ScreenState
        {
            start,
            questionDisplay,
            playing,
            ending,
        }

        internal QuestionScreen(Game game)
        {
            this.state = ScreenState.start;
            this.game = game;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
            this.BackColor = Color.FromArgb(32, 14, 88);
            ScoreLabel.Text = game.getTextScore();   
        }

        private void Reset()
        {
            foreach (AnswerButton butt in answerButtons)
            {
                QuestionPanel.Controls.Remove(butt);
                butt.Dispose();
            }
            QuestionLabel.Text = "";
            this.state = ScreenState.start;
            this.answerButtons = new List<AnswerButton>();
        }

        internal void ShowQuestion(bool reset)
        {
            Question currentQuestion = game.GetQuestion();
            timer1.Stop();
            if (currentQuestion != null)
            {
                for (int i = 0; i < answerButtons.Count; i++)
                {
                    AnswerButton button = answerButtons[i];
                    if (button.state != AnswerButtonState.Deactivated || reset)
                        button.Reset(letters[i] + currentQuestion.Answers[i], currentQuestion.isRight(i));
                }
            }
            else
            {
                MessageBox.Show("The End!");
                this.Dispose();
            }
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
            if (state == ScreenState.playing) { 
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
                            player.Play(@"sounds/correct.mp3");
                            button.state = AnswerButtonState.Right;
                            timer1.Start();
                            

                        }
                        else
                        {
                            
                            player.Play(@"sounds/wrong.mp3");
                            button.state = AnswerButtonState.Wrong;
                            timer1.Start();
                        }
                        game.raiseScore(button.isRight);
                        this.state = ScreenState.ending;
                        ScoreLabel.Text = game.getTextScore();
                        break;
                    case AnswerButtonState.Right:
                        break;
                    case AnswerButtonState.Wrong:
                        break;
                    case AnswerButtonState.Deactivated:
                        break;
                    default:
                        break;
                }
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

        private void QuestionScreen_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode != Keys.Space)
                return;
            else
            {
                
                switch (state)
                {
                    case ScreenState.start:
                        player.Play(@"sounds/theme.mp3");
                        this.state = ScreenState.questionDisplay;
                        Question currentQuestion = game.GetQuestion();
                        QuestionLabel.Text = currentQuestion.Text;
                        QuestionPanel_Resize(null, null);
                        break;
                    case ScreenState.questionDisplay:
                        if (answerButtons.Count < 4)
                        {
                            AnswerButton answerButton = new AnswerButton(this);
                            QuestionPanel.Controls.Add(answerButton);
                            answerButtons.Add(answerButton);
                            answerButton.KeyDown += new KeyEventHandler(QuestionScreen_KeyDown);
                            ShowQuestion(false);
                        }
                        if (answerButtons.Count == 4)
                        {
                            this.state = ScreenState.playing;
                        }
                        break;
                    case ScreenState.playing:
                        break;
                    case ScreenState.ending:
                        game.NextQuestion();
                        Reset();
                        this.Focus();
                        break;
                    default:
                        break;
                }
            }
        }


    }
    public class Player
    {
        WMPLib.WindowsMediaPlayer  wp = new WMPLib.WindowsMediaPlayer();


        public void Play(string url)
        {
            wp.controls.stop();
            wp.URL = url;
            wp.controls.play();
        }
    }
}

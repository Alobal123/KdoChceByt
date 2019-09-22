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
            pause,
            ending,
            epilog
            
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
      
            logo.Width = ClientRectangle.Width;
            logo.Height = ClientRectangle.Height;
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
            ((Control)sender).Visible=false;
        }

        public void AnswerClick(object sender, EventArgs e)
        {
            if (state == ScreenState.playing) { 
            AnswerButton button = (AnswerButton)sender;
                switch (button.state)
                {
                    case AnswerButtonState.Default:
                        player.Play(@"sounds/theme.mp3");
                        ShowQuestion(false);
                        button.BackColor = Color.Orange;
                        button.state = AnswerButtonState.Selected;
                        
                        break;
                    case AnswerButtonState.Selected:
                        this.state = ScreenState.ending;
                        if (button.isRight)
                        {
                            player.Play(@"sounds/correct.mp3");
                            button.state = AnswerButtonState.Right;
                            timer1.Start();
                            game.raiseScore(button.isRight);
                            ScoreLabel.Text = game.getTextScore();
                            game.NextQuestion();
                            if (game.QuestionIndex == 6)
                            {
                                pictureBox1.Visible = true;
                                pictureBox2.Visible = true;
                                pictureBox3.Visible = true;
                            }
;
                        }
                        else
                        {
                            if (game.QuestionIndex >= 6)
                                this.state = ScreenState.epilog;
                            else
                            {
                                pictureBox1.Visible = true;
                                pictureBox2.Visible = true;
                                pictureBox3.Visible = true;
                                game.QuestionIndex = 6;
                            }

                            player.Play(@"sounds/wrong.mp3");
                            button.state = AnswerButtonState.Wrong;
                            timer1.Start();
                        }
                        
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

            ((Control)sender).Visible = false;
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
                        this.tableLayoutPanel1.Visible = true;
                        this.ScoreLabel.Visible = true;
                        this.state = ScreenState.questionDisplay;

                        player.Play(@"sounds/letsplay.mp3");
                        logo.Visible = false;
                        Question currentQuestion = game.GetQuestion();
                        if (currentQuestion == null)
                        {
                            this.state = ScreenState.epilog;
                            QuestionScreen_KeyDown(sender, e);
                            return;
                        }
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
                        this.tableLayoutPanel1.Visible = false;
                        this.ScoreLabel.Visible = false;
                        this.logo.Visible = true;
                        this.state = ScreenState.pause;
                        break;
                    case ScreenState.pause:
                        Reset();
                        this.Focus();
                        break;
                    case ScreenState.epilog:
                        Reset();
                        this.state = ScreenState.epilog;
                        this.Focus();
                        this.tableLayoutPanel1.Visible = false;
                        ScoreLabel.Anchor = AnchorStyles.None;
                        ScoreLabel.Location= new Point(100,400);
                        ScoreLabel.Font = new Font(ScoreLabel.Font.FontFamily, 50, ScoreLabel.Font.Style);
                        ScoreLabel.Text = "Gratulujeme! " + game.getTextScore() + "!";
                        ScoreLabel.TextAlign = ContentAlignment.MiddleCenter;
                        break;
                    default:
                        break;
                }
            }
        }


    }
    public class Player
    {
        WMPLib.WindowsMediaPlayer wp = new WMPLib.WindowsMediaPlayer();
        public void Play(string url)
        {
            wp.URL = url;
            wp.controls.play();
        }
        public void Stop()
        {
            wp.controls.stop();
        }
    }
}

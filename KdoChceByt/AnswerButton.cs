using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KdoChceByt
{
    public enum AnswerButtonState
    {
        Default,
        Selected,
        Right,
        Wrong,  
        Deactivated
    }

    public partial class AnswerButton : Button
    {
        public AnswerButtonState state = AnswerButtonState.Default;
        public Color defaultColor = Color.FromArgb(140, 32, 14, 88);
        public bool isRight = false;

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath buttonPath =
                new System.Drawing.Drawing2D.GraphicsPath();

            // Set a new rectangle to the same size as the button's 
            // ClientRectangle property.
            Rectangle newRectangle = ClientRectangle;

            // Decrease the size of the rectangle.
            newRectangle.Inflate(-10, -10);

            // Draw the button's border.
            e.Graphics.DrawEllipse(System.Drawing.Pens.Black, newRectangle);

            // Increase the size of the rectangle to include the border.
            newRectangle.Inflate(1, 1);

            // Create a circle within the new rectangle.
            buttonPath.AddEllipse(newRectangle);

            // Set the button's Region property to the newly created 
            // circle region.
            Region = new System.Drawing.Region(buttonPath);
            base.OnPaint(e);
        }


        public AnswerButton(QuestionScreen qs)
        {
            this.Dock = DockStyle.Fill;
            this.ForeColor = Color.White;
            this.Font = new Font("Arial", 60 );
            Click += new EventHandler(qs.AnswerClick);
            
        }
        public void Reset(string text, bool isRight)
        {           
            this.state = AnswerButtonState.Default;
            this.BackColor = defaultColor;
            this.Text = text;
            this.isRight = isRight;
            this.Enabled = true;

        }
 
    }
    
    
}

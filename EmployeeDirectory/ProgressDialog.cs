using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DobsonUtilities
{
    public partial class ProgressDialog : Form
    {
        // Disable X in corner of window
        const int MF_BYPOSITION = 0x400;
        [DllImport("User32")]
        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("User32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("User32")]
        private static extern int GetMenuItemCount(IntPtr hWnd);

        // Constructors
        public ProgressDialog()
        {
            InitializeComponent();

            // Hide minimize, maximize, and close button on title bar
            //this.ControlBox = false;
            dialogProgressBar.Minimum = 0;
            dialogProgressBar.Maximum = 100;
            DialogText = "Gathering info...";
        }

        public ProgressDialog(string DialogText = "Gathering info...")
        {
            InitializeComponent();

            //this.ControlBox = false;
            dialogProgressBar.Minimum = 0;
            dialogProgressBar.Maximum = 100;
            this.DialogText = DialogText;
        }

        public ProgressDialog(string DialogText = "Gathering info...", int Minimum = 0, int Maximum = 100)
        {
            InitializeComponent();

            //this.ControlBox = false;
            dialogProgressBar.Minimum = Minimum;
            dialogProgressBar.Maximum = Maximum;
            this.DialogText = DialogText;
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            // Disable X in corner of window
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            int menuItemCount = GetMenuItemCount(hMenu);
            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
        }

        // Getters and Setters
        public int Minimum
        {
            get
            {
                return dialogProgressBar.Minimum;
            }
            set
            {
                dialogProgressBar.Minimum = value;
            }
        }

        public int Maximum
        {
            get
            {
                return dialogProgressBar.Maximum;
            }
            set
            {
                dialogProgressBar.Maximum = value;
            }
        }

        public int Step
        {
            get
            {
                return dialogProgressBar.Step;
            }
            set
            {
                dialogProgressBar.Step = value;
            }
        }

        public string DialogText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = DialogText;
            }
        }

        // Functions
        public void PerformStep()
        {
            dialogProgressBar.PerformStep();
            //this.Text = DialogText + " " + dialogProgressBar.Value + "%";
            label.Text = dialogProgressBar.Value + "%";
            //this.Text = string.Format("{0} {1}", DialogText, dialogProgressBar.Value);
            //label.Text = string.Format("{0}%", dialogProgressBar.Value);
        }

        /// <summary>
        /// Percent finished method
        /// </summary>
        /// <param name="Get">Gets the current percent completed value in the progress bar</param>
        /// <param name="Set">Sets the current percent completed value in the progress bar</param>
        public int PercentFinished
        {
            get
            {
                return dialogProgressBar.Value;
            }
            set
            {
                dialogProgressBar.Value = value;
                //this.Text = DialogText + " " + dialogProgressBar.Value + "%";
                label.Text = dialogProgressBar.Value + "%";
                //this.Text = string.Format("{0} {1}%", DialogText, dialogProgressBar.Value);
                //label.Text = string.Format("{0}%", dialogProgressBar.Value);
            }
        }

        /// <summary>
        /// Adds text into a System.Windows.Forms.ProgressBar
        /// </summary>
        /// <param name="Target">The target progress bar to add text into</param>
        /// <param name="Text">The text to add into the progress bar. 
        /// Leave null or empty to automatically add the percent.</param>
        /// <param name="Location">Where the text is to be placed</param>
        /// <param name="TextColor">The color the text should be drawn in</param>
        /// <param name="TextFont">The font the text should be drawn in</param>
        private void SetProgressBarText
            (
            System.Windows.Forms.ProgressBar Target, //The target progress bar
            string Text, //The text to show in the progress bar
            ProgressBarTextLocation Location, //Where the text is to be placed
            System.Drawing.Color TextColor, //The color the text is to be drawn in
            System.Drawing.Font TextFont //The font we use to draw the text
            )
        {

            //Make sure we didn't get a null progress bar
            if (Target == null) { throw new ArgumentException("Null Target"); }

            //Now we can get to the real code

            //Check to see if we are to add in the percent
            if (string.IsNullOrEmpty(Text))
            {
                //We are to add in the percent meaning we got a null or empty Text
                //We give text a string value representing the percent
                int percent = (int)(((double)(Target.Value - Target.Minimum) /
                    (double)(Target.Maximum - Target.Minimum)) * 100);
                Text = percent.ToString() + "%";
            }

            //Now we can add in the text

            //gr will be the graphics object we use to draw on Target
            using (Graphics gr = Target.CreateGraphics())
            {
                gr.DrawString(Text,
                    TextFont, //The font we will draw it it (TextFont)
                    new SolidBrush(TextColor), //The brush we will use to draw it

                    //Where we will draw it
                    new PointF(
                        // X location (Center or Left)
                        Location == ProgressBarTextLocation.Left ?
                        5 : //Left side
                        dialogProgressBar.Width / 2 - (gr.MeasureString(Text, //Centered
                        TextFont).Width / 2.0F),
                    // Y Location (This is the same regardless of Location)
                    dialogProgressBar.Height / 2 - (gr.MeasureString(Text,
                        TextFont).Height / 2.0F)));
            }
        }

        public enum ProgressBarTextLocation
        {
            Left,
            Centered
        }
    }
}
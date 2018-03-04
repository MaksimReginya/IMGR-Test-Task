using System;
using System.Drawing;
using System.Windows.Forms;

namespace UIWinForms
{
    public partial class MainForm : Form
    {
        private const string ImagePath = @"document.tif";
        private const int Eps = 2;
        private readonly double _coefficient;
        private readonly Graphics _graphics;
        private readonly Pen _rectanglePen = new Pen(Color.Red, 1);
        private readonly WordFinder _wordFinder;

        public MainForm()
        {
            InitializeComponent();

            pbImage.Image = Image.FromFile(ImagePath);
            _graphics = pbImage.CreateGraphics();
            _wordFinder = new WordFinder(this);
            _coefficient = (double) pbImage.Image.Height / pbImage.Height;
        }

        internal void HighlightWord(HtmlParser.HtmlParser.Word word)
        {
            int x1 = (int)(word.LeftUp.X / _coefficient);
            int y1 = (int)(word.LeftUp.Y / _coefficient);
            int x2 = (int)(word.RightBottom.X / _coefficient);
            int y2 = (int)(word.RightBottom.Y / _coefficient);
            var hoverRectangle = new Rectangle(x1, y1, x2 - x1 + Eps, y2 - y1 + Eps);
            _graphics.DrawRectangle(_rectanglePen, hoverRectangle);
        }

        private bool TextBoxIsValid()
        {
            if (string.IsNullOrWhiteSpace(this.tbSearchPhrase.Text))
            {
                MessageBox.Show(@"Please enter a search phrase!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (!TextBoxIsValid())
            {
                return;
            }

            this.pbImage.Refresh();
            _wordFinder.FindEntries(this.tbSearchPhrase.Text);
        }
    }
}

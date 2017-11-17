namespace TelevisionShowGame
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        private Competitor competitor1;
        private Competitor competitor2;
        private Competitor competitor3;
        private Competitor competitor4;
        private List<Competitor> competitors;
        private int initialY;
        private Competitor answeringPlayer;

        public Form1()
        {
            InitializeComponent();

            competitor1 = new Competitor();
            competitor2 = new Competitor();
            competitor3 = new Competitor();
            competitor4 = new Competitor();
            answeringPlayer = new Competitor();

            competitors = new List<Competitor> { competitor1, competitor2, competitor3, competitor4 };

            initialY = this.picBoxPlayer1.Location.Y;

            InitializedButtons(false, true);
        }

        public void btnNewRound_Click(object sender, EventArgs e)
        {
            if (CheckIfEmptyInput())
            {
                MessageBox.Show("Fill all the details!", "Error!");
                return;
            }

            int startingCredits = int.Parse(this.txtStartingCredits.Text);
            int creditsToWin = int.Parse(this.txtWinningCredits.Text);

            for (int i = 0; i < competitors.Count; i++)
            {
                var currPlayer = GetInfo()[i];
                competitors[i].initialise(currPlayer.Item1, startingCredits, creditsToWin, currPlayer.Item2, currPlayer.Item3);
            }

            var noDuplicates = this.competitors.Select(x => x.Name).Distinct();

            if (noDuplicates.Count() < 4)
            {
                MessageBox.Show("There cannot be duplicate names!", "Error!");
                return;
            }

            ///--------------------SETTING IMAGES----------------------------///
            this.picBoxPlayer1.ImageLocation = "images/sashka.jpg";
            this.picBoxPlayer1.SizeMode = PictureBoxSizeMode.StretchImage;

            this.picBoxPlayer2.ImageLocation = "images/dumbo.jpg";
            this.picBoxPlayer2.SizeMode = PictureBoxSizeMode.StretchImage;

            this.picBoxPlayer3.ImageLocation = "images/dumbi.jpg";
            this.picBoxPlayer3.SizeMode = PictureBoxSizeMode.StretchImage;

            this.picBoxPlayer4.ImageLocation = "images/stashko.jpg";
            this.picBoxPlayer4.SizeMode = PictureBoxSizeMode.StretchImage;

            for (int i = 0; i < competitors.Count; i++)
            {
                competitors[i].showNameAndCreditsInLabel();
                competitors[i].imageTakeStartingPosition(initialY);
            }

            InitializedButtons(true, true);
        }
        
        public void PlayerAnswering(object sender, EventArgs e)
        {
            var player = (Button)sender;
            string answeringPlayer = "";

            if (player.Text == "Player1")
            {
                answeringPlayer = $"{competitors[0].Name}";
                this.answeringPlayer = competitor1;
            }
            else if (player.Text == "Player2")
            {
                answeringPlayer = $"{competitors[1].Name}";
                this.answeringPlayer = competitor2;
            }
            else if (player.Text == "Player3")
            {
                answeringPlayer = $"{competitors[2].Name}";
                this.answeringPlayer = competitor3;
            }
            else if (player.Text == "Player4")
            {
                answeringPlayer = $"{competitors[3].Name}";
                this.answeringPlayer = competitor4;
            }

            this.lblAnsweringCompetitor.Text = answeringPlayer + " is answering...";
        }

        public void btnAnswerWrong_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < competitors.Count; i++)
            {
                if (answeringPlayer.Name == competitors[i].Name)
                {
                    if (competitors[i].isLoser())
                    {
                        competitors[i].showAsLoser();
                    }
                    else
                    {
                        competitors[i].decrementCredits();
                    }
                }
            }
        }

        public void btnAnswerCorrect_Click(object sender, EventArgs e)
        {
            this.btnMeUp.Visible = true;
            this.btnOthersDown.Visible = true;
        }

        public void btnMeUp_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < competitors.Count; i++)
            {
                if (answeringPlayer.Name == competitors[i].Name)
                {
                    competitors[i].incrementCredits();
                }

                if (competitors[i].isWinner())
                {
                    MessageBox.Show($"The winner is: {competitors[i].Name}");
                    RestartGame(false);
                }
            }

            HideButtons();
        }

        public void btnOthersDown_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < competitors.Count; i++)
            {
                if (answeringPlayer.Name != competitors[i].Name)
                {
                    if (competitors[i].isLoser())
                    {
                        competitors[i].showAsLoser();
                    }
                    else
                    {
                        competitors[i].decrementCredits();
                    }
                }
            }

            HideButtons();
        }

        private void HideButtons()
        {
            this.btnMeUp.Visible = false;
            this.btnOthersDown.Visible = false;
        }

        private void RestartGame(bool reset)
        {
            InitializedButtons(false, reset);

            for (int i = 0; i < competitors.Count; i++)
            {
                competitors[i].imageTakeStartingPosition(this.initialY);
            }
        }

        private void InitializedButtons(bool enabled, bool startNewRound)
        {
            var controls = Controls.Cast<Control>();

            Controls.OfType<GroupBox>()
                        .SelectMany(c => c.Controls.OfType<Button>())
                        .ToList()
                        .ForEach(x => x.Enabled = enabled);

            foreach (var control in controls)
            {
                if (control.GetType() == typeof(TextBox) && !startNewRound)
                {
                    ((TextBox)control).Text = "";
                }

                if (control.GetType() == typeof(Button) && 
                    ((Button)control).Name != "btnNewRound" &&
                    ((Button)control).Name != "btnFillDetails")
                {
                    ((Button)control).Enabled = enabled;
                }
            }
        }

        private void ImportNamesFromFile()
        {
            var filePath = "playersAndPoints.txt";
            var counter = 0;
            var textBoxes = new List<TextBox>
            {
                this.txtPlayer1Name,
                this.txtPlayer2Name,
                this.txtPlayer3Name,
                this.txtPlayer4Name,
                this.txtStartingCredits,
                this.txtWinningCredits
            };

            using (var fileStream = File.OpenRead(filePath))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    String line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        textBoxes[counter].Text = line;
                        counter++;
                    }
                }
            }
        }

        private void btnFillDetails_Click(object sender, EventArgs e)
        {
            ImportNamesFromFile();
        }

        private bool CheckIfEmptyInput()
        {
            var controls = Controls.Cast<Control>();

            foreach (var control in controls)
            {
                if (control.GetType() == typeof(TextBox) && control.Text == "")
                {
                    return true;
                }
            }

            return false;
        }

        private Dictionary<int, Tuple<string, Label, PictureBox>> GetInfo()
        {
            var result = new Dictionary<int, Tuple<string, Label, PictureBox>>()
            {
                {0, new Tuple<string, Label, PictureBox>(this.txtPlayer1Name.Text, this.lblCreditsPlayer1, this.picBoxPlayer1)},
                {1, new Tuple<string, Label, PictureBox>(this.txtPlayer2Name.Text, this.lblCreditsPlayer2, this.picBoxPlayer2)},
                {2, new Tuple<string, Label, PictureBox>(this.txtPlayer3Name.Text, this.lblCreditsPlayer3, this.picBoxPlayer3)},
                {3, new Tuple<string, Label, PictureBox>(this.txtPlayer4Name.Text, this.lblCreditsPlayer4, this.picBoxPlayer4)}
            };

            return result;
        }
    }
}


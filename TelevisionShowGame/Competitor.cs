namespace TelevisionShowGame
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class Competitor
    {
        private String name; 
        private int nrOfCredits;  
        private int creditsToWin;  
        private Label myLabel; 
        private PictureBox myPb;  

        public String Name { get { return this.name; } set { this.name = value; } }

        public void initialise(String name, int startingCredits,
                int creditsToWin, Label lbl, PictureBox pb)
        {
            this.name = name;
            this.nrOfCredits = startingCredits;
            this.creditsToWin = creditsToWin;
            this.myLabel = lbl;
            this.myPb = pb;
        }
		
        public void incrementCredits()
        {
            this.nrOfCredits++;
            this.showNameAndCreditsInLabel();
            this.moveImageUp5Pixel();
        }

        public void decrementCredits()
        {
            this.nrOfCredits--;
            this.showNameAndCreditsInLabel();
            this.moveImageDown5Pixel();
        }

        public bool isWinner()
        {
            return this.nrOfCredits >= this.creditsToWin;
        }

        public bool isLoser()
        {
            return this.nrOfCredits == 1;
        }
		
        private void moveImageUp5Pixel() 
        {
            Point p = this.myPb.Location;
            p.Y-=5;
            this.myPb.Location = p;

        }

        private void moveImageDown5Pixel() 
        {
            Point p = this.myPb.Location;
            p.Y+=5;
            this.myPb.Location = p;
        }

        public void showNameAndCreditsInLabel()
        {
            this.myLabel.Text = this.name + " - " +
                this.nrOfCredits.ToString() + " cts";
        }

        public void showAsLoser()
        {
            this.myPb.Visible = false;
            this.myLabel.Text = this.name + " has LOST";
        }

        public void imageTakeStartingPosition(int initialY)
        {
            Point p = this.myPb.Location;
            p.Y = initialY;
            this.myPb.Location = p;
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace ALP_UNO_Game
{
    public partial class Game : Form
    {
        public Game()
        {
            InitializeComponent();
            _soundPlayer = new SoundPlayer("UNO mix.wav");
        }

        #region Game Code

        #region Game Classes

        public class Card
        {
            public string cardName { get; set; }
            public string cardColor { get; set; }
            public string cardValue { get; set; }
            public int score { get; set; }
        }

        public class Player
        {
            public List<Card> playerCards = new List<Card>();
        }

        #endregion

        // Game variabel
        public Card theCard;
        public int playerNum = 0;
        public int playDirection = 1;
        public bool gameOver = false;
        public List<Player> playerList { get; set; }
        public List<Card> cardDeck { get; set; }

        public void generateStartingDeck()
        {
            cardDeck = new List<Card>();

            string[] colors = new string[] { "red", "yellow", "green", "blue" };
            string[] numValues = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string[] nonNumberValues = new string[] { "skip", "reverse", "drawTwo" };
            string[] wildValues = new string[] { "wildCard", "drawFour" };

            foreach (var color in colors)
            {
                foreach (var value in numValues)
                {
                    if (value != "0")
                        cardDeck.Add(new Card()
                        {
                            cardName = $"{color}_{value}",
                            cardColor = color,
                            cardValue = value,
                            score = Convert.ToInt32(value)
                        });

                    cardDeck.Add(new Card()
                    {
                        cardName = $"{color}_{value}",
                        cardColor = color,
                        cardValue = value,
                        score = Convert.ToInt32(value)
                    });;
                }

                foreach (var value in nonNumberValues)
                {
                    cardDeck.Add(new Card()
                    {
                        cardName = $"{color}_{value}",
                        cardColor = color,
                        cardValue = value,
                        score = 20
                    });

                    cardDeck.Add(new Card()
                    {
                        cardName = $"{color}_{value}",
                        cardColor = color,
                        cardValue = value,
                        score = 20
                    });
                }

                foreach (var value in wildValues)
                {
                    cardDeck.Add(new Card()
                    {
                        cardName = $"wild_{value}",
                        cardColor = "wild",
                        cardValue = value,
                        score = 50
                    });
                }
            }
        }

        public void shuffleDeck()
        {
            Random rand = new Random();
            cardDeck = cardDeck.OrderBy(x => rand.Next()).ToList();
            cardDeck = cardDeck.OrderBy(x => rand.Next()).ToList();
            cardDeck = cardDeck.OrderBy(x => rand.Next()).ToList();
            cardDeck = cardDeck.OrderBy(x => rand.Next()).ToList();
        }

        public void drawPlayerCard(int playerCount, int cardCount)
        {
            playerList = new List<Player>();
            for (int i = 0; i < playerCount; i++)
            {
                playerList.Add(new Player());
                for (int j = 0; j < cardCount; j++)
                {
                    playerList[i].playerCards.Add(cardDeck[j]);
                    cardDeck.RemoveAt(j);
                }
            }
        }

        public void sortPlayerCard()
        {
            playerList[playerNum].playerCards.Sort((x, y) => String.Compare(x.cardValue, y.cardValue));
            playerList[playerNum].playerCards.Sort((x, y) => String.Compare(x.cardColor, y.cardColor));
        }

        public Card getCard()
        {
            int temp = 0;
            Card card;
            while (cardDeck[temp].cardColor == "wild" || cardDeck[temp].cardValue == "reverse" || cardDeck[temp].cardValue == "skip" || cardDeck[temp].cardValue == "drawTwo")
                temp++;

            card = cardDeck[temp];
            cardDeck.RemoveAt(temp);
            return card;
        }

        public bool playerCannotPlay(List<Card> playerCard, Card theCard)
        {
            foreach (var card in playerCard)
                if (card.cardColor == "wild" || card.cardColor == theCard.cardColor || card.cardValue == theCard.cardValue)
                    return false; 
            return true;
        }

        public void drawCard(List<Card> playerCard, int cardCount)
        {
            for (int i = 0; i < cardCount; i++)
            {
                playerCard.Add(cardDeck[0]);
                cardDeck.RemoveAt(0);
            }
        }

        public void playGame()
        {
            MessageBox.Show($"Player {playerNum + 1}'s Turn");
            sortPlayerCard();
            showPlayerCard();
            showEnemyCard();

            if (playerCannotPlay(playerList[playerNum].playerCards, theCard))
            {
                if (cardDeck.Count() == 0)
                {
                    gameOver = true;
                    // >> Gameover Panel
                } 
                else
                {
                    MessageBox.Show($"Player {playerNum + 1} doesn't have a playable card, Draw 1 card !");
                    drawCard(playerList[playerNum].playerCards, 1);
                    // Kalo kartu diambil bisa dimainin bisa di mainkan.
                    sortPlayerCard();
                    updateCard(theCard);
                    endTurn();
                }
            }
        }

        public void StartGame(int playerCount, int cardCount)
        {
            generateStartingDeck();
            shuffleDeck();
            drawPlayerCard(playerCount, cardCount);

            theCard = getCard();
            updateCard(theCard);

            playGame();
        }

        public bool checkGameStatus()
        {
            if (playerList.Count() == 2)
            {
                return true;
            }
            else return gameOver;
        }

        public void endTurn()
        {
            gameOver = checkGameStatus();
            if (gameOver)
            {

            }
            else if (theCard.cardValue == "skip")
            {
                playerNum++;
            }
            else if (theCard.cardValue == "reverse")
            {
                playDirection *= -1;
            }
            else if (theCard.cardValue == "drawTwo")
            {
                drawCard(playerList[(playerNum + playDirection) % playerList.Count()].playerCards, 2);
                MessageBox.Show($"Player {(playerNum + playDirection) % playerList.Count() + 1} : Draw Two Cards, Skip turn !");
                showPlayerCard();
                showEnemyCard();
                updateCard(theCard);
                playerNum++;
            }
            else if (theCard.cardValue == "drawFour")
            {
                drawCard(playerList[(playerNum + playDirection) % playerList.Count()].playerCards, 4);
                MessageBox.Show($"Player {(playerNum + playDirection) % playerList.Count() + 1} : Draw Four Cards, Skip turn !");
                showPlayerCard();
                showEnemyCard();
                updateCard(theCard);
                playerNum++;
            }
            
            if (theCard.cardColor == "wild")
            {
                Form1 form = new Form1();
                form.ShowDialog();
                string WarnaPilih = Form1.Warna;

                if (WarnaPilih == "red")
                {
                    theCard.cardColor = "red";
                }
                else if(WarnaPilih == "blue")
                {
                    theCard.cardColor = "blue";
                }
                else if (WarnaPilih == "yellow")
                {
                    theCard.cardColor = "yellow";
                }
                else if (WarnaPilih == "green")
                {
                    theCard.cardColor = "green";
                }

            }

            playerNum = (playerNum + playDirection) % playerList.Count();
            playGame();
            
        }

        #endregion

        private void resetButtonSetting()
        {
            this.btn_StartButton.Size = new System.Drawing.Size(this.ClientSize.Width / 3, this.ClientSize.Height / 10);
            this.btn_StartButton.Location = new System.Drawing.Point(this.ClientSize.Width * 3 / 4 - this.btn_StartButton.Width / 2, this.ClientSize.Height / 2 + this.btn_StartButton.Height);
            this.btn_RulesButton.Size = new System.Drawing.Size(this.ClientSize.Width / 3, this.ClientSize.Height / 10);
            this.btn_RulesButton.Location = new System.Drawing.Point(this.ClientSize.Width * 3 / 4 - this.btn_StartButton.Width / 2, this.ClientSize.Height / 2 + this.btn_StartButton.Height * 2);
            this.btn_CreditsButton.Size = new System.Drawing.Size(this.ClientSize.Width / 3, this.ClientSize.Height / 10);
            this.btn_CreditsButton.Location = new System.Drawing.Point(this.ClientSize.Width * 3 / 4 - this.btn_StartButton.Width / 2, this.ClientSize.Height / 2 + this.btn_StartButton.Height * 3);
        }

        private void Game_Load(object sender, EventArgs e)
        {
            this.ClientSize = new System.Drawing.Size(800, 450);

            // Main menu panel
            this.pnl_MainMenu.Height = this.ClientSize.Height;
            this.pic_MenuPic.Width = this.ClientSize.Width / 2;
            this.pic_UnoPicture.Height = this.ClientSize.Height / 2;
            resetCardSetting();

            // Game panel
            this.pnl_GamePanel.Height = this.ClientSize.Height;
            this.pnl_PlayerCard.Height = this.ClientSize.Height / 3;
            this.pnl_EnemyCard1.Height = this.ClientSize.Height / 4;
            this.pic_TheCard.Location = new System.Drawing.Point((this.ClientSize.Width + this.ClientSize.Width / 10) / 2, (this.ClientSize.Height - this.ClientSize.Height / 3) / 2);
            this.pic_TheCard.Size = new System.Drawing.Size(this.ClientSize.Width / 10, this.ClientSize.Height / 4);
            this.pic_CardDeck.Location = new System.Drawing.Point((this.ClientSize.Width - this.ClientSize.Width / 4) / 2, (this.ClientSize.Height - this.ClientSize.Height / 3) / 2);
            this.pic_CardDeck.Size = new System.Drawing.Size(this.ClientSize.Width / 10, this.ClientSize.Height / 4);
            this.lbl_CardDeckCount.Location = new System.Drawing.Point((this.ClientSize.Width - this.ClientSize.Width / 4) / 2 + (this.ClientSize.Width / 60), (this.ClientSize.Height - this.ClientSize.Height / 3) / 2 + (this.ClientSize.Height / 4));
            this.lbl_CardDeckCount.Font = new System.Drawing.Font("Microsoft Sans Serif", (float)(this.ClientSize.Width / 100), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        private void Game_SizeChanged(object sender, EventArgs e)
        {
            if(pnl_MainMenu.Visible == true)
            {
                this.pnl_MainMenu.Height = this.ClientSize.Height;
                this.pic_MenuPic.Width = this.ClientSize.Width / 2;
                this.pic_UnoPicture.Height = this.ClientSize.Height / 2;
                resetButtonSetting();
            }
            else if (pnl_GamePanel.Visible == true)
            {
                this.pnl_GamePanel.Height = this.ClientSize.Height;
                this.pnl_PlayerCard.Height = this.ClientSize.Height / 3;
                this.pnl_EnemyCard1.Height = this.ClientSize.Height / 4;
                this.pic_TheCard.Location = new System.Drawing.Point((this.ClientSize.Width + this.ClientSize.Width / 10) / 2, (this.ClientSize.Height - this.ClientSize.Height / 3) / 2);
                this.pic_TheCard.Size = new System.Drawing.Size(this.ClientSize.Width / 10, this.ClientSize.Height / 4);
                this.pic_CardDeck.Location = new System.Drawing.Point((this.ClientSize.Width - this.ClientSize.Width / 4) / 2, (this.ClientSize.Height - this.ClientSize.Height / 3) / 2);
                this.pic_CardDeck.Size = new System.Drawing.Size(this.ClientSize.Width / 10, this.ClientSize.Height / 4);
                this.lbl_CardDeckCount.Location = new System.Drawing.Point((this.ClientSize.Width - this.ClientSize.Width / 4) / 2 + (this.ClientSize.Width / 50), (this.ClientSize.Height - this.ClientSize.Height / 3) / 2 + (this.ClientSize.Height / 4));
                this.lbl_CardDeckCount.Font = new System.Drawing.Font("Microsoft Sans Serif", (float)(this.ClientSize.Width / 100), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                resetCardSetting();
            }
        }

        #region Main Menu Panel

        private void btn_StartButton_MouseHover(object sender, EventArgs e)
        {
            btn_StartButton.Left -= 2;
            btn_StartButton.Top -= 2;
            btn_StartButton.Size = new System.Drawing.Size(this.btn_StartButton.Width + 4, this.btn_StartButton.Height + 4);
        }

        private void btn_StartButton_Click(object sender, EventArgs e)
        {
            pnl_MainMenu.Visible = false;
            // Choose player panel here
            int playerCount = 2;
            int cardCount = 7;
            //
            pnl_GamePanel.Visible = true;
            StartGame(playerCount, cardCount);
        }

        private void btn_StartButton_MouseLeave(object sender, EventArgs e)
        {
            resetButtonSetting();
        }

        #endregion

        #region Game Panel

        public void resetCardSetting()
        {
            for (int i = 0; i < pictureBoxes.Count(); i++)
            {
                pictureBoxes[i].Location = new Point((this.ClientSize.Width - (this.ClientSize.Width / 10)) / 2 + ((pictureBoxes.Count() / 2) - i) * this.ClientSize.Width / 20, 20);
                pictureBoxes[i].Size = new System.Drawing.Size(this.ClientSize.Width / 10, this.ClientSize.Height / 4);
            }

            for (int i = 0; i < pictureBoxes2.Count(); i++)
            {
                pictureBoxes2[i].Location = new Point((this.ClientSize.Width - (this.ClientSize.Width / 16)) / 2 + ((playerList[playerNum].playerCards.Count() / 2) - i) * this.ClientSize.Width / 50, 0);
                pictureBoxes2[i].Size = new System.Drawing.Size(this.ClientSize.Width / 20, this.ClientSize.Height / 8);
            }
        }

        private void picture_MouseHover(object sender, EventArgs e)
        {
            int index = pictureBoxes.IndexOf((PictureBox)sender);
            for (int i = 0; i < index; i++)
            {
                pictureBoxes[i].Left += (this.ClientSize.Width / 40);
            }
            for (int i = index + 1; i < pictureBoxes.Count(); i++)
            {
                pictureBoxes[i].Left -= (this.ClientSize.Width / 40);
            }
            pictureBoxes[index].Top -= 20;
        }

        private void picture_MouseLeave(object sender, EventArgs e)
        {
            resetCardSetting();
        }

        private void picture_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            var cardAttribute = pictureBox.Name.Split('_');
            if (cardAttribute[0] == "wild" || cardAttribute[0] == theCard.cardColor || cardAttribute[1] == theCard.cardValue)
            {
                for (int i = 0; i < playerList[playerNum].playerCards.Count(); i++)
                    if (playerList[playerNum].playerCards[i].cardName == pictureBox.Name)
                        {
                            theCard = playerList[playerNum].playerCards[i];
                            updateCard(theCard);

                            playerList[playerNum].playerCards.RemoveAt(i);
                            sortPlayerCard();
                            showPlayerCard();
                        }
                endTurn();
            }
        }

        public List<PictureBox> pictureBoxes = new List<PictureBox>();

        public void showPlayerCard()
        {
            for (int i = 0; i < pictureBoxes.Count(); i++)
                this.pnl_PlayerCard.Controls.Remove(pictureBoxes[i]);

            pictureBoxes.Clear();

            for (int i = 0; i < playerList[playerNum].playerCards.Count(); i++)
            {
                PictureBox picture = new PictureBox()
                {
                    Name = playerList[playerNum].playerCards[i].cardName,
                    Size = new System.Drawing.Size(this.ClientSize.Width / 10, this.ClientSize.Height / 4),
                    Location = new Point((this.ClientSize.Width - (this.ClientSize.Width / 10)) / 2 + ((playerList[playerNum].playerCards.Count() / 2) - i) * this.ClientSize.Width / 20, 20),
                    Image = (Image)Properties.Resources.ResourceManager.GetObject(playerList[playerNum].playerCards[i].cardName),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                picture.MouseHover += new System.EventHandler(this.picture_MouseHover);
                picture.MouseLeave += new System.EventHandler(this.picture_MouseLeave);
                picture.Click += new System.EventHandler(this.picture_Click);
                pictureBoxes.Add(picture);
                this.pnl_PlayerCard.Controls.Add(pictureBoxes[i]);
            }
        }

        public List<PictureBox> pictureBoxes2 = new List<PictureBox>();

        public void showEnemyCard()
        {
            if(playerList.Count() == 2)
            {
                for (int i = 0; i < pictureBoxes2.Count(); i++)
                    this.pnl_EnemyCard1.Controls.Remove(pictureBoxes2[i]);

                pictureBoxes2.Clear();

                var enemyIndex = (playerNum + 1) % playerList.Count();
                for (int i = 0; i < playerList[enemyIndex].playerCards.Count(); i++)
                {
                    PictureBox picture = new PictureBox()
                    {
                        Name = playerList[enemyIndex].playerCards[i].cardName,
                        Size = new System.Drawing.Size(this.ClientSize.Width / 20, this.ClientSize.Height / 8),
                        Location = new Point((this.ClientSize.Width - (this.ClientSize.Width / 16)) / 2 + ((playerList[playerNum].playerCards.Count() / 2) - i) * this.ClientSize.Width / 50, 0),
                        Image = Properties.Resources.unoCard,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    pictureBoxes2.Add(picture);
                    pnl_EnemyCard1.Controls.Add(pictureBoxes2[i]);
                }
            }
        }

        public void updateCard(Card theCard)
        {
            pic_TheCard.Image = (Image)Properties.Resources.ResourceManager.GetObject(theCard.cardName);
            lbl_CardDeckCount.Text = $"Card : {cardDeck.Count()}";
        }

        #endregion

        private SoundPlayer _soundPlayer;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
            if(PlayButton.Checked)
            {
                _soundPlayer.Play();
            }
            else 
            {
                _soundPlayer.Stop();
            }
        }

        private void pic_MenuPic_Click(object sender, EventArgs e)
        {

        }


    }
}


using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WheelOfFortuneBoard
{
    public class WheelOfFortuneBoard : UserControl
    {
        public PictureBox WheelImage;
        public Panel WheelPanel;
        public Panel BoardPanel;
        public Panel AlphabetPanel;
        public Panel ControlPanel;

        public ComboBox CmbPlayers;
        public TextBox SpinValue;
        public Button Bankrupt;
        public Button LoadNextPuzzle;
        public Button SolvePuzzle;
        public Label LblScores;
        public Label p1Name, p2Name, p3Name;
        public TextBox p1Score, p2Score, p3Score;
        public TextBox solveGuess;

        public event EventHandler<Button> AlphabetButtonClicked;

        private const int TileWidth = 86;
        private const int TileHeight = 114;
        private const int TileSpacing = 10;

        private const int LetterWidth = 20;
        private const int LetterHeight = 30;
        private const int LetterSpacing = 10;

        public WheelOfFortuneBoard()
        {
            InitializeBoardComponent();
        }

        private void InitializeBoardComponent()
        {
            //this.WheelImage = new PictureBox();
            this.WheelPanel = new Panel();
            this.BoardPanel = new Panel();
            this.AlphabetPanel = new Panel();
            this.ControlPanel = new Panel();

            // WheelPanel
            this.WheelPanel.BackColor = Color.Transparent;

            // BoardPanel
            this.BoardPanel.BackColor = Color.Transparent;

            // AlphabetPanel
            this.AlphabetPanel.BackColor = Color.Transparent;

            // ControlPanel
            this.ControlPanel.BackColor = Color.Transparent;

            // Initialize Controls inside ControlPanel
            InitializeControlPanelControls();

            // Add controls to ControlPanel
            this.ControlPanel.Controls.Add(this.CmbPlayers);
            this.ControlPanel.Controls.Add(this.SpinValue);
            this.ControlPanel.Controls.Add(this.Bankrupt);
            this.ControlPanel.Controls.Add(this.LoadNextPuzzle);
            this.ControlPanel.Controls.Add(this.SolvePuzzle);
            this.ControlPanel.Controls.Add(this.LblScores);
            this.ControlPanel.Controls.Add(this.p1Name);
            this.ControlPanel.Controls.Add(this.p2Name);
            this.ControlPanel.Controls.Add(this.p3Name);
            this.ControlPanel.Controls.Add(this.p1Score);
            this.ControlPanel.Controls.Add(this.p2Score);
            this.ControlPanel.Controls.Add(this.p3Score);
            this.ControlPanel.Controls.Add(this.solveGuess);

            // Add AlphabetButtons to Alphabet Panel
            InitializeAlphabetButtons();

            // Add each panel to UserControl
            this.Controls.Add(this.WheelPanel);
            this.Controls.Add(this.BoardPanel);
            this.Controls.Add(this.AlphabetPanel);
            this.Controls.Add(this.ControlPanel);
        }

        private void LoadImageIntoWheelPanel(string imagePath)
        {
            // Create PictureBox
            this.WheelImage = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                //BackColor = Color.Transparent
        };

            // Load the Image
            if (File.Exists(imagePath))
            {
                this.WheelImage.Image = Image.FromFile(imagePath);
            }
            else
            {
                MessageBox.Show($"Image not found: {imagePath}");
            }

            // Add PictureBox to WheelPanel
            this.WheelPanel.Controls.Add(this.WheelImage);
        }

        public void InitializeGameBoard()
        {
            this.BoardPanel.Controls.Clear();
            CreateBoardRow(1, 12, 0);
            CreateBoardRow(2, 14, TileHeight + TileSpacing);
            CreateBoardRow(3, 14, 2 * (TileHeight + TileSpacing));
            CreateBoardRow(4, 12, 3 * (TileHeight + TileSpacing));
        }

        private void CreateBoardRow(int boardRow, int tileCount, int yOffset)
        {
            int totalWidth = tileCount * (TileWidth + TileSpacing) - TileSpacing;
            int startX = (BoardPanel.Width - totalWidth) / 2;

            for (int i = 0; i < tileCount; i++)
            {
                Button tile = new Button
                {
                    Size = new Size(TileWidth, TileHeight),
                    Location = new Point(startX + i * (TileWidth + TileSpacing), yOffset),
                    BackColor = Color.MediumSeaGreen,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Enabled = false,
                    Name = "Row" + boardRow.ToString() + "Tile" + (i+1).ToString()
                };
                this.BoardPanel.Controls.Add(tile);
            }
        }

        public void InitializeAlphabetButtons()
        {
            this.AlphabetPanel.Controls.Clear();
            int alphabetLength = 26;
            string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                               "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            int totalWidth = alphabetLength * (LetterWidth + LetterSpacing) - LetterSpacing;
            int startX = (AlphabetPanel.Width - totalWidth) / 2;

            for (int i = 0; i < alphabetLength; i++)
            {
                Button letter = new Button
                {
                    Size = new Size(LetterWidth, LetterHeight),
                    Location = new Point(startX + i * (LetterWidth + LetterSpacing), 0),
                    BackColor = Color.DarkGray,
                    FlatStyle = FlatStyle.Flat,
                    Enabled = true,
                    Visible = true,
                    Text = alphabet[i]
                };

                letter.Font = new Font(letter.Font.Name, letter.Font.Size, FontStyle.Bold);
                letter.Click += AlphabetButton_Click;

                this.AlphabetPanel.Controls.Add(letter);
            }
        }

        public void InitializeControlPanelControls()
        {
            this.p1Name = new Label
            {
                Size = new Size(100, 30),
                Font = new Font("Arial", 20, FontStyle.Bold)
            };
            this.p2Name = new Label
            {
                Size = new Size(100, 30),
                Font = new Font("Arial", 20, FontStyle.Bold)
            };
            this.p3Name = new Label
            {
                Size = new Size(100, 30),
                Font = new Font("Arial", 20, FontStyle.Bold)
            };

            this.p1Score = new TextBox
            {
                Size = new Size(280, 100),
                BackColor = Color.Red,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Text = "0",
                TextAlign = HorizontalAlignment.Center,
                ForeColor = Color.White
            };

            this.p2Score = new TextBox
            {
                Size = new Size(280, 100),
                BackColor = Color.Yellow,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Text = "0",
                TextAlign = HorizontalAlignment.Center,
                ForeColor = Color.Black
            };

            this.p3Score = new TextBox
            {
                Size = new Size(280, 100),
                BackColor = Color.Blue,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Text = "0",
                TextAlign = HorizontalAlignment.Center,
                ForeColor = Color.White
            };
            int textBoxScorePadding = 210;

            p1Name.Left = 400;
            p1Name.Top = 20;

            p1Score.Left = 300;
            p1Score.Top = p1Name.Bottom + 10;

            p2Name.Left = p1Score.Right + textBoxScorePadding + 100;
            p2Name.Top = 20;

            p2Score.Left = p1Score.Right + textBoxScorePadding;
            p2Score.Top = p2Name.Bottom + 10;

            p3Name.Left = p2Score.Right + textBoxScorePadding + 100;
            p3Name.Top = 20;

            p3Score.Left = p2Score.Right + textBoxScorePadding;
            p3Score.Top = p3Name.Bottom + 10;

            this.CmbPlayers = new ComboBox 
            { 
                Size = new Size(150, 30),
                Top = p1Score.Bottom + 50
            };

            this.SpinValue = new TextBox
            {
                Size = new Size(150, 30),
                Top = p1Score.Bottom + 50,
                Left = CmbPlayers.Right + 10
            };

            this.Bankrupt = new Button
            {
                Text = "BANKRUPT",
                Size = new Size(100, 30),
                Top = p1Score.Bottom + 45,
                Left = SpinValue.Right + 10
            };

            this.solveGuess = new TextBox
            {
                Size = new Size(310, 30),
                Top = CmbPlayers.Bottom + 10
            };

            this.SolvePuzzle = new Button
            {
                Text = "Solve Puzzle",
                Size = new Size(100, 30),
                Top = Bankrupt.Bottom + 3,
                Left = solveGuess.Right + 10
            };

            this.LoadNextPuzzle = new Button
            {
                Text = "Load Next Puzzle",
                Size = new Size(100, 30),
                Top = p1Score.Bottom + 78,
                Left = SolvePuzzle.Right + 10
            };

            // Event Subscription is fundamentally different from Property Initialization
            // For this reason, we cannot add this Event Subscription when creating the
            // SpinValue Textbox.
            this.SpinValue.KeyPress += SpinValue_KeyPress;
        }

        private void AlphabetButton_Click(object sender, EventArgs e)
        {
            if (sender is Button clickedButton)
            {
                AlphabetButtonClicked?.Invoke(this, clickedButton);
            }
        }

        public void AdjustLayout(int width, int height)
        {
            int padding = 20;

            // Adjust WheelPanel
            this.WheelPanel.Location = new Point(padding, 0);
            this.WheelPanel.Size = new Size(width - 2 * padding, height / 6);

            // WheelImage
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gold_wof_smaller.png");
            LoadImageIntoWheelPanel(imagePath);

            // Adjust BoardPanel
            this.BoardPanel.Location = new Point(padding, this.WheelPanel.Bottom + padding);
            this.BoardPanel.Size = new Size(width - 2 * padding, height / 2);

            // Position AlphabetPanel below BoardPanel
            this.AlphabetPanel.Location = new Point(padding, this.BoardPanel.Bottom + padding);
            this.AlphabetPanel.Size = new Size(width - 2 * padding, height / 30);

            // Position ControlPanel below AlphabetPanel
            this.ControlPanel.Location = new Point(padding, this.AlphabetPanel.Bottom + padding);
            this.ControlPanel.Size = new Size(width - 2 * padding, height / 4);

            // Ensure tiles fit within the BoardPanel
            InitializeGameBoard();
            InitializeAlphabetButtons();
        }

        private void SpinValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        public string GetSelectedPlayer
        {
            get
            {
                return CmbPlayers.SelectedItem?.ToString(); // Returns the selected player's name as a string
            }
        }

        public string GetSpinValue
        {
            get
            {
                return SpinValue.Text.ToString(); // Returns the monetary amount stored in the textbox as a string
            }
        }
    }
}

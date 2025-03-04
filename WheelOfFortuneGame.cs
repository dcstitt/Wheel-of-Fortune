using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media; // For SoundPlayer

namespace WheelOfFortuneBoard
{
    public partial class WheelOfFortuneGame : Form
    {
        private List<string> players;
        private List<Puzzle> Puzzles;
        private Dictionary<string, int> scores;
        private int vowelCost = 1000;
        private int currentPuzzleID = 0;
        private WheelOfFortuneBoard board;

        public WheelOfFortuneGame()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            this.board = new WheelOfFortuneBoard();
            this.SuspendLayout();

            // Configure the board control
            this.board.Dock = DockStyle.Fill; // Allow the board to fill the form
            this.Controls.Add(this.board);

            // WheelOfFortuneGame
            this.WindowState = FormWindowState.Maximized; // Make the form fullscreen
            this.Text = "Wheel of Fortune Game";
            this.BackColor = Color.LightBlue;
            this.Resize += Form_Resize; // Handle resizing dynamically
            this.ResumeLayout(false);

            // Wire up events
            this.board.LoadNextPuzzle.Click += new EventHandler(this.LoadNextPuzzleButton_Click);
            this.board.SolvePuzzle.Click += new EventHandler(this.SolvePuzzleButton_Click);
            this.board.Bankrupt.Click += new EventHandler(this.BankruptButton_Click);

            // Subscribe to the WheelOfFortuneBoard's AlphabetButtonClicked event
            this.board.AlphabetButtonClicked += WheelOfFortuneBoard_AlphabetButtonClicked;

            PlayStartupAudio();

            this.Focus();
        }

        private void PlayStartupAudio()
        {
            try
            {
                // Path to the audio file in the program's directory
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\WoF_Friends.wav");

                // Play the WAV file
                using (SoundPlayer player = new SoundPlayer(filePath))
                {
                    player.Play(); // Use PlaySync() for synchronous playback
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing audio: {ex.Message}");
            }
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            // Adjust layout on form resize
            board.AdjustLayout(this.ClientSize.Width, this.ClientSize.Height);
        }

        private void InitializeGame()
        {
            // Player names are simply read from a file for those who 
            // wish to only run the executable
            players = LoadPlayerNames();
            board.p1Name.Text = players[0];
            board.p2Name.Text = players[1];
            board.p3Name.Text = players[2];
            
            scores = players.ToDictionary(player => player, player => 0);
            board.CmbPlayers.DataSource = new List<string>(players);
            LoadAllPuzzlesFromFile();
        }

        private List<string> LoadPlayerNames()
        {
            players = new List<string> { };
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "playerNames.txt");
            if (File.Exists(filePath))
            {
                // Read in each line from the puzzle and store it in its own slot in an array
                string[] playerNames = File.ReadAllLines(filePath);

                foreach (string name in playerNames)
                {
                    players.Add(name);
                }
            }
            else
            {
                MessageBox.Show("Player Names file not found.");
            }

            return players;
        }

        private void LoadAllPuzzlesFromFile()
        {
            int puzzleCounter = 0;
            Puzzles = new List<Puzzle> { };
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "puzzles.txt");
            if (File.Exists(filePath))
            {
                // Read in each line from the puzzle and store it in its own slot in an array
                string[] puzzleLine = File.ReadAllLines(filePath);

                foreach (string phrase in puzzleLine)
                {
                    int numWordsInPhrase = phrase.Split(' ').Count();
                    string[] individualWordsInPhrase = phrase.Split(' ');

                    Puzzle puzzle = new Puzzle
                    {
                        fullPhrase = phrase,
                        id = puzzleCounter,
                        numberOfWords = numWordsInPhrase,
                        individualWords = individualWordsInPhrase,
                        phraseLength = phrase.Length
                    };
                    puzzleCounter++;

                    Puzzles.Add(puzzle);
                }
            }
            else
            {
                MessageBox.Show("Puzzle file not found.");
            }
        }

        private void LoadNextPuzzleButton_Click(object sender, EventArgs e)
        {
            Puzzle puzzleToDisplay = new Puzzle();

            if (currentPuzzleID < Puzzles.Count)
                puzzleToDisplay = Puzzles[currentPuzzleID];
            else
                return;

            // Reset Board
            this.board.InitializeGameBoard();
            this.board.InitializeAlphabetButtons();

            bool onFirstWord = true;
            int remainingSpacesInRow = 12-2; // Start with Row 1 which has 12 spaces minus the 2 end tiles
            int currentTile = 1;
            int currentRow = 1;
            int tileStoppedOn = 0;
            
            foreach (string word in puzzleToDisplay.individualWords)
            {
                // Determine if word will fit on current line
                if (word.Length < remainingSpacesInRow)
                {
                    int index = 0;

                    // Loop through each tile on each row
                    foreach (Button tile in this.board.BoardPanel.Controls)
                    {
                        // Skip over any tile that doesn't contain our current row in its name
                        if (!tile.Name.Contains("Row" + currentRow))
                        {
                            continue;
                        }

                        if (currentTile == 1 || (currentTile <= tileStoppedOn && !onFirstWord))
                        {
                            currentTile++;
                            continue;
                        }

                        if (tile.Name.Contains("Row" + currentRow + "Tile" + currentTile))
                        {
                            tile.Text = word[index].ToString().ToUpper();
                            tile.Font = new Font(tile.Font.Name, 30, FontStyle.Bold);
                            tile.UseVisualStyleBackColor = false;
                            tile.FlatStyle = FlatStyle.Flat;
                            tile.BackColor = Color.White;
                            tile.ForeColor = Color.White; // this failed to hide the text

                            RecolorTileText(tile, Color.White);
                        }

                        if (index + 1 == word.Length)
                        {
                            tileStoppedOn = currentTile + 1; // Add 1 for the space in between words
                            currentTile = 1;
                            onFirstWord = false;
                            break;
                        }
                        else
                        {
                            index++;
                            currentTile++;
                        }
                    }

                    // Update RemainingSpacesInRow, remembering to add 1 for the space in between words
                    remainingSpacesInRow = remainingSpacesInRow - word.Length - 1;
                }
                else
                {
                    // Word will not fit on current row, so move to next row
                    currentRow++;

                    switch (currentRow)
                    {
                        case 2: case 3:
                            remainingSpacesInRow = 14 - 2;
                            currentTile = 1;
                            onFirstWord = true;
                            tileStoppedOn = 0;
                            break;
                        case 4:
                            remainingSpacesInRow = 12 - 2;
                            currentTile = 1;
                            onFirstWord = true;
                            tileStoppedOn = 0;
                            break;
                    }

                    int index = 0;

                    // Update tiles on Row 1, starting with Tile 2
                    foreach (Button tile in this.board.BoardPanel.Controls)
                    {
                        // Skip over any tile that doesn't contain our current row in its name
                        if (!tile.Name.Contains("Row" + currentRow))
                        {
                            continue;
                        }

                        if (currentTile == 1 || (currentTile < tileStoppedOn && !onFirstWord))
                        {
                            currentTile++;
                            continue;
                        }

                        if (tile.Name.Contains("Row" + currentRow + "Tile" + currentTile))
                        {
                            tile.Text = word[index].ToString().ToUpper();
                            tile.Font = new Font(tile.Font.Name, 30, FontStyle.Bold);
                            tile.UseVisualStyleBackColor = false;
                            tile.FlatStyle = FlatStyle.Flat;
                            tile.BackColor = Color.White;
                            tile.ForeColor = Color.White; // this should effectively hide the text for the time being

                            // Took me forever to figure out how to get the text to display in white, thus matching the tile
                            // BackColor and making the text appear invisible. When any "alphabet" button is clicked later on,
                            // we should call this same function on the tile to reset the text color back to black
                            RecolorTileText(tile, Color.White);
                        }
                        else
                        {
                            currentTile++;
                            continue;
                        }

                        if (index + 1 == word.Length)
                        {
                            tileStoppedOn = currentTile + 1; // Add 1 for a space in between words
                            currentTile = 1;
                            onFirstWord = false;
                            break;
                        }
                        else
                        {
                            index++;
                            currentTile++;
                        }
                    }

                    // Update RemainingSpacesInRow, remembering to add 1 for the space in between words
                    remainingSpacesInRow = remainingSpacesInRow - word.Length - 1;
                }
            }

            currentPuzzleID++;
        }

        private void BankruptButton_Click(object sender, EventArgs e)
        {
            UpdateScore(board.GetSelectedPlayer, -1);
        }

        private async void SolvePuzzleButton_Click(object sender, EventArgs e)
        {
            Puzzle currentPuzzle = Puzzles[currentPuzzleID-1];
            if (currentPuzzle.fullPhrase.ToLower() == board.solveGuess.Text.ToLower())
            {
                foreach (Button tile in this.board.BoardPanel.Controls)
                {
                    if (tile.Text != "")
                    {
                        string placeholder = tile.Text;
                        tile.Text = "";
                        await ChangeTileColorFor1Second(tile);
                        tile.Text = placeholder;
                        RecolorTileText(tile, Color.Black);
                    }
                }

                UpdateScore(board.GetSelectedPlayer, 5000);

                MessageBox.Show("You win!");
                board.solveGuess.Text = "";
            }
            else
            {
                MessageBox.Show("I'm sorry.  That phrase is incorrect.");
                board.solveGuess.Text = "";
            }
        }

        private async void WheelOfFortuneBoard_AlphabetButtonClicked(object sender, Button button)
        {
            // Check for situation where user failed to add a number
            if (board.GetSpinValue == "")
            {
                MessageBox.Show("Please add the spin amount before pressing an Alphabet Button.");
                return;
            }

            // Check to see if the user has requested a vowel.  If so, make sure they have enough money to purchase a vowel
            var vowels = new HashSet<string> { "A", "E", "I", "O", "U" };
            if (vowels.Contains(button.Text))
            {
                if (RetrieveScore(board.GetSelectedPlayer) >= vowelCost)
                {
                    UpdateScore(board.GetSelectedPlayer, -vowelCost);
                }
                else
                {
                    MessageBox.Show(board.GetSelectedPlayer + " needs at least $" +
                        vowelCost + " to purchase a vowel!");
                    return;
                }
            }

            if (int.TryParse(board.GetSpinValue, out int spinAmount))
            {
                Console.WriteLine($"Parsed spin amount: {spinAmount}");
            }

            button.Enabled = false;
            button.BackColor = Color.Red;

            int numberOfLetters = 0;

            foreach (Button tile in this.board.BoardPanel.Controls)
            {
                if (tile.Text == button.Text)
                {
                    // Set text to empty string, change tile color for 2 seconds, repopulate text, and recolor tile
                    tile.Text = "";
                    await ChangeTileColorFor2Seconds(tile);
                    tile.Text = button.Text;
                    RecolorTileText(tile, Color.Black);
                    numberOfLetters++;
                }
            }

            // Take the spun monetary amount (Ex: $100, $500, etc) and add it to current player's score
            int amountToAdd = numberOfLetters * spinAmount;
            UpdateScore(board.GetSelectedPlayer, amountToAdd);
        }

        int RetrieveScore(string key)
        {
            if (scores.ContainsKey(key))
            {
                return scores[key];
            }
            return 0;
        }

        void UpdateScore(string key, int newValue)
        {
            if (scores.ContainsKey(key))
            {
                if (newValue == -1)
                {
                    // BANKRUPT button was pressed.  Reset current player's score to 0
                    if (board.p1Name.Text == key)
                    {
                        board.p1Score.Text = "0";
                        scores[key] = 0;
                    }
                    else if (board.p2Name.Text == key)
                    {
                        board.p2Score.Text = "0";
                        scores[key] = 0;
                    }
                    else if (board.p3Name.Text == key)
                    {
                        board.p3Score.Text = "0";
                        scores[key] = 0;
                    }
                }
                else
                {
                    scores[key] += newValue; // Update the value associated with the key

                    // Update Appropriate Player Textbox
                    if (board.p1Name.Text == key)
                        board.p1Score.Text = scores[key].ToString();
                    else if (board.p2Name.Text == key)
                        board.p2Score.Text = scores[key].ToString();
                    else if (board.p3Name.Text == key)
                        board.p3Score.Text = scores[key].ToString();
                }
            }
            else
            {
                Console.WriteLine($"Key '{key}' does not exist in the dictionary.");
            }
        }

        /// <summary>
        /// Change the text color of a particular tile to the color passed
        /// </summary>
        /// <param name="myTile"></param>
        /// <param name="color"></param>
        private void RecolorTileText(Button myTile, Color color)
        {
            myTile.Paint += (s, es) =>
            {
                es.Graphics.Clear(Color.White); // Draw the background

                TextRenderer.DrawText(
                    es.Graphics,
                    myTile.Text,
                    myTile.Font,
                    myTile.ClientRectangle,
                    color, // Custom text color
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            };
        }

        private void RecolorTileItself(Button myTile, Color color)
        {
            myTile.Paint += (s, es) =>
            {
                es.Graphics.Clear(Color.Blue); // Draw the background

                TextRenderer.DrawText(
                    es.Graphics,
                    myTile.Text,
                    myTile.Font,
                    myTile.ClientRectangle,
                    color, // Custom text color
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            };
        }

        private async Task ChangeTileColorFor2Seconds(Button myTile)
        {
            RecolorTileItself(myTile, Color.Black);
            await Task.Delay(2000);        // Wait for 2 seconds asynchronously
        }

        private async Task ChangeTileColorFor1Second(Button myTile)
        {
            RecolorTileItself(myTile, Color.Black);
            await Task.Delay(500);        // Wait for half a second asynchronously
        }
    }
}

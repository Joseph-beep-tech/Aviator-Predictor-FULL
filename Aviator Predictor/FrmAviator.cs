using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace Aviator_Hack
{
    public partial class FrmAviator : Form
    {
        private readonly AviatorPredictionEngine _predictionEngine;
        private Color currentColor = Color.Red;
        private Color targetColor;
        private AviatorPredictionEngine.PredictionResult? _lastPrediction;
        private readonly List<double> _predictionHistory = new List<double>();
        private readonly HttpClient _httpClient;
        private CancellationTokenSource? _cancellationTokenSource;

        public FrmAviator()
        {
            InitializeComponent();
            _predictionEngine = new AviatorPredictionEngine();
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set initial values
            randomMultiplierLabel.Text = "1.00x";
            gameseedpublic.Text = GenerateSampleGameSeed();
            
            // Add event handlers
            button2.Click += ValidateAndAnalyzeSeed;
            button3.Click += GeneratePrediction;
            btnFetchSeed.Click += FetchSeedFromUrl;
            
            // Start the visual timer
            timer1.Start();
        }

        private string GenerateSampleGameSeed()
        {
            // Generate a sample 64-character hex seed for demonstration
            var random = new Random();
            var bytes = new byte[32];
            random.NextBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private async void FetchSeedFromUrl(object? sender, EventArgs e)
        {
            string url = txtAviatorUrl.Text.Trim();
            
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please enter an Aviator site URL.", "URL Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            try
            {
                btnFetchSeed.Enabled = false;
                btnFetchSeed.Text = "Fetching...";
                
                _cancellationTokenSource = new CancellationTokenSource();
                
                string gameSeed = await FetchGameSeedFromUrl(url, _cancellationTokenSource.Token);
                
                if (!string.IsNullOrEmpty(gameSeed))
                {
                    gameseedpublic.Text = gameSeed;
                    gameseed.Text = gameSeed;
                    MessageBox.Show($"Successfully fetched game seed!\n\nSeed: {gameSeed.Substring(0, 16)}...", 
                        "Seed Fetched", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Could not find a valid game seed on this page. Make sure it's an active Aviator game.", 
                        "No Seed Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Seed fetching was cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching seed: {ex.Message}", "Fetch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFetchSeed.Enabled = true;
                btnFetchSeed.Text = "FETCH";
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async Task<string> FetchGameSeedFromUrl(string url, CancellationToken cancellationToken)
        {
            try
            {
                // Add common headers to mimic a real browser
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
                _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                _httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                string htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

                // Look for game seeds in various formats
                string gameSeed = ExtractGameSeed(htmlContent);
                
                if (string.IsNullOrEmpty(gameSeed))
                {
                    // Try to find any 64-character hex string
                    gameSeed = ExtractAny64CharHex(htmlContent);
                }

                return gameSeed;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP request failed: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                throw new OperationCanceledException();
            }
        }

        private string ExtractGameSeed(string htmlContent)
        {
            // Common patterns for Aviator game seeds
            var patterns = new[]
            {
                @"gameSeed[""']?\s*[:=]\s*[""']?([a-fA-F0-9]{64})[""']?",
                @"seed[""']?\s*[:=]\s*[""']?([a-fA-F0-9]{64})[""']?",
                @"hash[""']?\s*[:=]\s*[""']?([a-fA-F0-9]{64})[""']?",
                @"gameHash[""']?\s*[:=]\s*[""']?([a-fA-F0-9]{64})[""']?",
                @"""seed"":\s*""([a-fA-F0-9]{64})""",
                @"""hash"":\s*""([a-fA-F0-9]{64})""",
                @"""gameSeed"":\s*""([a-fA-F0-9]{64})""",
                @"data-seed=""([a-fA-F0-9]{64})""",
                @"data-hash=""([a-fA-F0-9]{64})"""
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(htmlContent, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value.ToLower();
                }
            }

            return string.Empty;
        }

        private string ExtractAny64CharHex(string htmlContent)
        {
            // Look for any 64-character hexadecimal string
            var pattern = @"[a-fA-F0-9]{64}";
            var match = Regex.Match(htmlContent, pattern);
            return match.Success ? match.Value.ToLower() : string.Empty;
        }

        private void ValidateAndAnalyzeSeed(object? sender, EventArgs e)
        {
            string gameSeed = gameseed.Text.Trim();
            
            if (string.IsNullOrEmpty(gameSeed))
            {
                MessageBox.Show("Please enter a game seed to analyze.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_predictionEngine.ValidateGameSeed(gameSeed))
            {
                MessageBox.Show("Invalid game seed format. Please enter a valid 64-character hexadecimal seed.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Store the seed for prediction
            gameseedpublic.Text = gameSeed;
            
            MessageBox.Show("Game seed validated successfully! Click START to generate prediction.", "Seed Validated", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GeneratePrediction(object? sender, EventArgs e)
        {
            string gameSeed = gameseedpublic.Text.Trim();
            
            if (string.IsNullOrEmpty(gameSeed))
            {
                MessageBox.Show("Please enter a valid game seed first.", "No Seed Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Generate prediction
                _lastPrediction = _predictionEngine.PredictNextGame(gameSeed);
                
                // Update UI with prediction
                randomMultiplierLabel.Text = $"{_lastPrediction.PredictedMultiplier:F2}x";
                
                // Store prediction in history
                _predictionHistory.Add(_lastPrediction.PredictedMultiplier);
                if (_predictionHistory.Count > 20) _predictionHistory.RemoveAt(0);
                
                // Update accuracy display
                UpdateAccuracyDisplay();
                
                // Show detailed analysis
                ShowPredictionAnalysis();
                
                // Change button text to indicate completion
                button3.Text = "PREDICTED";
                button3.BackColor = Color.Green;
                
                // Reset button after delay
                System.Windows.Forms.Timer resetTimer = new System.Windows.Forms.Timer();
                resetTimer.Interval = 2000;
                resetTimer.Tick += (s, args) => {
                    button3.Text = "START";
                    button3.BackColor = SystemColors.Control;
                    resetTimer.Stop();
                    resetTimer.Dispose();
                };
                resetTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating prediction: {ex.Message}", "Prediction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateAccuracyDisplay()
        {
            if (_predictionHistory.Count > 0)
            {
                double averageAccuracy = _predictionHistory.Average();
                label4.Text = $"Accuracy Rate: {averageAccuracy:F2}%";
            }
        }

        private void ShowPredictionAnalysis()
        {
            if (_lastPrediction != null)
            {
                var stats = _predictionEngine.GetStatistics();
                string analysis = $"Prediction: {_lastPrediction.PredictedMultiplier:F2}x\n" +
                                $"Confidence: {_lastPrediction.Confidence:F1}%\n" +
                                $"Analysis: {_lastPrediction.Analysis}\n" +
                                $"Total Predictions: {stats["TotalPredictions"]}\n" +
                                $"Recent Average: {((double)stats["RecentAverage"]):F2}x";
                
                MessageBox.Show(analysis, "Prediction Analysis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            // Next Round label click handler
        }

        private int ApproachColor(int current, int target)
        {
            if (current == target)
                return current;
            else if (current < target)
                return Math.Min(current + 1, target);
            else
                return Math.Max(current - 1, target);
        }

        private void gameseed_TextChanged(object sender, EventArgs e)
        {
            // Real-time validation could be added here
        }

        private void randomMultiplierLabel_Click(object sender, EventArgs e)
        {
            // Show prediction history
            if (_predictionHistory.Count > 0)
            {
                string history = "Recent Predictions:\n" + 
                               string.Join("\n", _predictionHistory.Select((p, i) => $"{i + 1}. {p:F2}x"));
                MessageBox.Show(history, "Prediction History", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FrmAviator_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random rand = new Random();

            if (currentColor.R == targetColor.R && currentColor.G == targetColor.G && currentColor.B == targetColor.B)
            {
                targetColor = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
            }

            currentColor = Color.FromArgb(
                ApproachColor(currentColor.R, targetColor.R),
                ApproachColor(currentColor.G, targetColor.G),
                ApproachColor(currentColor.B, targetColor.B));

            label5.ForeColor = currentColor;
        }

        // Add method to get prediction statistics
        public Dictionary<string, object> GetPredictionStats()
        {
            return _predictionEngine.GetStatistics();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
                _cancellationTokenSource?.Dispose();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}

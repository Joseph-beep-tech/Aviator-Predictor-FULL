using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace Aviator_Hack
{
    public partial class Form1 : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        
        // Store hashed credentials (in production, these would come from a secure database)
        private readonly Dictionary<string, string> _validCredentials = new Dictionary<string, string>
        {
            // Default admin account - password: admin123
            { "admin", "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa7898096c202bdc16" },
            // Demo account - password: demo123
            { "demo", "6c7ca345f63f835cb353ff15bd6c5e052ec08e7a" }
        };

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(660, 600);
            this.BackColor = Color.White;
            Form1_Load(this, EventArgs.Empty);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateRoundedForm();
            SetupForm();
        }

        private void SetupForm()
        {
            // Clear any existing text
            APUsername.Text = "";
            APPassword.Text = "";
            
            // Set focus to username field
            APUsername.Focus();
            
            // Add enter key support
            APPassword.KeyPress += (s, e) => {
                if (e.KeyChar == (char)13) // Enter key
                {
                    button2_Click(s, e);
                }
            };
        }

        private void CreateRoundedForm()
        {
            GraphicsPath path = new GraphicsPath();
            int radius = 20;

            path.AddArc(new Rectangle(0, 0, radius * 2, radius * 2), 180, 90);
            path.AddArc(new Rectangle(this.Width - 2 * radius, 0, radius * 2, radius * 2), 270, 90);
            path.AddArc(new Rectangle(this.Width - 2 * radius, this.Height - 2 * radius, radius * 2, radius * 2), 0, 90);
            path.AddArc(new Rectangle(0, this.Height - 2 * radius, radius * 2, radius * 2), 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Panel paint handler
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Title label click handler
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("For password reset, please contact support at support@aviatortool.com", "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // Background picture click handler
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Clicks == 1 && e.Y <= this.Height && e.Y >= 0)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            string username = APUsername.Text.Trim();
            string password = APPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username and password cannot be blank.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                APUsername.Focus();
                return;
            }

            if (ValidateCredentials(username, password))
            {
                MessageBox.Show("Login successful! Welcome to Aviator Predictor.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OpenForm2();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                APPassword.Text = "";
                APPassword.Focus();
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            if (_validCredentials.ContainsKey(username))
            {
                string hashedPassword = HashPassword(password);
                return _validCredentials[username].Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void OpenForm2()
        {
            Form2 form2 = new Form2();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Username text changed handler
        }

        private void Aviator_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start https://spribe.co/games/aviator",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening Aviator link: {ex.Message}");
            }
        }

        // Method to add new users (for admin purposes)
        public void AddUser(string username, string password)
        {
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                string hashedPassword = HashPassword(password);
                _validCredentials[username] = hashedPassword;
            }
        }

        // Method to remove users (for admin purposes)
        public void RemoveUser(string username)
        {
            if (_validCredentials.ContainsKey(username))
            {
                _validCredentials.Remove(username);
            }
        }
    }
}
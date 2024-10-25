using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace AffinityTest
{ 
    public class MainForm : Form
    {
        // Define constants for SetWindowDisplayAffinity
        private const uint WDA_NONE = 0x0;
        private const uint WDA_MONITOR = 0x1;

        // Import necessary Windows API functions
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private bool affinityEnabled = false;
        private Timer animationTimer;
        private Rectangle animatedRectangle;
        private int dx = 5;  // Speed of the animation
        private Label statusLabel;

        public MainForm()
        {
            // Setup the form
            this.Text = "Display Affinity Example";
            this.Width = 400;
            this.Height = 300;
            this.DoubleBuffered = true; // To prevent flickering
            this.StartPosition = FormStartPosition.CenterScreen;

            // Status label to show whether display affinity is enabled or disabled
            statusLabel = new Label();
            statusLabel.Text = "Display Affinity: Disabled";
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(statusLabel);

            // Initialize the animated rectangle
            animatedRectangle = new Rectangle(10, 100, 50, 50);

            // Setup the animation timer
            animationTimer = new Timer();
            animationTimer.Interval = 50; // Update every 50ms
            animationTimer.Tick += new EventHandler(OnTimerTick);
            animationTimer.Start();

            // Setup the KeyDown event to handle hotkey (F3)
            this.KeyPreview = true; // Allows form to capture key events before controls
            this.KeyDown += new KeyEventHandler(OnKeyDown);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // Update the position of the rectangle
            animatedRectangle.X += dx;

            // Bounce the rectangle off the walls
            if (animatedRectangle.X < 0 || animatedRectangle.X + animatedRectangle.Width > this.ClientSize.Width)
            {
                dx = -dx;
            }

            // Redraw the form
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the animated rectangle
            e.Graphics.FillRectangle(Brushes.Blue, animatedRectangle);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Toggle display affinity on F3 key press
            if (e.KeyCode == Keys.F3)
            {
                ToggleDisplayAffinity();
            }
        }

        private void ToggleDisplayAffinity()
        {
            // Get the handle of the current window
            IntPtr hwnd = this.Handle;

            // Toggle between WDA_NONE and WDA_MONITOR
            if (!affinityEnabled)
            {
                // Enable display affinity
                if (SetWindowDisplayAffinity(hwnd, WDA_MONITOR))
                {
                    affinityEnabled = true;
                    statusLabel.Text = "Display Affinity: Enabled";
                }
                else
                {
                    MessageBox.Show("Failed to enable display affinity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Disable display affinity
                if (SetWindowDisplayAffinity(hwnd, WDA_NONE))
                {
                    affinityEnabled = false;
                    statusLabel.Text = "Display Affinity: Disabled";
                }
                else
                {
                    MessageBox.Show("Failed to disable display affinity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new MainForm());
        //}
    }
}

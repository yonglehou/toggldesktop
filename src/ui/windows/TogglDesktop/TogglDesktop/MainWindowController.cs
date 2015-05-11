using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace TogglDesktop
{
    public partial class MainWindowController : TogglForm
    {
        private bool isResizing = false;
        private bool isTracking = false;

        private LoginViewController loginViewController;
        private TimeEntryListViewController timeEntryListViewController;
        private TimeEntryEditViewController timeEntryEditViewController;
        private AboutWindowController aboutWindowController;
        private PreferencesWindowController preferencesWindowController;
        private FeedbackWindowController feedbackWindowController;
        private IdleNotificationWindowController idleNotificationWindowController;

        private KeyboardHooksController keyboardHooksController;
        private StatusIconController statusIconController;

        // FIXME: main window controller should probably not deal with the edit form at all
        private EditForm editForm;

        private bool remainOnTop = false;
        private bool topDisabled = false;

        private Point defaultContentPosition = new System.Drawing.Point(0, 0);
        private Point errorContentPosition = new System.Drawing.Point(0, 28);

        private static MainWindowController instance;

        private Timer runScriptTimer;

        public MainWindowController()
        {
            InitializeComponent();

            instance = this;
        }

        public void toggleMenu()
        {
            Point pt = new Point(Width - 80, 0);
            pt = PointToScreen(pt);
            trayIconMenu.Show(pt);
        }

        public void BringAppToFront()
        {
            if (Visible)
            {
                Hide();
                if (editForm.Visible)
                {
                    editForm.CloseButton_Click(null, null);
                }
                feedbackWindowController.Close();
                aboutWindowController.Close();
                preferencesWindowController.Close();
            }
            else
            {
                show();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            hideHorizontalScrollBar();

            base.OnShown(e);
        }

        public static void DisableTop()
        {
            instance.topDisabled = true;
            instance.displayOnTop();
        }

        public static void EnableTop()
        {
            instance.topDisabled = false;
            instance.displayOnTop();
        }

        public void RemoveTrayIcon()
        {
            trayIcon.Visible = false;
        }

        private void MainWindowController_Load(object sender, EventArgs e)
        {
            troubleBox.BackColor = Color.FromArgb(239, 226, 121);
            contentPanel.Location = defaultContentPosition;

            keyboardHooksController = new KeyboardHooksController(this);
            statusIconController = new StatusIconController(this);

            Toggl.OnApp += OnApp;
            Toggl.OnError += OnError;
            Toggl.OnLogin += OnLogin;
            Toggl.OnTimeEntryList += OnTimeEntryList;
            Toggl.OnTimeEntryEditor += OnTimeEntryEditor;
            Toggl.OnReminder += OnReminder;
            Toggl.OnURL += OnURL;
            Toggl.OnRunningTimerState += OnRunningTimerState;
            Toggl.OnStoppedTimerState += OnStoppedTimerState;
            Toggl.OnSettings += OnSettings;
            Toggl.OnIdleNotification += OnIdleNotification;

            loginViewController = new LoginViewController();
            timeEntryListViewController = new TimeEntryListViewController();
            timeEntryEditViewController = new TimeEntryEditViewController();
            aboutWindowController = new AboutWindowController();
            preferencesWindowController = new PreferencesWindowController();
            feedbackWindowController = new FeedbackWindowController();
            idleNotificationWindowController = new IdleNotificationWindowController();

            // FIXME: move into edit form
            editForm = new EditForm
            {
                ControlBox = false,
                StartPosition = FormStartPosition.Manual
            };
            editForm.Controls.Add(timeEntryEditViewController);
            editForm.editView = timeEntryEditViewController;

            if (!Toggl.StartUI(TogglDesktop.Program.Version()))
            {
                try
                {
                    DisableTop();
                    MessageBox.Show("Missing callback. See the log file for details");
                } finally {
                    EnableTop();
                }
                TogglDesktop.Program.Shutdown(1);
            }

            Utils.LoadWindowLocation(this, editForm);

            aboutWindowController.initAndCheck();

            runScriptTimer = new Timer();
            runScriptTimer.Interval = 1000;
            runScriptTimer.Tick += runScriptTimer_Tick;
            runScriptTimer.Start();
        }

        void runScriptTimer_Tick(object sender, EventArgs e)
        {
            runScriptTimer.Stop();

            if (null == Toggl.ScriptPath)
            {
                return;
            }

            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                if (!File.Exists(Toggl.ScriptPath))
                {
                    Console.WriteLine("Script file does not exist: " + Toggl.ScriptPath);
                    TogglDesktop.Program.Shutdown(0);
                }

                string script = File.ReadAllText(Toggl.ScriptPath);
                
                Int64 err = 0;
                string result = Toggl.RunScript(script, ref err);
                if (0 != err)
                {
                    Console.WriteLine(string.Format("Failed to run script, err = {0}", err));
                }
                Console.WriteLine(result);

                if (0 == err)
                {
                    TogglDesktop.Program.Shutdown(0);
                }
            }, null);
        }

        void OnRunningTimerState(Toggl.TimeEntry te)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnRunningTimerState(te); });
                return;
            }
            isTracking = true;
            enableMenuItems();

            updateResizeHandleBackground();
        }

        public void DisplayText(string newText)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { DisplayText(newText); });
                return;
            }

            Text = newText;

            if (null != trayIcon)
            {
                trayIcon.Text = newText;
            }
        }

        public void DisplayIcon(Icon form, Icon tray)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { DisplayIcon(form, tray); });
                return;
            }

            if (Icon != form)
            {
                Icon = form;
            }

            if (null != trayIcon && trayIcon.Icon != tray)
            {
                trayIcon.Icon = tray;
            }
        }

        void OnStoppedTimerState()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnStoppedTimerState(); });
                return;
            }
            isTracking = false;
            enableMenuItems();

            runningToolStripMenuItem.Text = "Timer is not tracking";

            updateResizeHandleBackground();
        }

        void OnSettings(bool open, Toggl.Settings settings)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnSettings(open, settings); });
                return;
            }
            remainOnTop = settings.OnTop;

            displayOnTop();

            timerIdleDetection.Enabled = settings.UseIdleDetection;
        }

        void OnURL(string url)
        {
            Process.Start(url);
        }

        void OnApp(bool open)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnApp(open); });
                return;
            }
            if (open) {
                show();
            }
        }

        void OnError(string errmsg, bool user_error)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnError(errmsg, user_error); });
                return;
            }

            errorLabel.Text = errmsg;
            errorToolTip.SetToolTip(errorLabel, errmsg);
            troubleBox.Visible = true;
            contentPanel.Location = errorContentPosition;
        }

        void OnIdleNotification(
            string guid,
            string since,
            string duration,
            UInt64 started,
            string description)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnIdleNotification(guid, since, duration, started, description); });
                return;
            }

            idleNotificationWindowController.ShowWindow();
        }

        void OnLogin(bool open, UInt64 user_id)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnLogin(open, user_id); });
                return;
            }
            if (open) {
                if (editForm.Visible)
                {
                    editForm.Hide();
                    editForm.GUID = null;
                }
                contentPanel.Controls.Remove(timeEntryListViewController);
                contentPanel.Controls.Remove(timeEntryEditViewController);
                contentPanel.Controls.Add(loginViewController);
                MinimumSize = new Size(loginViewController.MinimumSize.Width, loginViewController.MinimumSize.Height + 40);
                loginViewController.SetAcceptButton(this);
                resizeHandle.BackColor = Color.FromArgb(69, 69, 69);
            }
            enableMenuItems();

            if (open || 0 == user_id)
            {
                runningToolStripMenuItem.Text = "Timer is not tracking";
            }
        }

        private void enableMenuItems()
        {
            bool isLoggedIn = TogglDesktop.Program.IsLoggedIn;

            newToolStripMenuItem.Enabled = isLoggedIn;
            continueToolStripMenuItem.Enabled = isLoggedIn && !isTracking;
            stopToolStripMenuItem.Enabled = isLoggedIn && isTracking;
            syncToolStripMenuItem.Enabled = isLoggedIn;
            logoutToolStripMenuItem.Enabled = isLoggedIn;
            clearCacheToolStripMenuItem.Enabled = isLoggedIn;
            sendFeedbackToolStripMenuItem.Enabled = isLoggedIn;
            openInBrowserToolStripMenuItem.Enabled = isLoggedIn;
        }

        void OnTimeEntryList(bool open, List<Toggl.TimeEntry> list)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnTimeEntryList(open, list); });
                return;
            }
            if (open)
            {
                troubleBox.Visible = false;
                contentPanel.Location = defaultContentPosition;
                contentPanel.Controls.Remove(loginViewController);
                MinimumSize = new Size(230, 86);
                contentPanel.Controls.Add(timeEntryListViewController);
                timeEntryListViewController.SetAcceptButton(this);
                if (editForm.Visible)
                {
                    editForm.Hide();
                    editForm.GUID = null;
                }
            }
        }

        // FIXME: move into edit form
        public void PopupInput(Toggl.TimeEntry te)
        {
            if (te.GUID == editForm.GUID) {
                editForm.CloseButton_Click(null, null);
                return;
            }
            editForm.reset();
            setEditFormLocation(te.DurationInSeconds < 0);
            editForm.GUID = te.GUID;
            editForm.Show();
        }

        void OnTimeEntryEditor(
            bool open,
            Toggl.TimeEntry te,
            string focused_field_name)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnTimeEntryEditor(open, te, focused_field_name); });
                return;
            }
            if (open)
            {
                contentPanel.Controls.Remove(loginViewController);
                MinimumSize = new Size(230, 86);
                timeEntryEditViewController.setupView(this, focused_field_name);
                PopupInput(te);                
            }
        }

        private void MainWindowController_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utils.SaveWindowLocation(this, editForm);

            if (!TogglDesktop.Program.ShuttingDown)
            {
                Hide();
                e.Cancel = true;
            }

            if (editForm.Visible)
            {
                editForm.ClosePopup();
            }
        }

        private void buttonDismissError_Click(object sender, EventArgs e)
        {
            troubleBox.Visible = false;
            contentPanel.Location = defaultContentPosition;
        }

        private void sendFeedbackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            feedbackWindowController.Show();
            feedbackWindowController.TopMost = true;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Visible)
            {
                Utils.SaveWindowLocation(this, editForm);
            }

            TogglDesktop.Program.Shutdown(0);
        }

        private void toggleVisibility()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                show();
                return;
            }
            if (Visible)
            {
                Hide();
                return;
            }
            show();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggl.Start("", "", 0, 0);
        }

        private void continueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggl.ContinueLatest();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
           Toggl.Stop();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            show();
        }

        private void syncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggl.Sync();
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggl.OpenInBrowser();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggl.EditPreferences();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutWindowController.ShowUpdates();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggl.Logout();
        }

        private void show()
        {
            Show();
            TopMost = true;
            displayOnTop();
        }

        private void displayOnTop()
        {
            if (remainOnTop && !topDisabled)
            {
                Win32helper.SetWindowTopMost(Handle);

                // FIXME: move into edit form
                if (editForm != null)
                {
                    Win32helper.SetWindowTopMost(editForm.Handle);
                }
                return;
            }
            
            Win32helper.UnsetWindowTopMost(Handle);

            // FIXME: move into edit form
            if (editForm != null)
            {
                Win32helper.UnsetWindowTopMost(editForm.Handle);
            }
        }

        void OnReminder(string title, string informative_text)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnReminder(title, informative_text); });
                return;
            }
            trayIcon.ShowBalloonTip(6000 * 100, title, informative_text, ToolTipIcon.None);
        }

        private void clearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            try
            {
                DisableTop();
                dr = MessageBox.Show(
                    "This will remove your Toggl user data from this PC and log you out of the Toggl Desktop app. " +
                    "Any unsynced data will be lost." +
                    Environment.NewLine + "Do you want to continue?",
                    "Clear Cache",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            finally
            {
                EnableTop();
            }
            if (DialogResult.Yes == dr) 
            {
                Toggl.ClearCache();
            }
        }

        private void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            show();
        }

        private void timerIdleDetection_Tick(object sender, EventArgs e)
        {
            Toggl.SetIdleSeconds((ulong)Win32helper.GetIdleSeconds());
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                toggleVisibility();
            }
        }

        private void MainWindowController_Activated(object sender, EventArgs e)
        {
            Toggl.SetWake();
        }

        private void MainWindowController_LocationChanged(object sender, EventArgs e)
        {
            // FIXME: recalculatePopupPosition();
        }

        private void setEditFormLocation(bool running)
        {
            if (Screen.AllScreens.Length > 1)
            {
                foreach (Screen s in Screen.AllScreens)
                {
                    if (s.WorkingArea.IntersectsWith(DesktopBounds))
                    {
                        // FIXME: calculateEditFormPosition(running, s);
                        break;
                    }
                }
            }
            else
            {
                // FIXME: calculateEditFormPosition(running,Screen.PrimaryScreen);
            }
        }

        private void MainWindowController_SizeChanged(object sender, EventArgs e)
        {
            // FIXME: recalculatePopupPosition();

            if (timeEntryListViewController != null)
            {
                hideHorizontalScrollBar();
            }
            resizeHandle.Location = new Point(Width-16, Height-56);

            updateResizeHandleBackground();
        }

        private void updateResizeHandleBackground()
        {
            if (contentPanel.Controls.Contains(loginViewController))
            {
                resizeHandle.BackColor = Color.FromArgb(69, 69, 69);
            }
            else if (Height <= MinimumSize.Height)
            {
                String c = "#4dd965";
                if(isTracking) {
                    c = "#ff3d32";
                } 
                resizeHandle.BackColor = ColorTranslator.FromHtml(c);
            }
            else
            {
                resizeHandle.BackColor = System.Drawing.Color.Transparent;
            }
        }

        private void resizeHandle_MouseDown(object sender, MouseEventArgs e)
        {
            isResizing = true;
        }

        private void resizeHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                isResizing = (e.Button == MouseButtons.Left);

                Win32helper.DoSomething(Handle, isResizing);
            }
        }
    }
}

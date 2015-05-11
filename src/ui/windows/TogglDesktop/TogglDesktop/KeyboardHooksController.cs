using System;
using System.Windows.Forms;

namespace TogglDesktop
{
    class KeyboardHooksController
    {
        KeyboardHook startHook = new KeyboardHook();
        KeyboardHook showHook = new KeyboardHook();

        // FIXME: use delegate instead
        MainWindowController window;

        private bool isTracking = false;

        public KeyboardHooksController(MainWindowController wnd)
        {
            window = wnd;

            startHook.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(hookStartKeyPressed);

            showHook.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(hookShowKeyPressed);

            Toggl.OnRunningTimerState += OnRunningTimerState;
            Toggl.OnStoppedTimerState += OnStoppedTimerState;
            Toggl.OnSettings += OnSettings;
        }

        void OnSettings(bool open, Toggl.Settings settings)
        {
            try
            {
                startHook.Clear();
                string startKey = Properties.Settings.Default.StartKey;
                if (startKey != null && startKey != "")
                {
                    startHook.RegisterHotKey(
                        Properties.Settings.Default.StartModifiers,
                        (Keys)Enum.Parse(typeof(Keys), startKey));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not register start shortcut: ", e);
            }

            try
            {
                showHook.Clear();
                string showKey = Properties.Settings.Default.ShowKey;
                if (showKey != null && showKey != "")
                {
                    showHook.RegisterHotKey(
                        Properties.Settings.Default.ShowModifiers,
                        (Keys)Enum.Parse(typeof(Keys), showKey));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not register show hotkey: ", e);
            }
        }

        void OnRunningTimerState(Toggl.TimeEntry te)
        {
            isTracking = true;
        }

        void OnStoppedTimerState()
        {
            isTracking = false;
        }

        void hookStartKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (isTracking)
            {
                Toggl.Stop();
            }
            else
            {
                Toggl.ContinueLatest();
            }
        }

        void hookShowKeyPressed(object sender, KeyPressedEventArgs e)
        {
            window.BringAppToFront();
        }
    }
}

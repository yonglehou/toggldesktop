using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TogglDesktop
{
    class StatusIconController
    {
        private List<Icon> statusIcons = new List<Icon>();

        private bool isTracking = false;

        private const int kTogglTray = 0;
        private const int kTogglTrayInactive = 1;
        private const int kToggl = 2;
        private const int kTogglInactive = 3;
        private const int kTogglOfflineActive = 4;
        private const int kTogglOfflineInactive = 5;

        // FIXME: use delegate instead
        private MainWindowController window;

        public StatusIconController(MainWindowController wnd)
        {
            window = wnd;

            Toggl.OnRunningTimerState += OnRunningTimerState;
            Toggl.OnStoppedTimerState += OnStoppedTimerState;
            Toggl.OnOnlineState += OnOnlineState;
            Toggl.OnLogin += OnLogin;
        }

        void OnLogin(bool open, UInt64 user_id)
        {
            updateStatusIcons(true);
        }

        void OnRunningTimerState(Toggl.TimeEntry te)
        {
            isTracking = true;

            updateStatusIcons(true);

            string newText = "Toggl Desktop";
            if (te.Description.Length > 0)
            {
                newText = te.Description + " - Toggl Desktop";
            }
            if (newText.Length > 63)
            {
                newText = newText.Substring(0, 60) + "...";
            }

            window.DisplayText(newText);
        }

        void OnStoppedTimerState()
        {
            isTracking = false;

            updateStatusIcons(true);

            window.DisplayText("Timer is not tracking");
        }

        void OnOnlineState(Int64 state)
        {
            updateStatusIcons(0 == state);
        }

        private void updateStatusIcons(bool is_online)
        {
            if (0 == statusIcons.Count)
            {
                return;
            }

            Icon tray = null;
            Icon form = null;

            if (is_online)
            {
                if (TogglDesktop.Program.IsLoggedIn && isTracking)
                {
                    tray = statusIcons[kTogglTray];
                    form = statusIcons[kToggl];
                }
                else
                {
                    tray = statusIcons[kTogglTrayInactive];
                    form = statusIcons[kTogglInactive];
                }
            }
            else
            {
                if (TogglDesktop.Program.IsLoggedIn && isTracking)
                {
                    tray = statusIcons[kTogglOfflineActive];
                    form = statusIcons[kToggl];
                }
                else
                {
                    tray = statusIcons[kTogglOfflineInactive];
                    form = statusIcons[kTogglInactive];
                }
            }

            window.DisplayIcon(form, tray);
        }

        private void loadStatusIcons()
        {
            if (statusIcons.Count > 0)
            {
                throw new InvalidOperationException("Status images already loaded");
            }
            statusIcons.Add(Properties.Resources.toggltray);
            statusIcons.Add(Properties.Resources.toggltray_inactive);
            statusIcons.Add(Properties.Resources.toggl);
            statusIcons.Add(Properties.Resources.toggl_inactive);
            statusIcons.Add(Properties.Resources.toggl_offline_active);
            statusIcons.Add(Properties.Resources.toggl_offline_inactive);
        }
    }
}

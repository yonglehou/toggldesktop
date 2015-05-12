using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace TogglDesktop
{
    public partial class TimeEntryListViewController : UserControl
    {
        // Will use for rendering mutex
        private Object rendering = new Object();

        public TimeEntryListViewController()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;

            Toggl.OnTimeEntryList += OnTimeEntryList;
            Toggl.OnLogin += OnLogin;

            timerEditViewController.DescriptionTextBox.MouseWheel += TimeEntryListViewController_MouseWheel;
            timerEditViewController.DurationTextBox.MouseWheel += TimeEntryListViewController_MouseWheel;
        }

        void TimeEntryListViewController_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!timerEditViewController.isAutocompleteOpened())
            {
                elementHost.Focus();
            }
        }

        public void SetAcceptButton(Form frm)
        {
            timerEditViewController.SetAcceptButton(frm);
        }

        void OnTimeEntryList(bool open, List<Toggl.TimeEntry> list)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnTimeEntryList(open, list); });
                return;
            }

            DateTime start = DateTime.Now;

            lock (rendering)
            {
                renderTimeEntryList(list);
            }

            Console.WriteLine(String.Format(
                "Time entries list view rendered in {0} ms",
                DateTime.Now.Subtract(start).TotalMilliseconds));
        }

        private void renderTimeEntryList(List<Toggl.TimeEntry> list)
        {
            emptyLabel.Visible = (list.Count == 0);

            /* FIXME: move actual rendering into wpf user control
            entries.SuspendLayout();

            // Hide entry list for initial loading to avoid crazy flicker
            if (entries.Controls.Count == 0)
            {
                entries.Visible = false;
            }

            for (int i = 0; i < list.Count; i++)
            {
                Toggl.TimeEntry te = list.ElementAt(i);

                TimeEntryCell cell = null;
                if (entries.Controls.Count > i)
                {
                    cell = entries.Controls[i] as TimeEntryCell;
                }

                if (cell == null)
                {
                    cell = new TimeEntryCell(this);
                    entries.Controls.Add(cell);
                    if (i == 0)
                    {
                        cell.Width = entries.Width;
                    }
                    else
                    {
                        cell.Dock = DockStyle.Top;
                    }
                }

                cell.Display(te);
                entries.Controls.SetChildIndex(cell, i);
            }

            while (entries.Controls.Count > list.Count)
            {
                entries.Controls[list.Count].Dispose();
                // Dispose() will remove the control from collection
            }

            entries.ResumeLayout();
            entries.PerformLayout();
            */
        }

        void OnLogin(bool open, UInt64 user_id)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnLogin(open, user_id); });
                return;
            }
            if (open || user_id == 0)
            {
                elementHost.Controls.Clear();
            }
        }

        private void entries_MouseEnter(object sender, EventArgs e)
        {
            if (!timerEditViewController.focusList())
            {
                elementHost.Focus();
            }
        }

        private void emptyLabel_Click(object sender, EventArgs e)
        {
            Toggl.OpenInBrowser();
        }
    }
}

﻿using System;
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
    private Object rendering = new Object();
    public TimeEntryCell currentEntry = null;

    public TimeEntryListViewController()
    {
        InitializeComponent();

        Dock = DockStyle.Fill;

        entries.AutoScroll = false;
        entries.HorizontalScroll.Enabled = false;
        entries.AutoScroll = true;

        Toggl.OnTimeEntryList += OnTimeEntryList;
        Toggl.OnLogin += OnLogin;

        timerEditViewController.DescriptionTextBox.MouseWheel += TimeEntryListViewController_MouseWheel;
        timerEditViewController.DurationTextBox.MouseWheel += TimeEntryListViewController_MouseWheel;
    }

    void TimeEntryListViewController_MouseWheel(object sender, MouseEventArgs e)
    {
        if (!timerEditViewController.isAutocompleteOpened())
        {
            entries.Focus();
        }
    }

    public int EntriesTop
    {
        get {
            return entries.Location.Y;
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
            Invoke((MethodInvoker)delegate {
                OnTimeEntryList(open, list);
            });
            return;
        }
        if (open && currentEntry != null)
        {
            currentEntry.opened = false;
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
        entries.SuspendLayout();

        // Hide entry list for initial loading to avoid crazy flicker
        if (entries.Controls.Count == 0)
        {
            entries.Visible = false;
        }

        // We cannot render more than N time entries using winforms,
        // because we run out of window handles. As a temporary fix,
        // dont even attempt to render more than N time entries.
        int maxCount = Math.Min(200, list.Count);

        for (int i = 0; i < maxCount; i++)
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

        if (!entries.Visible)
        {
            entries.Visible = true;
        }
    }

    private void TimeEntryListViewController_Load(object sender, EventArgs e)
    {
    }

    void OnLogin(bool open, UInt64 user_id)
    {
        if (InvokeRequired)
        {
            Invoke((MethodInvoker)delegate {
                OnLogin(open, user_id);
            });
            return;
        }
        if (open || user_id == 0)
        {
            entries.SuspendLayout();
            entries.Controls.Clear();
            entries.ResumeLayout();
            entries.PerformLayout();
        }
    }

    private void entries_ClientSizeChanged(object sender, EventArgs e)
    {
        if (entries.Controls.Count > 0)
        {
            entries.SuspendLayout();
            entries.Controls[0].Width = entries.ClientSize.Width;
            entries.ResumeLayout();
        }
    }

    private void entries_MouseEnter(object sender, EventArgs e)
    {
        if (!timerEditViewController.focusList()) {
            entries.Focus();
        }
    }

    private void emptyLabel_Click(object sender, EventArgs e)
    {
        Toggl.OpenInBrowser();
    }

    internal FlowLayoutPanel getListing()
    {
        return entries;
    }

    internal Control findControlByGUID(string GUID)
    {
        if (timerEditViewController.durationFocused)
        {
            for (int i = 0; i < entries.Controls.Count; i++)
            {
                if ((entries.Controls[i] as TimeEntryCell).GUID == GUID)
                {
                    return entries.Controls[i];
                }
            }
        }
        return null;
    }

    internal void setEditPopup(EditForm editForm)
    {
        timerEditViewController.editForm = editForm;
    }
}
}

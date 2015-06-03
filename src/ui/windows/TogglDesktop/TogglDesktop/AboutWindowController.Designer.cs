﻿namespace TogglDesktop
{
partial class AboutWindowController
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindowController));
        this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
        this.labelAppName = new System.Windows.Forms.Label();
        this.labelVersion = new System.Windows.Forms.Label();
        this.labelReleaseChannel = new System.Windows.Forms.Label();
        this.comboBoxChannel = new System.Windows.Forms.ComboBox();
        this.label1 = new System.Windows.Forms.Label();
        this.linkLabelGithub = new System.Windows.Forms.LinkLabel();
        this.panelMain = new System.Windows.Forms.Panel();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
        this.panelMain.SuspendLayout();
        this.SuspendLayout();
        //
        // pictureBoxLogo
        //
        this.pictureBoxLogo.Anchor = System.Windows.Forms.AnchorStyles.Top;
        this.pictureBoxLogo.Image = global::TogglDesktop.Properties.Resources.logo64;
        this.pictureBoxLogo.Location = new System.Drawing.Point(117, 12);
        this.pictureBoxLogo.Name = "pictureBoxLogo";
        this.pictureBoxLogo.Size = new System.Drawing.Size(65, 68);
        this.pictureBoxLogo.TabIndex = 0;
        this.pictureBoxLogo.TabStop = false;
        //
        // labelAppName
        //
        this.labelAppName.Anchor = System.Windows.Forms.AnchorStyles.Top;
        this.labelAppName.AutoSize = true;
        this.labelAppName.Font = new System.Drawing.Font("Lucida Sans Unicode", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(186)));
        this.labelAppName.Location = new System.Drawing.Point(92, 91);
        this.labelAppName.Name = "labelAppName";
        this.labelAppName.Size = new System.Drawing.Size(116, 17);
        this.labelAppName.TabIndex = 1;
        this.labelAppName.Text = "TogglDesktop";
        this.labelAppName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        //
        // labelVersion
        //
        this.labelVersion.Anchor = System.Windows.Forms.AnchorStyles.Top;
        this.labelVersion.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
        this.labelVersion.Location = new System.Drawing.Point(110, 117);
        this.labelVersion.Name = "labelVersion";
        this.labelVersion.Size = new System.Drawing.Size(80, 15);
        this.labelVersion.TabIndex = 2;
        this.labelVersion.Text = "Version 1.0.0";
        this.labelVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        //
        // labelReleaseChannel
        //
        this.labelReleaseChannel.AutoSize = true;
        this.labelReleaseChannel.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
        this.labelReleaseChannel.Location = new System.Drawing.Point(13, 256);
        this.labelReleaseChannel.Name = "labelReleaseChannel";
        this.labelReleaseChannel.Size = new System.Drawing.Size(92, 15);
        this.labelReleaseChannel.TabIndex = 5;
        this.labelReleaseChannel.Text = "Release channel";
        //
        // comboBoxChannel
        //
        this.comboBoxChannel.AccessibleDescription = "Select which channel you want to receive the app updates from";
        this.comboBoxChannel.AccessibleName = "Release channel";
        this.comboBoxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.comboBoxChannel.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
        this.comboBoxChannel.FormattingEnabled = true;
        this.comboBoxChannel.Items.AddRange(new object[] {
            "stable",
            "beta",
            "dev"
        });
        this.comboBoxChannel.Location = new System.Drawing.Point(164, 253);
        this.comboBoxChannel.Name = "comboBoxChannel";
        this.comboBoxChannel.Size = new System.Drawing.Size(121, 23);
        this.comboBoxChannel.TabIndex = 6;
        this.comboBoxChannel.SelectedIndexChanged += new System.EventHandler(this.comboBoxChannel_SelectedIndexChanged);
        //
        // label1
        //
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
        this.label1.Location = new System.Drawing.Point(13, 148);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(224, 15);
        this.label1.TabIndex = 7;
        this.label1.Text = "Toggl Desktop is an open source project";
        //
        // linkLabelGithub
        //
        this.linkLabelGithub.AutoSize = true;
        this.linkLabelGithub.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
        this.linkLabelGithub.Location = new System.Drawing.Point(13, 176);
        this.linkLabelGithub.Name = "linkLabelGithub";
        this.linkLabelGithub.Size = new System.Drawing.Size(223, 15);
        this.linkLabelGithub.TabIndex = 8;
        this.linkLabelGithub.TabStop = true;
        this.linkLabelGithub.Text = "https://github.com/toggl/toggldesktop";
        this.linkLabelGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGithub_LinkClicked);
        //
        // panelMain
        //
        this.panelMain.Controls.Add(this.linkLabelGithub);
        this.panelMain.Controls.Add(this.pictureBoxLogo);
        this.panelMain.Controls.Add(this.label1);
        this.panelMain.Controls.Add(this.labelAppName);
        this.panelMain.Controls.Add(this.comboBoxChannel);
        this.panelMain.Controls.Add(this.labelVersion);
        this.panelMain.Controls.Add(this.labelReleaseChannel);
        this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panelMain.Location = new System.Drawing.Point(0, 0);
        this.panelMain.Name = "panelMain";
        this.panelMain.Size = new System.Drawing.Size(300, 288);
        this.panelMain.TabIndex = 0;
        //
        // AboutWindowController
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(300, 288);
        this.Controls.Add(this.panelMain);
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.MaximumSize = new System.Drawing.Size(300, 328);
        this.MinimumSize = new System.Drawing.Size(300, 328);
        this.Name = "AboutWindowController";
        this.Text = "About";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AboutWindowController_FormClosing);
        ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
        this.panelMain.ResumeLayout(false);
        this.panelMain.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBoxLogo;
    private System.Windows.Forms.Label labelAppName;
    private System.Windows.Forms.Label labelVersion;
    private System.Windows.Forms.Label labelReleaseChannel;
    private System.Windows.Forms.ComboBox comboBoxChannel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.LinkLabel linkLabelGithub;
    private System.Windows.Forms.Panel panelMain;
}
}
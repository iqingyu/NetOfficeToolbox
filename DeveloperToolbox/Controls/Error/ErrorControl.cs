﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetOffice.DeveloperToolbox.Controls.Error
{
    /// <summary>
    /// Control to display errors
    /// </summary>
    [ResourceTable("Resources.ErrorFormStrings.txt")]
    public partial class ErrorControl : UserControl, ILocalizationDesign
    {
        #region Fields

        private ErrorCategory _category;
        private int[] _columnSizes = new int[] { 25, 246, 112, 151 };

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        public ErrorControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// User want close the control
        /// </summary>
        public event EventHandler UserClose;

        private void RaiseUserClose()
        {
            if (null != UserClose)
            {
                UserClose(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Show exception in user frontend
        /// </summary>
        /// <param name="exception">exception to display</param>
        /// <param name="message">user friendly message</param>
        /// <param name="category"error category></param>
        /// <param name="currentLanguageID">user preferred lcid</param>
        internal void ShowError(Exception exception, string message, ErrorCategory category, int currentLanguageID)
        {
            _category = category;
            labelErrorMessage.Text = message;
            labelErrorMessage.Visible = true;
            if (ErrorCategory.Critical == category)
            {
                labelExitMessage.Visible = true;
            }
            DisplayException(exception);
            currentLanguageID = ValidateLanguageID(currentLanguageID);
            Translation.Translator.TranslateControls(this, "Resources.ErrorFormStrings.txt", currentLanguageID);
        }

        /// <summary>
        /// Show exception in user frontend
        /// </summary>
        /// <param name="exception">exception to display</param>
        /// <param name="category"error category></param>
        /// <param name="currentLanguageID">user preferred lcid</param>
        internal void ShowError(Exception exception, ErrorCategory category, int currentLanguageID)
        {
            _category = category;
            if (ErrorCategory.Critical == category)
            {
                labelExitMessage.Visible = true;
            }
            DisplayException(exception);
            currentLanguageID = ValidateLanguageID(currentLanguageID);
            Translation.Translator.TranslateControls(this, "Resources.ErrorFormStrings.txt", currentLanguageID);
        }

        private int ValidateLanguageID(int currentLanguageID)
        {
            switch (currentLanguageID)
            {
                case 1:
                    currentLanguageID = 1031;
                    break;
                default:
                    currentLanguageID = 1033;
                    break;

            }

            return currentLanguageID;
        }

        private void DisplayException(Exception exception)
        {
            int i = 1;
            while (exception != null)
            {
                ListViewItem viewItem = listViewTrace.Items.Add(i.ToString());
                viewItem.SubItems.Add(exception.Message);
                viewItem.SubItems.Add(exception.GetType().Name.ToString());
                if (null != exception.TargetSite)
                {
                    viewItem.SubItems.Add(exception.TargetSite.ToString());
                }
                else
                {
                    viewItem.SubItems.Add("");
                }
                viewItem.Tag = exception;
                exception = exception.InnerException;
                i++;
            }
        }

        #endregion

        #region ILocalizationDesign

        public void EnableDesignView(int lcid, string parentComponentName)
        {
            labelErrorMessage.Visible = true;
            labelExitMessage.Visible = true;
        }

        public void Localize(Translation.ItemCollection strings)
        {
            Translation.Translator.TranslateControls(this, strings);
        }

        public void Localize(string name, string text)
        {
            Translation.Translator.TranslateControl(this, name, text);
        }

        public string GetCurrentText(string name)
        {
            return Translation.Translator.TryGetControlText(this, name);
        }

        public System.ComponentModel.IContainer Components
        {
            get
            {
                return components;
            }
        }

        public string NameLocalization
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<ILocalizationChildInfo> Childs
        {
            get
            {
                return new ILocalizationChildInfo[0];
            }
        }

        #endregion

        #region Trigger

        private void ErrorControl_Resize(object sender, EventArgs e)
        {
            try
            {
                columnHeaderSpace.Width = _columnSizes[0];
                columnHeaderType.Width = _columnSizes[2];
                columnHeaderSource.Width = _columnSizes[3];
                columnHeaderMessage.Width = listViewTrace.Width - (_columnSizes[3] + _columnSizes[2] + _columnSizes[0] + _columnSizes[0]);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                RaiseUserClose();
                if (ErrorCategory.Critical == _category)
                {
                    Application.Exit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void buttonCopyToClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                string clipboardContent = "";

                foreach (ListViewItem item in listViewTrace.Items)
                {
                    clipboardContent += item.SubItems[0].Text + " | " + item.SubItems[1].Text + " | " + item.SubItems[2].Text + " | " + item.SubItems[3].Text + Environment.NewLine;
                }

                Clipboard.SetData(DataFormats.Text, clipboardContent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void linkLabelDiscussionBoard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start((sender as LinkLabel).Tag as string);
            }
            catch
            {
                ;
            }
        }

        private void listViewTrace_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (listViewTrace.SelectedItems.Count > 0)
                {
                    Exception exception = listViewTrace.SelectedItems[0].Tag as Exception;
                    if (null != exception)
                    {
                        string details = String.Format("{0}{2}{2}{1}", exception.Message, exception, Environment.NewLine);
                        MessageBox.Show(this, details, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}

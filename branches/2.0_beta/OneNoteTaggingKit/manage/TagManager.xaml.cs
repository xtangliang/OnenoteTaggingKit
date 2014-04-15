﻿using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using WetHatLab.OneNote.TaggingKit.common;
using WetHatLab.OneNote.TaggingKit.common.ui;

namespace WetHatLab.OneNote.TaggingKit.manage
{
    /// <summary>
    /// Interaction logic for TagManager.xaml user control
    /// </summary>
    /// <remarks>Implements the tag management dialog logic</remarks>
    public partial class TagManager : Window, IOneNotePageWindow<TagManagerModel>
    {
        private TagManagerModel _model;

        /// <summary>
        /// Create a new instance of a tag management dialog.
        /// </summary>
        public TagManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add a new tag to the list of suggestions when the button is pressed
        /// </summary>
        /// <param name="sender">control emitting the event</param>
        /// <param name="e">event details</param>
        private void NewTagButton_Click(object sender, RoutedEventArgs e)
        {
            _model.SuggestedTags.AddAll(from t in tagInput.Tags where !_model.SuggestedTags.ContainsKey(t) select new RemovableTagModel(new TagPageSet(t)));
            tagInput.Clear();
            e.Handled = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _model.SaveChanges();
            e.Handled = true;
            DialogResult = true;
        }

        #region IOneNotePageDialog<TagManagerModel>
        /// <summary>
        /// Get or set the dialog's view model.
        /// </summary>
        /// <remarks>As soon as the view model is defined a background collection of tags is started</remarks>
        public TagManagerModel ViewModel
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                DataContext = _model;
            }
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (_model != null)
            {
                _model.Dispose();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;

            string navigateUri = hl.NavigateUri.ToString();

            Process.Start(new ProcessStartInfo(navigateUri));  

            e.Handled = true;
        }

        private void Copy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, _model.TagList);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_model != null)
            {
                await _model.LoadTagSuggestionsAsync();
                pBar.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void TagInputBox_Input(object sender, TagInputEventArgs e)
        {
            if (e.TagInputComplete)
            {
                NewTagButton_Click(sender, e);
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
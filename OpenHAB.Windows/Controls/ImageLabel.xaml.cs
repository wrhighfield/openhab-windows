﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Text.RegularExpressions;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Control that represents an image with a label
    /// </summary>
    public sealed partial class ImageLabel : UserControl
    {
        /// <summary>
        /// Bindable property for the control icon
        /// </summary>
        public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register(
            "IconPath", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), IconChangedCallback));

        private static void IconChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ImageLabel)dependencyObject;
            if (control == null)
            {
                return;
            }

            // fix IconPathState by removeing empty space and special characters
            string iconPath = control.IconPath;
            Match state = Regex.Match(iconPath, @"state=(.+?)&");
            if (state != null)
            {
                if (!string.IsNullOrEmpty(state.Value))
                {
                    string newstate = Regex.Replace(state.Groups[1].Value, "[^0-9a-zA-Z.&]", string.Empty);
                    iconPath = control.IconPath.Replace(state.Groups[1].Value, newstate, StringComparison.InvariantCulture);
                }
            }

            control.Icon.Source = new BitmapImage(new Uri(iconPath));
        }

        /// <summary>
        /// Gets or sets the IconPath
        /// </summary>
        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value ?? string.Empty);
        }

        /// <summary>
        /// Bindable property for the label
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), TextChangedCallback));

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        public string LabelText
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value ?? string.Empty);
        }

        private static void TextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ImageLabel)dependencyObject;

            if (control == null)
            {
                return;
            }

            control.Label.Text = control.LabelText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLabel"/> class.
        /// </summary>
        public ImageLabel()
        {
            InitializeComponent();
        }
    }
}

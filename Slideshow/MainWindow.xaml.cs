// Slideshow
// Copyright (C) 2013  Andrei Nicholson
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

// https://github.com/waveface/ExifLibrary-for-NET

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ExifLib;

namespace Slideshow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int imageDurationSeconds = 10;
        private int imageDurationOffset = 4;
        private List<Media> images = new List<Media>();
        private int currentImageIndex = 0;
        private DispatcherTimer imageTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            this.Cursor = Cursors.None;
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            LoadImages();

            imageTimer.Tick += new EventHandler(timer_Tick);
            imageTimer.Interval = new TimeSpan(0, 0, 1);
            imageTimer.Start();
        }

        private void Toast(string notification)
        {
            AppInfo.Content = notification;
            AppInfo.Visibility = Visibility.Visible;

            DispatcherTimer toastTimer = new DispatcherTimer();
            toastTimer.Interval = new TimeSpan(0, 0, 2);
            toastTimer.Tick += (EventHandler)delegate(object snd, EventArgs ea)
            {
                AppInfo.Visibility = Visibility.Collapsed;
                ((DispatcherTimer)snd).Stop();
            };
            toastTimer.Start();
        }

        /// <summary>
        /// In-application controls
        /// </summary>
        /// <remarks>
        /// Development machine registered the plus and minus keys as OEM
        /// keys. This was not using the number pad.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;

                case Key.Add:
                // Break intentionally omitted.
                case Key.OemPlus:
                    imageDurationSeconds += imageDurationOffset;
                    imageTimer.Interval = new TimeSpan(0, 0, imageDurationSeconds);
                    Toast(String.Format("Increased duration to {0} seconds", imageDurationSeconds));
                    break;

                case Key.Subtract:
                // Break intentionally omitted.
                case Key.OemMinus:
                    if (imageDurationSeconds - imageDurationOffset < 1)
                    {
                        break;
                    }

                    imageDurationSeconds -= imageDurationOffset;
                    imageTimer.Interval = new TimeSpan(0, 0, imageDurationSeconds);
                    Toast(String.Format("Decreased duration to {0} seconds", imageDurationSeconds));
                    break;

                case Key.Right:
                    ChangeImageIndex(1);
                    DisplayImage(false);
                    imageTimer.Interval = new TimeSpan(0, 0, imageDurationSeconds);
                    break;

                case Key.Left:
                    ChangeImageIndex(-1);
                    DisplayImage(false);
                    imageTimer.Interval = new TimeSpan(0, 0, imageDurationSeconds);
                    break;

                default:
                    break;
            }
        }

        private void DisplayImage(bool advanceImageIndex = true)
        {
            using (FileStream imageStream = new FileStream(images[currentImageIndex].File.FullName, FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite))
            {
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.SetLength(imageStream.Length);
                imageStream.Read(memoryStream.GetBuffer(), 0, (int)imageStream.Length);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();

                memoryStream.Flush();

                ImagePresented.Source = bitmapImage;

                imageStream.Seek(0, SeekOrigin.Begin);
                ReadExifInfo(imageStream);
            }

            List<string> info = new List<string>
            {
                images[currentImageIndex].PrimaryText,
                images[currentImageIndex].SecondaryText
            };

            this.ImageInfo.Content = String.Join("\n", info);

            if (advanceImageIndex)
            {
                ChangeImageIndex(1);
            }
        }

        private void ChangeImageIndex(int offset)
        {
            currentImageIndex += offset;

            if (currentImageIndex >= images.Count)
            {
                currentImageIndex = 0;
            }
            else if (currentImageIndex < 0)
            {
                currentImageIndex = images.Count - 1;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (imageTimer.Interval.Seconds != imageDurationSeconds)
            {
                imageTimer.Interval = new TimeSpan(0, 0, imageDurationSeconds);
            }

            DisplayImage();
        }

        private void ReadExifInfo(FileStream imageStream)
        {
            if (images[currentImageIndex].PrimaryText != null && images[currentImageIndex].SecondaryText != null)
            {
                return;
            }

            try
            {
                ExifReader exifReader = new ExifReader(imageStream);

                string exifValue;

                if (images[currentImageIndex].PrimaryText == null)
                {
                    exifReader.GetTagValue<string>(ExifTags.Artist, out exifValue);
                    images[currentImageIndex].PrimaryText = exifValue;
                }

                if (images[currentImageIndex].SecondaryText == null)
                {
                    exifReader.GetTagValue<string>(ExifTags.ImageDescription, out exifValue);
                    images[currentImageIndex].SecondaryText = exifValue;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Store paths to all applicable images in current directory
        /// </summary>
        private void LoadImages()
        {
            string[] validExtensions = new string[] { ".jpg", ".jpeg" };
            DirectoryInfo directory = new DirectoryInfo(GetAssemblyDirectory());
            IEnumerable<FileInfo> files = from f in directory.EnumerateFiles()
                                          where validExtensions.Any(f.Extension.ToLower().Contains)
                                          select f;

            files.ToList().ForEach(x => images.Add(new Media { File = x }));
            Toast("Found " + images.Count.ToString() + " images for slideshow");
        }

        /// <summary>
        /// Full directory path to this executable
        /// </summary>
        /// <returns></returns>
        private string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string assemblyPath = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(assemblyPath);
        }
    }
}

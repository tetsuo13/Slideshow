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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
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
        /// <summary>
        /// Number of seconds to show each image.
        /// </summary>
        private int imageDurationSeconds = 10;

        /// <summary>
        /// Number of seconds to increment/decrement image duration at a time.
        /// </summary>
        private int imageDurationOffset = 4;

        /// <summary>
        /// Collection of images that were found and their properties.
        /// </summary>
        private List<Media> images = new List<Media>();

        private int currentImageIndex = 0;
        private DispatcherTimer imageTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            this.Cursor = Cursors.None;
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            LoadImages();

            if (images.Count > 0)
            {
                imageTimer.Tick += new EventHandler(timer_Tick);
                imageTimer.Interval = new TimeSpan(0, 0, 1);
                imageTimer.Start();
            }
        }

        /// <summary>
        /// Flash a notification message for a few seconds.
        /// </summary>
        /// <param name="notification">Message to display.</param>
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
        /// keys.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
                return;
            }

            if (images.Count == 0)
            {
                return;
            }

            switch (e.Key)
            {
                // Increment image duration.
                case Key.Add:
                // Break intentionally omitted.
                case Key.OemPlus:
                    imageDurationSeconds += imageDurationOffset;
                    imageTimer.Interval = new TimeSpan(0, 0, imageDurationSeconds);
                    Toast(String.Format("Increased duration to {0} seconds", imageDurationSeconds));
                    break;

                // Decrement image duration.
                case Key.Subtract:
                // Break intentionally omitted.
                case Key.OemMinus:
                    if (imageDurationSeconds <= 1)
                    {
                        break;
                    }
                    if (imageDurationSeconds >= (imageDurationOffset * 2))
                    {
                        imageDurationSeconds -= imageDurationOffset;
                    }
                    else
                    {
                        imageDurationSeconds /= 2;
                    }
                    imageTimer.Interval = new TimeSpan(0, 0, imageDurationSeconds);
                    Toast(String.Format("Decreased duration to {0} seconds", imageDurationSeconds));
                    break;

                // Show next image immediately.
                case Key.Right:
                    ChangeImageIndex(1);
                    DisplayImage(false);
                    imageTimer.Interval = new TimeSpan(0, 0, imageDurationSeconds);
                    break;

                // Show previous image immediately.
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

            DisplayImageInfo();

            if (advanceImageIndex)
            {
                ChangeImageIndex(1);
            }
        }

        private void DisplayImageInfo()
        {
            this.ImageInfo.Content = images[currentImageIndex].InfoText();
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
            if (!String.IsNullOrEmpty(images[currentImageIndex].InfoText()))
            {
                return;
            }

            try
            {
                ExifReader exifReader = new ExifReader(imageStream);
                images[currentImageIndex].Info.ForEach(x => x.SetValue(exifReader));
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
            DirectoryInfo directory;
            if (Slideshow.App.dragNdropArg.Length > 0)
            {
                directory = new DirectoryInfo(GetSelectedDirectory(Slideshow.App.dragNdropArg));
            }
            else
            {
                directory = new DirectoryInfo(GetAssemblyDirectory());
            }
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

        private string GetSelectedDirectory(string arg)
        {
            UriBuilder uri = new UriBuilder(arg);
            string argPath = Uri.UnescapeDataString(uri.Path);
            if (Directory.Exists(argPath))
            {
                return argPath;
            }
            return Path.GetDirectoryName(argPath);
        }
    }
}

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
using ExifLib;

namespace Slideshow
{
    class Media
    {
        public FileInfo File { get; set; }

        /// <summary>
        /// Collection of EXIF tags that will comprise the text info. Order matters.
        /// </summary>
        public List<MediaInfo> Info { get; private set; }

        public Media()
        {
            Info = new List<MediaInfo>()
            {
                new MediaInfo(ExifTags.Artist),
                new MediaInfo(ExifTags.ImageDescription)
            };
        }

        /// <summary>
        /// Concatenate all concerned EXIF tags each on a line.
        /// </summary>
        /// <returns></returns>
        public string InfoText()
        {
            return String.Join("\n", Info.Select(x => x.TagValue)
                .Where(x => !String.IsNullOrEmpty(x))
                .ToArray());
        }
    }
}

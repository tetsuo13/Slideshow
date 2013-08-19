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

using ExifLib;

namespace Slideshow
{
    public class MediaInfo
    {
        public ExifTags Tag { get; set; }
        public string TagValue { get; private set; }

        public MediaInfo(ExifTags tag)
        {
            Tag = tag;
        }

        public void SetValue(ExifReader reader)
        {
            string tmp;
            reader.GetTagValue<string>(Tag, out tmp);
            TagValue = tmp;
        }
    }
}

using ExifLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            return string.Join("\n", Info.Select(x => x.TagValue)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray());
        }
    }
}

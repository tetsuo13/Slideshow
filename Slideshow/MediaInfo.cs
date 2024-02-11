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
            reader.GetTagValue(Tag, out string tmp);
            TagValue = tmp;
        }
    }
}

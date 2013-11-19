Slideshow
=========

Slideshow is an extremely simplistic slideshow application for Windows. The
aim of this project was to provide a way in Windows to show a slideshow of
images in a classroom setting with as little effort as possible.

The other requirement was to be able to show some text on each of the images
shown. This is done using the tags stored with images, specifically author and
title tags. After editing the tags of each image to show the person and
location, for example, Slideshow is then able to do what would normally be
done with PowerPoint in a fraction of the time.

The work done is also transferable to other computers: edit the tags of the
images, copy the images along with Slideshow to a thumbdrive, and use it
anywhere without having to install anything.

## Requirements ##

A Windows XP/Vista/7/8 machine with a minimum of .NET Framework 4.

## Setup ##

Download the latest version of Slideshow from the
[releases page](https://github.com/tetsuo13/Slideshow/releases). Save the
file in the same folder as the images you wish to use. From here you can start
Slideshow and it will simply display all images it finds (JPG or JPEG).

If you want to show any text along with the images you'll need to edit the
EXIF information for each image. There are dozens of applications freely
available online that will help but Windows also provides a way when bringing
up the properties of an image).

Currently, the only tags that are used are:

* Authors
* Title

When one or both of these tags are populated, they are shown at the
bottom-right of the screen with the image. The order shown above is the same
order as they are shown, with each on a separate line.

## Usage ##

By default each image is shown for a short period of time and then the next
image is shown, with the entire set of images looping. Certain keys perform
different functions however:

* `<Escape>`: Exit Slideshow.
* `<Left Arrow>`: Go to previous image.
* `<Right Arrow>`: Go to next image.
* `<Plus>`: Increase amount of time spent on each image. There is no limit to
  how large this can be. If you want to effectively pause on an image, hold
  down this key until a large amount of seconds is used.
* `<Minus>`: Decrease amount of time spent on each image. One second is the
  minimum amount of time required.

## Contributing ##

There are multiple areas where you can contribute to Slideshow. If you'd like
to make a feature request to satisfy a requirement to meet your needs, please
submit an issue request as it's likely your feature would benefit others using
Slideshow as well. To also make Slideshow better for everyone you can also
submit bugs.

To view existing issues please visit
[https://github.com/tetsuo13/Slideshow/issues](https://github.com/tetsuo13/Slideshow/issues)

If all else fails please drop the main developer an email at
[contact@andreinicholson.com](mailto:contact@andreinicholson.com)

## Development Notes ##

Project embeds
[ExifLib - A Fast Exif Data Extractor for .NET 2.0+](http://www.codeproject.com/Articles/36342/ExifLib-A-Fast-Exif-Data-Extractor-for-NET-2-0)
by [Simon McKenzie](http://www.codeproject.com/Members/SimonMcKenzie)
for all EXIF extractions.
Embedding the source within the project over using the
[NuGet](https://www.nuget.org/packages/ExifLib) package was chosen to keep
the final binary distribution down to one file.

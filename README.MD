# cur2png

Requires .NET 4.6 and C# 7

> This tool allows you to convert cursor (.cur) files into png files.

## Features 

* Supports download by specifing a folder by CLI.
* It takes care of current folder hierarchy.

### Supported platforms

Only Windows (Vista or greater).

## Usage

Call this program on a shell/terminal with thre `-f` parameter to specify a folder and this will generate.

**Example:**

```cs
$ cur2png -f "<folder to analyze>"
```

## TODO LIST

* Implement CLI (batch download, toggle hierarchy feature, convert type (jpg, bmp, png, dxt, tga or tiff), allow conversion from ani to gif (ANI MODE)))

* Implement xcur2png on this project. (https://github.com/eworm-de/xcur2png)

* Implement json serialization to allow design model classes to store data.

* Merge with Texture Packer and create a new tool.

## Issues

Having issues? Just report in [the issue section](/issues). **Thanks for the feedback!**

## Contribute

Fork this repository, make your changes and then issue a pull request. If you find bugs or have new ideas that you do not want to implement yourself, file a bug report.

## Donate

Become a patron, by simply clicking on this button (**very appreciated!**):

[![](https://c5.patreon.com/external/logo/become_a_patron_button.png)](https://www.patreon.com/z3nth10n)

... Or if you prefer a one-time donation:

[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/z3nth10n)

## Copyright

Copyright (c) 2019 z3nth10n (United Teamwork Association).

License: MIT
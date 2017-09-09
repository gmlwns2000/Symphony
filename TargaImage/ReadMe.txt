// ==========================================================
// TargaImage
//
// Design and implementation by
// - David Polomis (paloma_sw@cox.net)
//
//
// This source code, along with any associated files, is licensed under
// The Code Project Open License (CPOL) 1.02
// A copy of this license can be found in the CPOL.html file 
// which was downloaded with this source code
// or at http://www.codeproject.com/info/cpol10.aspx
//
// 
// COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS,
// WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,
// INCLUDING, WITHOUT LIMITATION, WARRANTIES THAT THE COVERED CODE IS
// FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE OR
// NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE
// OF THE COVERED CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE
// DEFECTIVE IN ANY RESPECT, YOU (NOT THE INITIAL DEVELOPER OR ANY
// OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY SERVICING,
// REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN
// ESSENTIAL PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS
// AUTHORIZED HEREUNDER EXCEPT UNDER THIS DISCLAIMER.
//
// Use at your own risk!
//
// ==========================================================


.NET Targa Image Reader
Current: C# (C# 1.0, C# 2.0, C# 3.0, C#), Windows (Windows, WinXP), .NET (.NET, .NET 3.5, .NET 3.0, .NET 2.0), GDI+, WebForms, VS2008, Dev, Intermediate


Loads Targa image files into a Bitmap using pure .NET code

TargaImage is availble on CodeProject.com

http://www.codeproject.com/KB/GDI-plus/dotnettargareader.aspx


TargaImage was created with Visual Studio 2008 Standard using C# 3.0 and the .NET Framework 2.0


To use TargaImage copy the TargaImage.dll file from the Release folder in the Bin folder to your own project.
Then include a reference to TargaImage.dll in your project.

To load a targa image call the LoadTargaImage() method of the Paloma.TargaImage class.
Or if you want access to the image properties create an instance of the TargaImage class.

EXAMPLES:
    

    //   C# Sample   
    //   Loads a targa image and assigns it to the Image of a picturebox control.
    this.PictureBox1.Image = Paloma.TargaImage.LoadTargaImage(@"c:\targaimage.tga");
    
    //   Creates an instance of the TargaImage class with the specifed file
    //   displays a few targa properties and then assigns the targa image
    //   to the Image of a picturebox control
    Paloma.TargaImage tgaImage = new Paloma.TargaImage(@"c:\targaimage.tga");
    this.Label1.Text = tgaImage.Format.ToString();
    this.Label2.Text = tgaImage.Header.ImageType.ToString();
    this.Label3.Text = tgaImage.Header.PixelDepth.ToString();
    this.PictureBox1.Image = Paloma.TargaImage.Image;
    
    
    
    '   VB.NET Sample 
    '   Loads a targa image and assigns it to the Image of a picturebox control.
    Me.PictureBox1.Image = Paloma.TargaImage.LoadTargaImage("c:\targaimage.tga")

    
    '   Creates an instance of the TargaImage class with the specifed file
    '   displays a few targa properties and then assigns the targa image
    '   to the Image of a picturebox control
    Dim tgaImage As New Paloma.TargaImage("c:\targaimage.tga")
    Me.Label1.Text = tgaImage.Format.ToString()
    Me.Label2.Text = tgaImage.Header.ImageType.ToString()
    Me.Label3.Text = tgaImage.Header.PixelDepth.ToString()
    Me.PictureBox1.Image = Paloma.TargaImage.Image


MaterialSkin for .NET WinForms
=====================

Theming .NET WinForms, C# or VB.Net, to Google's Material Design Principles.

<a href="https://www.youtube.com/watch?v=A8osVM_SXlg" target="_blank">![alt tag](http://i.imgur.com/JAttoOo.png)</a>

---

#### Current state of the MaterialSkin components
 | Supported | Dark & light version | Disabled mode | Animated
--- | --- | --- | --- | ---
Checkbox | Yes | Yes | Yes | Yes 
Divider | Yes | Yes | N/A | N/A 
Flat Button | Yes | Yes | Yes | Yes 
Label | Yes | Yes | N/A | N/A
Radio Button | Yes | Yes | Yes | Yes
Raised Button | Yes | Yes | Yes | Yes 
Single-line text field | Yes | Yes | No | Yes
TabControl | Yes | N/A | N/A | Yes
ContextMenuStrip | Yes | Yes | Yes | Yes
FloatingActionButton | No | No | No | No
ListView | No | No | No | No
Dialogs | No | No | No | No
Switch | No | No | No | No
More... | No | No | No | No

---

#### Implementing MaterialSkin in your application

**1. Add the library to your project**

  You can do this on multiple ways. The easiest way would be adding the binary as found in [releases](https://github.com/IgnaceMaes/MaterialSkin/releases). Right click on your project and add the library as reference. I highly recommand to enable copy local so you always have the lib in your build folder.
  
  Another way of doing this step would be downloading the project from GitHub, compiling the library yourself and adding it as a reference.
  
**2. Add the MaterialSkin components to your ToolBox**

  Simply drag the MaterialSkin.dll file into your IDE's ToolBox and all the controls should be added there.
  
**3. Inherit from MaterialForm**

  Open the code behind your Form you wish to skin. Make it inherit from MaterialForm rather than Form. Don't forget to put the library in your imports, so it can find the MaterialForm class!
  
  C# (Form1.cs)
  ```cs
  public partial class Form1 : MaterialForm
  ```
  
  VB.NET (Form1.Designer.vb)
  ```vb
  Partial Class Form1
    Inherits MaterialSkin.Controls.MaterialForm
  ```
  
**4. Initialize your colorscheme**

  Set your prefered colors & theme. Also add the form to the manager so it keeps updated if the color scheme or theme changes later on.

C# (Form1.cs)
  ```cs
  public Form1()
  {
      InitializeComponent();

      //Initialize MaterialSkinManager (example in C#)
      var materialSkinManager = MaterialSkinManager.Instance;
      materialSkinManager.AddFormToManage(this);
      materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
      materialSkinManager.PrimaryColor = Color.FromArgb(63, 81, 181);
      materialSkinManager.PrimaryColorDark = Color.FromArgb(48, 63, 159);
      materialSkinManager.AccentColor = Color.FromArgb(255, 64, 129);
  }
  ```

VB.NET (Form1.vb)
```vb
Imports MaterialSkin

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim SkinManager As MaterialSkinManager = MaterialSkinManager.Instance
        SkinManager.AddFormToManage(Me)
        SkinManager.Theme = MaterialSkinManager.Themes.LIGHT
        SkinManager.PrimaryColor = Color.FromArgb(63, 81, 181)
        SkinManager.PrimaryColorDark = Color.FromArgb(48, 63, 159)
        SkinManager.AccentColor = Color.FromArgb(255, 64, 129)
    End Sub
End Class
```

---

#### Contact

If you wish to contact me for anything, hit me up @

- Twitter: https://twitter.com/Ignace_Maes
- Google+: https://google.com/+IgnaceMaes
- Facebook: https://www.facebook.com/ignace.maes
- Personal Website: http://ignacemaes.com

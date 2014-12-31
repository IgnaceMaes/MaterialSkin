MaterialSkin for .NET WinForms
=====================

Theming .NET WinForms, C# or VB.Net, to Google's Material Design Principles.

![alt tag](http://i.imgur.com/f0QLfpa.gif)

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
ContextMenuStrip | No | No | No | No
FloatingActionButton | No | No | No | No
ListView | No | No | No | No
Dialogs | No | No | No | No
Switch | No | No | No | No
More... | No | No | No | No

---

#### Implementing MaterialSkin in your application

**1. Add the library to your project**

  You can do this on 2 ways. The easiest way would be adding the NuGet Package. Right click on your project and click 'Manage NuGet Packages...'. Search for 'MaterialSkin' and click on install. Once installed the library will be included in your project references.
  
  Another way of doing this step would be downloading the project from GitHub, compiling the library yourself and adding it as a reference.
  
**2. Add the MaterialSkin components to your ToolBox**

  If you have installed the NuGet package, the MaterialSkin.dll file should be in the folder <Solution>/<Project>/bin/Debug. Simply drag the MaterialSkin.dll file into your IDE's ToolBox and all the controls should be added there.
  
**3. Inherit from MaterialForm**

  Open the code behind your Form you wish to skin. Make it inherit from MaterialForm rather than Form. Don't forget to put the library in your imports, so it can find the MaterialForm class!
  ```cs
  public partial class Form1 : MaterialForm //Example in C#
  ```
  
**4. Initialize your colorscheme**

  Set your prefered colors & theme. Also add the form to the manager so it keeps updated if the color scheme or theme changes later on.

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

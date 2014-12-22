WinForms MaterialSkin
=====================

Theming WinForms (C# or VB.Net) to Google's Material Design Principles.

---

#### Current state of the MaterialSkin components
 | Supported | Dark & light version | Disabled mode | Animated
--- | --- | --- | --- | ---
Checkbox | Yes | Yes | Yes | No 
Divider | Yes | Yes | N/A | N/A 
Flat Button | Yes | Yes | Yes | Yes 
Label | Yes | Yes | N/A | N/A
Radio Button | Yes | Yes | Yes | Yes
Raised Button | Yes | Yes | Yes | Yes 
Single-line text field | Yes | Yes | No | Yes

Light Theme | Dark Theme
--- | ---
![alt tag](http://puu.sh/dCnJ0/1a09990e52.png) | ![alt tag](http://puu.sh/dCnNa/96861a2fdf.png)

---

#### Implementing MaterialSkin in your application (C#)

**1. Download the library from GitHub**

  You only need the library, not the example. ([As found here](https://github.com/IgnaceMaes/WinForms-MaterialSkin/tree/master/MaterialSkin))
  
**2. Compile the library**

  Open the library in Visual Studio (or whatever IDE you use for C#) and compile the library. If succesfull you now have a dll file.
  
**3. Add the library as a reference in the project you wish to skin**

  In Visual Studio this can be done by right clicking your project and select Add>Reference. Now browse to the dll you've just compiled and click add.

**4. Rebuild your project**

  The MaterialSkin components should now appear in your ToolBox.
  
**5. Inherit from MaterialForm**

  Open the code behind your Form you wish to skin. Make it inherit from MaterialForm rather than Form. (Don't forget to import the library)
  ```cs
  public partial class Form1 : MaterialForm // Example in C#
  ```
  
**6. Initialize your colorscheme**

  Set your prefered colors & theme. Also add the form to the manager so it keeps updated if the color scheme or theme changes later on.

  ```cs
  public Form1()
  {
      InitializeComponent();

      // Initialize MaterialSkinManager (example in C#)
      var materialSkinManager = MaterialSkinManager.Instance;
      materialSkinManager.AddFormToManage(this);
      materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
      materialSkinManager.PrimaryColor = Color.FromArgb(63, 81, 181);
      materialSkinManager.PrimaryColorDark = Color.FromArgb(48, 63, 159);
      materialSkinManager.AccentColor = Color.FromArgb(255, 64, 129);
  }
  ```

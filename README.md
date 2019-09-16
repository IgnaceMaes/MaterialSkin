# MaterialSkin for .NET WinForms

Theming .NET WinForms, C# or VB.Net, to Google's Material Design Principles.

*A video of the old version is still available here:*

<a href="https://www.youtube.com/watch?v=A8osVM_SXlg" target="_blank">![Material Skin Video - old](http://i.imgur.com/JAttoOo.png)</a>

*High quality images can be found at the bottom of this page.*

**Current version:**

![Home](https://user-images.githubusercontent.com/8310271/64971649-609ea780-d87e-11e9-97eb-7a1e047fe8db.png)

---

## Updates

* [2019-09-13] New controls added, some bugfixes.
* [2018-12-24] New controls added 
* [2018-12-26] Added support to theme / color non Material controls on a Material form
* [2018-12-27] Added Messagebox control

## BugFixes

* New controls now use this to define the background color they should use
* Fixed rendering bug when Theme is changed
* Updated MaterialMultiLineTextBox to use richtext for rendering
* Updated the MaterialMessageBox control to now use MaterialMultiLineTextBox
* Updated the MaterialMessageBox control to properly calculate the spacing for controls on the form .

## New Controls

* Combobox
* Checkedlistbox 
* Floating Action Button
* Flexible Messagebox
* Switch
* Drawer

## State of the project

> This project is **ACTIVE**

## Current state of the MaterialSkin components

| Component                    | Supported | Dark Theme | Disabled mode | Animated  |
| ---------------------------- | :-------: | :--------: | :-----------: | :-------: |
| Buttons                      |    Yes    |    Yes     |      Yes      |    Yes    |
| Backdrop                     |  **No**   |     -      |       -       |     -     |
| Cards                        |  **No**   |     -      |       -       |     -     |
| Check Box                    |    Yes    |    Yes     |      Yes      |    Yes    |
| Check Box List               |    Yes    |    Yes     |      Yes      |    Yes    |
| Combobox                     |    Yes    |    Yes     |      Yes      |    Yes    |
| Context Menu                 |    Yes    |    Yes     |      Yes      |    Yes    |
| Dialog                       |  **No**   |     -      |       -       |     -     |
| Divider                      |    Yes    |    Yes     |      N/A      |    N/A    |
| Drawer                       |    Yes    |    Yes     |      N/A      |    Yes    |
| Flexible Dialog (big)        |    Yes    |    Yes     |      Yes      |    N/A    |
| FAB - Floating Action Button |    Yes    |    Yes     |    **No**     |    Yes    |
| Label                        |    Yes    |    Yes     |      N/A      |    N/A    |
| ListView                     |    Yes    |    Yes     |    **No**     |    N/A    |
| Progress Bar                 | _Partial_ |    Yes     |    **No**     |  **No**   |
| Radio Button                 |    Yes    |    Yes     |      Yes      |    Yes    |
| Text field                   | _Partial_ |    Yes     |    **No**     | _Partial_ |
| Sliders                      |  **No**   |     -      |       -       |     -     |
| Switch                       |    Yes    |    Yes     |      Yes      |    Yes    |
| Tabs                         |    Yes    |    N/A     |      N/A      |    Yes    |

## Planned components and improvements

1. Improved text field
2. Progress bar - Animation, circular and variants
3. Sliders
4. Dialog
5. Cards
6. Backdrop

---

## Implementing MaterialSkin in your application

### 1. Add the library to your project

> This will be available as a updated nuget package, but I'm still working on this part

A way of doing this step is by cloning the project from GitHub, compiling the library yourself and adding it as a reference.

Precompiled DLL is available at the releases section
  
### 2. Add the MaterialSkin components to your ToolBox

Simply drag the MaterialSkin.dll file into your IDE's ToolBox and all the controls should be added there.

### 3. Inherit from MaterialForm

Open the code behind your Form you wish to skin. Make it inherit from MaterialForm rather than Form. Don't forget to put the library in your imports, so it can find the MaterialForm class!
  
#### C# (Form1.cs)

```cs
public partial class Form1 : MaterialForm
```
  
#### VB.NET (Form1.Designer.vb)

```vb
Partial Class Form1
  Inherits MaterialSkin.Controls.MaterialForm
```
  
### 4. Initialize your colorscheme

Set your preferred colors & theme. Also add the form to the manager so it keeps updated if the color scheme or theme changes later on.

#### C# (Form1.cs)

```cs
public Form1()
{
    InitializeComponent();

    var materialSkinManager = MaterialSkinManager.Instance;
    materialSkinManager.AddFormToManage(this);
    materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
    materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
}
```

#### VB.NET (Form1.vb)

```vb
Imports MaterialSkin

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim SkinManager As MaterialSkinManager = MaterialSkinManager.Instance
        SkinManager.AddFormToManage(Me)
        SkinManager.Theme = MaterialSkinManager.Themes.LIGHT
        SkinManager.ColorScheme = New ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE)
    End Sub
End Class
```

---

## Material Design in WPF

If you love .NET and Material Design, you should definitely check out [Material Design Xaml Toolkit](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit) by ButchersBoy. It's a similar project but for WPF instead of WinForms.

## Contact

If you have any issues please open an issue or pull request. 

This project was forked from [donaldsteele/MaterialSkin](https://github.com/donaldsteele/MaterialSkin) and he forked it from [IgnaceMaes/MaterialSkin](https://github.com/IgnaceMaes/MaterialSkin)

---

## Images

![Home](https://user-images.githubusercontent.com/8310271/64971649-609ea780-d87e-11e9-97eb-7a1e047fe8db.png)
*A simple demo interface with MaterialSkin components.*

![Drawer](https://user-images.githubusercontent.com/8310271/64971650-609ea780-d87e-11e9-96c6-bc0630da366d.png)
*The MaterialSkin Drawer (menu).*

![Buttons](https://user-images.githubusercontent.com/8310271/64971652-609ea780-d87e-11e9-82ab-3359bd30f4e4.png)
*Every MaterialSkin button variant - this is 1 control, 3 properties*

![Checkbox and Swicthes](https://user-images.githubusercontent.com/8310271/64971656-609ea780-d87e-11e9-92f3-4c20390e8a3c.png)
*The MaterialSkin checkboxes and Switch.*

![Radio buttons](https://user-images.githubusercontent.com/8310271/64971657-609ea780-d87e-11e9-9eb3-06b6622495eb.png)
*The MaterialSkin radiobuttons*

![Progress bar](https://user-images.githubusercontent.com/8310271/64971660-61373e00-d87e-11e9-9f1a-130b586488dc.png)
*List view*

![8](https://user-images.githubusercontent.com/8310271/64971662-61cfd480-d87e-11e9-85a0-a9a215d171c4.png)
*Additional controls*

![9](https://user-images.githubusercontent.com/8310271/64971665-61cfd480-d87e-11e9-8aeb-44d2c096208c.png)
*MaterialSkin using a custom color scheme.*

![alt tag](https://i.imgur.com/i35Lfgn.png)
*FlexibleMaterialMessagebox*

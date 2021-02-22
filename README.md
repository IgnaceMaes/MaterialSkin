# MaterialSkin for .NET WinForms

Theming .NET WinForms, C# or VB.Net, to Google's Material Design Principles.

This project is **temporarily paused** _(I still read every issue and check every PR and occasionally release new versions when enough pull requests have pilled up)_

![home](https://user-images.githubusercontent.com/8310271/66237904-9dff9380-e6cc-11e9-9f08-3c5ba182e144.png)

## Nuget Package

A nuget package version is available [here](https://www.nuget.org/packages/MaterialSkin.2/)

Or simply search for MaterialSkin.2 on the **Nuget Package Manager** inside Visual Studio

## WIKI Available!

But there's not much in there for now, please contribute if you can. :smile:

You can access it [here](https://github.com/leocb/MaterialSkin/wiki)

## Current state of the MaterialSkin components

| Component                    | Supported | Disabled mode | Animated |
| ---------------------------- | :-------: | :-----------: | :------: |
| Buttons                      |    Yes    |      Yes      |   Yes    |
| Backdrop                     |  **No**   |       -       |    -     |
| Cards                        |    Yes    |      N/A      |   N/A    |
| Check Box                    |    Yes    |      Yes      |   Yes    |
| Check Box List               |    Yes    |      Yes      |   Yes    |
| Combobox                     |    Yes    |      Yes      |   Yes    |
| Context Menu                 |    Yes    |      Yes      |   Yes    |
| Dialog                       |  **No**   |       -       |    -     |
| Divider                      |    Yes    |      N/A      |   N/A    |
| Drawer                       |    Yes    |      N/A      |   Yes    |
| Flexible Dialog (big)        |    Yes    |      Yes      |   N/A    |
| FAB - Floating Action Button |    Yes    |    **No**     |   Yes    |
| Label                        |    Yes    |      Yes      |   N/A    |
| ListView                     |    Yes    |    **No**     |   N/A    |
| Progress Bar                 | _Partial_ |    **No**     |  **No**  |
| Radio Button                 |    Yes    |      Yes      |   Yes    |
| Text field                   |    Yes    |      Yes      |   Yes    |
| Sliders                      |  **No**   |       -       |    -     |
| Switch                       |    Yes    |      Yes      |   Yes    |
| Tabs                         |    Yes    |      N/A      |   Yes    |

All supported components have a dark theme

## TODO List

- Progress bar - Animation and variants, maybe round loading thingy
- Sliders
- Dialog (!= message box)
- Backdrop (maybe)
- Better FAB
- Better Listview
- Some Color code improvements and refactoring

---

## Contributting

If you have any issues please open an issue; have an improvement? open a pull request.

>This project was heavily updated by @leocb [leocb/MaterialSkin](https://github.com/leocb/MaterialSkin)
>
>forked from [donaldsteele/MaterialSkin](https://github.com/donaldsteele/MaterialSkin)
>
>and he forked it from the original [IgnaceMaes/MaterialSkin](https://github.com/IgnaceMaes/MaterialSkin)

---

## Implementing MaterialSkin in your application

### 1. Add the library to your project

There are a few methods to add this lib:

#### The Easy way

Search for MaterialSkin.2 on the Nuget Package manager inside VisualStudio and add it to your project.

#### Manual way

Download the precompiled DLL available on the releases section and add it as a external reference on your project.

#### Compile from the latest master

Clone the project from GitHub, then add the MaterialSkin.csproj to your own solution, then add it as a project reference on your project.
  
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

---

## Images

*A simple demo interface with MaterialSkin components.*
![home](https://user-images.githubusercontent.com/8310271/66237904-9dff9380-e6cc-11e9-9f08-3c5ba182e144.png)

*The MaterialSkin Drawer (menu).*
![drawer](https://user-images.githubusercontent.com/8310271/66237910-a0fa8400-e6cc-11e9-8f1d-0bc424f404c3.png)

*Every MaterialSkin button variant - this is 1 control, 3 properties*
![buttons](https://user-images.githubusercontent.com/8310271/66237911-a0fa8400-e6cc-11e9-8781-3e4c8cb0362b.png)

*The MaterialSkin checkboxes, radio and Switch.*
![selection](https://user-images.githubusercontent.com/8310271/66237912-a0fa8400-e6cc-11e9-9fb8-2cb247d2eff1.png)

*Material skin textfield and labels*
![text](https://user-images.githubusercontent.com/8310271/66237914-a0fa8400-e6cc-11e9-8afa-b9f6da2382fe.png)

*Table control*
![table](https://user-images.githubusercontent.com/8310271/66237915-a1931a80-e6cc-11e9-8e68-bc919f533366.png)

*Progress bar*
![progress bar](https://user-images.githubusercontent.com/8310271/66237916-a1931a80-e6cc-11e9-836b-157596b4ed33.png)

*Cards*
![cards](https://user-images.githubusercontent.com/8310271/66237917-a1931a80-e6cc-11e9-9b32-47374554bc07.png)

*MaterialSkin using a custom color scheme.*
![custom](https://user-images.githubusercontent.com/8310271/66237918-a1931a80-e6cc-11e9-820e-8c811629d937.png)

*FlexibleMaterial Messagebox*
![messagebox](https://user-images.githubusercontent.com/8310271/66238105-25e59d80-e6cd-11e9-88c9-5a21ceae1a5a.png)

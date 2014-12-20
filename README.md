WinForms MaterialSkin
=====================

Theming WinForms (C# or VB.Net) to Google's Material Design Principles.

---

##### Current supported controls
- Checkbox
- Divider
- Flat Button
- Label
- Radio Button
- Raised Button
- Single-line text field

Light Theme | Dark Theme
--- | ---
![alt tag](http://puu.sh/dCnJ0/1a09990e52.png) | ![alt tag](http://puu.sh/dCnNa/96861a2fdf.png)

---

##### Initializing your color scheme (C#)
```cs
var materialSkinManager = MaterialSkinManager.Instance;
materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
materialSkinManager.PrimaryColor = Color.FromArgb(63, 81, 181);
materialSkinManager.PrimaryColorDark = Color.FromArgb(48, 63, 159);
materialSkinManager.AccentColor = Color.FromArgb(255, 64, 129);
```

---

##### Early release
Please note this is a very early release, a lot will be changed.

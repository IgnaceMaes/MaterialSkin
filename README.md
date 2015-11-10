A branch of <a href="https://github.com/IgnaceMaes/MaterialSkin">IgnaceMaes/MaterialSkin</a>
=====================

This branch tackles two topics in **MaterialListView**:

1. **Replaced the font hack** that sometime caused ArgumentExceptions when drawing ListViewItems with a different hack.

	My approach uses roboto in the ListViewItems only if it's installed, otherwise it uses a default font to avoid exceptions.
2. Added a "HoveredItem" property to the ListView to **keep track of the last hovered item**.

	This allowed me to reduce the redraws on the control when the user moved his mouse around the ListViewItem.

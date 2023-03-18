#region Imports

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using MaterialSkin;
//using MaterialSkin.Controls;

using System.Drawing.Text;
using System.Windows.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;


#endregion

namespace System.Windows.Forms
{
    public class MaterialItemCollectionEditor : CollectionEditor
    {
        public MaterialItemCollectionEditor() : base(typeof(MaterialItemCollection))
        {

        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(MaterialListBoxItem);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] {
            typeof(MaterialListBoxItem)
         };
        }
    }

    //public class MaterialItemCollectionEditor : CollectionEditor
    //{
    //    // Define a static event to expose the inner PropertyGrid's
    //    // PropertyValueChanged event args...
    //    public delegate void MyPropertyValueChangedEventHandler(object sender,
    //                                        PropertyValueChangedEventArgs e);
    //    public static event MyPropertyValueChangedEventHandler MyPropertyValueChanged;

    //    // Inherit the default constructor from the standard
    //    // Collection Editor...
    //    public MyCollectionEditor(Type type) : base(type) { }

    //    // Override this method in order to access the containing user controls
    //    // from the default Collection Editor form or to add new ones...
    //    protected override CollectionForm CreateCollectionForm()
    //    {
    //        // Getting the default layout of the Collection Editor...
    //        CollectionForm collectionForm = base.CreateCollectionForm();
    //        Form frmCollectionEditorForm = collectionForm as Form;
    //        TableLayoutPanel tlpLayout = frmCollectionEditorForm.Controls[0] as TableLayoutPanel;

    //        if (tlpLayout != null)
    //        {
    //            // Get a reference to the inner PropertyGrid and hook
    //            // an event handler to it.
    //            if (tlpLayout.Controls[5] is PropertyGrid)
    //            {
    //                PropertyGrid propertyGrid = tlpLayout.Controls[5] as PropertyGrid;
    //                propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
    //            }
    //        }
    //        return collectionForm;
    //    }

    //    void propertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
    //    {
    //        // Fire our customized collection event...
    //        if (MyCollectionEditor.MyPropertyValueChanged != null)
    //        {
    //            MyCollectionEditor.MyPropertyValueChanged(this, e);
    //        }
    //    }
    //}
}

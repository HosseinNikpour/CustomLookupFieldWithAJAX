using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.WebControls;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CustomLookupFieldWithAJAX
{
    public class CustomLookupFieldEditor : UserControl, IFieldEditor
    {



        protected DropDownList ddlListNameLookup;
        //   protected DropDownList ddlFieldNameLookup;
        protected CheckBoxList ddlRelatedFields;
        protected CheckBox cbxAllowMultiple;
        protected TextBox txtMasterFieldName;
        protected Label lblSelectedListLookup;
        protected Label lblMasterFieldName;
        protected RequiredFieldValidator RequiredFieldValidator1;
        private string listNameValue = "";
        private string fieldNameValue = "";
        private string dependentValue = "";

        private string internalName = "";
        private string masterFieldName = "";
        private const string TEXT_FIELD = "Value";
        private const string VALUE_FIELD = "Key";

        /// <summary>
        /// Method called when the editor control gets loaded
        /// </summary>
        /// <param name="field"></param>
        public void InitializeWithField(SPField field)
        {

            CustomLookup myField = field as CustomLookup;



            if (myField != null)
            {
                this.listNameValue = myField.ListNameLookup;
                //  this.fieldNameValue = myField.FieldNameLookup;
                this.dependentValue = myField.RelatedFields;
                this.internalName = myField.InternalName;
                //  ddlListNameLookup.SelectedValue = myField.ListNameLookup;
                this.txtMasterFieldName.Text = myField.MasterFieldNameLookup;
            }



            //The field exists, so just show the list name and does not allow
            //the selection of another list
            //if (!IsPostBack)
            //{
            //    if (ddlListNameLookup.SelectedIndex != -1)
            //    {
            //        listNameValue = ddlListNameLookup.SelectedValue;

            //        SPList list = Web.Lists[new Guid(listNameValue)];

            //        BindDisplayColumns(list);
            //        ddlFieldNameLookup.Enabled = true;
            //        ddlDependentFields.Enabled = true;
            //    }

            //    ListItem aggreitem = ddlFieldNameLookup.Items.FindByText(this.fieldNameValue);
            //    if (aggreitem != null)
            //        aggreitem.Selected = true;
            //    if (dependentValue != "")
            //    {
            //        string[] dependentValueArray = this.dependentValue.Split('|');
            //        foreach (string str in dependentValueArray)
            //        {
            //            ListItem kk = ddlDependentFields.Items.FindByValue(str);
            //            kk.Selected = true;
            //        }
            //    }
            //}

        }
        private void SetAsReadOnly(Label itemLabel, string itemText, DropDownList dropDownList)
        {
            itemLabel.Text = itemText;
            itemLabel.Visible = true;
            dropDownList.Visible = false;


        }
        public void OnSaveChange(SPField field, bool bNewField)
        {

            string dependentsValue = "";
            foreach (ListItem li in ddlRelatedFields.Items)
            {

                if (li.Selected)
                {
                    dependentsValue += li.Value + "|";

                }
            }
            dependentsValue = dependentsValue.TrimEnd('|');
            string listValue = this.ddlListNameLookup.SelectedValue;
            string masterFieldName = this.txtMasterFieldName.Text;


            CustomLookup CurrentLookupField = field as CustomLookup;

            if (bNewField)
            {



                CurrentLookupField.UpdateListNameLookup(listValue);

                CurrentLookupField.UpdateRelatedFields(dependentsValue);
                CurrentLookupField.UpdatedMasterFieldNameLookup(this.txtMasterFieldName.Text);



            }
            else
            {
                CurrentLookupField.ListNameLookup = listValue;

                CurrentLookupField.RelatedFields = dependentsValue;
                CurrentLookupField.MasterFieldNameLookup = txtMasterFieldName.Text;

            }




        }


        /// <summary>
        /// Just display the properties in the same section
        /// </summary>

        public bool DisplayAsNewSection
        {
            get
            {
                return false;
            }
        }




        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            if (!this.IsPostBack)
            {


                using (SPWeb Web = SPContext.Current.Site.OpenWeb())
                {
                    foreach (SPList List in Web.Lists)
                    {
                        if (!List.Hidden)
                            ddlListNameLookup.Items.Add(new ListItem(List.Title, List.ID.ToString()));
                    }
                    if (this.listNameValue != "")
                    {
                        ddlListNameLookup.SelectedValue = this.listNameValue;
                        SetAsReadOnly(lblSelectedListLookup, ddlListNameLookup.SelectedItem.Text, ddlListNameLookup);

                        lblMasterFieldName.Text = txtMasterFieldName.Text;
                        lblMasterFieldName.Visible = true;
                        txtMasterFieldName.Visible = false;
                        RequiredFieldValidator1.Enabled = false;

                    }
                    else
                    {
                        ddlListNameLookup.SelectedIndex = 0;
                    }
                    if (ddlListNameLookup.SelectedIndex != -1)
                    {
                        SPList list = Web.Lists[new Guid(ddlListNameLookup.SelectedValue)];

                        BindDisplayColumns(list);
                    }
                }

                // ListItem aggreitem = ddlFieldNameLookup.Items.FindByText(this.fieldNameValue);
                //if (aggreitem != null)
                //    aggreitem.Selected = true;

                ListItem listitem = ddlListNameLookup.Items.FindByValue(this.listNameValue);
                if (listitem != null)
                    listitem.Selected = true;

                if (dependentValue != "")
                {
                    string[] dependentValueArray = this.dependentValue.Split('|');
                    foreach (string str in dependentValueArray)
                    {
                        ListItem kk = ddlRelatedFields.Items.FindByValue(str);
                        if (kk != null)
                            kk.Selected = true;
                        //  kk.Selected =true;
                    }
                }




                //}

            }
        }



        protected void ddlListNameLookup_SelectedIndexChanged(object sender, EventArgs e)
        {

            DropDownList dropDownList = sender as DropDownList;

            if (dropDownList.SelectedItem != null)
            {
                using (SPWeb web = SPContext.Current.Site.OpenWeb())
                {
                    Guid listId = new Guid(dropDownList.SelectedItem.Value);
                    SPList list = web.Lists.GetList(listId, false);
                    BindDisplayColumns(list);


                }
                //ddlFieldNameLookup.Enabled = true;
                ddlRelatedFields.Enabled = true;

                //lblSelectedLookupList.Visible = false;

            }
            else
            {
                //ddlFieldNameLookup.Enabled = false;
                ddlRelatedFields.Enabled = false;
            }



        }


        private void BindDisplayColumns(SPList list)
        {
            ddlRelatedFields.Items.Clear();
            //ddlFieldNameLookup.Items.Clear();
            foreach (SPField field in list.Fields)
            {
                if (!field.Hidden && !field.FromBaseType && (field.InternalName != txtMasterFieldName.Text || field.InternalName == this.internalName) || (!field.Hidden && field.InternalName == "Title"))
                {
                    // ddlFieldNameLookup.Items.Add(new ListItem { Text = field.Title, Value = field.Id.ToString() });
                    ddlRelatedFields.Items.Add(new ListItem { Text = field.Title, Value = field.InternalName.ToString() });
                }
            }

        }




    }
}

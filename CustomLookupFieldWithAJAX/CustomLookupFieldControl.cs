using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Diagnostics;

namespace CustomLookupFieldWithAJAX
{
    public class CustomLookupFieldControl : BaseFieldControl
    {




        List<String> _relatedFields;

        //Returns the ASCX template to use
        protected override string DefaultTemplateName
        {
            get { return "MultiColumnLookupField"; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            GetRelatedFields();
            // add save handler
            if ((ControlMode == SPControlMode.Edit) || (ControlMode == SPControlMode.New))
            {
                //SetFieldsReadOnly();
                HideFields();
                SPContext.Current.FormContext.OnSaveHandler += new EventHandler(SaveHandler);
            }
        }

        private void GetRelatedFields()
        {
            _relatedFields = new List<String>();
            SPFieldCollection fields = SPContext.Current.List.Fields;
            foreach (SPField field in fields)
            {
                if (field.RelatedField == Field.Title)
                {
                    _relatedFields.Add(field.InternalName);
                }
            }

        }

        private void HideFields()
        {
            foreach (String relatedField in _relatedFields)
            {
                HideField(relatedField);
            }
        }

        protected void HideField(String fieldName)
        {
            BaseFieldControl fieldControl = GetFieldControlByName(fieldName);
            fieldControl.Visible = false;
            fieldControl.Parent.Parent.Visible = false;
        }

        private void SaveHandler(object sender, EventArgs e)
        {
            Trace.TraceInformation("Updating related fields");

            String viewFields = String.Empty;

            foreach (String relatedField in _relatedFields)
            {
                viewFields = String.Format("<FieldRef Name='{0}'></FieldRef>", relatedField);
            }

            //int currentId = ((SPFieldLookupValue)Value).LookupId;
            int currentId = int.Parse((String)Value);
            SPList list = Web.Lists[new Guid(((SPFieldLookup)Field).LookupList)];



            //SPQuery query = new SPQuery
            //{
            //    Query = String.Format("<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>{0}</Value></Eq></Where>", currentId),
            //    ViewFields = viewFields
            //};
            //SPListItemCollection items = list.GetItems(query);

            //if (items.Count > 0)
            //{

            foreach (String relatedField in _relatedFields)
            {
                Trace.TraceInformation("Updating field (internal name: {0})", relatedField);
                LookupField lookupField = (LookupField)GetFieldControlByName(relatedField);
                //String lookupValue = (String)items[0][relatedField];
                //lookupField.Value = new SPFieldLookupValue(currentId, lookupValue);
                //lookupField.UpdateFieldValueInItem();
                lookupField.ListItemFieldValue = currentId;
                Trace.TraceInformation("Updated");

            }
            //}
            SPContext.Current.ListItem.Update();
            Trace.TraceInformation("Updating related finished");


        }

        protected BaseFieldControl GetFieldControlByName(String fieldNameToSearch)
        {
            String iteratorId = GetIteratorByFieldControl(this).ClientID;
            foreach (IValidator validator in Page.Validators)
            {
                if (validator is BaseFieldControl)
                {
                    BaseFieldControl baseField = (BaseFieldControl)validator;
                    String fieldName = baseField.FieldName;
                    if ((fieldName == fieldNameToSearch) &&
                        (GetIteratorByFieldControl(baseField).ClientID == iteratorId))
                    {
                        return baseField;
                    }
                }
            }
            return null;
        }

        private ListFieldIterator GetIteratorByFieldControl(BaseFieldControl fieldControl)
        {
            return (ListFieldIterator)this.Parent.Parent.Parent.Parent.Parent;
        }

    }
}

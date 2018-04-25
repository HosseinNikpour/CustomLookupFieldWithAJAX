using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Xml;
using Microsoft.SharePoint.Utilities;
using System.Diagnostics;
using Microsoft.SharePoint.WebControls;
using System.Xml.Serialization;

namespace CustomLookupFieldWithAJAX
{
    [Serializable]
    public class CustomLookup : SPFieldLookup
    {
        public CustomLookup(SPFieldCollection fields, string fieldName)
            : base(fields, fieldName)
        {

            Init();
        }

        public CustomLookup(Microsoft.SharePoint.SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {


            Init();
        }
        private void Init()
        {
            this.ListNameLookup = this.GetCustomProperty("ListNameLookup") + "";
            // this.FieldNameLookup = this.GetCustomProperty("FieldNameLookup") + "";
            // this.FieldValueLookup = this.GetCustomProperty("FieldValueLookup") + "";

            this.RelatedFields = this.GetCustomProperty("RelatedFields") + "";
            this.MasterFieldNameLookup = this.GetCustomProperty("MasterFieldNameLookup") + "";
            // this.QueryLookup = this.GetCustomProperty("QueryLookup") + "";
        }



        public Guid ValueColumnId
        {
            get { return SPBuiltInFieldId.ID; }
        }

        /// <summary>
        /// This method ensures that a value is provided if the field is mandatory
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// 




        public new string GetProperty(string propertyName)
        {
            Trace.TraceInformation("MultiColumnLookupField.GetProperty: " + propertyName);
            return base.GetProperty(propertyName);
        }

        private static Dictionary<int, string> updatedListNameLookup = new Dictionary<int, string>();
        private string listNameLookup;

        [XmlElement("ListNameLookup")]
        public string ListNameLookup
        {
            get
            {
                return updatedListNameLookup.ContainsKey(ContextId) ? updatedListNameLookup[ContextId] : listNameLookup;
            }
            set
            {
                this.listNameLookup = value;
            }
        }
        public void UpdateListNameLookup(string value)
        {
            updatedListNameLookup[ContextId] = value;
        }



        // }

        private static Dictionary<int, string> updatedRelatedFields = new Dictionary<int, string>();
        private string relatedFields;

        [XmlElement("RelatedFields")]
        public string RelatedFields
        {
            get
            {
                return updatedRelatedFields.ContainsKey(ContextId) ? updatedRelatedFields[ContextId] : relatedFields;
            }
            set
            {
                this.relatedFields = value;
            }
        }
        public void UpdateRelatedFields(string value)
        {
            updatedRelatedFields[ContextId] = value;
        }


        private static Dictionary<int, string> updatedMasterFieldNameLookup = new Dictionary<int, string>();
        private string masterFieldNameLookup;

        [XmlElement("MasterFieldNameLookup")]
        public string MasterFieldNameLookup
        {
            get
            {
                return updatedMasterFieldNameLookup.ContainsKey(ContextId) ? updatedMasterFieldNameLookup[ContextId] : masterFieldNameLookup;
            }
            set
            {
                this.masterFieldNameLookup = value;
            }
        }
        public void UpdatedMasterFieldNameLookup(string value)
        {
            updatedMasterFieldNameLookup[ContextId] = value;
        }



        public int ContextId
        {
            get
            {
                return SPContext.Current.GetHashCode();
            }
        }
        public override void Update()
        {
            this.SetCustomProperty("ListNameLookup", this.ListNameLookup);
            // this.SetCustomProperty("FieldNameLookup", this.FieldNameLookup);

            this.SetCustomProperty("RelatedFields", this.RelatedFields);


            this.SetCustomProperty("MasterFieldNameLookup", this.MasterFieldNameLookup);
            AddMasterField(this.MasterFieldNameLookup, this.ListNameLookup);

            base.Update();
            if (updatedListNameLookup.ContainsKey(ContextId))
                updatedListNameLookup.Remove(ContextId);
            //if (updatedFieldNameLookup.ContainsKey(ContextId))
            //    updatedFieldNameLookup.Remove(ContextId);

            if (updatedRelatedFields.ContainsKey(ContextId))
                updatedRelatedFields.Remove(ContextId);


            if (updatedMasterFieldNameLookup.ContainsKey(ContextId))
                updatedMasterFieldNameLookup.Remove(ContextId);




        }
        public override void OnAdded(SPAddFieldOptions op)
        {
            Trace.TraceInformation("MultiColumnLookupField.OnAdded: {0}", op);

            if (AllowMultipleValues)
            {
                throw new ApplicationException("OnAdded: Current version of the field supports only single value lookups!");
            }


            Update();
            //string value = RelatedFields;
            //string masterFieldName = MasterFieldNameLookup;
            string lookupList = ListNameLookup;
            //  AddMasterField(MasterFieldNameLookup, lookupList);
            //  AddField(value);
            base.OnAdded(op);




            //  AddMasterField(this.MasterFieldNameLookup);
        }

        public override void OnDeleting()
        {
            Trace.TraceInformation("MultiColumnLookupField.OnDeleting");

            DeleteMasterField(this.MasterFieldNameLookup, this.ListNameLookup);

            base.OnDeleting();
        }

        public override void OnUpdated()
        {
            Trace.TraceInformation("MultiColumnLookupField.OnUpdated");
            string value = RelatedFields;
            Trace.TraceInformation("RelatedFields value: {0}", value);

            //RemoveRelatedFields(value);
            //using (var scope = new DisabledFieldEventsScope())
            //{

            //    AddField(value);
            //}
            //string masterFieldName = MasterFieldNameLookup;
            //string lookupList=LookupList;
            //AddMasterField(masterFieldName, lookupList);
            base.OnUpdated();
        }

        //Returns a field control for the page
        public override BaseFieldControl FieldRenderingControl
        {
            get
            {
                BaseFieldControl fldControl = new CustomLookupFieldControl();
                fldControl.FieldName = InternalName;
                return fldControl;
            }
        }


        private void RemoveRelatedFields(String fieldNamesNew)
        {
            Trace.TraceInformation("RemoveRelatedFields started: {0}", fieldNamesNew);
            String[] fieldNames = fieldNamesNew.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<String> fieldNameList = new List<String>(fieldNames);
            // we trim all strings in the list
            fieldNameList = fieldNameList.Select(x => x.Trim()).ToList();

            Trace.TraceInformation("Removing related fields");
            SPFieldCollection fields = SPContext.Current.List.Fields;
            for (int i = fields.Count - 1; i > -1; i--)
            {
                SPField field = fields[i];
                // we remove only if the new field collection does not contain the field
                // we check field type also to be sure
                if ((field.RelatedField == Title) && (!fieldNameList.Contains(field.Title))
                    && (field.Type == SPFieldType.Lookup))
                {

                    Trace.TraceInformation("Removing field name:{0}, internal name: {1}", field.Title, field.InternalName);
                    field.ReadOnlyField = false;
                    field.Update();
                    fields.Delete(field.InternalName);
                    Trace.TraceInformation("Removed");
                }
            }
            Trace.TraceInformation("RemoveRelatedFields finished");
        }

        private void AddField(string fieldNamesNew)
        {
            Trace.TraceInformation("AddField started: {0}", fieldNamesNew);
            String[] fieldNames = fieldNamesNew.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<String> fieldNameList = new List<String>(fieldNames);
            // we trim all strings in the list
            fieldNameList = fieldNameList.Select(x => x.Trim()).ToList();
            List<String> fieldsAdded = new List<String>();
            SPList list = SPContext.Current.List;
            SPWeb web = SPContext.Current.Web;

            foreach (String fieldName in fieldNameList)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(web.Site.ID))
                    {
                        using (SPWeb Web = site.OpenWeb())
                        {

                            list = Web.Lists[list.ID];
                            Trace.TraceInformation("Adding new field: {0}", fieldName);
                            SPFieldCollection fields = list.Fields;

                            // we add the new field only if the list does not contain a field having the same name
                            if ((!String.IsNullOrEmpty(fieldName)) && (!list.Fields.ContainsField(fieldName)))
                            {
                                SPFieldLookup field = new SPFieldLookup(fields, SPFieldType.Lookup.ToString(), fieldName);
                                field.ShowInDisplayForm = false;
                                field.ShowInEditForm = false;
                                field.ShowInNewForm = false;
                                field.ShowInListSettings = false;
                                field.RelatedField = Title;
                                field.LookupList = ListNameLookup;
                                //  field.LookupField = FieldNameLookup;

                                Web.AllowUnsafeUpdates = true;
                                //  site.WebApplication.FormDigestSettings.Enabled = false;
                                fields.Add(field);
                                list.Update();


                                fieldsAdded.Add(fieldName);
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    }
                });
                Trace.TraceInformation("Field added");
            }

            // update new lookup fields with the ID of the master lookup field
            // it is important only when changing related lookup fields
            // but not when creating the field
            if (fieldsAdded.Count > 0)
            {
                UpdateItems(list, fieldsAdded);
            }

            Trace.TraceInformation("AddField finished");

        }

        private void AddMasterField(string fieldName, string lookupList)
        {
            SPWeb web = SPContext.Current.Web;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(web.Site.ID))
                {
                    using (SPWeb Web = site.OpenWeb())
                    {
                        Web.AllowUnsafeUpdates = true;

                        SPList list = Web.Lists[new Guid(lookupList)];
                        Trace.TraceInformation("Adding new field: {0}", fieldName);

                        if (list.Fields.ContainsFieldWithStaticName(fieldName))
                            list.Fields.Delete(fieldName);
                        list.Fields.AddLookup(fieldName, SPContext.Current.ListId, true);
                        SPFieldLookup lkp = (SPFieldLookup)list.Fields[fieldName];
                        lkp.LookupField = "Title";
                        lkp.Indexed = true;
                        lkp.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Cascade;

                        lkp.Update();
                        list.Update();
                        Web.AllowUnsafeUpdates = false;


                    }
                }
            });
        }

        private void DeleteMasterField(string fieldName, string lookupList)
        {
            SPWeb web = SPContext.Current.Web;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(web.Site.ID))
                {
                    using (SPWeb Web = site.OpenWeb())
                    {
                        Web.AllowUnsafeUpdates = true;

                        SPList list = Web.Lists[new Guid(lookupList)];
                        Trace.TraceInformation("Deleting new field: {0}", fieldName);

                        if (list.Fields.ContainsFieldWithStaticName(fieldName))
                            list.Fields.Delete(fieldName);

                        list.Update();
                        Web.AllowUnsafeUpdates = false;


                    }
                }
            });
        }

        private void UpdateItems(SPList list, List<String> fieldsAdded)
        {

            String fieldFormat = "<SetVar Name='urn:schemas-microsoft-com:office:office#{0}'>{{1}}</SetVar>";
            StringBuilder fieldBuilder = new StringBuilder();
            foreach (String fieldName in fieldsAdded)
            {
                fieldBuilder.Append(String.Format(fieldFormat, fieldName));
            }

            StringBuilder methodBuilder = new StringBuilder();
            String batchFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
              "<ows:Batch OnError='Continue'>{0}</ows:Batch>";

            String methodFormat = String.Format("<Method ID='{{0}}'>" +
             "<SetList>{0}</SetList>" +
             "<SetVar Name='Cmd'>Save</SetVar>" +
             "<SetVar Name='ID'>{{0}}</SetVar>" +
             "{1}" +
             "</Method>", list.ID, fieldBuilder);

            SPQuery query = new SPQuery();
            query.ViewFields = String.Format("<FieldRef Name='{0}' />", InternalName);
            query.Query = String.Format("<Where><IsNotNull><FieldRef Name='{0}'/></IsNotNull></Where>", InternalName);
            query.ViewAttributes = "Scope='Recursive'";
            SPListItemCollection items = list.GetItems(query);

            // Build the CAML update commands.
            foreach (SPItem item in items)
            {
                // we use the item ID as method ID
                // value mustn't be null, as we filtered items for IsNotNull
                methodBuilder.AppendFormat(methodFormat, item["ID"], ((SPFieldLookupValue)item[Title]).LookupId);
            }

            String batch = String.Format(batchFormat, methodBuilder);
            string batchReturn = list.ParentWeb.ProcessBatchData(batch);
        }

        ////////////

        public override Type FieldValueType
        {
            get
            {
                return typeof(string);
            }
        }

        ////////////////

    }

}

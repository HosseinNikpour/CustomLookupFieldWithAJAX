<%@ Control Language="C#" Inherits="CustomLookupFieldWithAJAX.CustomLookupFieldEditor,CustomLookupFieldWithAJAX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9eac5c3aaf535f76"    AutoEventWireup="false" compilationMode="Always" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/15/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/15/InputFormSection.ascx" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
 <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>


<wssuc:InputFormSection runat="server" id="MySections" Title="My Custom Section">
       <Template_InputFormControls>
             <wssuc:InputFormControl runat="server"
                    LabelText="Select Detail List">
                    <Template_Control>
                           <asp:DropDownList id="ddlListNameLookup" runat="server" OnSelectedIndexChanged="ddlListNameLookup_SelectedIndexChanged" AutoPostBack="true">
                           </asp:DropDownList>
                           <asp:Label id="lblSelectedListLookup" runat="server" Visible="false"></asp:Label>
                    </Template_Control>


             </wssuc:InputFormControl>
          <%-- <wssuc:InputFormControl runat="server"
                    LabelText="Select Field for Query">
                    <Template_Control>
                           <asp:DropDownList id="ddlFieldNameLookup" runat="server" Enabled="false" >
                           </asp:DropDownList>
                    </Template_Control>


             </wssuc:InputFormControl>--%>
             <wssuc:InputFormControl runat="server"
                    LabelText="Related Fields">
                    <Template_Control>
                           <asp:CheckBoxList id="ddlRelatedFields" runat="server"  >
                           </asp:CheckBoxList>
                    </Template_Control>


             </wssuc:InputFormControl>



            <wssuc:InputFormControl runat="server"
                    LabelText="Master Field Name">
                    <Template_Control>
                        <asp:TextBox ID="txtMasterFieldName" runat="server"  AssociatedControlID="TextBoxMaster"></asp:TextBox> 
                        <asp:Label ID="lblMasterFieldName" runat="server" Visible="false"></asp:Label> 

                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="txtMasterFieldName" BreakBefore="true" ErrorMessage="وارد کردن فیلد مستر الزامی است" ></asp:RequiredFieldValidator>
                       
                    </Template_Control>
                </wssuc:InputFormControl>
       </Template_InputFormControls>
</wssuc:InputFormSection>
 


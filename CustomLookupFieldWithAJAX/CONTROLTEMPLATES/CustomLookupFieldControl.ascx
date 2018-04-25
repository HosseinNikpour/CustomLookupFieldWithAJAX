<%@ Control Language="C#" Debug=true  %>

<%@Assembly Name="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@Register TagPrefix="SharePoint" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" namespace="Microsoft.SharePoint.WebControls"%>

<!-- 
Created by Stephane Eyskens
http://blogs.ezos.com/blog/sey
--> 

<SharePoint:RenderingTemplate ID="CustomLookupFieldControl" runat="server">
    <Template>
    <script type="text/javascript" language="javascript">
        //Escapes some characters for the CAML query. Can't use htmlentities since the CAML
        //isn't compliant with all the entities...

        function CAMLCompliant(StrValue) {
            var CAMLString = StrValue.replace("<", "&lt;");
            CAMLString = CAMLString.replace(">", "&gt;");
            CAMLString = CAMLString.replace("&", "&amp;");
            return CAMLString;
        }

        QueryHandler = null;
        //This function is the callback function called when the AJAX query is over.
        function AjaxAnswer() {
            if (QueryHandler == null || typeof (QueryHandler) == "undefined") {
                return false;
            }

            //When the answer is back and the http request was successfull
            if (QueryHandler.HttpObject.readyState == 4 && QueryHandler.HttpObject.status == 200) {
                window.status = "SOAP request successfully processed...";
                var xml = QueryHandler.HttpObject.responseXML;
                if (xml.documentElement) {
                    //retrieving the returned rows
                    var rows = xml.documentElement.getElementsByTagName("z:row");
                    if (rows.length == 0) {
                        rows = xml.documentElement.getElementsByTagName("row");
                    }
                    //Cleaning the DOM list
                    QueryHandler.TargetObject.length = 0;
                    QueryHandler.TargetObject.disabled = true;
                    if (rows.length > 0) {
                        //Creating the first list item                       
                        var NewOption = document.createElement("option");
                        NewOption.value = -1;
                        NewOption.text = "Choose...";
                        try {
                            QueryHandler.TargetObject.add(NewOption, null);
                        }
                        catch (ex) {
                            QueryHandler.TargetObject.add(NewOption);
                        }
                        //Fetching the result set   
                        for (var i = 0; i < rows.length; i++) {

                            var TitleValue = "";
                            var IdValue = "";
                            if (rows[i].getAttributeNode("ows_Title") != null) {
                                TitleValue = rows[i].getAttributeNode("ows_Title").nodeValue;
                            }

                            if (rows[i].getAttributeNode("ows_ID") != null) {
                                IdValue = rows[i].getAttributeNode("ows_ID").nodeValue;
                            }
                            //Adding the options to the target list
                            NewOption = document.createElement("option");
                            NewOption.value = IdValue + ";#" + TitleValue;
                            NewOption.text = TitleValue;
                            try {
                                QueryHandler.TargetObject.add(NewOption, null);
                            }
                            catch (ex) {
                                QueryHandler.TargetObject.add(NewOption);
                            }
                            QueryHandler.TargetObject.disabled = false;
                        }
                    }
                }
            }
            else {
                window.status = "SOAP request failed...";
            }
        }
        //This function is called from the custom field to perform the query via the SharePoint web service
        function AjaxQuery(Host, SiteURL, ListName, Query, QueryOptions, ItemLimit, TgtObject) {
            var SpCAMLQuery;
            this.HttpObject = null;
            this.TargetObject = document.getElementById(TgtObject);
            //Preparing the web service call
            SpCAMLQuery = '<?xml version="1.0" encoding="utf-8"?>';
            SpCAMLQuery += '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">';
            SpCAMLQuery += '<soap:Body>';
            SpCAMLQuery += '<GetListItems xmlns="http://schemas.microsoft.com/sharepoint/soap/">';
            SpCAMLQuery += '<listName>' + ListName + '</listName>';
            SpCAMLQuery += '<rowLimit>' + ItemLimit + '</rowLimit>';
            SpCAMLQuery += '<query>' + Query + '</query>';
            SpCAMLQuery += '<queryOptions>' + QueryOptions + '</queryOptions>';
            SpCAMLQuery += '</GetListItems></soap:Body></soap:Envelope>';
            var serverUrl = SiteURL + '/_vti_bin/Lists.asmx?wsdl';
            if (this.HttpObject == null) {
                if (window.XMLHttpRequest) {
                    this.HttpObject = new XMLHttpRequest();
                }
                else if (window.ActiveXObject) {
                    this.HttpObject = new ActiveXObject("MSXML2.XMLHTTP.3.0");
                }
            }
            //Executing the call
            this.HttpObject.open("POST", SiteURL + "/_vti_bin/lists.asmx", true);
            this.HttpObject.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
            this.HttpObject.setRequestHeader("Host", Host);
            this.HttpObject.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/sharepoint/soap/GetListItems");
            this.readyStateChangeHandler = AjaxAnswer;
            this.HttpObject.onreadystatechange = this.readyStateChangeHandler;
            this.HttpObject.send(SpCAMLQuery);
            window.status = "SOAP request sent...";
        }
</script>
        <asp:TextBox ID="LookupValue" runat="server" MaxLength="15"/>                
        <asp:TextBox ID="LookupHiddenValue" runat="server"/>
        <asp:Label ID="ErrorLabel" ForeColor="red" runat="server" />
    </Template>
    
</SharePoint:RenderingTemplate>


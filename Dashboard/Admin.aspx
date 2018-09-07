<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="Dashboard.Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        label {
            width: 400px;
            clear: left;
            text-align: left;
            padding-right: 10px;
            margin-top: 3%;
            font-size: larger;
        }
        input {
            width: 400px;
        }
        .table {
            margin-left: 50px;
            margin-right: 0;
            width: 60%;
            font-size:larger; 
            float: left;
        }
        .table, label {
            font-weight: bold;
        }
        .subtext {
            font-size: medium;
            font-weight:normal;
            margin-bottom: 5%;
        }
        .td-width-height {
            width: 600px;
            height: 50px;
        }
        .ddlActiveScripts {
            float: left;
        }
    </style>
    <div class="jumbotron">  
        <h2>Welcome to the Admin Portal!</h2>
        <br />
        <p>Enter script information into the fields below to add new metadata information to the database:</p>
    </div>   
    <!--put inside panel or gridview in order to be able to use ClientID for textbox controls in the case of the jquery datetime picker!-->

    <div class="table">
        <asp:Label ID="lblConfirmation" runat="server" Text=""></asp:Label>
           <table>
               <tr>
                   <td class="td-width-height">
                       <asp:Label ID="lblScriptName" runat="server" Text="Script Name"></asp:Label><br />
                       <span class="subtext">50 character maximum</span>
                   </td>
                   <td>
                       <asp:TextBox ID="txtBoxScriptName" runat="server" Width="256px" MaxLength="50"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="td-width-height">
                       <asp:Label ID="lblAuthor" runat="server" Text="Author"></asp:Label><br />
                       <span class="subtext">Who wrote the script</span>
                   </td>
                   <td>
                       <asp:TextBox ID="txtBoxAuthor" runat="server" Width="256px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="td-width-height">
                       <asp:Label ID="lblExtension" runat="server" Text="Phone Extension"></asp:Label><br />
                       <span class="subtext">The phone extension of the author</span>
                   </td>
                   <td>
                       <asp:TextBox ID="txtBoxExtension" runat="server" Width="256px"></asp:TextBox>                       
                   </td>
               </tr>
               <tr>
                   <td class="td-width-height">
                       <asp:Label ID="lblDescription" runat="server" Text="Script Description"></asp:Label><br />
                       <span class="subtext">A description of what the script does</span>
                   </td>
                   <td>
                       <asp:TextBox ID="txtBoxDescription" runat="server" Height="90px" TextMode="MultiLine" Width="256px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="td-width-height">
                       <asp:Label ID="lblLastUpdated" runat="server" Text="Last Updated (MM-DD-YYYY HH:MM)"></asp:Label><br />
                       <span class="subtext">The date and time of when the script was last updated. Use values 0-23 (military time) for hours. </span>
                   </td>
                   <td>
                       <!--TODO make this a date picker and figure out how to format it correctly into the excel sheet-->
                       <asp:TextBox ID="txtBoxLastUpdated" runat="server" Width="256px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="td-width-height">
                       <asp:Label ID="lblReportingInterval" runat="server" Text="Reporting Interval"></asp:Label><br />
                       <span class="subtext">A time span (in minutes). This will indicate how often the script information will be reported.</span>
                   </td>
                   <td>
                       <asp:TextBox ID="txtBoxReportingInterval" runat="server" Width="256px"></asp:TextBox>
                   </td>
                </tr>
                <tr>
                   <td class="td-width-height">
                       <asp:Label ID="lblRollingWindow" runat="server" Text="Rolling Window Interval"></asp:Label><br />
                       <span class="subtext">A time interval (in hours). This view will show how many records have been completed in the specified time interval.</span>
                   </td>
                   <td>
                       <asp:TextBox ID="txtBoxRollingWindow" runat="server" Width="256px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="td-width-height">
                       <asp:Label ID="lblHumanSPR" runat="server" Text="Human Seconds Per Record"></asp:Label><br />
                       <span class="subtext">Approximately how many seconds it would take a human to complete one record</span>
                   </td>
                   <td>
                       <asp:TextBox ID="txtBoxHumanSPR" runat="server" Width="256px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td>&nbsp;</td>
                   <td><asp:Button ID="btnSubmit" runat="server" Text="Submit" Width="150px" style="float:right;" OnClick="btnSubmit_Click"/></td>
               </tr>
           </table>
        </div>

    <div class="ddlActiveScripts">
        <p>Production Scripts:</p>
        <asp:DropDownList ID="ddlActiveScripts" runat="server" CssClass="ddl">
        </asp:DropDownList>
        <asp:Button ID="btnSelectScript" runat="server" Text="Select Script" CssClass="btn" OnClick="btnSelectScript_Click"/>
    </div>
</asp:Content>

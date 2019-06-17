<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="WebApplication.WebSite.Register" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
       <title>Register</title>
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        <h2>Register</h2>
    
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>


        <!-- Add players Dialog-->
        <div class="modal fade" id="addPlayersDialogModel" role="dialog" aria-labelledby="modalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                <h4 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Player"></asp:Label></h4>
                            </div>
                            <div class="modal-body">
                                <div class="form-horizontal" runat="server" style="align-content:center" id="addplayerform">
                                    <div class="row" style=" align-items:center; margin:0; width:100%;">
                                    
                                        <div class="col-sm-5">
                                                <asp:Label style=" text-align:left"  runat="server">First Name</asp:Label>
                                                <asp:TextBox style="border-color:black" runat="server" id="firstnameTB" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ValidationGroup="valGroup2" runat="server" ControlToValidate="firstnameTB" CssClass="text-danger" ErrorMessage="First name field is required." />
                                         </div>
                                    
                                         <div class="col-sm-5">
                                                <asp:Label style="text-align:left"  runat="server">Last Name</asp:Label>
                                                <asp:TextBox style="border-color:black" runat="server" id="lastnameTB" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ValidationGroup="valGroup2"  runat="server" ControlToValidate="lastnameTB" CssClass="text-danger" ErrorMessage="Last name field is required." />
                                        </div>

                                    </div>
                                
                                  <div class="row" style="margin:0; width:100%;">
                                         <div class="col-sm-5">
                                                <asp:LinkButton id="AddBT" runat="server" CssClass="btn btn-primary" ValidationGroup="valGroup2" CausesValidation="true"  OnClick="AddPlayer_Click"  Style="background:white;color:black;border-color:black;" >
                                                    <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>  Add
                                                </asp:LinkButton>
                                        </div>
                                   </div>

                                 
                                </div>
                            </div>
                        
                        
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>


        
                        <div class="form-horizontal" id="REGISTERFORM">

                            <h4>Create a new account</h4>
                            <asp:Label CssClass="text-danger" ID="GeneralError" style="text-align:left" runat="server" ></asp:Label>

                            <hr />
                            <div class="form-group">
                                <asp:Label style="text-align:left" runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">Email</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox style="border-color:black" CausesValidation="true" runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                                    <asp:RequiredFieldValidator ValidationGroup="valGroup1" runat="server" ControlToValidate="Email"
                                        CssClass="text-danger" ErrorMessage="The email field is required." />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label style="text-align:left" runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox style="border-color:black" CausesValidation="true" runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ValidationGroup="valGroup1" runat="server" ControlToValidate="Password"
                                        CssClass="text-danger" ErrorMessage="The password field is required." />
                                </div>
                            </div>
                            <div  class="form-group">
                                       <div class="col-sm-2">
                                           <div style="text-align:left">
                                               <asp:label style="background:transparent; margin:0; text-align:left;" runat="server"  AssociatedControlID="nickname">Team name</asp:label>
                                               <h6 style="color:green;"><asp:label style="background:transparent;margin:0; text-align:left" runat="server">will be displayed in game</asp:label></h6>
                                            </div>
                                        </div>


                                       <div class="col-sm-5">
                                            <asp:textbox style="border-color:black" CausesValidation="true" runat="server" ID="nickname"  cssclass="form-control" />
                                            <asp:requiredfieldvalidator ValidationGroup="valGroup1" runat="server" controltovalidate="nickname"
                                                cssclass="text-danger" errormessage="The team name field is required." />
                                       </div>
                           </div>
                          
                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                         <asp:updatepanel runat="server">
                                             <ContentTemplate>
                                                <asp:linkbutton  OnClick="AddPlayers_Click" ValidationGroup="none" style="background:white;color:black;border-color:black;" id="submitbtn" runat="server" cssclass="btn btn-primary">
                                                    <span aria-hidden="true" class="glyphicon glyphicon-plus"></span>  add players
                                                </asp:linkbutton>
                                           </ContentTemplate>
                                        </asp:updatepanel>
                                     <asp:UpdatePanel ID="UpdatePanelPlayerError" runat="server" UpdateMode="Always">
                                            <ContentTemplate>
                                                 <asp:label style="display:none" CssClass="text-danger" ID="playersError" runat="server"  AssociatedControlID="nickname">At least one player should be added</asp:label>
                                            </ContentTemplate>
                                            <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="AddBT" />
                                            </Triggers>
                                    </asp:UpdatePanel>
     
                                </div>
                            </div>
                            
                            

                            
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                        <div class="form-group"  id="playersDiv" style="display:none" runat="server">
                                            <div class="col-md-offset-2 col-md-10" style="overflow-y:auto; height:50px; width:28%">
                                                <ul class="list-group" id="playersList" runat ="server">
                                                </ul>
                                            </div>
                                        </div>
                                </ContentTemplate>
                                <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="AddBT" />
                                </Triggers>
                            </asp:UpdatePanel>

                            

                            <div class="form-group">
                                    <div class="col-md-offset-2 col-md-10">
                                        <asp:Button OnClick="CreateUser_Click" ValidationGroup="valGroup1"  CausesValidation="true"   Text="Register"   Style="background:white;color:black;border-color:black;" ID="RegisterBT" runat="server" CssClass="btn btn-primary">
                                        </asp:Button>
                                </div>
                            </div>

                        </div>
</asp:Content>

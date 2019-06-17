<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditAccount.aspx.cs" Inherits="WebApplication.WebSite.EditAccount" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
       <title>Account</title>
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        <h2>Edit account</h2>
    

    <div class="row" >
            <div class="col-md-7">
            <h4><asp:Label runat="server" Text="Edit Profile" /></h4> 
            <hr />
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="ErrorMessage" />
                        </p>
                        <div class="form-horizontal" id="REGISTERFORM" >
                            <asp:Label CssClass="text-danger" ID="GeneralError" style="text-align:left" runat="server" ></asp:Label>

                       
                            <div class="form-group">
                                <asp:Label style="text-align:left" runat="server" AssociatedControlID="Email" CssClass="col-md-4 control-label">Email</asp:Label>
                                <div class="col-md-8">
                                    <asp:TextBox style="border-color:black" CausesValidation="true" runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                                    <asp:RequiredFieldValidator ValidationGroup="valGroup1" runat="server" ControlToValidate="Email"
                                        CssClass="text-danger" ErrorMessage="The email field is required." />
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:Label style="text-align:left" runat="server" AssociatedControlID="OldPassword" CssClass="col-md-4 control-label">Old Password</asp:Label>
                                <div class="col-md-8">
                                    <asp:TextBox Text="" oninput="this.type='password'" autocomplete="Off" style="border-color:black" CausesValidation="true" runat="server" ID="OldPassword" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ValidationGroup="valGroup1" runat="server" ControlToValidate="OldPassword"
                                        CssClass="text-danger" ErrorMessage="The password field is required." />
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:Label style="text-align:left" runat="server" AssociatedControlID="NewPassword" CssClass="col-md-4 control-label">New Password</asp:Label>
                                <div class="col-md-8">
                                    <asp:TextBox Text="" oninput="this.type='password'" autocomplete="Off" style="border-color:black" CausesValidation="true" runat="server" ID="NewPassword" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ValidationGroup="valGroup1" runat="server" ControlToValidate="NewPassword"
                                        CssClass="text-danger" ErrorMessage="The password field is required." />
                                </div>
                            </div>


                            <div  class="form-group">
                                       <div class="col-md-4">
                                           <div style="text-align:left">
                                               <asp:label style="background:transparent; margin:0; text-align:left;" runat="server"  AssociatedControlID="nickname">Team name</asp:label>
                                               <h6 style="color:green;"><asp:label style="background:transparent;margin:0; text-align:left" runat="server">will be displayed in game</asp:label></h6>
                                            </div>
                                        </div>


                                        <div class="col-md-8">
                                            <asp:textbox style="border-color:black" CausesValidation="true" runat="server" ID="nickname"  cssclass="form-control" />
                                            <asp:requiredfieldvalidator ValidationGroup="valGroup1" runat="server" controltovalidate="nickname"
                                                cssclass="text-danger" errormessage="The team name field is required." />
                                        </div>
                           </div>   
                               
                           

                        </div>

                </div>

        <div class="col-md-5">
            <h4><asp:Label runat="server" Text="Remove players" /></h4> 
            <hr />


            <asp:UpdatePanel ID="UpdatePanelPlayersLV" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                    <asp:ListBox ID="PlayersLV" style="width:100%; overflow-y:auto; height:40%;" runat="server">
                    </asp:ListBox>
                    </ContentTemplate>
                    <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="RemovePlayerBT" />
                    </Triggers>
                 </asp:UpdatePanel>

            

            <div class="col-md-offset-4 col-md-10">
            <asp:Button   Text="Remove Selected Player" OnClick="OnRemovePlayer"  Style="position:relative; top:10px; background:white;color:black;border-color:black;" ID="RemovePlayerBT" runat="server" CssClass="btn btn-primary">
            </asp:Button>
            </div>

            <div style="position:relative; top:20px;">
                 <h4><asp:Label  runat="server" Text="Remove Games" /></h4> 
                <hr />

                 <asp:UpdatePanel ID="UpdatePanelGamesLV" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                    <asp:ListBox ID="GamesLV" style="width:100%; overflow-y:auto; height:40%;" runat="server">
                    </asp:ListBox>                                            </ContentTemplate>
                    <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="RemoveGameBT" />
                    </Triggers>
                 </asp:UpdatePanel>

               



                <div class="col-md-offset-4 col-md-10">
            <asp:Button   Text="Remove Selected Game"  OnClick="OnRemoveGame" Style="position:relative; top:10px; background:white;color:black;border-color:black;" ID="RemoveGameBT" runat="server" CssClass="btn btn-primary">
            </asp:Button>
            </div>

            </div>
        </div>
            
        </div>
    <div>
        <div class="col-md-offset-5 col-md-10">
            <asp:Button OnClick="OnSaveClick" ValidationGroup="valGroup1"  CausesValidation="true"   Text="Update Changes"   Style="background:white;color:black;border-color:black;" ID="RegisterBT" runat="server" CssClass="btn btn-primary">
            </asp:Button>
        </div>

    </div>
</asp:Content>

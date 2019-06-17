<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="WebApplication.WebSite.Game" Async="true" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
       <title>Game</title>
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

            
            <div class="row" >
                <div style="text-align:center; padding-top:20px; background:transparent;" class="col-md-4">
                       <h4><asp:Label runat="server" Text="Start New Game" /></h4>
                        <hr />

                        <div class="form-horizontal">
                       
                            <div class="form-group">
                                <asp:Label style="background:transparent;" runat="server" CssClass="col-md-2 control-label">Name</asp:Label>
                                <div class="col-md-10" style="text-align:left; background:transparent;">
                                    <asp:TextBox style="padding:0;" runat="server" ID="GameNameTB" CssClass="form-control" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="GameNameTB"
                                        CssClass="text-danger" ErrorMessage="The GameName field is required." />
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:Label style="background:transparent;" runat="server" CssClass="col-md-2 control-label">Player</asp:Label>
                                <div style="background:transparent;" class="col-md-10">
                                     <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                     <triggers>
                                     <asp:asyncpostbacktrigger controlid="PlayerList" eventname="SelectedIndexChanged" />
                                     </triggers>
                                     <ContentTemplate>
                                     <asp:DropDownList  style="padding:0; margin-left:0;" CssClass="form-control" runat="server"  id="PlayerList" AutoPostBack="true">
                                       </asp:DropDownList>
                                          </ContentTemplate>
                                         </asp:UpdatePanel>
                                </div>
                            </div>

                             <div style="position:relative; top:20px;" class="form-group" >
                                <div>
                                     <div  class="col-md-12">
                                        <asp:Button OnClick="OnStartGameClick"  CausesValidation="true"   Text="Start"   Style="width:100%; background:white;color:black;border-color:black;" ID="RegisterBT" runat="server" CssClass="btn btn-primary">
                                        </asp:Button>
                                         </div>
                                </div>
                            </div>

                             <asp:UpdatePanel ID="UpdatePanelErrorStartGame" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                 <ContentTemplate>
                                       <div  class="col-md-12" style="text-align:left; position:relative; top:5px;">
                                        <asp:Label Text="" CssClass="text-danger" ID="GeneralErrorStartGame" style="text-align:left" runat="server" ></asp:Label>
                                       </div>
                                  </ContentTemplate>
                            </asp:updatepanel>


                        </div>
              </div>
             

                <div style="text-align:center; padding-top:20px;" class="col-md-8">
                   <h4><asp:Label runat="server" Text="Join Game" /></h4>
                   <hr />
                    
                    <div class="row">
                            <div class="col-md-1">
                                <div style="width:10px; height:10px; background:#55fb58;"></div>
                            </div>
                            <div class="col-md-3" style="text-align:left">
                                <asp:Label style="position:relative; top:-5px;" runat="server">Registered</asp:Label>
                            </div>
                            <div class="col-md-1">
                                <div style="width:10px; height:10px; background:#f78a05;"></div>
                            </div>
                            <div class="col-md-3" style="text-align:left">
                                <asp:Label style="position:relative; top:-5px;" runat="server">Playing</asp:Label>
                            </div>
                             <div class="col-md-1">
                                <div style="width:10px; height:10px; background:#c4fdff;"></div>
                            </div>
                            <div class="col-md-3" style="text-align:left">
                                <asp:Label style="position:relative; top:-5px;" runat="server">Wating for player</asp:Label>
                            </div>
                    </div>

                    <div style="overflow-y:auto; height:100px;">
                    <asp:UpdatePanel style="text-align:center;" ID="gamesGridViewPanel" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                        <triggers>
                        <asp:asyncpostbacktrigger controlid="gamesGV" eventname="SelectedIndexChanged" />
                        </triggers>
                        <ContentTemplate>
                            <asp:GridView HorizontalAlign="Center" AutoGenerateSelectButton="True" OnRowDataBound="GamePlayes_RowDataBound"  OnSelectedIndexChanged="OnGameSelected" style="border:black; text-align:center;" class="table table-bordered" ID="gamesGV" runat="server">
                        </asp:GridView>
                         </ContentTemplate>
                     </asp:UpdatePanel>

                    </div>

                    <div  class="row" style="padding-top:20px;">

                        <div  class="col-md-2">
                            <asp:Button OnClick="OnJoinGameClick"  CausesValidation="false"   Text="Join"   Style="width:100%; background:white;color:black;border-color:black;" ID="JoinBT" runat="server" CssClass="btn btn-primary">
                            </asp:Button>
                        </div>

                        <div  class="col-md-2">
                            <asp:Button OnClick="OnCloseGameClick"  CausesValidation="false"   Text="Close"   Style="width:100%; background:white;color:black;border-color:black;" ID="CloseBT" runat="server" CssClass="btn btn-primary">
                            </asp:Button>
                        </div>

                        <div  class="col-md-2">
                            <asp:Button OnClick="OnExistGameClick"  CausesValidation="false"   Text="Exist"   Style="width:100%; background:white;color:black;border-color:black;" ID="Button1" runat="server" CssClass="btn btn-primary">
                            </asp:Button>
                        </div>

                        <div  class="col-md-6">
                        <asp:Label style="position:relative; top:8px; background:transparent;" runat="server" CssClass="col-md-2 control-label">Player</asp:Label>
                                <div style="background:transparent;" class="col-md-10">
                                     <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                     <triggers>
                                     <asp:asyncpostbacktrigger controlid="PlayerList2" eventname="SelectedIndexChanged" />
                                     </triggers>
                                     <ContentTemplate>
                                     <asp:DropDownList OnSelectedIndexChanged="onJoiningPlayerSelectedIndexChanges" style="padding:0; margin-left:0;" CssClass="form-control" runat="server" id="PlayerList2" AutoPostBack="true">
                                       </asp:DropDownList>
                                          </ContentTemplate>
                                         </asp:UpdatePanel>
                                </div>
                        </div>



                    </div>
                       <asp:UpdatePanel ID="UpdatePanelGeneralError" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                         <ContentTemplate>
                               <div  class="col-md-12" style="text-align:left">
                                <asp:Label Text="" CssClass="text-danger" ID="GeneralError" style="text-align:left" runat="server" ></asp:Label>
                               </div>
                          </ContentTemplate>
                    </asp:updatepanel>
                  
                 </div>



            </div>

</asp:Content>
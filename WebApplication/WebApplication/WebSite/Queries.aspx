<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Queries.aspx.cs" Inherits="WebApplication.WebSite.Queries" Async="true" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
       <title>Queries</title>
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

            
            <div class="row" >
                <div style="text-align:center; padding-top:10px; background:transparent;" class="col-md-4">
                       <h4><asp:Label runat="server" Text="Please Select query" /></h4>
                        <hr />

                        <div class="form-horizontal">
                       
                           <div style="position:relative; top:10px;" class="form-group" >
                                <div>
                                     <div  class="col-md-12">
                                        <asp:Button OnClick="OnClickShowAllPlayersInformation"  Text="Show All Players Information"   Style="width:100%; background:white;color:black;border-color:black;" ID="RegisterBT" runat="server" CssClass="btn btn-primary">
                                        </asp:Button>
                                         </div>
                                </div>
                            </div>

                            <div style="position:relative; top:10px;" class="form-group" >
                                <div>
                                     <div  class="col-md-12">
                                        <asp:Button OnClick="OnClickGetAllGames"  Text="Show All Games Information"   Style="width:100%; background:white;color:black;border-color:black;" ID="Button1" runat="server" CssClass="btn btn-primary">
                                        </asp:Button>
                                         </div>
                                </div>
                            </div>

                             <div style="position:relative; top:10px;" class="form-group" >
                                <div>
                                     <div  class="col-md-12">
                                        <asp:Button OnClick="OnClickGetPlayerNGames"  Text="Show number of games for player"   Style="width:100%; background:white;color:black;border-color:black;" ID="Button2" runat="server" CssClass="btn btn-primary">
                                        </asp:Button>
                                         </div>
                                </div>
                            </div>

                            <div style="position:relative; top:10px;" class="row" >
                                <div>
                                    <div  class="col-md-6">
                                        <asp:Label Text="Show player games"   Style="position:relative; top:7px; width:100%; background:white;color:black;border-color:black;" runat="server" >
                                        </asp:Label>
                                      </div>

                                     <div  class="col-md-6">
                                        <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                             <triggers>
                                             <asp:asyncpostbacktrigger controlid="PlayerList" eventname="SelectedIndexChanged" />
                                             </triggers>
                                             <ContentTemplate>
                                                 <asp:DropDownList ViewStateMode="Enabled" OnSelectedIndexChanged="OnSelectedIndexChangeShowAllPlayerPlayedGames" style="padding:0; margin-left:0;" CssClass="form-control" runat="server"  id="PlayerList" AutoPostBack="true">
                                                 </asp:DropDownList>
                                             </ContentTemplate>
                                         </asp:UpdatePanel>
                                      </div>
                                </div>
                            </div>


                               <div style="position:relative; top:20px;" class="row" >
                                <div>
                                    <div  class="col-md-6">
                                        <asp:Label Text="Show game players"   Style="position:relative; top:7px; width:100%; background:white;color:black;border-color:black;" runat="server" >
                                        </asp:Label>
                                      </div>

                                     <div  class="col-md-6">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                             <triggers>
                                             <asp:asyncpostbacktrigger controlid="GamesList" eventname="SelectedIndexChanged" />
                                             </triggers>
                                             <ContentTemplate>
                                                 <asp:DropDownList ViewStateMode="Enabled" OnSelectedIndexChanged="OnSelectedIndexChangeShowPlayerForGame" style="padding:0; margin-left:0;" CssClass="form-control" runat="server"  id="GamesList" AutoPostBack="true">
                                                 </asp:DropDownList>
                                             </ContentTemplate>
                                         </asp:UpdatePanel>
                                      </div>
                                </div>
                            </div>






                        </div>
              </div>
             

                <div style="text-align:center; padding-top:10px;" class="col-md-8">
                   <h4><asp:Label runat="server" Text="Query result " /></h4>
                   <hr />
                    
                   

                    <div style="overflow-y:auto; height:100px;">
                    <asp:UpdatePanel style="text-align:center;" ID="tableGridViewPanel" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                       <triggers>
                        <asp:asyncpostbacktrigger controlid="tableGV" eventname="SelectedIndexChanged" />
                        </triggers>
                        <ContentTemplate>
                            <asp:GridView AutoGenerateSelectButton="True" HorizontalAlign="Center" OnRowDataBound="ResultTable_RowDataBound"  style="border:black; text-align:center;" class="table table-bordered" ID="tableGV" runat="server">
                        </asp:GridView>
                         </ContentTemplate>
                     </asp:UpdatePanel>

                    </div>

                    
                  
                 </div>



            </div>

</asp:Content>
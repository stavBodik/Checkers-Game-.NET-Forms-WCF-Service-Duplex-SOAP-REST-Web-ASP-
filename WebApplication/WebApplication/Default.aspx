<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <div class="row">
           <div class="col-md-8">
            <h1>Checkers Game</h1>
            <p class="lead">Checkers is a free online game play for team or individual players.</p>
            <p><a href="WebSite/Register" class="btn btn-primary btn-lg">Register Now &raquo;</a></p>
           </div>

            <div class="col-md-4">
            <asp:Image Width="100%" Height="100%" ImageUrl ="~/Images/gamess.png" runat="server" />       
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>How To Play</h2>
            <p>
            Nothing could be more simple than that,
            <br>
            Registar new account, download the client and start new game at play page get your game key and enjoy !

            </p>

            
        </div>
        <div class="col-md-8">
            <h2>About The Game</h2>
            <p>
            In checkers game there are two players. The players are at opposite ends of the board. One player has dark pieces, and one player has light pieces. They take turns moving their pieces. Players move their pieces diagonally from one square to another square. When a player jumps over their opponent's (the other player's) piece, you take that piece from the board. If you can take a piece, then you must take a piece.
            <p>
                <a class="btn btn-default" target="_blank" href="https://simple.wikipedia.org/wiki/Checkers">Read more at wiki &raquo;</a>
            </p>
        </div>
        
    </div>

</asp:Content>

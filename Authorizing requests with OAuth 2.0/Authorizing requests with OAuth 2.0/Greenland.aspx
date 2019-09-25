<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Greenland.aspx.cs" Inherits="Authorizing_requests_with_OAuth_2._0.Greenland" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Greenland</title>
    <meta charset="utf-8">
      <meta name="viewport" content="width=device-width, initial-scale=1">
      <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
      <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
      <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
</head>
<body>
    <script>
      window.fbAsyncInit = function() {
        FB.init({
          appId      : '509256986304712',
          cookie     : true,
          xfbml      : true,
          version    : 'v3.2'
        });
      
        FB.AppEvents.logPageView();   
      
      };

      (function(d, s, id){
         var js, fjs = d.getElementsByTagName(s)[0];
         if (d.getElementById(id)) {return;}
         js = d.createElement(s); js.id = id;
         js.src = "https://connect.facebook.net/en_US/sdk.js";
         fjs.parentNode.insertBefore(js, fjs);
      }(document, 'script', 'facebook-jssdk'));


      FB.getLoginStatus(function (response) {
          statusChangeCallback(response);
      });
    </script>

    <form id="form1" runat="server">
        <div class="col-md-12 row">
            <asp:Button ID="btnSignIn" runat="server" Text="Sign in with Facebook" OnClick="btnSignIn_Click" />
        </div>
        <div class="col-md-12 row">
            <div class="col-md-4"></div>
            <div class="col-md-4">                
                <asp:Image ID="imgProfilePic" runat="server" Visible="False" Width="200px" Height="200px" />
            </div>
            <div class="col-md-4"></div>
        </div>
    </form>
</body>
</html>

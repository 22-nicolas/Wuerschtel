<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="bestellungs-rechner.aspx.vb" Inherits="Würschtel.bestellungs_rechner" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-sRIl4kxILFvY47J16cr9ZwB07vP4J8+LH7qKQnuqkuIAvNWLzeN8tE5YBujZqJLB" crossorigin="anonymous">
    <link rel="stylesheet" href="css/styles.css" />
    <!--<link rel="stylesheet" href="css/stylesheet.css" />-->
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <nav class="navbar bg-body-tertiary navbar-expand-lg">
            <div class="container-fluid">
                <span class="navbar-brand mb-0 h1">Würschtel</span>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarNav">
                    <ul class="navbar-nav">
                        <li class="nav-item">
                            <a class="nav-link" href="home.aspx">Bestellen</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link active" aria-current="page" href="bestellungs-rechner.aspx">Bestellungen ansehen</a>
                        </li>
                    </ul>
                </div>
                <div class="nav-item center-flex d-none d-lg-flex">
                    <p runat="server" id="dateP" style="margin-right: 2ch"></p>
                    <p runat="server" id="usernameP"></p>
                </div>
            </div>
        </nav>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>          
                    <div class="container table-responsive">
                        <table class="table table-striped-columns align-middle table-responsive">
                            <thead runat="server" id="tableHead"></thead>
                            <tbody runat="server" id="tableBody"></tbody>
                            <tfoot runat="server" id="tableFoot">
						        
                            </tfoot>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js" integrity="sha384-FKyoEForCGlyvwx9Hj09JcYn3nv7wiPVlz7YYwJrWVcXK/BmnVDxM+D2scQbITxI" crossorigin="anonymous"></script>
</body>

</html>

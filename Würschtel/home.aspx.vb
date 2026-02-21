Imports System.Data.Common
Imports System.Diagnostics.Eventing
Imports System.Drawing
Imports System.EnterpriseServices
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports Mysqlx.XDevAPI.Common

Public Class home
    Inherits System.Web.UI.Page
    Dim username As String = My.User.Name.Split("\")(1)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        usernameP.InnerHtml = username
        dateP.InnerHtml = Date.Now.Date

        produkteLaden()

        If Not IsPostBack Then
            bestellungLaden()
        End If
    End Sub

    Private Sub speicherBtn_ServerClick(sender As Object, e As EventArgs) Handles speicherBtn.ServerClick
        eingabeSpeichern()
    End Sub

    Private Sub bestellungLaden()
        Dim db As New MySqlConnection(My.Settings.connection)
        Dim sql As New MySqlCommand
        sql.Connection = db
        Dim Reader As MySqlDataReader
        Dim produkteReader As MySqlDataReader
        Dim produkte As New Dictionary(Of String, Decimal)

        Try
            db.Open()
            'Produkte ermitteln
            sql.CommandText = "SELECT * FROM produkte"
            produkteReader = sql.ExecuteReader


            While produkteReader.Read()
                produkte.Add(produkteReader("name").ToString(), CDec(produkteReader("preis")))
            End While

            produkteReader.Close()

            sql.Parameters.Clear()
            sql.Parameters.AddWithValue("@name", username)
            sql.CommandText = "
                SELECT * FROM bestellungen
                WHERE name=@name AND DATE(datum)=DATE(NOW());
            "
            Reader = sql.ExecuteReader
            While Reader.Read
                For Each produkt As KeyValuePair(Of String, Decimal) In produkte
                    Dim input As TextBox = tabelle.FindControl(produkt.Key)
                    input.Text = Reader(produkt.Key)
                Next
            End While
            Reader.Close()
            db.Close()
        Catch ex As Exception
            db.Close()
        End Try
    End Sub

    Private Sub eingabeSpeichern()
        Dim db As New MySqlConnection(My.Settings.connection)
        Dim sql As New MySqlCommand
        sql.Connection = db
        Dim Reader As MySqlDataReader
        Dim produkteReader As MySqlDataReader
        Dim produkte As New Dictionary(Of String, Decimal)

        Try
            db.Open()

            'Produkte ermitteln
            sql.CommandText = "SELECT * FROM produkte"
            produkteReader = sql.ExecuteReader


            While produkteReader.Read()
                produkte.Add(produkteReader("name").ToString(), CDec(produkteReader("preis")))
            End While

            produkteReader.Close()

            'such nach vorhandener Bestellung
            sql.Parameters.Clear()
            sql.Parameters.AddWithValue("@name", username)
            sql.CommandText = "
                SELECT id FROM bestellungen
                WHERE name=@name AND DATE(datum)=DATE(NOW());
            "

            Dim bestellung As Object
            bestellung = sql.ExecuteScalar()
            Dim bestellungsId As Integer

            If Not IsDBNull(bestellung) Then
                bestellungsId = bestellung
            End If

            sql.Parameters.AddWithValue("@id", bestellungsId)
            For Each produkt As KeyValuePair(Of String, Decimal) In produkte
                Dim input As TextBox = tabelle.FindControl(produkt.Key)

                sql.Parameters.AddWithValue("@" + produkt.Key, input.Text)
            Next

            If bestellungsId > 0 Then
                'Bestellung ändern
                sql.CommandText = "
                    UPDATE bestellungen
                    SET wiener = @wiener, weiß = @weiß, bauern = @bauern, brezel = @brezel, semmel = @semmel, datum = NOW()
                    WHERE id=@id;
                "
            Else
                'Bestellung erstellen
                sql.CommandText = "
                    INSERT INTO bestellungen (name, wiener, weiß, bauern, brezel, semmel, datum)
                    VALUES (@name, @wiener, @weiß, @bauern, @brezel, @semmel, NOW());
                "
            End If

            sql.ExecuteNonQuery()
            db.Close()
        Catch ex As Exception
            db.Close()
        End Try
    End Sub

    Private Sub produkteLaden()
        Dim db As New MySqlConnection(My.Settings.connection)
        Dim sql As New MySqlCommand
        sql.Connection = db
        Dim Reader As MySqlDataReader

        Try
            'such nach vorhandener Bestellung
            db.Open()
            sql.CommandText = "SELECT * FROM produkte"
            Reader = sql.ExecuteReader

            tabelle.InnerHtml = ""

            While Reader.Read()

                Dim infoPanel As Panel = HTMLProduktInfoElementErstellen(Reader)
                tabelle.Controls.Add(infoPanel)


                Dim stückPanel As Panel = HTMLProduktStückElementErstellen(Reader)
                tabelle.Controls.Add(stückPanel)
            End While

            db.Close()
        Catch ex As Exception
            db.Close()
        End Try
    End Sub

    Private Function HTMLProduktInfoElementErstellen(Reader As MySqlDataReader)
        Dim infoPanel As New Panel()
        infoPanel.CssClass = "col-12 col-lg-6 d-flex justify-content-center justify-content-lg-start"

        Dim div As New Panel()
        div.CssClass = "justify-content-center justify-content-lg-start"
        infoPanel.Controls.Add(div)

        Dim name As String = Reader("name")
        name = name.Substring(0, 1).ToUpper + name.Substring(1)

        Dim nameLbl As New Label()
        nameLbl.Text = name
        nameLbl.CssClass = "fw-bold"
        div.Controls.Add(nameLbl)

        Dim preisLbl As New Label()
        preisLbl.Text = CStr(Reader("preis")) + "€"
        preisLbl.CssClass = "d-block"
        div.Controls.Add(preisLbl)

        Return infoPanel
    End Function

    Private Function HTMLProduktStückElementErstellen(Reader As MySqlDataReader)
        Dim stückPanel As New Panel()
        stückPanel.CssClass = "col-12 col-lg-6 center-flex mb-4 mb-lg-0"

        Dim stückInput As New TextBox()
        stückInput.TextMode = TextBoxMode.Number
        stückInput.Text = "0"
        stückInput.CssClass = "stück-input"
        stückInput.ID = Reader("name")
        stückInput.Attributes.Add("min", 0)
        stückPanel.Controls.Add(stückInput)

        Dim stückLbl As New Label()
        stückLbl.Text = "Stück"
        stückPanel.Controls.Add(stückLbl)

        Return stückPanel
    End Function
End Class
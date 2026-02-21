Imports System.Data.SqlClient
Imports System.Diagnostics.Eventing
Imports System.Reflection
Imports MySql.Data.MySqlClient

Public Class bestellungs_rechner
    Inherits System.Web.UI.Page
    Dim username As String = My.User.Name.Split("\")(1)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        usernameP.InnerHtml = username
        dateP.InnerHtml = Date.Now.Date

        gesammtAnzeigeLaden()
        produktBarLaden()

        bestellungenLaden()
    End Sub

    Protected Sub preisInput_TextChanged(sender As Object, e As EventArgs)
        Dim preisInput As TextBox = CType(sender, TextBox)

        preiseUpdaten(preisInput.ID, preisInput.Text)

        bestellungenLaden()
    End Sub

    Protected Sub bezahltCheckbox_CheckedChanged(sender As Object, e As EventArgs)
        Dim checkbox As CheckBox = CType(sender, CheckBox)
        Dim bestellungsElement = checkbox.Parent.Parent
        Dim bestellungsId = bestellungsElement.ID

        bezahltUpdaten(bestellungsId, checkbox.Checked)
    End Sub

    Private Sub bestellungenLaden()
        Dim db As New MySqlConnection(My.Settings.connection)
        Dim sql As New MySqlCommand
        sql.Connection = db

        Dim Reader As MySqlDataReader
        Dim productReader As MySqlDataReader

        Dim productNames = New List(Of String)
        Dim prices As New Dictionary(Of String, Decimal)
        Dim totalPrice
        totalPrice = 0
        Dim totalAmounts As New Dictionary(Of String, Integer)

        tableBody.Controls.Clear()

        Try
            db.Open()

            'Preis dictionary einrichten
            sql.CommandText = "SELECT * FROM produkte"
            productReader = sql.ExecuteReader

            While productReader.Read()
                productNames.Add(productReader("name").ToString())
                prices.Add(productReader("name").ToString(), CDec(productReader("preis")))
            End While
            productReader.Close()

            'totalAmounts auf die Länge von productNames bringen mit default Wert 0
            For index As Integer = 0 To productNames.Count - 1
                totalAmounts.Add(productNames(index), 0)
            Next

            'Bestellungen für heute holen
            sql.Parameters.Clear()
            sql.Parameters.AddWithValue("@name", username)
            sql.CommandText = "SELECT * FROM bestellungen
                                WHERE DATE(datum)=DATE(NOW());"
            Reader = sql.ExecuteReader
            While Reader.Read
                Dim amounts As New Dictionary(Of String, Integer)
                'Zwischensumme für jeden Nutzer ausrechnen
                Dim subTotal
                subTotal = 0
                For index As Integer = 0 To productNames.Count - 1
                    Dim product = productNames(index)

                    amounts.Add(product, Reader(product))
                    subTotal += amounts(product) * prices(product)
                    totalAmounts(product) += amounts(product)
                Next

                'Zwischensumme zu Gesammt Addieren
                totalPrice += subTotal

                'subTotal auf 2 Nachkommastellen bringen
                subTotal = aufZweiNachkommastellen(subTotal)

                'Listen Elemente erstellen
                If subTotal > 0 Then
                    Dim HTMLBestellungsElement As TableRow = HTMLBestellungsElementErstellen(Reader, productNames, amounts, subTotal)

                    tableBody.Controls.Add(HTMLBestellungsElement)
                End If
            End While

            'Gesammt Anzeige erneuern
            Dim gesammtAnzeige = tableFoot.FindControl("gesammtAnzeige")
            For index As Integer = 0 To productNames.Count - 1
                Dim lbl As TableCell = gesammtAnzeige.FindControl("total" + CStr(productNames(index)) + "Display")
                lbl.Text = CStr(totalAmounts(productNames(index)))
            Next
            totalPrice = aufZweiNachkommastellen(totalPrice)
            Dim totalPriceDisplay As TableCell = gesammtAnzeige.FindControl("totalPriceDisplay")
            totalPriceDisplay.Text = totalPrice + "€"

            Reader.Close()
            db.Close()
        Catch ex As Exception
            db.Close()
        End Try
    End Sub

    Private Sub gesammtAnzeigeLaden()
        Dim db As New MySqlConnection(My.Settings.connection)
        Dim sql As New MySqlCommand
        sql.Connection = db

        Dim Reader As MySqlDataReader

        Try
            db.Open()

            sql.CommandText = "SELECT * FROM produkte"
            Reader = sql.ExecuteReader


            tableFoot.Controls.Clear()

            Dim footRow As TableRow = HTMLGesammtAnzeigeElementErstellen(Reader)

            tableFoot.Controls.Add(footRow)

            Reader.Close()
            db.Close()

        Catch ex As Exception
            db.Close()
        End Try
    End Sub

    Private Sub produktBarLaden()
        Dim db As New MySqlConnection(My.Settings.connection)
        Dim sql As New MySqlCommand
        sql.Connection = db

        Dim Reader As MySqlDataReader

        Try
            db.Open()

            sql.CommandText = "SELECT * FROM produkte"
            Reader = sql.ExecuteReader

            tableHead.Controls.Clear()

            Dim bar As TableRow = HTMLProduktElementErstellen(Reader)

            tableHead.Controls.Add(bar)

            Reader.Close()
            db.Close()

        Catch ex As Exception
            db.Close()
        End Try
    End Sub

    Private Function HTMLGesammtAnzeigeElementErstellen(Reader As MySqlDataReader)
        Dim footRow As New TableRow()
        footRow.CssClass = "table-active"
        footRow.ID = "gesammtAnzeige"

        Dim totalLabel As New TableCell()
        totalLabel.Text = "Gesamt:"
        footRow.Cells.Add(totalLabel)

        While Reader.Read()
            Dim cell As New TableCell()
            cell.Text = "0"
            cell.ID = "total" + CStr(Reader("name")) + "Display"
            footRow.Cells.Add(cell)
        End While

        Dim totalCell As New TableCell()
        totalCell.Text = 0 & "€"
        totalCell.ID = "totalPriceDisplay"
        footRow.Cells.Add(totalCell)


        Return footRow
    End Function

    Private Function HTMLProduktElementErstellen(Reader As MySqlDataReader)
        Dim bar As New TableRow()
        bar.CssClass = "table-active"

        'Nutzer Spalte
        Dim nutzerSpalte As New TableCell()
        Dim nutzerLbl As New Label()
        nutzerLbl.Text = "Nutzer:"
        nutzerSpalte.Controls.Add(nutzerLbl)
        bar.Cells.Add(nutzerSpalte)

        While Reader.Read()
            Dim spalte = HTMLSpaltenElementErstellen(Reader("name"), Reader("preis"))

            bar.Cells.Add(spalte)
        End While

        'Zwischensumme Spalte
        Dim zwischensumme As New TableCell()
        Dim zwischensummeLbl As New Label()
        zwischensummeLbl.Text = "Zwischensumme:"

        zwischensumme.Controls.Add(zwischensummeLbl)
        bar.Cells.Add(zwischensumme)

        Return bar
    End Function

    Private Function HTMLBestellungsElementErstellen(Reader As MySqlDataReader, productNames As List(Of String), amounts As Dictionary(Of String, Integer), subTotal As String)
        Dim rowPanel As New TableRow()


        Dim namePanel As New TableCell()
        namePanel.CssClass = "col d-flex justify-content-left align-items-center"
        rowPanel.Cells.Add(namePanel)

        Dim checkbox As New CheckBox()
        checkbox.Checked = Reader("bezahlt")
        checkbox.AutoPostBack = True
        'checkbox.ID = CStr(Reader("id"))
        AddHandler checkbox.CheckedChanged, AddressOf bezahltCheckbox_CheckedChanged
        namePanel.Controls.Add(checkbox)

        Dim nameLbl As New Label()
        nameLbl.Text = CStr(Reader("name"))
        namePanel.Controls.Add(nameLbl)

        For index As Integer = 0 To productNames.Count - 1
            Dim lbl As New TableCell()
            lbl.CssClass = "col"
            lbl.Text = CStr(amounts(productNames(index)))
            rowPanel.Cells.Add(lbl)
        Next

        Dim subTotalLbl As New TableCell()
        subTotalLbl.Text = subTotal + "€"
        rowPanel.Cells.Add(subTotalLbl)

        rowPanel.ID = CStr(Reader("id"))

        Return rowPanel
    End Function

    Private Function HTMLSpaltenElementErstellen(name, preis)
        Dim spalte As New TableCell()

        Dim lbl As New Label()
        lbl.Text = name.Substring(0, 1).ToUpper + name.Substring(1)

        Dim preisPanel As New Panel()
        preisPanel.CssClass = "d-flex justify-content-start align-items-center"

        Dim txtBox As New TextBox()
        txtBox.Text = CStr(preis).Replace(",", ".")
        txtBox.TextMode = TextBoxMode.Number
        txtBox.CssClass = "stück-input"
        txtBox.Attributes.Add("min", 0)
        txtBox.ID = name
        txtBox.AutoPostBack = True
        AddHandler txtBox.TextChanged, AddressOf preisInput_TextChanged


        Dim euroZeichen As New Label()
        euroZeichen.Text = "€"

        preisPanel.Controls.Add(txtBox)
        preisPanel.Controls.Add(euroZeichen)

        spalte.Controls.Add(lbl)
        spalte.Controls.Add(preisPanel)

        Return spalte
    End Function

    Private Sub preiseUpdaten(produktName, neuerPreis)
        Dim db As New MySqlConnection(My.Settings.connection)
        Dim sql As New MySqlCommand
        sql.Connection = db

        Try
            db.Open()
            sql.Parameters.Clear()
            sql.Parameters.AddWithValue("@produktName", produktName)
            sql.Parameters.AddWithValue("@neuerPreis", neuerPreis)

            sql.CommandText = "
                UPDATE produkte
                SET preis = @neuerPreis
                WHERE name = @produktName
            "

            sql.ExecuteNonQuery()

            db.Close()
        Catch ex As Exception
            db.Close()
        End Try
    End Sub

    Private Sub bezahltUpdaten(bestellungsId, bezahlt)
        Dim db As New MySqlConnection(My.Settings.connection)
        Dim sql As New MySqlCommand
        sql.Connection = db

        Try
            db.Open()
            sql.Parameters.Clear()
            sql.Parameters.AddWithValue("@id", bestellungsId)
            sql.Parameters.AddWithValue("@bezahlt", bezahlt)

            sql.CommandText = "
                UPDATE bestellungen
                SET bezahlt = @bezahlt
                WHERE id = @id
            "

            sql.ExecuteNonQuery()

            db.Close()
        Catch ex As Exception
            db.Close()
        End Try
    End Sub

    Private Function aufZweiNachkommastellen(number)
        number = CStr(number)

        If number.Contains(",") = False Then
            number += ",00"
            Return number
        End If

        Dim nachkomma As String = number.Split(",")(1)
        If nachkomma.Length < 2 Then
            number += "0"
        End If

        Return number
    End Function
End Class
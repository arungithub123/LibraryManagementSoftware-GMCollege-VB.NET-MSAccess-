﻿Imports System.Data.OleDb
Imports Excel = Microsoft.Office.Interop.Excel
Public Class frmBookIssueRecord_Staff2
    Dim rdr As OleDbDataReader = Nothing
    Dim dtable As DataTable
    Dim con As OleDbConnection = Nothing
    Dim adp As OleDbDataAdapter
    Dim ds As DataSet
    Dim cmd As OleDbCommand = Nothing
    Dim dt As New DataTable
    Dim gender As String
    Dim cs As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\LMS_DB.accdb;Persist Security Info=False;"

    Sub Reset()
        txtBookName.Text = ""
        txtBookName1.Text = ""
        txtStaffName.Text = ""
        cmbStaffName.Text = ""
        cmbStaffName1.Text = ""
        dtpDateFrom.Text = Today
        dtpDateTo.Text = Today
        DateTimePicker1.Text = Today
        DateTimePicker2.Text = Today
        DateTimePicker3.Text = Today
        DateTimePicker4.Text = Today
        DateTimePicker5.Text = Today
        DateTimePicker6.Text = Today
        DateTimePicker7.Text = Today
        DateTimePicker8.Text = Today
        DataGridView1.DataSource = Nothing
    End Sub
    Sub fillStaffName()
        Try
            Dim CN As New OleDbConnection(cs)
            CN.Open()
            adp = New OleDbDataAdapter()
            adp.SelectCommand = New OleDbCommand("SELECT distinct RTRIM(Staffname) FROM Staff,BookIssue_Staff where Staff.StaffID=BookIssue_Staff.StaffID", CN)
            ds = New DataSet("ds")
            adp.Fill(ds)
            dtable = ds.Tables(0)
            cmbStaffName.Items.Clear()
            cmbStaffName1.Items.Clear()
            For Each drow As DataRow In dtable.Rows
                cmbStaffName.Items.Add(drow(0).ToString())
                cmbStaffName1.Items.Add(drow(0).ToString())
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub frmBookIssueRecord_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        fillStaffName()
        fillAccNO()

    End Sub
    Public Sub fillAccNO()
        Try
            Dim sql As String = "select Distinct(AccessionNo) from BookIssue_Staff"
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand(sql, con)
            rdr = cmd.ExecuteReader()
            cmbBookCode.Items.Clear()
            While rdr.Read
                cmbAccNo.Items.Add(rdr("AccessionNo").ToString())
            End While
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub txtBookName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBookName.TextChanged
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand("SELECT TransactionID as [Transaction ID], IssueDate as [Issue Date], DueDate as [Due Date], Book.AccessionNo as [Accession No],BookIssue_Staff.BCode as [Book Code],BookTitle as [Book Title],Author,Subject,ISBN,Edition,Staff.StaffID as [Staff ID],StaffName as [Staff Name],Staff.Department, Status from Book,BookIssue_Staff,Staff where Book.AccessionNo=BookIssue_Staff.AccessionNo and BookIssue_Staff.StaffID=Staff.StaffID and Status='Issued' and Status='Issued' and Issuedate Between #" & dtpDateFrom.Text & "# and #" & dtpDateTo.Text & "# and BookTitle like '" & txtBookName.Text & "%' order by Issuedate desc ", con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookIssue_Staff")
            myDA.Fill(myDataSet, "Book")
            myDA.Fill(myDataSet, "Staff")
            DataGridView1.DataSource = myDataSet.Tables("BookIssue_Staff").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Book").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Staff").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Reset()
    End Sub

    Private Sub btnExportExcel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExportExcel.Click
        If DataGridView1.RowCount = Nothing Then
            MessageBox.Show("Sorry nothing to export into excel sheet.." & vbCrLf & "Please retrieve data in datagridview", "", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        Dim rowsTotal, colsTotal As Short
        Dim I, j, iC As Short
        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
        Dim xlApp As New Excel.Application
        Try
            Dim excelBook As Excel.Workbook = xlApp.Workbooks.Add
            Dim excelWorksheet As Excel.Worksheet = CType(excelBook.Worksheets(1), Excel.Worksheet)
            xlApp.Visible = True

            rowsTotal = DataGridView1.RowCount - 1
            colsTotal = DataGridView1.Columns.Count - 1
            With excelWorksheet
                .Cells.Select()
                .Cells.Delete()
                For iC = 0 To colsTotal
                    .Cells(1, iC + 1).Value = DataGridView1.Columns(iC).HeaderText
                Next
                For I = 0 To rowsTotal - 1
                    For j = 0 To colsTotal
                        .Cells(I + 2, j + 1).value = DataGridView1.Rows(I).Cells(j).Value
                    Next j
                Next I
                .Rows("1:1").Font.FontStyle = "Bold"
                .Rows("1:1").Font.Size = 12

                .Cells.Columns.AutoFit()
                .Cells.Select()
                .Cells.EntireColumn.AutoFit()
                .Cells(1, 1).Select()
            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            'RELEASE ALLOACTED RESOURCES
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
            xlApp = Nothing
        End Try
    End Sub

    Private Sub txtBookName1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBookName1.TextChanged
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand("SELECT TransactionID as [Transaction ID], IssueDate as [Issue Date], DueDate as [Due Date], Book.AccessionNo as [Accession No],BookIssue_Staff.BCode as [Book Code],BookTitle as [Book Title],Author,Subject,ISBN,Edition,Staff.StaffID as [Staff ID],StaffName as [Staff Name],Staff.Department, Status from Book,BookIssue_Staff,Staff where Book.AccessionNo=BookIssue_Staff.AccessionNo and BookIssue_Staff.StaffID=Staff.StaffID and Status='Issued' and DueDate Between #" & DateTimePicker2.Text & "# and #" & DateTimePicker1.Text & "# and BookTitle like '" & txtBookName1.Text & "%' order by DueDate desc ", con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookIssue_Staff")
            myDA.Fill(myDataSet, "Book")
            myDA.Fill(myDataSet, "Staff")
            DataGridView1.DataSource = myDataSet.Tables("BookIssue_Staff").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Book").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Staff").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand("SELECT TransactionID as [Transaction ID], IssueDate as [Issue Date], DueDate as [Due Date], Book.AccessionNo as [Accession No],BookIssue_Staff.BCode as [Book Code],BookTitle as [Book Title],Author,Subject,ISBN,Edition,Staff.StaffID as [Staff ID],StaffName as [Staff Name],Staff.Department, Status from Book,BookIssue_Staff,Staff where Book.AccessionNo=BookIssue_Staff.AccessionNo and BookIssue_Staff.StaffID=Staff.StaffID and Status='Issued' and IssueDate Between #" & DateTimePicker8.Text & "# and #" & DateTimePicker7.Text & "#  order by IssueDate desc ", con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookIssue_Staff")
            myDA.Fill(myDataSet, "Book")
            myDA.Fill(myDataSet, "Staff")
            DataGridView1.DataSource = myDataSet.Tables("BookIssue_Staff").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Book").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Staff").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmbStaffName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbStaffName.SelectedIndexChanged
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand("SELECT TransactionID as [Transaction ID], IssueDate as [Issue Date], DueDate as [Due Date], Book.AccessionNo as [Accession No],BookIssue_Staff.BCode as [Book Code],BookTitle as [Book Title],Author,Subject,ISBN,Edition,Staff.StaffID as [Staff ID],StaffName as [Staff Name],Staff.Department, Status from Book,BookIssue_Staff,Staff where Book.AccessionNo=BookIssue_Staff.AccessionNo and BookIssue_Staff.StaffID=Staff.StaffID and Status='Issued' and IssueDate Between #" & DateTimePicker4.Text & "# and #" & DateTimePicker3.Text & "# and StaffName= '" & cmbStaffName.Text & "' order by IssueDate desc ", con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookIssue_Staff")
            myDA.Fill(myDataSet, "Book")
            myDA.Fill(myDataSet, "Staff")
            DataGridView1.DataSource = myDataSet.Tables("BookIssue_Staff").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Book").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Staff").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub cmbStaffName1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbStaffName1.SelectedIndexChanged
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand("SELECT TransactionID as [Transaction ID], IssueDate as [Issue Date], DueDate as [Due Date], Book.AccessionNo as [Accession No],BookIssue_Staff.BCode as [Book Code],BookTitle as [Book Title],Author,Subject,ISBN,Edition,Staff.StaffID as [Staff ID],StaffName as [Staff Name],Staff.Department, Status from Book,BookIssue_Staff,Staff where Book.AccessionNo=BookIssue_Staff.AccessionNo and BookIssue_Staff.StaffID=Staff.StaffID and Status='Issued' and DueDate Between #" & DateTimePicker6.Text & "# and #" & DateTimePicker5.Text & "# and StaffName= '" & cmbStaffName1.Text & "' order by DueDate desc ", con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookIssue_Staff")
            myDA.Fill(myDataSet, "Book")
            myDA.Fill(myDataSet, "Staff")
            DataGridView1.DataSource = myDataSet.Tables("BookIssue_Staff").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Book").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Staff").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtStaffName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtStaffName.TextChanged
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand("SELECT TransactionID as [Transaction ID], IssueDate as [Issue Date], DueDate as [Due Date], Book.AccessionNo as [Accession No],BookIssue_Staff.BCode as [Book Code],BookTitle as [Book Title],Author,Subject,ISBN,Edition,Staff.StaffID as [Staff ID],StaffName as [Staff Name],Staff.Department, Status from Book,BookIssue_Staff,Staff where Book.AccessionNo=BookIssue_Staff.AccessionNo and BookIssue_Staff.StaffID=Staff.StaffID and Status='Issued' and StaffName like '" & txtStaffName.Text & "%' order by Staffname ", con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookIssue_Staff")
            myDA.Fill(myDataSet, "Book")
            myDA.Fill(myDataSet, "Staff")
            DataGridView1.DataSource = myDataSet.Tables("BookIssue_Staff").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Book").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Staff").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridView1_RowHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DataGridView1.RowHeaderMouseClick
        Try
            Dim dr As DataGridViewRow = DataGridView1.SelectedRows(0)
            Me.Hide()
            frmBookReturn.Show()
            frmBookReturn.txtTransactionID1.Text = dr.Cells("Transaction ID").Value.ToString()
            frmBookReturn.dtpIssueDate1.Text = dr.Cells("Issue Date").Value.ToString()
            frmBookReturn.dtpDueDate1.Text = dr.Cells("Due Date").Value.ToString()
            frmBookReturn.txtAccessionNo1.Text = dr.Cells("Accession No").Value.ToString()
            frmBookReturn.txtBookCode1.Text = dr.Cells("Book Code").Value.ToString()
            frmBookReturn.txtBookTitle1.Text = dr.Cells("Book Title").Value.ToString()
            frmBookReturn.txtAuthor1.Text = dr.Cells("Author").Value.ToString()
            frmBookReturn.txtCategory1.Text = dr.Cells("Subject").Value.ToString()
            frmBookReturn.txtISBN1.Text = dr.Cells("ISBN").Value.ToString()
            frmBookReturn.txtEdition1.Text = dr.Cells("Edition").Value.ToString()
            frmBookReturn.txtStaffID.Text = dr.Cells("Staff ID").Value.ToString()
            frmBookReturn.txtStaffName.Text = dr.Cells("Staff Name").Value.ToString()
            frmBookReturn.txtDepartment1.Text = dr.Cells("Department").Value.ToString()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridView1_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles DataGridView1.RowPostPaint
        Dim strRowNumber As String = (e.RowIndex + 1).ToString()
        Dim size As SizeF = e.Graphics.MeasureString(strRowNumber, Me.Font)
        If DataGridView1.RowHeadersWidth < Convert.ToInt32((size.Width + 20)) Then
            DataGridView1.RowHeadersWidth = Convert.ToInt32((size.Width + 20))
        End If
        Dim b As Brush = SystemBrushes.ControlText
        e.Graphics.DrawString(strRowNumber, Me.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2))

    End Sub

    Private Sub dtpDateTo_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles dtpDateTo.Validating
        If (dtpDateFrom.Value.Date) > (dtpDateTo.Value.Date) Then
            MessageBox.Show("Invalid Selection", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            dtpDateTo.Focus()
        End If
    End Sub

    Private Sub DateTimePicker1_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles DateTimePicker1.Validating
        If (DateTimePicker2.Value.Date) > (DateTimePicker1.Value.Date) Then
            MessageBox.Show("Invalid Selection", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            DateTimePicker1.Focus()
        End If
    End Sub

    Private Sub DateTimePicker7_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles DateTimePicker7.Validating
        If (DateTimePicker8.Value.Date) > (DateTimePicker7.Value.Date) Then
            MessageBox.Show("Invalid Selection", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            DateTimePicker7.Focus()
        End If
    End Sub

    Private Sub DateTimePicker3_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles DateTimePicker3.Validating
        If (DateTimePicker4.Value.Date) > (DateTimePicker3.Value.Date) Then
            MessageBox.Show("Invalid Selection", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            DateTimePicker3.Focus()
        End If
    End Sub

    Private Sub DateTimePicker5_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles DateTimePicker5.Validating
        If (DateTimePicker6.Value.Date) > (DateTimePicker5.Value.Date) Then
            MessageBox.Show("Invalid Selection", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            DateTimePicker5.Focus()
        End If
    End Sub

    Private Sub cmbAccNo_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbAccNo.SelectedIndexChanged
        Try
            Dim sql As String = "select Distinct(BCode) from BookIssue_Staff where AccessionNo='" & Trim(cmbAccNo.Text) & "'"
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand(sql, con)
            rdr = cmd.ExecuteReader()
            cmbBookCode.Items.Clear()
            While rdr.Read
                cmbBookCode.Items.Add(rdr("BCode").ToString())
            End While
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand("SELECT TransactionID as [Transaction ID], IssueDate as [Issue Date], DueDate as [Due Date], Book.AccessionNo as [Accession No],BookIssue_Staff.BCode as [Book Code],BookTitle as [Book Title],Author,Subject,ISBN,Edition,Staff.StaffID as [Staff ID],StaffName as [Staff Name],Staff.Department, Status from Book,BookIssue_Staff,Staff where Book.AccessionNo=BookIssue_Staff.AccessionNo and BookIssue_Staff.StaffID=Staff.StaffID and Status='Issued' and BookIssue_Staff.AccessionNo='" & cmbAccNo.Text & "' and BookIssue_Staff.BCode='" & cmbBookCode.Text & "' order by IssueDate desc ", con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookIssue_Staff")
            myDA.Fill(myDataSet, "Book")
            myDA.Fill(myDataSet, "Staff")
            DataGridView1.DataSource = myDataSet.Tables("BookIssue_Staff").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Book").DefaultView
            DataGridView1.DataSource = myDataSet.Tables("Staff").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
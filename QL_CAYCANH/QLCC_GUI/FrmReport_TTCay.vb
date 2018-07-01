Imports System.Data.SqlClient
Imports CrystalDecisions.CrystalReports.Engine.ReportDocument

Public Class FrmReport_TTCay
    Dim con As New SqlClient.SqlConnection
    Dim dt As New DataTable
    Dim cmd As New SqlClient.SqlCommand
    Dim da As New SqlClient.SqlDataAdapter
    Dim ds As New DS_ChiPhi
    Dim rpt As New CrystalReport_TTCay
    Private Sub GetData()
        Try
            ds.Clear()
            con.ConnectionString = "Data Source=DESKTOP-SNJJV8M;Initial Catalog=QLCC;Integrated Security=True"
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM BCCT_TINHTRANGCAY"
            cmd.CommandType = CommandType.Text
            da.SelectCommand = cmd
            da.Fill(ds, "BCCT_TINHTRANGCAY")
            rpt.SetDataSource(ds)
            CrystalReportViewer1.ReportSource = rpt
            CrystalReportViewer1.Refresh()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
    Private Sub FrmReport_TTCay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GetData()
    End Sub
End Class
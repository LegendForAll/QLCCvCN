Imports QLCC_BUS
Imports QLCC_DTO
Imports Utility
Imports System.Data.SqlClient
Imports System.Configuration

Public Class FrmThemCay
    Private cayBus As CayBUS
    Private vitri As ViTriCayBUS
    ' Read ConnectionString value from App.config file
    Dim connection As New SqlConnection(ConfigurationManager.AppSettings("ConnectionString"))

    Public Sub FiterData(valSearch As String)
        Dim searchQuery As String = "SELECT C.ID_CAY AS '" & "ID" & "', C.TENCAY AS '" & "Tree name" & "', L.TEN_LOAICAY AS '" & "Type" & "', VT.TEN_VITRI AS '" & "Loacation" & "', C.NGAYTRONG AS '" & "Planting date" & "'
        FROM [CAY] C, [VITRI] VT, [LOAICAY] L
        WHERE C.ID_VITRI = VT. ID_VITRI AND C.ID_LOAICAY = L.ID_LOAICAY"
        Dim command As New SqlCommand(searchQuery, connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable()
        adapter.Fill(table)

        DataGridView1.DataSource = table
    End Sub

    Public Sub SoCay()
        'Update count tree
        Dim Query As String = "SELECT COUNT(ID_CAY) FROM [CAY] WHERE ID_VITRI =  '" & Convert.ToString(cbx_ViTri.SelectedValue) & "'"
        connection.Open()
        Dim Command As New System.Data.SqlClient.SqlCommand(Query, connection)
        Dim Count As Integer = Command.ExecuteScalar()
        connection.Close()
    End Sub

    Private Sub FrmThemCay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cayBus = New CayBUS()
        vitri = New ViTriCayBUS()

        FiterData("")
        ' Load loai cay list

        Dim command As New SqlCommand("SELECT * FROM [LOAICAY]", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable()
        adapter.Fill(table)
        cbx_LoaiCay.DataSource = table
        cbx_LoaiCay.DisplayMember = "TEN_LOAICAY"
        cbx_LoaiCay.ValueMember = "ID_LOAICAY"

        ' Load vi tri cay list

        Dim command_vt As New SqlCommand("SELECT * FROM [VITRI]", connection)
        Dim adapter_vt As New SqlDataAdapter(command_vt)
        Dim table_vt As New DataTable()
        adapter_vt.Fill(table_vt)
        cbx_ViTri.DataSource = table_vt
        cbx_ViTri.DisplayMember = "TEN_VITRI"
        cbx_ViTri.ValueMember = "ID_VITRI"

        'load so cay toi da
        Dim command_cm As New SqlCommand("SELECT * FROM [QUIDINH]", connection)
        Dim adapter_cm As New SqlDataAdapter(command_cm)
        Dim table_cm As New DataTable()
        adapter_cm.Fill(table_cm)
        cbx_CayMax.DataSource = table_cm
        cbx_CayMax.DisplayMember = "SL_MAX"
        cbx_CayMax.ValueMember = "ID_QD"

        'set MSSH auto
        Dim result As Result
        Dim nextMshs = "1"
        result = CayBUS.buildMS_Cay(nextMshs)
        If (result.FlagResult = False) Then
            MessageBox.Show("Get the autocomplete code tree failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            System.Console.WriteLine(result.SystemMessage)
            Return
        End If
        tbx_IDCay.Text = nextMshs
    End Sub

    Private Sub btn_ThemCay_Click(sender As Object, e As EventArgs) Handles btn_ThemCay.Click
        Dim Cay As CayDTO
        Cay = New CayDTO()

        'Update count tree
        Dim Query As String = "SELECT COUNT(ID_CAY) FROM [CAY] WHERE ID_VITRI =  '" & Convert.ToString(cbx_ViTri.SelectedValue) & "'"
        connection.Open()
        Dim Command As New System.Data.SqlClient.SqlCommand(Query, connection)
        Dim Count As Integer = Command.ExecuteScalar()
        connection.Close()

        If (Count >= Convert.ToInt32(cbx_CayMax.Text)) Then
            MessageBox.Show("Tree/Location is MAX.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            '1. Mapping data from GUI control
            Cay.MS_Cay = tbx_IDCay.Text
            Cay.TenCay = tbx_TenCay.Text
            Cay.MS_LoaiCay = Convert.ToString(cbx_LoaiCay.SelectedValue)
            Cay.ViTriCay = Convert.ToString(cbx_ViTri.SelectedValue)
            Cay.NgayTrong = dtp_NgayTrong.Value

            '2. Business .....
            If (cayBus.isValidName(Cay) = False) Then
                MessageBox.Show("Tree name is not correct")
                tbx_TenCay.Focus()
                Return
            End If
            '3. Insert to DB
            Dim result As Result
            result = cayBus.insert(Cay)
            If (result.FlagResult = True) Then
                MessageBox.Show("Add tree Success.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)

                'set MSSH auto
                Dim nextMshs = "1"
                result = cayBus.buildMS_Cay(nextMshs)
                If (result.FlagResult = False) Then
                    MessageBox.Show("Get the autocomplete code tree failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                tbx_IDCay.Text = nextMshs
                tbx_TenCay.Text = String.Empty

                'update data
                FiterData("")

            Else
                MessageBox.Show("Add tree unsuccessful.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                System.Console.WriteLine(result.SystemMessage)
            End If
        End If

    End Sub

    Private Sub btn_huy_Click(sender As Object, e As EventArgs) Handles btn_huy.Click
        Me.Close()
    End Sub

    Private Sub btn_Update_Click(sender As Object, e As EventArgs) Handles btn_Update.Click
        Dim frm As FrmUpdateCay = New FrmUpdateCay()
        frm.Show()
    End Sub
End Class
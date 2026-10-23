Imports AccpacCOMAPI

''' <summary>
''' Alta / Baja / Cambio / Consulta de usuarios (VB6: frmAltas, frmBajas, frmCambios).
''' El modo se recibe en el constructor, así cada opción del menú abre su propia instancia.
''' </summary>
Public Class frmUsuarios
    Private ReadOnly _modo As ModoCrud

    Public Sub New(modo As ModoCrud)
        InitializeComponent()
        _modo = modo
    End Sub

    ' Sin usuario seleccionado se limpian los datos mostrados (en Alta los captura la persona, no se tocan).
    Private Sub LimpiaDetalle()
        If _modo = ModoCrud.Alta Then Return
        For Each txt As TextBox In {txtNombre, txtApellido, txtSucursal, txtPuesto, txtFuncion}
            txt.Clear()
        Next
    End Sub

    Private Sub frmUsuarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AplicaBordeEmpresa(Me)
        HabilitaBusqueda(cmbUsuarios, AddressOf LimpiaDetalle)
        Try
            Select Case _modo
                Case ModoCrud.Alta
                    Text = "Alta de Usuarios"
                    btnAceptar.Text = "Aceptar"
                    For Each u As String In UsuariosSageSinAlta()
                        cmbUsuarios.Items.Add(u)
                    Next
                Case Else
                    Select Case _modo
                        Case ModoCrud.Baja
                            Text = "Baja de Usuarios"
                            btnAceptar.Text = "Eliminar"
                        Case ModoCrud.Cambio
                            Text = "Actualización de Usuarios"
                            btnAceptar.Text = "Aceptar"
                        Case Else
                            Text = "Consulta de Usuarios"
                            btnAceptar.Visible = False
                    End Select
                    For Each u As String In UsuariosRegistrados()
                        cmbUsuarios.Items.Add(u)
                    Next
            End Select

            Dim soloLectura As Boolean = (_modo = ModoCrud.Baja OrElse _modo = ModoCrud.Consulta)
            For Each txt As TextBox In {txtNombre, txtApellido, txtSucursal, txtPuesto, txtFuncion}
                txt.ReadOnly = soloLectura
            Next
        Catch ex As Exception
            MessageBox.Show("Error al cargar usuarios: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>Usuarios de Sage 300 (vista AS0003) que todavía no están en la tabla USUARIOS.</summary>
    Private Function UsuariosSageSinAlta() As List(Of String)
        Dim principal As frmPrincipal = TryCast(MdiParent, frmPrincipal)
        If principal Is Nothing OrElse principal.mSession Is Nothing Then
            Throw New InvalidOperationException("No hay una sesión activa de Sage 300.")
        End If

        Dim registrados As New HashSet(Of String)(UsuariosRegistrados(), StringComparer.OrdinalIgnoreCase)
        Dim resultado As New List(Of String)
        Dim enlace As AccpacDBLink = Nothing
        Dim vista As AccpacView = Nothing

        Try
            ' Los usuarios de Sage viven en la base de datos de sistema.
            enlace = principal.mSession.OpenDBLink(tagDBLinkTypeEnum.DBLINK_SYSTEM, tagDBLinkFlagsEnum.DBLINK_FLG_READONLY)
            enlace.OpenView("AS0003", vista)
            vista.Browse("", True)

            Dim hayRegistro As Boolean = vista.GoTop()
            While hayRegistro
                Dim usuario As String = CStr(vista.Fields.FieldByName("USERID").Value).Trim()
                If Not registrados.Contains(usuario) Then resultado.Add(usuario)
                hayRegistro = vista.GoNext()
            End While
        Finally
            If vista IsNot Nothing Then System.Runtime.InteropServices.Marshal.ReleaseComObject(vista)
            If enlace IsNot Nothing Then System.Runtime.InteropServices.Marshal.ReleaseComObject(enlace)
        End Try

        Return resultado
    End Function

    Private Sub cmbUsuarios_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbUsuarios.SelectedIndexChanged
        If _modo = ModoCrud.Alta OrElse cmbUsuarios.SelectedItem Is Nothing Then Return
        Try
            Dim fila As DataRow = ObtenerUsuario(cmbUsuarios.Text.Trim())
            If fila Is Nothing Then Return
            txtNombre.Text = ChecaNullStr(fila("nombre"))
            txtApellido.Text = ChecaNullStr(fila("apellido"))
            txtSucursal.Text = ChecaNullStr(fila("sucursal"))
            txtPuesto.Text = ChecaNullStr(fila("puesto"))
            txtFuncion.Text = ChecaNullStr(fila("funcion"))
        Catch ex As Exception
            MessageBox.Show("Error al consultar el usuario: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function ValidaCampos() As Boolean
        Dim faltantes As New List(Of String)
        If cmbUsuarios.SelectedItem Is Nothing Then faltantes.Add("la Clave de Usuario")
        If _modo <> ModoCrud.Baja Then
            If txtNombre.Text.Trim().Length = 0 Then faltantes.Add("el Nombre")
            If txtApellido.Text.Trim().Length = 0 Then faltantes.Add("el Apellido")
            If txtSucursal.Text.Trim().Length = 0 Then faltantes.Add("la Sucursal")
        End If
        If faltantes.Count > 0 Then
            MessageBox.Show("Es necesario ingresar " & String.Join(", ", faltantes), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function

    Private Sub btnAceptar_Click(sender As Object, e As EventArgs) Handles btnAceptar.Click
        If Not ValidaCampos() Then Return
        Dim usuario As String = cmbUsuarios.Text.Trim()

        Try
            Select Case _modo
                Case ModoCrud.Alta
                    InsertarUsuario(usuario, txtNombre.Text.Trim(), txtApellido.Text.Trim(), txtSucursal.Text.Trim(),
                                    txtPuesto.Text.Trim(), txtFuncion.Text.Trim())
                    MessageBox.Show("Usuario Ingresado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Case ModoCrud.Baja
                    If MessageBox.Show("¿Está seguro de eliminar este usuario y todos sus privilegios?", "Confirmar",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
                    EliminarUsuario(usuario)
                    MessageBox.Show("Usuario eliminado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Case ModoCrud.Cambio
                    If MessageBox.Show("¿Está seguro de actualizar este usuario y todos sus privilegios?", "Confirmar",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
                    ActualizarUsuario(usuario, txtNombre.Text.Trim(), txtApellido.Text.Trim(), txtSucursal.Text.Trim(),
                                      txtPuesto.Text.Trim(), txtFuncion.Text.Trim())
                    MessageBox.Show("Usuario actualizado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Select
            Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Close()
    End Sub

End Class

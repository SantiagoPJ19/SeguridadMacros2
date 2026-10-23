''' <summary>
''' Alta / Baja / Cambio / Consulta de privilegios por usuario, módulo y pantalla
''' (VB6: frmAltaPriv, frmBajaPriv, frmCamPriv).
''' </summary>
Public Class frmPrivilegios
    Private ReadOnly _modo As ModoCrud

    Public Sub New(modo As ModoCrud)
        InitializeComponent()
        _modo = modo
    End Sub

    Private Sub frmPrivilegios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AplicaBordeEmpresa(Me)
        Select Case _modo
            Case ModoCrud.Alta
                Text = "Alta de Privilegios"
            Case ModoCrud.Baja
                Text = "Baja de Privilegios"
                btnAceptar.Text = "Eliminar"
                cmbAplica.Enabled = False
            Case ModoCrud.Cambio
                Text = "Cambio de Privilegios"
            Case Else
                Text = "Consulta de Privilegios"
                cmbAplica.Enabled = False
                btnAceptar.Visible = False
                btnSalir.Text = "Salir"
        End Select

        Try
            For Each u As String In UsuariosRegistrados()
                cmbUsuarios.Items.Add(u)
            Next
            For Each m As String In CatalogoClaves(TipoCatalogo.Modulo)
                cmbModulos.Items.Add(m)
            Next
        Catch ex As Exception
            MessageBox.Show("Error al cargar los catálogos: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmbUsuarios_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbUsuarios.SelectedIndexChanged
        If cmbUsuarios.SelectedItem Is Nothing Then Return
        Try
            Dim fila As DataRow = ObtenerUsuario(cmbUsuarios.Text.Trim())
            If fila IsNot Nothing Then
                txtNombre.Text = ChecaNullStr(fila("nombre")).Trim()
                txtApellido.Text = ChecaNullStr(fila("apellido")).Trim()
                txtSucursal.Text = ChecaNullStr(fila("sucursal")).Trim()
                txtPuesto.Text = ChecaNullStr(fila("puesto")).Trim()
            End If
        Catch ex As Exception
            MessageBox.Show("Error al consultar el usuario: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Evalua()
    End Sub

    Private Sub cmbModulos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbModulos.SelectedIndexChanged
        If cmbModulos.SelectedItem Is Nothing Then Return
        Try
            txtModulo.Text = If(CatalogoDescripcion(TipoCatalogo.Modulo, cmbModulos.Text.Trim()), "")

            ' Las pantallas dependen del módulo: se recargan y la selección anterior ya no es válida.
            cmbPantallas.Items.Clear()
            cmbPantallas.SelectedIndex = -1
            txtPantalla.Clear()
            For Each p As String In PantallasDeModulo(cmbModulos.Text.Trim())
                cmbPantallas.Items.Add(p)
            Next
        Catch ex As Exception
            MessageBox.Show("Error al consultar el módulo: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Evalua()
    End Sub

    Private Sub cmbPantallas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPantallas.SelectedIndexChanged
        If cmbPantallas.SelectedItem Is Nothing Then Return
        Try
            txtPantalla.Text = If(CatalogoDescripcion(TipoCatalogo.Pantalla, cmbPantallas.Text.Trim()), "")
        Catch ex As Exception
            MessageBox.Show("Error al consultar la pantalla: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Evalua()
    End Sub

    ''' <summary>
    ''' Con usuario, módulo y pantalla elegidos revisa si el privilegio ya existe:
    ''' el Alta solo procede si NO existe; Baja y Cambio solo si existe.
    ''' </summary>
    Private Sub Evalua()
        btnAceptar.Enabled = False
        If cmbUsuarios.SelectedItem Is Nothing OrElse cmbModulos.SelectedItem Is Nothing OrElse cmbPantallas.SelectedItem Is Nothing Then Return

        Try
            Dim aplica As Integer? = PrivilegioObtener(cmbUsuarios.Text.Trim(), cmbModulos.Text.Trim(), cmbPantallas.Text.Trim())

            If _modo = ModoCrud.Alta Then
                If aplica.HasValue Then
                    MessageBox.Show("El usuario ya tiene asignado este privilegio... corrija por favor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                btnAceptar.Enabled = True
            Else
                If Not aplica.HasValue Then
                    MessageBox.Show("El usuario no tiene asignado este privilegio... corrija por favor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    cmbAplica.SelectedIndex = -1
                    Return
                End If
                ' Muestra el valor actual (el VB6 lo dejaba en blanco).
                cmbAplica.SelectedIndex = If(aplica.Value <> 0, 0, 1)
                btnAceptar.Enabled = (_modo <> ModoCrud.Consulta)
            End If
        Catch ex As Exception
            MessageBox.Show("Error al consultar el privilegio: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnAceptar_Click(sender As Object, e As EventArgs) Handles btnAceptar.Click
        Dim usuario As String = cmbUsuarios.Text.Trim()
        Dim modulo As String = cmbModulos.Text.Trim()
        Dim pantalla As String = cmbPantallas.Text.Trim()

        If (_modo = ModoCrud.Alta OrElse _modo = ModoCrud.Cambio) AndAlso cmbAplica.SelectedItem Is Nothing Then
            MessageBox.Show("Es necesario indicar si aplica seguridad (Sí / No)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim aplica As Integer = If(cmbAplica.SelectedIndex = 0, 1, 0)

        Try
            Select Case _modo
                Case ModoCrud.Alta
                    If MessageBox.Show("¿Está seguro de ingresar estos privilegios?", "Confirmar",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
                    PrivilegioInsertar(usuario, modulo, pantalla, aplica)
                    MessageBox.Show("Privilegio asignado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Case ModoCrud.Baja
                    If MessageBox.Show("¿Está seguro de eliminar estos privilegios?", "Confirmar",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
                    PrivilegioEliminar(usuario, modulo, pantalla)
                    MessageBox.Show("Privilegio eliminado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Case ModoCrud.Cambio
                    If MessageBox.Show("¿Está seguro de actualizar estos privilegios?", "Confirmar",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
                    PrivilegioActualizar(usuario, modulo, pantalla, aplica)
                    MessageBox.Show("Privilegio actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

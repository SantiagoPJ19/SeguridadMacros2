''' <summary>
''' Alta / Baja / Cambio / Consulta de Módulos y de Pantallas
''' (VB6: frmAltasModulo, frmBajasModulo, frmCambiosModulo y sus equivalentes de Pantallas).
''' </summary>
Public Class frmCatalogo
    Private ReadOnly _tipo As TipoCatalogo
    Private ReadOnly _modo As ModoCrud
    Private ReadOnly _nombre As String      ' "módulo" / "pantalla"
    Private ReadOnly _longitud As Integer   ' longitud fija de la clave (5 / 6)

    Public Sub New(tipo As TipoCatalogo, modo As ModoCrud)
        InitializeComponent()
        _tipo = tipo
        _modo = modo
        If tipo = TipoCatalogo.Modulo Then
            _nombre = "módulo"
            _longitud = 5
        Else
            _nombre = "pantalla"
            _longitud = 6
        End If
    End Sub

    Private ReadOnly Property NombreCap As String
        Get
            Return Char.ToUpper(_nombre(0)) & _nombre.Substring(1)
        End Get
    End Property

    ' Artículo y terminaciones en femenino/masculino para los mensajes ("el módulo" / "la pantalla").
    Private ReadOnly Property Fem As Boolean
        Get
            Return _tipo = TipoCatalogo.Pantalla
        End Get
    End Property

    Private Function Art() As String
        Return If(Fem, "la", "el")
    End Function

    Private Function Part(masc As String, fem As String) As String
        Return If(Me.Fem, fem, masc)
    End Function

    Private Sub frmCatalogo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AplicaBordeEmpresa(Me)
        lblClave.Text = NombreCap
        Dim sufijo As String = If(Fem, "Pantallas", "Módulos")

        Select Case _modo
            Case ModoCrud.Alta
                Text = "Alta de " & sufijo
                cmbClave.Visible = False
                txtClave.Visible = True
                txtClave.MaxLength = _longitud
            Case ModoCrud.Baja
                Text = "Baja de " & sufijo
                btnAceptar.Text = "Eliminar"
                txtDescripcion.ReadOnly = True
            Case ModoCrud.Cambio
                Text = "Cambio de " & sufijo
            Case Else
                Text = "Consulta de " & sufijo
                txtDescripcion.ReadOnly = True
                btnAceptar.Visible = False
        End Select

        If _modo <> ModoCrud.Alta Then
            Try
                For Each clave As String In CatalogoClaves(_tipo)
                    cmbClave.Items.Add(clave)
                Next
            Catch ex As Exception
                MessageBox.Show("Error al cargar el catálogo: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub cmbClave_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbClave.SelectedIndexChanged
        If cmbClave.SelectedItem Is Nothing Then Return
        Try
            txtDescripcion.Text = If(CatalogoDescripcion(_tipo, cmbClave.Text.Trim()), "")
        Catch ex As Exception
            MessageBox.Show("Error al consultar: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>Valida el alta; informa todos los problemas de una vez.</summary>
    Private Function ValidaAlta() As Boolean
        Dim clave As String = txtClave.Text.Trim()
        Dim errores As New List(Of String)
        If clave.Length = 0 Then
            errores.Add("Es necesario ingresar " & Art() & " " & _nombre)
        ElseIf clave.Length <> _longitud Then
            errores.Add("La longitud " & If(Fem, "de la ", "del ") & _nombre & " debe ser de " & _longitud & " caracteres")
        End If
        If txtDescripcion.Text.Trim().Length = 0 Then errores.Add("Es necesario ingresar la descripción")

        If errores.Count > 0 Then
            MessageBox.Show(String.Join(Environment.NewLine, errores), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If CatalogoExiste(_tipo, clave) Then
            MessageBox.Show(NombreCap & Part(" ya ha sido dado", " ya ha sido dada") & " de alta... corrija por favor", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function

    Private Sub btnAceptar_Click(sender As Object, e As EventArgs) Handles btnAceptar.Click
        Try
            Select Case _modo
                Case ModoCrud.Alta
                    If Not ValidaAlta() Then Return
                    CatalogoInsertar(_tipo, txtClave.Text.Trim(), txtDescripcion.Text.Trim())
                    MessageBox.Show(NombreCap & Part(" agregado", " agregada") & " exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Case ModoCrud.Baja
                    If cmbClave.SelectedItem Is Nothing Then
                        MessageBox.Show("Seleccione " & Art() & " " & _nombre, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                    Dim clave As String = cmbClave.Text.Trim()
                    If CatalogoTienePrivilegios(_tipo, clave) Then
                        MessageBox.Show("No es posible eliminar " & Part("este módulo", "esta pantalla") &
                                        " ya que hay usuarios con privilegios sobre " & Part("el mismo", "la misma"),
                                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                    If MessageBox.Show("¿Está seguro de eliminar " & Part("este módulo", "esta pantalla") & "?", "Confirmar",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
                    CatalogoEliminar(_tipo, clave)
                    MessageBox.Show(NombreCap & Part(" eliminado", " eliminada") & " correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Case ModoCrud.Cambio
                    If cmbClave.SelectedItem Is Nothing Then
                        MessageBox.Show("Seleccione " & Art() & " " & _nombre, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                    If txtDescripcion.Text.Trim().Length = 0 Then
                        MessageBox.Show("Es necesario ingresar la descripción", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                    If MessageBox.Show("¿Está seguro de actualizar " & Part("este módulo", "esta pantalla") & "?", "Confirmar",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
                    CatalogoActualizar(_tipo, cmbClave.Text.Trim(), txtDescripcion.Text.Trim())
                    MessageBox.Show(NombreCap & Part(" actualizado", " actualizada") & " correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

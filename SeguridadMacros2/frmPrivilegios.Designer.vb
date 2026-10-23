<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPrivilegios
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblUsuario = New System.Windows.Forms.Label()
        Me.cmbUsuarios = New System.Windows.Forms.ComboBox()
        Me.lblNombre = New System.Windows.Forms.Label()
        Me.txtNombre = New System.Windows.Forms.TextBox()
        Me.lblApellido = New System.Windows.Forms.Label()
        Me.txtApellido = New System.Windows.Forms.TextBox()
        Me.lblSucursal = New System.Windows.Forms.Label()
        Me.txtSucursal = New System.Windows.Forms.TextBox()
        Me.lblPuesto = New System.Windows.Forms.Label()
        Me.txtPuesto = New System.Windows.Forms.TextBox()
        Me.lblModulo = New System.Windows.Forms.Label()
        Me.cmbModulos = New System.Windows.Forms.ComboBox()
        Me.txtModulo = New System.Windows.Forms.TextBox()
        Me.lblPantalla = New System.Windows.Forms.Label()
        Me.cmbPantallas = New System.Windows.Forms.ComboBox()
        Me.txtPantalla = New System.Windows.Forms.TextBox()
        Me.lblAplica = New System.Windows.Forms.Label()
        Me.cmbAplica = New System.Windows.Forms.ComboBox()
        Me.btnAceptar = New System.Windows.Forms.Button()
        Me.btnSalir = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblUsuario
        '
        Me.lblUsuario.AutoSize = True
        Me.lblUsuario.Location = New System.Drawing.Point(20, 24)
        Me.lblUsuario.Name = "lblUsuario"
        Me.lblUsuario.Text = "Usuario"
        '
        'cmbUsuarios
        '
        Me.cmbUsuarios.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cmbUsuarios.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbUsuarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown
        Me.cmbUsuarios.FormattingEnabled = True
        Me.cmbUsuarios.Location = New System.Drawing.Point(140, 20)
        Me.cmbUsuarios.Name = "cmbUsuarios"
        Me.cmbUsuarios.Size = New System.Drawing.Size(200, 21)
        Me.cmbUsuarios.TabIndex = 0
        '
        'lblNombre
        '
        Me.lblNombre.AutoSize = True
        Me.lblNombre.Location = New System.Drawing.Point(20, 60)
        Me.lblNombre.Name = "lblNombre"
        Me.lblNombre.Text = "Nombre"
        '
        'txtNombre
        '
        Me.txtNombre.Location = New System.Drawing.Point(140, 56)
        Me.txtNombre.Name = "txtNombre"
        Me.txtNombre.ReadOnly = True
        Me.txtNombre.Size = New System.Drawing.Size(340, 20)
        Me.txtNombre.TabStop = False
        '
        'lblApellido
        '
        Me.lblApellido.AutoSize = True
        Me.lblApellido.Location = New System.Drawing.Point(20, 92)
        Me.lblApellido.Name = "lblApellido"
        Me.lblApellido.Text = "Apellido"
        '
        'txtApellido
        '
        Me.txtApellido.Location = New System.Drawing.Point(140, 88)
        Me.txtApellido.Name = "txtApellido"
        Me.txtApellido.ReadOnly = True
        Me.txtApellido.Size = New System.Drawing.Size(340, 20)
        Me.txtApellido.TabStop = False
        '
        'lblSucursal
        '
        Me.lblSucursal.AutoSize = True
        Me.lblSucursal.Location = New System.Drawing.Point(20, 124)
        Me.lblSucursal.Name = "lblSucursal"
        Me.lblSucursal.Text = "Sucursal"
        '
        'txtSucursal
        '
        Me.txtSucursal.Location = New System.Drawing.Point(140, 120)
        Me.txtSucursal.Name = "txtSucursal"
        Me.txtSucursal.ReadOnly = True
        Me.txtSucursal.Size = New System.Drawing.Size(120, 20)
        Me.txtSucursal.TabStop = False
        '
        'lblPuesto
        '
        Me.lblPuesto.AutoSize = True
        Me.lblPuesto.Location = New System.Drawing.Point(20, 156)
        Me.lblPuesto.Name = "lblPuesto"
        Me.lblPuesto.Text = "Puesto"
        '
        'txtPuesto
        '
        Me.txtPuesto.Location = New System.Drawing.Point(140, 152)
        Me.txtPuesto.Name = "txtPuesto"
        Me.txtPuesto.ReadOnly = True
        Me.txtPuesto.Size = New System.Drawing.Size(340, 20)
        Me.txtPuesto.TabStop = False
        '
        'lblModulo
        '
        Me.lblModulo.AutoSize = True
        Me.lblModulo.Location = New System.Drawing.Point(20, 204)
        Me.lblModulo.Name = "lblModulo"
        Me.lblModulo.Text = "Módulo"
        '
        'cmbModulos
        '
        Me.cmbModulos.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cmbModulos.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbModulos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown
        Me.cmbModulos.FormattingEnabled = True
        Me.cmbModulos.Location = New System.Drawing.Point(140, 200)
        Me.cmbModulos.Name = "cmbModulos"
        Me.cmbModulos.Size = New System.Drawing.Size(120, 21)
        Me.cmbModulos.TabIndex = 1
        '
        'txtModulo
        '
        Me.txtModulo.Location = New System.Drawing.Point(270, 200)
        Me.txtModulo.Name = "txtModulo"
        Me.txtModulo.ReadOnly = True
        Me.txtModulo.Size = New System.Drawing.Size(210, 20)
        Me.txtModulo.TabStop = False
        '
        'lblPantalla
        '
        Me.lblPantalla.AutoSize = True
        Me.lblPantalla.Location = New System.Drawing.Point(20, 240)
        Me.lblPantalla.Name = "lblPantalla"
        Me.lblPantalla.Text = "Pantalla"
        '
        'cmbPantallas
        '
        Me.cmbPantallas.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cmbPantallas.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbPantallas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown
        Me.cmbPantallas.FormattingEnabled = True
        Me.cmbPantallas.Location = New System.Drawing.Point(140, 236)
        Me.cmbPantallas.Name = "cmbPantallas"
        Me.cmbPantallas.Size = New System.Drawing.Size(120, 21)
        Me.cmbPantallas.TabIndex = 2
        '
        'txtPantalla
        '
        Me.txtPantalla.Location = New System.Drawing.Point(270, 236)
        Me.txtPantalla.Name = "txtPantalla"
        Me.txtPantalla.ReadOnly = True
        Me.txtPantalla.Size = New System.Drawing.Size(210, 20)
        Me.txtPantalla.TabStop = False
        '
        'lblAplica
        '
        Me.lblAplica.AutoSize = True
        Me.lblAplica.Location = New System.Drawing.Point(20, 276)
        Me.lblAplica.Name = "lblAplica"
        Me.lblAplica.Text = "Aplica Seguridad"
        '
        'cmbAplica
        '
        Me.cmbAplica.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cmbAplica.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbAplica.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown
        Me.cmbAplica.FormattingEnabled = True
        Me.cmbAplica.Items.AddRange(New Object() {"Sí", "No"})
        Me.cmbAplica.Location = New System.Drawing.Point(140, 272)
        Me.cmbAplica.Name = "cmbAplica"
        Me.cmbAplica.Size = New System.Drawing.Size(120, 21)
        Me.cmbAplica.TabIndex = 3
        '
        'btnAceptar
        '
        Me.btnAceptar.Enabled = False
        Me.btnAceptar.Location = New System.Drawing.Point(290, 325)
        Me.btnAceptar.Name = "btnAceptar"
        Me.btnAceptar.Size = New System.Drawing.Size(90, 30)
        Me.btnAceptar.TabIndex = 4
        Me.btnAceptar.Text = "Aceptar"
        Me.btnAceptar.UseVisualStyleBackColor = True
        '
        'btnSalir
        '
        Me.btnSalir.Location = New System.Drawing.Point(390, 325)
        Me.btnSalir.Name = "btnSalir"
        Me.btnSalir.Size = New System.Drawing.Size(90, 30)
        Me.btnSalir.TabIndex = 5
        Me.btnSalir.Text = "Cancelar"
        Me.btnSalir.UseVisualStyleBackColor = True
        '
        'frmPrivilegios
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(510, 375)
        Me.Controls.Add(Me.lblUsuario)
        Me.Controls.Add(Me.cmbUsuarios)
        Me.Controls.Add(Me.lblNombre)
        Me.Controls.Add(Me.txtNombre)
        Me.Controls.Add(Me.lblApellido)
        Me.Controls.Add(Me.txtApellido)
        Me.Controls.Add(Me.lblSucursal)
        Me.Controls.Add(Me.txtSucursal)
        Me.Controls.Add(Me.lblPuesto)
        Me.Controls.Add(Me.txtPuesto)
        Me.Controls.Add(Me.lblModulo)
        Me.Controls.Add(Me.cmbModulos)
        Me.Controls.Add(Me.txtModulo)
        Me.Controls.Add(Me.lblPantalla)
        Me.Controls.Add(Me.cmbPantallas)
        Me.Controls.Add(Me.txtPantalla)
        Me.Controls.Add(Me.lblAplica)
        Me.Controls.Add(Me.cmbAplica)
        Me.Controls.Add(Me.btnAceptar)
        Me.Controls.Add(Me.btnSalir)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPrivilegios"
        Me.Text = "Privilegios"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblUsuario As Label
    Friend WithEvents cmbUsuarios As ComboBox
    Friend WithEvents lblNombre As Label
    Friend WithEvents txtNombre As TextBox
    Friend WithEvents lblApellido As Label
    Friend WithEvents txtApellido As TextBox
    Friend WithEvents lblSucursal As Label
    Friend WithEvents txtSucursal As TextBox
    Friend WithEvents lblPuesto As Label
    Friend WithEvents txtPuesto As TextBox
    Friend WithEvents lblModulo As Label
    Friend WithEvents cmbModulos As ComboBox
    Friend WithEvents txtModulo As TextBox
    Friend WithEvents lblPantalla As Label
    Friend WithEvents cmbPantallas As ComboBox
    Friend WithEvents txtPantalla As TextBox
    Friend WithEvents lblAplica As Label
    Friend WithEvents cmbAplica As ComboBox
    Friend WithEvents btnAceptar As Button
    Friend WithEvents btnSalir As Button
End Class

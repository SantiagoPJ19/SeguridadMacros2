<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCatalogo
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
        Me.lblClave = New System.Windows.Forms.Label()
        Me.cmbClave = New System.Windows.Forms.ComboBox()
        Me.txtClave = New System.Windows.Forms.TextBox()
        Me.lblDescripcion = New System.Windows.Forms.Label()
        Me.txtDescripcion = New System.Windows.Forms.TextBox()
        Me.btnAceptar = New System.Windows.Forms.Button()
        Me.btnSalir = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblClave
        '
        Me.lblClave.AutoSize = True
        Me.lblClave.Location = New System.Drawing.Point(20, 24)
        Me.lblClave.Name = "lblClave"
        Me.lblClave.Text = "Clave"
        '
        'cmbClave
        '
        Me.cmbClave.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cmbClave.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbClave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown
        Me.cmbClave.FormattingEnabled = True
        Me.cmbClave.Location = New System.Drawing.Point(120, 20)
        Me.cmbClave.Name = "cmbClave"
        Me.cmbClave.Size = New System.Drawing.Size(150, 21)
        Me.cmbClave.TabIndex = 0
        '
        'txtClave
        '
        Me.txtClave.Location = New System.Drawing.Point(120, 20)
        Me.txtClave.Name = "txtClave"
        Me.txtClave.Size = New System.Drawing.Size(150, 20)
        Me.txtClave.TabIndex = 1
        Me.txtClave.Visible = False
        '
        'lblDescripcion
        '
        Me.lblDescripcion.AutoSize = True
        Me.lblDescripcion.Location = New System.Drawing.Point(20, 64)
        Me.lblDescripcion.Name = "lblDescripcion"
        Me.lblDescripcion.Text = "Descripción"
        '
        'txtDescripcion
        '
        Me.txtDescripcion.Location = New System.Drawing.Point(120, 60)
        Me.txtDescripcion.Name = "txtDescripcion"
        Me.txtDescripcion.Size = New System.Drawing.Size(320, 20)
        Me.txtDescripcion.TabIndex = 2
        '
        'btnAceptar
        '
        Me.btnAceptar.Location = New System.Drawing.Point(250, 110)
        Me.btnAceptar.Name = "btnAceptar"
        Me.btnAceptar.Size = New System.Drawing.Size(90, 30)
        Me.btnAceptar.TabIndex = 3
        Me.btnAceptar.Text = "Aceptar"
        Me.btnAceptar.UseVisualStyleBackColor = True
        '
        'btnSalir
        '
        Me.btnSalir.Location = New System.Drawing.Point(350, 110)
        Me.btnSalir.Name = "btnSalir"
        Me.btnSalir.Size = New System.Drawing.Size(90, 30)
        Me.btnSalir.TabIndex = 4
        Me.btnSalir.Text = "Salir"
        Me.btnSalir.UseVisualStyleBackColor = True
        '
        'frmCatalogo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(470, 160)
        Me.Controls.Add(Me.lblClave)
        Me.Controls.Add(Me.cmbClave)
        Me.Controls.Add(Me.txtClave)
        Me.Controls.Add(Me.lblDescripcion)
        Me.Controls.Add(Me.txtDescripcion)
        Me.Controls.Add(Me.btnAceptar)
        Me.Controls.Add(Me.btnSalir)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCatalogo"
        Me.Text = "Catálogo"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblClave As Label
    Friend WithEvents cmbClave As ComboBox
    Friend WithEvents txtClave As TextBox
    Friend WithEvents lblDescripcion As Label
    Friend WithEvents txtDescripcion As TextBox
    Friend WithEvents btnAceptar As Button
    Friend WithEvents btnSalir As Button
End Class

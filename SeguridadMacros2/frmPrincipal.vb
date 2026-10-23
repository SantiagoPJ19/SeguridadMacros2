Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar
Imports System
Imports System.Collections.Specialized.BitVector32
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Text
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Forms.LinkLabel
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Window
Imports AccpacCOMAPI
Imports AccpacDataSrc
Imports AccpacSessionManager
Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices
Imports ClosedXML.Excel
Imports System.IO

Public Class frmPrincipal
    Public mSession As AccpacSession
    Public sUsuario As String
    Public sEmpresa As String
    Public sEmpresaNombre As String

    Private toolTip1 As ToolTip
    ' Color de la compañía; los formularios hijos lo usan para dibujar su propio borde (ver Bordes.vb).
    Public ColorEmpresa As Color = Color.Empty

    ' Borde del formulario con el color de la compañía. En un contenedor MDI el área de trabajo (MdiClient)
    ' cubre todo el cliente y tapa lo que se dibuje en Paint, por eso el borde se arma con 4 paneles
    ' acoplados a los lados, que el MdiClient sí respeta.
    Private Sub AplicaBorde(color As Color)
        For Each lado As DockStyle In {DockStyle.Left, DockStyle.Right, DockStyle.Bottom, DockStyle.Top}
            Dim p As New Panel()
            p.Name = "borde" & lado.ToString()
            p.Dock = lado
            p.BackColor = color
            If lado = DockStyle.Left OrElse lado = DockStyle.Right Then p.Width = AnchoBorde Else p.Height = AnchoBorde
            Me.Controls.Add(p)
        Next
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'SESIÓN SAGE 300: inicia sesión con UserAcc / PassAcc / CompanyAcc de Config.ini. Si no se puede abrir
        '(credenciales faltantes o incorrectas, versión incorrecta...) no hay con qué operar
        Dim usuarioAcc As String = Nothing
        Dim claveAcc As String = Nothing
        Dim empresaAcc As String = Nothing
        If Not ConectaDB.LeerCredencialesSage(usuarioAcc, claveAcc, empresaAcc) Then
            CerrarPorError("No se pudieron leer las credenciales de Sage 300 de Config.ini.", "Error de configuración")
            Return
        End If

        Try
            mSession = New AccpacSession()
            mSession.Init("", "AS", "AS1000", "73A") ' ObjectHandle, AppID, ProgramName, AppVersion
            mSession.Open(usuarioAcc, claveAcc, empresaAcc, Date.Today, 0, "")
            claveAcc = Nothing

            If Not mSession.IsOpened Then
                CerrarPorError("No se pudo abrir la sesión de Sage 300. Verifique UserAcc, PassAcc y CompanyAcc en Config.ini.", "Error de sesión")
                Return
            End If

            sUsuario = mSession.UserID
            sEmpresa = mSession.CompanyID
            sEmpresaNombre = mSession.CompanyName

            'COMPANY COLOR
            Dim companyColorCode As Integer = mSession.GetCompanyColorCode(sEmpresa)
            Debug.Print("Company Color Code " & companyColorCode)
            Dim netColor As Color = ColorTranslator.FromOle(CInt(companyColorCode))
            ColorEmpresa = netColor
            AplicaBorde(netColor)
            'COMPANY COLOR
        Catch ex As Exception
            CerrarPorError("No fue posible iniciar la sesión de Sage 300:" & Environment.NewLine & DetalleErroresSage(ex), "Error de sesión")
            Return
        End Try

        'RESIZE
        'Me.Height = 711
        'Me.Width = 1021
        'RESIZE

        'CONEXIONES SQL: DB SAGE, USUARIOS y DESARROLLOS
        ' Cada ConectaXxx ya muestra el detalle del error; si cualquiera falla (se detiene en la
        ' primera que falle) la aplicación no puede operar, así que se cierra.
        If Not (ConectaDB.ConectaSage(sEmpresa) AndAlso ConectaDB.ConectaUsuarios() AndAlso ConectaDB.ConectaDesarrollo()) Then
            CerrarPorError("No fue posible conectarse a la base de datos.", "Error de conexión")
            Return
        End If

        Me.Text = sEmpresa & " - Seguridad de Macros ver.2.0"

        ' Crear e inicializar el ToolTip
        toolTip1 = New ToolTip()

        ' Configuración opcional del ToolTip
        toolTip1.AutoPopDelay = 5000   ' Tiempo que permanece visible (ms)
        toolTip1.InitialDelay = 500    ' Retraso antes de aparecer (ms)
        toolTip1.ReshowDelay = 200     ' Retraso entre apariciones (ms)
        toolTip1.ShowAlways = True     ' Mostrar incluso si el formulario no está activo

        ' Asignar texto de ToolTip a controles
        ''toolTip1.SetToolTip(chkIN, "Facturas")
        ''toolTip1.SetToolTip(chkNC, "Nota de Crédito")
        ''toolTip1.SetToolTip(chkND, "Nota de Débito")
        ''toolTip1.SetToolTip(chkINT, "Interés")
        ''toolTip1.SetToolTip(chkPP, "Anticipo")
        ''toolTip1.SetToolTip(chkPY, "Pago")

    End Sub

    ''' <summary>Avisa del error, libera las conexiones abiertas y cierra la aplicación.</summary>
    Private Sub CerrarPorError(mensaje As String, titulo As String)
        MessageBox.Show(mensaje & Environment.NewLine & Environment.NewLine & "La aplicación se cerrará.",
                        titulo, MessageBoxButtons.OK, MessageBoxIcon.Error)
        ConectaDB.CerrarConexion()
        Close()
    End Sub

    ''' <summary>Mensaje de la excepción más los errores que Sage 300 haya dejado en la sesión.</summary>
    Private Function DetalleErroresSage(ex As Exception) As String
        Dim detalle As New System.Text.StringBuilder(ex.Message)
        Try
            If mSession IsNot Nothing AndAlso mSession.Errors IsNot Nothing Then
                For i As Integer = 0 To mSession.Errors.Count - 1
                    detalle.AppendLine().Append(mSession.Errors.Item(i))
                Next
                mSession.Errors.Clear()
            End If
        Catch
            ' si la sesión está dañada nos quedamos con el mensaje de la excepción
        End Try
        Return detalle.ToString()
    End Function

    ' La sesión de Sage la abre esta aplicación (Open), así que también debe cerrarla para liberar la licencia.
    Private Sub frmPrincipal_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Try
            If mSession IsNot Nothing Then
                If mSession.IsOpened Then mSession.Close()
                System.Runtime.InteropServices.Marshal.ReleaseComObject(mSession)
                mSession = Nothing
            End If
        Catch ex As Exception
            Debug.WriteLine("Error al cerrar la sesión de Sage: " & ex.Message)
        End Try
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        'GroupBox1.Width = Me.Width - 32
        'GroupBox1.Height = Me.Height - 79
    End Sub

    Private Sub AbrirUsuarios(modo As ModoCrud)
        Dim f As New frmUsuarios(modo)
        f.MdiParent = Me
        f.Show()
    End Sub

    Private Sub AltaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AltaToolStripMenuItem.Click
        AbrirUsuarios(ModoCrud.Alta)
    End Sub

    Private Sub BajaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BajaToolStripMenuItem.Click
        AbrirUsuarios(ModoCrud.Baja)
    End Sub

    Private Sub CambioToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CambioToolStripMenuItem.Click
        AbrirUsuarios(ModoCrud.Cambio)
    End Sub

    Private Sub ConsultaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConsultaToolStripMenuItem.Click
        AbrirUsuarios(ModoCrud.Consulta)
    End Sub

    Private Sub AbrirCatalogo(tipo As TipoCatalogo, modo As ModoCrud)
        Dim f As New frmCatalogo(tipo, modo)
        f.MdiParent = Me
        f.Show()
    End Sub

    ' --- Módulos ---
    Private Sub AltaToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles AltaToolStripMenuItem1.Click
        AbrirCatalogo(TipoCatalogo.Modulo, ModoCrud.Alta)
    End Sub

    Private Sub BajaToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles BajaToolStripMenuItem1.Click
        AbrirCatalogo(TipoCatalogo.Modulo, ModoCrud.Baja)
    End Sub

    Private Sub CambioToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles CambioToolStripMenuItem1.Click
        AbrirCatalogo(TipoCatalogo.Modulo, ModoCrud.Cambio)
    End Sub

    Private Sub ConsultaToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ConsultaToolStripMenuItem1.Click
        AbrirCatalogo(TipoCatalogo.Modulo, ModoCrud.Consulta)
    End Sub

    ' --- Pantallas ---
    Private Sub AltaToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles AltaToolStripMenuItem2.Click
        AbrirCatalogo(TipoCatalogo.Pantalla, ModoCrud.Alta)
    End Sub

    Private Sub BajaToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles BajaToolStripMenuItem2.Click
        AbrirCatalogo(TipoCatalogo.Pantalla, ModoCrud.Baja)
    End Sub

    Private Sub CambioToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles CambioToolStripMenuItem2.Click
        AbrirCatalogo(TipoCatalogo.Pantalla, ModoCrud.Cambio)
    End Sub

    Private Sub ConsultaToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ConsultaToolStripMenuItem2.Click
        AbrirCatalogo(TipoCatalogo.Pantalla, ModoCrud.Consulta)
    End Sub

    ' --- Privilegios ---
    Private Sub AbrirPrivilegios(modo As ModoCrud)
        Dim f As New frmPrivilegios(modo)
        f.MdiParent = Me
        f.Show()
    End Sub

    Private Sub AltaToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles AltaToolStripMenuItem3.Click
        AbrirPrivilegios(ModoCrud.Alta)
    End Sub

    Private Sub BajaToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles BajaToolStripMenuItem3.Click
        AbrirPrivilegios(ModoCrud.Baja)
    End Sub

    Private Sub CambioToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles CambioToolStripMenuItem3.Click
        AbrirPrivilegios(ModoCrud.Cambio)
    End Sub

    Private Sub ConsultaToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ConsultaToolStripMenuItem3.Click
        AbrirPrivilegios(ModoCrud.Consulta)
    End Sub

    Private Sub CerrarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CerrarToolStripMenuItem.Click
        ConectaDB.CerrarConexion()
        Close()
    End Sub

End Class

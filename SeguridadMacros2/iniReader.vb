Imports System.Collections.Specialized.BitVector32
Imports System.ComponentModel ' Necesario para usar las clases Session, DBLink y View
Imports System.Data ' Necesario para ConnectionState
Imports System.Data.SqlClient
Imports System.Runtime.InteropServices ' Necesario para DllImport
Imports System.Windows.Forms.LinkLabel
Imports System.Text
Imports System.IO
Imports System.Security.Cryptography

Public Class VariablesGlobales
    Public Shared intUsuario As Integer
    Public Shared intModulo As Integer
    Public Shared intPantalla As Integer
    Public Shared intPrivilegio As Integer
End Class
Public Class IniReader
    ' Declara la función GetPrivateProfileString para leer valores de tipo String
    <DllImport("kernel32.dll", EntryPoint:="GetPrivateProfileString")>
    Private Shared Function GetIniString(
        ByVal lpAppName As String,
        ByVal lpKeyName As String,
        ByVal lpDefault As String,
        ByVal lpReturnedString As System.Text.StringBuilder,
        ByVal nSize As Integer,
        ByVal lpFileName As String) As Integer
    End Function

    ' Declaración de la función para leer valores de tipo Integer
    <DllImport("kernel32.dll", EntryPoint:="GetPrivateProfileInt")>
    Private Shared Function GetIniInt(
        ByVal lpAppName As String,
        ByVal lpKeyName As String,
        ByVal nDefault As Integer,
        ByVal lpFileName As String) As Integer
    End Function

    ' ------------------------------------------------------------------
    ' B. Métodos de Lectura (Wrappers)
    ' ------------------------------------------------------------------

    ' Función pública para obtener un valor STRING
    Public Shared Function LeerValor(ByVal ArchivoINI As String, ByVal Seccion As String, ByVal Clave As String, Optional ByVal ValorPorDefecto As String = "") As String
        Const BUFFER_SIZE As Integer = 255 ' Tamaño del buffer para la respuesta

        ' Crea un StringBuilder para almacenar el resultado
        Dim temp As New System.Text.StringBuilder(BUFFER_SIZE)

        ' Llama a la función API
        GetIniString(Seccion, Clave, ValorPorDefecto, temp, BUFFER_SIZE, ArchivoINI)

        Return temp.ToString()
    End Function

    ' Función pública para obtener un valor INTEGER
    Public Shared Function LeerNumero(ByVal ArchivoINI As String, ByVal Seccion As String, ByVal Clave As String, Optional ByVal ValorPorDefecto As Integer = 0) As Integer

        ' Llama a la función API
        Return GetIniInt(Seccion, Clave, ValorPorDefecto, ArchivoINI)
    End Function

End Class

Public Class ConectaDB
    ' La variable de conexión ahora es del tipo SqlConnection (el equivalente moderno de ADO.NET)
    ' En .NET, es preferible gestionar la conexión dentro de un "Using" para asegurar el cierre,
    ' pero se mantiene Pública para reflejar el diseño original.
    Public Shared cnDB As SqlConnection
    Public Shared cnDBU As SqlConnection
    Public Shared cnDBD As SqlConnection
    Public Shared sConnectionString As String
    Public Shared sConnectionStringU As String
    Public Shared sConnectionStringD As String
    ''' <summary>
    ''' Intenta establecer una conexión con la base de datos SQL Server.
    ''' </summary>
    ''' <param name="sDatabase">Nombre de la base de datos a la que conectarse.</param>
    ''' <returns>True si la conexión fue exitosa, False en caso contrario.</returns>
    Public Shared Function ConectaSage(ByVal sDatabase As String) As Boolean

        ' Variables (manteniendo los nombres originales para la equivalencia)
        Dim m_Name As String
        Dim m_Clave As String
        Dim m_User As String
        Dim sINIFile As String

        Dim appDir As String = My.Application.Info.DirectoryPath
        'Dim exePath As String = Application.ExecutablePath

        sINIFile = appDir & "\" & "Config.ini"
        Debug.Print("Archivo: " + sINIFile)
        'BaseDatos = IniReader.LeerValor(rutaIni, "settings", "Database", "?")
        'Debug.Print("Base de Datos " + BaseDatos)
        'If BaseDatos = "?" Then
        '    MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - B")
        '    Exit Function
        'End If
        m_Name = IniManager.LeerValor("settings", "Server", sINIFile, "?") ' IniReader.LeerValor(sINIFile, "settings", "Server", "?")
        Debug.Print("Servidor " + m_Name)
        If m_Name = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - A")
            ConectaSage = False
            Exit Function
        End If
        m_User = IniManager.LeerValor("settings", "User", sINIFile, "?") 'IniReader.LeerValor(sINIFile, "settings", "User", "?")
        Debug.Print("Usuario " + m_User)
        If m_User = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - B")
            ConectaSage = False
            Exit Function
        End If

        m_Clave = IniManager.LeerValor("settings", "Pass", sINIFile, "?") 'IniReader.LeerValor(sINIFile, "settings", "Pass", "?")
        If m_Clave = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - C")
            ConectaSage = False
            Exit Function
        End If
        m_Clave = CryptoModule.Desencriptar(m_Clave)

        ' ----------------------------------------------------
        ' 2. Creación de la Cadena de Conexión (SqlClient)
        ' Se usa interpolación de cadenas ($"...") para mayor legibilidad.
        ' El ConnectionTimeout se establece directamente en la cadena.
        ' ----------------------------------------------------
        sConnectionString = $"Data Source={m_Name};Initial Catalog={sDatabase};User ID={m_User};Password={m_Clave};Connect Timeout=1000;"
        ' Inicializar el objeto de conexión
        cnDB = New SqlConnection(sConnectionString)

        Try
            ' Intenta abrir la conexión. En ADO.NET, esto es sincrónico.
            cnDB.Open()

            ' Si llegamos aquí, la conexión fue exitosa.
            ConectaSage = True

        Catch ex As SqlException
            ' Manejo específico para errores de SQL Server (servidor no encontrado, login fallido, etc.)
            MessageBox.Show($"Error de conexión a SQL Server: {ex.Message}{Environment.NewLine}Servidor: {m_Name} / Base de datos: {sDatabase}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ConectaSage = False

        Catch ex As Exception
            ' Manejo de otros errores inesperados
            MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ConectaSage = False
        End Try
    End Function

    Public Shared Function ConectaUsuarios() As Boolean

        ' Variables (manteniendo los nombres originales para la equivalencia)
        Dim m_Name As String
        Dim m_Clave As String
        Dim m_User As String
        Dim sINIFile As String
        Dim sDatabase As String

        Dim appDir As String = My.Application.Info.DirectoryPath
        'Dim exePath As String = Application.ExecutablePath

        sINIFile = appDir & "\" & "Config.ini"
        Debug.Print("Archivo: " + sINIFile)
        'BaseDatos = IniReader.LeerValor(rutaIni, "settings", "Database", "?")
        'Debug.Print("Base de Datos " + BaseDatos)
        'If BaseDatos = "?" Then
        '    MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - B")
        '    Exit Function
        'End If
        sDatabase = IniManager.LeerValor("settings", "DBUsuarios", sINIFile, "?") ' IniReader.LeerValor(sINIFile, "settings", "Server", "?")
        Debug.Print("Base Usuarios: " + sDatabase)
        If sDatabase = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - A")
            ConectaUsuarios = False
            Exit Function
        End If
        m_Name = IniManager.LeerValor("settings", "Server", sINIFile, "?") ' IniReader.LeerValor(sINIFile, "settings", "Server", "?")
        Debug.Print("Servidor " + m_Name)
        If m_Name = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - A")
            ConectaUsuarios = False
            Exit Function
        End If
        m_User = IniManager.LeerValor("settings", "User", sINIFile, "?") 'IniReader.LeerValor(sINIFile, "settings", "User", "?")
        Debug.Print("Usuario " + m_User)
        If m_User = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - B")
            ConectaUsuarios = False
            Exit Function
        End If

        m_Clave = IniManager.LeerValor("settings", "Pass", sINIFile, "?") 'IniReader.LeerValor(sINIFile, "settings", "Pass", "?")
        If m_Clave = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - C")
            ConectaUsuarios = False
            Exit Function
        End If
        m_Clave = CryptoModule.Desencriptar(m_Clave)

        ' ----------------------------------------------------
        ' 2. Creación de la Cadena de Conexión (SqlClient)
        ' Se usa interpolación de cadenas ($"...") para mayor legibilidad.
        ' El ConnectionTimeout se establece directamente en la cadena.
        ' ----------------------------------------------------
        sConnectionStringU = $"Data Source={m_Name};Initial Catalog={sDatabase};User ID={m_User};Password={m_Clave};Connect Timeout=1000;"
        ' Inicializar el objeto de conexión
        cnDBU = New SqlConnection(sConnectionStringU)

        Try
            ' Intenta abrir la conexión. En ADO.NET, esto es sincrónico.
            cnDBU.Open()

            ' Si llegamos aquí, la conexión fue exitosa.
            ConectaUsuarios = True

        Catch ex As SqlException
            ' Manejo específico para errores de SQL Server (servidor no encontrado, login fallido, etc.)
            MessageBox.Show($"Error de conexión a SQL Server: {ex.Message}{Environment.NewLine}Servidor: {m_Name} / Base de datos: {sDatabase}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ConectaUsuarios = False

        Catch ex As Exception
            ' Manejo de otros errores inesperados
            MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ConectaUsuarios = False
        End Try
    End Function

    Public Shared Function ConectaDesarrollo() As Boolean

        ' Variables (manteniendo los nombres originales para la equivalencia)
        Dim m_Name As String
        Dim m_Clave As String
        Dim m_User As String
        Dim sDatabase As String
        Dim sINIFile As String

        Dim appDir As String = My.Application.Info.DirectoryPath
        'Dim exePath As String = Application.ExecutablePath

        sINIFile = appDir & "\" & "Config.ini"
        Debug.Print("Archivo: " + sINIFile)
        sDatabase = IniManager.LeerValor("settings", "DBDesarrollos", sINIFile, "?") ' IniReader.LeerValor(sINIFile, "settings", "Server", "?")
        Debug.Print("Dase Desarrollos: " + sDatabase)
        If sDatabase = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - A")
            ConectaDesarrollo = False
            Exit Function
        End If

        m_Name = IniManager.LeerValor("settings", "Server", sINIFile, "?") ' IniReader.LeerValor(sINIFile, "settings", "Server", "?")
        Debug.Print("Servidor " + m_Name)
        If m_Name = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - A")
            ConectaDesarrollo = False
            Exit Function
        End If
        m_User = IniManager.LeerValor("settings", "User", sINIFile, "?") 'IniReader.LeerValor(sINIFile, "settings", "User", "?")
        Debug.Print("Usuario " + m_User)
        If m_User = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - B")
            ConectaDesarrollo = False
            Exit Function
        End If

        m_Clave = IniManager.LeerValor("settings", "Pass", sINIFile, "?") 'IniReader.LeerValor(sINIFile, "settings", "Pass", "?")
        If m_Clave = "?" Then
            MsgBox("No existe el archivo INI, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - C")
            ConectaDesarrollo = False
            Exit Function
        End If
        m_Clave = CryptoModule.Desencriptar(m_Clave)

        ' ----------------------------------------------------
        ' 2. Creación de la Cadena de Conexión (SqlClient)
        ' Se usa interpolación de cadenas ($"...") para mayor legibilidad.
        ' El ConnectionTimeout se establece directamente en la cadena.
        ' ----------------------------------------------------
        sConnectionStringD = $"Data Source={m_Name};Initial Catalog={sDatabase};User ID={m_User};Password={m_Clave};Connect Timeout=1000;"
        ' Inicializar el objeto de conexión
        cnDBD = New SqlConnection(sConnectionStringD)

        Try
            ' Intenta abrir la conexión. En ADO.NET, esto es sincrónico.
            cnDBD.Open()

            ' Si llegamos aquí, la conexión fue exitosa.
            ConectaDesarrollo = True

        Catch ex As SqlException
            ' Manejo específico para errores de SQL Server (servidor no encontrado, login fallido, etc.)
            MessageBox.Show($"Error de conexión a SQL Server: {ex.Message}{Environment.NewLine}Servidor: {m_Name} / Base de datos: {sDatabase}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ConectaDesarrollo = False

        Catch ex As Exception
            ' Manejo de otros errores inesperados
            MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ConectaDesarrollo = False
        End Try
    End Function

    ''' <summary>
    ''' Lee de Config.ini los datos para iniciar sesión en Sage 300: UserAcc (usuario), PassAcc (contraseña
    ''' encriptada, igual que Pass). La compañía no va en el .ini: sale de la sesión identificada en Sage.
    ''' Devuelve False si falta alguno.
    ''' </summary>
    Public Shared Function LeerCredencialesSage(ByRef usuario As String, ByRef clave As String) As Boolean
        Dim sINIFile As String = My.Application.Info.DirectoryPath & "\" & "Config.ini"

        usuario = IniManager.LeerValor("settings", "UserAcc", sINIFile, "?")
        If usuario = "?" OrElse usuario.Trim().Length = 0 Then
            MsgBox("Falta el parámetro UserAcc en Config.ini, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - UserAcc")
            Return False
        End If

        Dim claveEncriptada As String = IniManager.LeerValor("settings", "PassAcc", sINIFile, "?")
        If claveEncriptada = "?" OrElse claveEncriptada.Trim().Length = 0 Then
            MsgBox("Falta el parámetro PassAcc en Config.ini, favor de llamar al Administrador del sistema", vbOKOnly + vbCritical, "Archivo INI - PassAcc")
            Return False
        End If

        Try
            clave = CryptoModule.Desencriptar(claveEncriptada)
        Catch ex As Exception
            MsgBox("No se pudo desencriptar PassAcc de Config.ini: " & ex.Message, vbOKOnly + vbCritical, "Archivo INI - PassAcc")
            Return False
        End Try

        usuario = usuario.Trim()
        Return True
    End Function

    ''' <summary>
    ''' Cierra y libera los recursos de la conexión global.
    ''' </summary>
    Public Shared Sub CerrarConexion()
        Try
            If cnDB IsNot Nothing AndAlso cnDB.State = ConnectionState.Open Then
                cnDB.Close()
                ' Dispose libera recursos no administrados, buena práctica en .NET
                cnDB.Dispose()
                cnDB = Nothing
            End If
            If cnDBU IsNot Nothing AndAlso cnDBU.State = ConnectionState.Open Then
                cnDBU.Close()
                ' Dispose libera recursos no administrados, buena práctica en .NET
                cnDBU.Dispose()
                cnDBU = Nothing
            End If
            If cnDBD IsNot Nothing AndAlso cnDBD.State = ConnectionState.Open Then
                cnDBD.Close()
                ' Dispose libera recursos no administrados, buena práctica en .NET
                cnDBD.Dispose()
                cnDBD = Nothing
            End If
        Catch ex As Exception
            ' Manejo de otros errores inesperados
            MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class

''' <summary>
''' Clase para gestionar la lectura y escritura de archivos .ini utilizando la API de Windows (ANSI).
''' </summary>
Public Class IniManager

    ' --- EJEMPLO DE USO ---
    ' Dim server As String = IniManager.LeerValor("CONEXION", "Server", "C:\Config\Setup.ini", "localhost")

    ' Importación de la función GetPrivateProfileString de kernel32.dll
    ' El uso de CharSet.Ansi asegura que se maneje la codificación solicitada.
    <DllImport("kernel32", EntryPoint:="GetPrivateProfileStringA", CharSet:=CharSet.Ansi)>
    Private Shared Function GetPrivateProfileString(
        ByVal lpAppName As String,
        ByVal lpKeyName As String,
        ByVal lpDefault As String,
        ByVal lpReturnedString As StringBuilder,
        ByVal nSize As Integer,
        ByVal lpFileName As String) As Integer
    End Function

    ''' <summary>
    ''' Lee un valor de un archivo .ini.
    ''' </summary>
    ''' <param name="seccion">Sección del archivo (ej. [Database]).</param>
    ''' <param name="clave">Clave dentro de la sección (ej. Server).</param>
    ''' <param name="archivoPath">Ruta completa al archivo .ini.</param>
    ''' <param name="valorDefecto">Valor a devolver si no se encuentra la clave.</param>
    ''' <returns>El valor leído o el valor por defecto.</returns>
    Public Shared Function LeerValor(ByVal seccion As String, ByVal clave As String, ByVal archivoPath As String, Optional ByVal valorDefecto As String = "") As String
        ' Buffer para recibir la información
        Dim sb As New StringBuilder(500)

        ' Llamada a la API de Windows
        Dim resultado As Integer = GetPrivateProfileString(seccion, clave, valorDefecto, sb, sb.Capacity, archivoPath)

        ' Devolver el contenido del buffer
        Return sb.ToString()
    End Function

End Class


''' <summary>
''' Proporciona métodos para encriptación y desencriptación simple basados en lógica legada.
''' </summary>
Public Module CryptoModule

    'Private Const CRYPT_KEY As String = "$#C@%&#G%@J#P%#C%#A%#J%#E%#D%#L%#O%#%#%#%&*"
    Private ReadOnly Key As Byte() = Encoding.UTF8.GetBytes("$#C@%&#G%@J#P%#C") ' 16 bytes  $#C@%&#G%@J#P%#C%#A%#J%#E%#D%#L*
    Private ReadOnly IV As Byte() = Encoding.UTF8.GetBytes("$#C@%&#G%@J#P%#C") ' 16 bytes
    ' Función para encriptar texto
    Public Function Encriptar(textoPlano As String) As String
        If String.IsNullOrEmpty(textoPlano) Then
            Throw New ArgumentException("El texto a encriptar no puede estar vacío.")
        End If

        Try
            Using aes As Aes = Aes.Create()
                aes.Key = Key
                aes.IV = IV
                aes.Mode = CipherMode.CBC
                aes.Padding = PaddingMode.PKCS7

                Dim encryptor As ICryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV)

                Using ms As New MemoryStream()
                    Using cs As New CryptoStream(ms, encryptor, CryptoStreamMode.Write)
                        Using sw As New StreamWriter(cs)
                            sw.Write(textoPlano)
                        End Using
                    End Using
                    Return Convert.ToBase64String(ms.ToArray())
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Error al encriptar: " & ex.Message, ex)
        End Try
    End Function

    ' Función para desencriptar texto
    Public Function Desencriptar(textoEncriptado As String) As String
        If String.IsNullOrEmpty(textoEncriptado) Then
            Throw New ArgumentException("El texto a desencriptar no puede estar vacío.")
        End If

        Try
            Dim buffer As Byte() = Convert.FromBase64String(textoEncriptado)

            Using aes As Aes = Aes.Create()
                aes.Key = Key
                aes.IV = IV
                aes.Mode = CipherMode.CBC
                aes.Padding = PaddingMode.PKCS7

                Dim decryptor As ICryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV)

                Using ms As New MemoryStream(buffer)
                    Using cs As New CryptoStream(ms, decryptor, CryptoStreamMode.Read)
                        Using sr As New StreamReader(cs)
                            Return sr.ReadToEnd()
                        End Using
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Error al desencriptar: " & ex.Message, ex)
        End Try
    End Function
End Module

Public Class Funciones
    ''' <summary>
    ''' True si el usuario tiene seguridad aplicada a esa pantalla. Sin registro, o ante cualquier error,
    ''' se considera restringido (igual que el VB6). Solo aplica = 0 libera la restricción.
    ''' </summary>
    Public Function Restringido(sUsuario As String, sModulo As String, sPantalla As String) As Boolean
        Try
            Dim dt As DataTable = Datos.Consultar(
                "SELECT aplica FROM PRIVILEGIOS WHERE UACCPAC = @Usuario AND MODULO = @Modulo AND Pantalla = @Pantalla",
                "@Usuario", sUsuario, "@Modulo", sModulo, "@Pantalla", sPantalla)
            If dt.Rows.Count = 0 Then Return True
            Return Convert.ToInt32(dt.Rows(0)("aplica")) <> 0
        Catch ex As Exception
            Debug.WriteLine("Error: " & ex.Message)
            Return True
        End Try
    End Function
End Class

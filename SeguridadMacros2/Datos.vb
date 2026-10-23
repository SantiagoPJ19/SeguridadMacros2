Imports System.Data.SqlClient

''' <summary>Modo de operación de las pantallas de mantenimiento (coincide con el menú Alta/Baja/Cambio/Consulta).</summary>
Public Enum ModoCrud
    Alta = 1
    Baja = 2
    Cambio = 3
    Consulta = 4
End Enum

''' <summary>
''' Acceso a la base de Usuarios (tablas USUARIOS, MODULOS, PANTALLAS y PRIVILEGIOS).
''' Reemplaza los Recordset/cnDB.Execute del VB6; todo va parametrizado.
''' </summary>
Public Module Datos

    Private Function Conexion() As SqlConnection
        Dim cn As SqlConnection = ConectaDB.cnDBU
        If cn Is Nothing Then
            Throw New InvalidOperationException("No hay conexión a la base de datos de usuarios.")
        End If
        If cn.State <> ConnectionState.Open Then cn.Open()
        Return cn
    End Function

    ''' <param name="args">Pares nombre/valor alternados: "@Usuario", valor, "@Modulo", valor...</param>
    Private Function CrearComando(sql As String, cn As SqlConnection, args As Object()) As SqlCommand
        Dim cmd As New SqlCommand(sql, cn)
        If args IsNot Nothing Then
            For i As Integer = 0 To args.Length - 2 Step 2
                cmd.Parameters.AddWithValue(CStr(args(i)), If(args(i + 1), DBNull.Value))
            Next
        End If
        Return cmd
    End Function

    Public Function Consultar(sql As String, ParamArray args As Object()) As DataTable
        Dim dt As New DataTable()
        Using cmd As SqlCommand = CrearComando(sql, Conexion(), args)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        Return dt
    End Function

    Public Function Ejecutar(sql As String, ParamArray args As Object()) As Integer
        Using cmd As SqlCommand = CrearComando(sql, Conexion(), args)
            Return cmd.ExecuteNonQuery()
        End Using
    End Function

    Public Function Existe(sql As String, ParamArray args As Object()) As Boolean
        Using cmd As SqlCommand = CrearComando(sql, Conexion(), args)
            Return cmd.ExecuteScalar() IsNot Nothing
        End Using
    End Function

    ''' <summary>Devuelve la primera columna como lista de texto (para llenar combos).</summary>
    Public Function Lista(sql As String, ParamArray args As Object()) As List(Of String)
        Dim res As New List(Of String)
        For Each fila As DataRow In Consultar(sql, args).Rows
            Dim texto As String = fila(0).ToString().Trim()
            If texto.Length > 0 Then res.Add(texto) ' hay filas con clave en blanco en los datos
        Next
        Return res
    End Function

    ' ---------------------------------------------------------------
    ' Equivalentes de ChecaNullStr / ChecaNullInt de Funciones.bas
    ' ---------------------------------------------------------------
    Public Function ChecaNullStr(valor As Object) As String
        If valor Is Nothing OrElse IsDBNull(valor) Then Return ""
        Return valor.ToString()
    End Function

    ' ---------------------------------------------------------------
    ' MODULOS / PANTALLAS (mismo esquema: clave + descripcion)
    ' Los nombres de tabla y columna no se pueden parametrizar, por eso salen de un Select Case fijo.
    ' ---------------------------------------------------------------
    Public Enum TipoCatalogo
        Modulo = 1
        Pantalla = 2
    End Enum

    Private Function TablaCatalogo(tipo As TipoCatalogo) As String
        Return If(tipo = TipoCatalogo.Modulo, "MODULOS", "PANTALLAS")
    End Function

    Private Function ColumnaCatalogo(tipo As TipoCatalogo) As String
        Return If(tipo = TipoCatalogo.Modulo, "MODULO", "PANTALLA")
    End Function

    Public Function CatalogoClaves(tipo As TipoCatalogo) As List(Of String)
        Return Lista($"SELECT {ColumnaCatalogo(tipo)} FROM {TablaCatalogo(tipo)} ORDER BY {ColumnaCatalogo(tipo)}")
    End Function

    ''' <summary>Descripción de la clave, o Nothing si no existe.</summary>
    Public Function CatalogoDescripcion(tipo As TipoCatalogo, clave As String) As String
        Dim dt As DataTable = Consultar($"SELECT descripcion FROM {TablaCatalogo(tipo)} WHERE {ColumnaCatalogo(tipo)} = @C", "@C", clave)
        Return If(dt.Rows.Count > 0, ChecaNullStr(dt.Rows(0)(0)), Nothing)
    End Function

    Public Function CatalogoExiste(tipo As TipoCatalogo, clave As String) As Boolean
        Return Existe($"SELECT 1 FROM {TablaCatalogo(tipo)} WHERE {ColumnaCatalogo(tipo)} = @C", "@C", clave)
    End Function

    Public Function CatalogoTienePrivilegios(tipo As TipoCatalogo, clave As String) As Boolean
        Return Existe($"SELECT 1 FROM PRIVILEGIOS WHERE {ColumnaCatalogo(tipo)} = @C", "@C", clave)
    End Function

    Public Sub CatalogoInsertar(tipo As TipoCatalogo, clave As String, descripcion As String)
        Ejecutar($"INSERT INTO {TablaCatalogo(tipo)} VALUES (@C, @D)", "@C", clave, "@D", descripcion)
    End Sub

    Public Sub CatalogoActualizar(tipo As TipoCatalogo, clave As String, descripcion As String)
        Ejecutar($"UPDATE {TablaCatalogo(tipo)} SET descripcion = @D WHERE {ColumnaCatalogo(tipo)} = @C", "@D", descripcion, "@C", clave)
    End Sub

    Public Sub CatalogoEliminar(tipo As TipoCatalogo, clave As String)
        Ejecutar($"DELETE FROM {TablaCatalogo(tipo)} WHERE {ColumnaCatalogo(tipo)} = @C", "@C", clave)
    End Sub

    ' ---------------------------------------------------------------
    ' PRIVILEGIOS (UACCPAC, MODULO, PANTALLA, APLICA)
    ' ---------------------------------------------------------------

    ''' <summary>Pantallas de un módulo: las que comparten con él los 2 primeros caracteres.</summary>
    Public Function PantallasDeModulo(modulo As String) As List(Of String)
        Dim prefijo As String = If(modulo.Length >= 2, modulo.Substring(0, 2), modulo)
        Return Lista("SELECT PANTALLA FROM PANTALLAS WHERE LEFT(PANTALLA, 2) = @P ORDER BY PANTALLA", "@P", prefijo)
    End Function

    ''' <summary>Valor de APLICA del privilegio, o Nothing si no existe.</summary>
    Public Function PrivilegioObtener(usuario As String, modulo As String, pantalla As String) As Integer?
        Dim dt As DataTable = Consultar(
            "SELECT APLICA FROM PRIVILEGIOS WHERE UACCPAC = @U AND MODULO = @M AND PANTALLA = @P",
            "@U", usuario, "@M", modulo, "@P", pantalla)
        If dt.Rows.Count = 0 Then Return Nothing
        If IsDBNull(dt.Rows(0)(0)) Then Return 0
        Return Convert.ToInt32(dt.Rows(0)(0))
    End Function

    Public Sub PrivilegioInsertar(usuario As String, modulo As String, pantalla As String, aplica As Integer)
        Ejecutar("INSERT INTO PRIVILEGIOS VALUES (@U, @M, @P, @A)",
                 "@U", usuario, "@M", modulo, "@P", pantalla, "@A", aplica)
    End Sub

    Public Sub PrivilegioActualizar(usuario As String, modulo As String, pantalla As String, aplica As Integer)
        Ejecutar("UPDATE PRIVILEGIOS SET APLICA = @A WHERE UACCPAC = @U AND MODULO = @M AND PANTALLA = @P",
                 "@A", aplica, "@U", usuario, "@M", modulo, "@P", pantalla)
    End Sub

    Public Sub PrivilegioEliminar(usuario As String, modulo As String, pantalla As String)
        Ejecutar("DELETE FROM PRIVILEGIOS WHERE UACCPAC = @U AND MODULO = @M AND PANTALLA = @P",
                 "@U", usuario, "@M", modulo, "@P", pantalla)
    End Sub

    ' ---------------------------------------------------------------
    ' USUARIOS
    ' ---------------------------------------------------------------
    Public Function UsuariosRegistrados() As List(Of String)
        Return Lista("SELECT UACCPAC FROM USUARIOS ORDER BY UACCPAC")
    End Function

    Public Function ObtenerUsuario(uaccpac As String) As DataRow
        Dim dt As DataTable = Consultar("SELECT * FROM USUARIOS WHERE UACCPAC = @U", "@U", uaccpac)
        Return If(dt.Rows.Count > 0, dt.Rows(0), Nothing)
    End Function

    Public Sub InsertarUsuario(uaccpac As String, nombre As String, apellido As String,
                               sucursal As String, puesto As String, funcion As String)
        Ejecutar("INSERT INTO USUARIOS VALUES (@U, @N, @A, @S, @P, @F)",
                 "@U", uaccpac, "@N", nombre, "@A", apellido, "@S", sucursal, "@P", puesto, "@F", funcion)
    End Sub

    Public Sub ActualizarUsuario(uaccpac As String, nombre As String, apellido As String,
                                 sucursal As String, puesto As String, funcion As String)
        Ejecutar("UPDATE USUARIOS SET nombre = @N, apellido = @A, sucursal = @S, puesto = @P, funcion = @F WHERE UACCPAC = @U",
                 "@N", nombre, "@A", apellido, "@S", sucursal, "@P", puesto, "@F", funcion, "@U", uaccpac)
    End Sub

    ''' <summary>Elimina el usuario y todos sus privilegios en una sola transacción.</summary>
    Public Sub EliminarUsuario(uaccpac As String)
        Dim cn As SqlConnection = Conexion()
        Using tx As SqlTransaction = cn.BeginTransaction()
            Try
                Using cmd As New SqlCommand("DELETE FROM PRIVILEGIOS WHERE UACCPAC = @U", cn, tx)
                    cmd.Parameters.AddWithValue("@U", uaccpac)
                    cmd.ExecuteNonQuery()
                End Using
                Using cmd As New SqlCommand("DELETE FROM USUARIOS WHERE UACCPAC = @U", cn, tx)
                    cmd.Parameters.AddWithValue("@U", uaccpac)
                    cmd.ExecuteNonQuery()
                End Using
                tx.Commit()
            Catch
                tx.Rollback()
                Throw
            End Try
        End Using
    End Sub

End Module

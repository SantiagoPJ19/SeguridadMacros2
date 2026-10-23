''' <summary>
''' Borde con el color de la compañía para los formularios hijos (mismo grosor que el de frmPrincipal).
''' Un formulario normal no tiene MdiClient encima, así que basta con dibujarlo en su Paint.
''' </summary>
Public Module Bordes

    Public Const AnchoBorde As Integer = 5

    ''' <summary>Dibuja el borde del color de la empresa de la ventana principal (frmPrincipal.ColorEmpresa).</summary>
    Public Sub AplicaBordeEmpresa(frm As Form)
        Dim principal As frmPrincipal = TryCast(frm.MdiParent, frmPrincipal)
        If principal Is Nothing OrElse principal.ColorEmpresa.IsEmpty Then Return

        Dim color As Color = principal.ColorEmpresa
        AddHandler frm.Paint,
            Sub(sender As Object, e As PaintEventArgs)
                Using br As New SolidBrush(color)
                    Dim r As Rectangle = frm.ClientRectangle
                    e.Graphics.FillRectangle(br, 0, 0, r.Width, AnchoBorde)
                    e.Graphics.FillRectangle(br, 0, r.Height - AnchoBorde, r.Width, AnchoBorde)
                    e.Graphics.FillRectangle(br, 0, 0, AnchoBorde, r.Height)
                    e.Graphics.FillRectangle(br, r.Width - AnchoBorde, 0, AnchoBorde, r.Height)
                End Using
            End Sub
        frm.Invalidate()
    End Sub

End Module

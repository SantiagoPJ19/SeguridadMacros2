''' <summary>
''' Combos de búsqueda: se puede escribir (con autocompletado sobre la lista) y el valor escrito debe ser
''' uno de los elementos. Al salir del combo o con Enter, el texto se convierte en la selección real
''' (SelectedIndex), así que SelectedIndexChanged y las validaciones de los formularios siguen igual.
''' </summary>
Public Module Combos

    ''' <param name="alVaciar">Se invoca cuando queda sin selección (vacío o texto que no está en la lista),
    ''' para limpiar los datos que dependían de ella.</param>
    Public Sub HabilitaBusqueda(cmb As ComboBox, Optional alVaciar As Action = Nothing)
        AddHandler cmb.Validating, Sub(sender As Object, e As System.ComponentModel.CancelEventArgs) ResuelveSeleccion(cmb, alVaciar)
        AddHandler cmb.KeyDown,
            Sub(sender As Object, e As KeyEventArgs)
                If e.KeyCode = Keys.Enter Then
                    ResuelveSeleccion(cmb, alVaciar)
                    e.Handled = True
                    e.SuppressKeyPress = True
                End If
            End Sub
    End Sub

    Private Sub ResuelveSeleccion(cmb As ComboBox, alVaciar As Action)
        Dim texto As String = cmb.Text.Trim()
        Dim indice As Integer = -1

        If texto.Length > 0 Then
            For i As Integer = 0 To cmb.Items.Count - 1
                If String.Equals(cmb.Items(i).ToString().Trim(), texto, StringComparison.OrdinalIgnoreCase) Then
                    indice = i
                    Exit For
                End If
            Next
        End If

        If indice >= 0 Then
            If cmb.SelectedIndex <> indice Then cmb.SelectedIndex = indice ' dispara SelectedIndexChanged
            Return
        End If

        ' Vacío o no existe en la lista: se descarta lo escrito y se limpian los datos dependientes.
        If cmb.SelectedIndex <> -1 Then cmb.SelectedIndex = -1
        cmb.Text = ""
        If alVaciar IsNot Nothing Then alVaciar()
    End Sub

End Module

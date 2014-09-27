Imports System
Imports System.Collections
Imports System.IO
Imports System.IO.Ports
Imports System.ComponentModel

Public Class Form1

    Dim startTime As DateTime
    Dim endTime As DateTime
    Dim data1, data2, data3, data4 As Double
    Dim filename As String

    Public Sub DataHandler1() Handles SerialPort1.DataReceived
        Dim reading As Double
        Dim newbits As BitArray
        newbits = readPort(1)
        Try
            reading = analyzeBits(newbits)
        Catch ex As InvalidDataException
            reading = data1
            SerialPort1.DiscardInBuffer()
        End Try
        data1 = reading
        SetText(reading, 1)
    End Sub

    Public Sub DataHandler2() Handles SerialPort2.DataReceived
        Dim reading As Double
        Dim newbits As BitArray
        newbits = readPort(2)
        Try
            reading = analyzeBits(newbits)
        Catch ex As InvalidDataException
            reading = data2
            SerialPort2.DiscardInBuffer()
        End Try
        data2 = reading
        SetText(reading, 2)
    End Sub

    Public Sub DataHandler3() Handles SerialPort3.DataReceived
        Dim reading As Double
        Dim newbits As BitArray
        newbits = readPort(3)
        Try
            reading = analyzeBits(newbits)
        Catch ex As InvalidDataException
            reading = data3
            SerialPort3.DiscardInBuffer()
        End Try
        data3 = reading
        SetText(reading, 3)
    End Sub

    Public Sub DataHandler4() Handles SerialPort4.DataReceived
        Dim reading As Double
        Dim newbits As BitArray
        newbits = readPort(4)
        Try
            reading = analyzeBits(newbits)
        Catch ex As InvalidDataException
            reading = data4
            SerialPort4.DiscardInBuffer()
        End Try
        data4 = reading
        SetText(reading, 4)
    End Sub

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim s As String
        For Each s In SerialPort.GetPortNames()
            ComboBox1.Items.Add(s)
            ComboBox2.Items.Add(s)
            ComboBox3.Items.Add(s)
            ComboBox4.Items.Add(s)
        Next s
    End Sub

    Private Function analyzeBits(ByVal bits As BitArray) As Double
        Dim sign As Int32
        Dim divider As Int32
        Dim digit(4) As Int32
        Dim result As Double
        Dim x As Int32
        Dim a As Int32
        Dim s(7) As Boolean
        If bits.Get(12) = True Then
            sign = -1
        Else
            sign = 1
        End If

        If bits.Get(28) = True Then
            divider = 1000
        ElseIf bits.Get(44) = True Then
            divider = 100
        ElseIf bits.Get(60) = True Then
            divider = 10
        Else
            divider = 1
        End If

        For i As Integer = 0 To 3
            x = i * 16 + 13
            For j As Integer = 0 To 6
                If j < 3 Then
                    a = 0
                Else
                    a = 4
                End If
                s(j) = bits.Get(x + j + a)
            Next



            If s(0) And s(1) And s(2) And s(3) And s(4) And Not s(5) And s(6) Then
                digit(i) = 0
            ElseIf Not s(0) And Not s(1) And Not s(2) And Not s(3) And s(4) And Not s(5) And s(6) Then
                digit(i) = 1
            ElseIf s(0) And Not s(1) And s(2) And s(3) And Not s(4) And s(5) And s(6) Then
                digit(i) = 2
            ElseIf Not s(0) And Not s(1) And s(2) And s(3) And s(4) And s(5) And s(6) Then
                digit(i) = 3
            ElseIf Not s(0) And s(1) And Not s(2) And Not s(3) And s(4) And s(5) And s(6) Then
                digit(i) = 4
            ElseIf Not s(0) And s(1) And s(2) And s(3) And s(4) And s(5) And Not s(6) Then
                digit(i) = 5
            ElseIf s(0) And s(1) And s(2) And s(3) And s(4) And s(5) And Not s(6) Then
                digit(i) = 6
            ElseIf Not s(0) And Not s(1) And s(2) And Not s(3) And s(4) And Not s(5) And s(6) Then
                digit(i) = 7
            ElseIf s(0) And s(1) And s(2) And s(3) And s(4) And s(5) And s(6) Then
                digit(i) = 8
            ElseIf s(0) And s(1) And s(2) And s(3) And s(4) And Not s(5) And s(6) Then
                digit(i) = 9
            Else
                Throw New InvalidDataException
            End If
        Next

        result = sign * (digit(0) * 1000 + digit(1) * 100 + digit(2) * 10 + digit(3)) / divider
        Return result
    End Function

    Private Function readPort(ByVal portNum As Int32) As BitArray
        Dim buffer(14) As Byte
        Dim s As String
        Dim out As String
        Dim bits As BitArray
        Dim newbits As BitArray
        Dim index As Int32 = 0
        Select Case portNum
            Case 1
                SerialPort1.Read(buffer, 0, 14)
            Case 2
                SerialPort2.Read(buffer, 0, 14)
            Case 3
                SerialPort3.Read(buffer, 0, 14)
            Case 4
                SerialPort4.Read(buffer, 0, 14)
        End Select

        For Each b In buffer
            s = Convert.ToString(b, 2)
            out = out & s & "-"
        Next

        bits = New BitArray(buffer)
        newbits = New BitArray(120)
        For i As Integer = 0 To 13
            For j As Integer = 0 To 7
                newbits.Set(index, bits.Get(i * 8 + 7 - j))
                index = index + 1
            Next
        Next
        Return newbits
    End Function

    Delegate Sub SetTextCallback(text As String, field As Int32)

    Private Sub SetText(ByVal text As String, ByVal field As Int32)

        ' InvokeRequired required compares the thread ID of the
        ' calling thread to the thread ID of the creating thread.
        ' If these threads are different, it returns true.
        If Me.Label1.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf SetText)
            Me.Invoke(d, New Object() {text, field})
        Else
            Select Case field
                Case 1
                    Me.Label1.Text = text
                Case 2
                    Me.Label2.Text = text
                Case 3
                    Me.Label6.Text = text
                Case 4
                    Me.Label7.Text = text
            End Select
        End If
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        If Not SerialPort1.IsOpen Then
            Try
                SerialPort1.PortName = ComboBox1.Text
                SerialPort1.BaudRate = 2400
                SerialPort1.Parity = IO.Ports.Parity.None
                SerialPort1.DataBits = 8
                SerialPort1.StopBits = IO.Ports.StopBits.One
                SerialPort1.Handshake = IO.Ports.Handshake.None
                SerialPort1.ReadTimeout = 500
                SerialPort1.ReceivedBytesThreshold = 14
                SerialPort1.DtrEnable = True
                SerialPort1.Open()
                GroupBox1.Text = ComboBox1.Text
            Catch ex As Exception
                SerialPort1.Close()
            End Try
        Else
            SerialPort1.DiscardInBuffer()
        End If
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        If Not SerialPort2.IsOpen Then
            Try
                SerialPort2.PortName = ComboBox2.Text
                SerialPort2.BaudRate = 2400
                SerialPort2.Parity = IO.Ports.Parity.None
                SerialPort2.DataBits = 8
                SerialPort2.StopBits = IO.Ports.StopBits.One
                SerialPort2.Handshake = IO.Ports.Handshake.None
                SerialPort2.ReadTimeout = 5000
                SerialPort2.ReceivedBytesThreshold = 14
                SerialPort2.DtrEnable = True
                SerialPort2.Open()
                GroupBox2.Text = ComboBox2.Text
            Catch ex As Exception
                SerialPort2.Close()
            End Try
        Else
            SerialPort2.DiscardInBuffer()
        End If
    End Sub

    Private Sub Button7_Click(sender As System.Object, e As System.EventArgs) Handles Button7.Click
        If Not SerialPort3.IsOpen Then
            Try
                SerialPort3.PortName = ComboBox3.Text
                SerialPort3.BaudRate = 2400
                SerialPort3.Parity = IO.Ports.Parity.None
                SerialPort3.DataBits = 8
                SerialPort3.StopBits = IO.Ports.StopBits.One
                SerialPort3.Handshake = IO.Ports.Handshake.None
                SerialPort3.ReadTimeout = 5000
                SerialPort3.ReceivedBytesThreshold = 14
                SerialPort3.DtrEnable = True
                SerialPort3.Open()
                GroupBox3.Text = ComboBox3.Text
            Catch ex As Exception
                SerialPort3.Close()
            End Try
        Else
            SerialPort3.DiscardInBuffer()
        End If
    End Sub

    Private Sub Button9_Click(sender As System.Object, e As System.EventArgs) Handles Button9.Click
        If Not SerialPort4.IsOpen Then
            Try
                SerialPort4.PortName = ComboBox4.Text
                SerialPort4.BaudRate = 2400
                SerialPort4.Parity = IO.Ports.Parity.None
                SerialPort4.DataBits = 8
                SerialPort4.StopBits = IO.Ports.StopBits.One
                SerialPort4.Handshake = IO.Ports.Handshake.None
                SerialPort4.ReadTimeout = 5000
                SerialPort4.ReceivedBytesThreshold = 14
                SerialPort4.DtrEnable = True
                SerialPort4.Open()
                GroupBox4.Text = ComboBox4.Text
            Catch ex As Exception
                SerialPort4.Close()
            End Try
        Else
            SerialPort4.DiscardInBuffer()
        End If
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        SerialPort1.Close()
        GroupBox1.Text = "closed"
        Label1.Text = " "
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        SerialPort2.Close()
        GroupBox2.Text = "closed"
        Label2.Text = " "
    End Sub

    Private Sub Button6_Click(sender As System.Object, e As System.EventArgs) Handles Button6.Click
        SerialPort3.Close()
        GroupBox3.Text = "closed"
        Label6.Text = " "
    End Sub

    Private Sub Button8_Click(sender As System.Object, e As System.EventArgs) Handles Button8.Click
        SerialPort4.Close()
        GroupBox4.Text = "closed"
        Label7.Text = " "
    End Sub

    Private Sub startAcqButton_Click(sender As System.Object, e As System.EventArgs) Handles startAcqButton.Click
        filename = TextBox1.Text
        If File.Exists(filename) Then
            File.Delete(filename)
        End If
        Call resetCharts()
        Timer2.Interval = Convert.ToInt32(Convert.ToDouble(TextBox2.Text) * 1000)
        startTime = Now
        Timer1.Start()
        Timer2.Start()
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Dim elapsed As TimeSpan
        elapsed = Now - startTime
        Label8.Text = New DateTime(elapsed.Ticks).ToString("HH:mm:ss")
    End Sub

    Private Sub Button10_Click(sender As System.Object, e As System.EventArgs) Handles Button10.Click
        Dim elapsed As TimeSpan
        If Timer1.Enabled Then
            Timer1.Stop()
            endTime = Now
            elapsed = endTime - startTime
            Label8.Text = New DateTime(elapsed.Ticks).ToString("HH:mm:ss")
        End If
        If Timer2.Enabled Then
            Timer2.Stop()

        End If
    End Sub

    Private Sub SaveFileDialog1_FileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles SaveFileDialog1.FileOk
        TextBox1.Text = SaveFileDialog1.FileName
    End Sub

    Private Sub Button11_Click(sender As System.Object, e As System.EventArgs) Handles Button11.Click
        SaveFileDialog1.ShowDialog()
    End Sub

    Private Sub Timer2_Tick(sender As System.Object, e As System.EventArgs) Handles Timer2.Tick
        Dim line As String
        line = Convert.ToString(Now) + ", "
        If Not GroupBox1.Text = "closed" Then
            line = line + Convert.ToString(data1) + ", "
            Chart1.Series.Item(0).Points.AddXY(Now, data1)
        Else
            line = line + Convert.ToString(0.0) + ", "
        End If
        If Not GroupBox2.Text = "closed" Then
            line = line + Convert.ToString(data2) + ", "
            Chart2.Series.Item(0).Points.AddXY(Now, data2)
        Else
            line = line + Convert.ToString(0.0) + ", "
        End If
        If Not GroupBox3.Text = "closed" Then
            line = line + Convert.ToString(data3) + ", "
            Chart3.Series.Item(0).Points.AddXY(Now, data3)
        Else
            line = line + Convert.ToString(0.0) + ", "
        End If
        If Not GroupBox4.Text = "closed" Then
            line = line + Convert.ToString(data4) + ", "
            Chart4.Series.Item(0).Points.AddXY(Now, data4)
        Else
            line = line + Convert.ToString(0.0) + ", "
        End If
        File.AppendAllText(filename, line + Environment.NewLine)
    End Sub

    Private Sub resetCharts()
        For Each Chart As System.Windows.Forms.DataVisualization.Charting.Chart In {Chart1, Chart2, Chart3, Chart4}
            Chart.Series.Item(0) = New System.Windows.Forms.DataVisualization.Charting.Series
            Chart.Series.Item(0).ChartType = DataVisualization.Charting.SeriesChartType.Line
            Chart.Series.Item(0).XValueType = DataVisualization.Charting.ChartValueType.Time
            Chart.Series.Item(0).BorderWidth = 2
        Next
        Chart1.Series.Item(0).Color = Color.Blue
        Chart2.Series.Item(0).Color = Color.Red
        Chart3.Series.Item(0).Color = Color.Lime
        Chart4.Series.Item(0).Color = Color.Magenta
    End Sub


End Class

Public Class InvalidDataException
    Inherits Exception

    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class
Imports System.Net.Sockets
Imports System.Text.Encoding
Imports System.Net

Public Class udp_frm

    Dim client As New UdpClient(514)
    Dim ep As New IPEndPoint(IPAddress.Any, 0)
    Dim rcvbytes As [Byte]()

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Timer1.Interval = 10
        Timer1.Start()
        Button1.Enabled = False
        Button2.Enabled = True
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If BackgroundWorker1.IsBusy = False Then
            BackgroundWorker1.RunWorkerAsync(ep)
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button2.Enabled = False
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Timer1.Stop()
        Button1.Enabled = True
        Button2.Enabled = False
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        rcvbytes = client.Receive(e.Argument)
        e.Result = ASCII.GetString(rcvbytes) + vbCrLf
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        TextBox1.AppendText(e.Result)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox1.Clear()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Clipboard.Clear()
        TextBox1.SelectAll()
        TextBox1.Copy()
    End Sub
End Class

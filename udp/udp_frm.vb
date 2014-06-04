Imports System.Net.Sockets
Imports System.Text.Encoding
Imports System.Net
Imports System.IO

Public Class udp_frm

    Dim fileStream As FileStream
    Dim streamWriter As StreamWriter
    Dim streamReader As StreamReader
    Dim strPath As String = Application.StartupPath & "\" & "udp514.log"

    Dim client As UdpClient
    Dim ep As New IPEndPoint(IPAddress.Any, 0)
    Dim rcvbytes As [Byte]()

    Dim fake As New IPEndPoint(IPAddress.Loopback, 514)
    Dim fakesend As [Byte]() = {1}

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        client = New UdpClient(514)
        Timer1.Interval = 10
        Timer1.Start()
        Button1.Enabled = False
        Button2.Enabled = True
        TextBox1.AppendText(Button1.Text + vbCrLf)
        WriteLog(Button1.Text)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If BackgroundWorker1.IsBusy = False Then
            BackgroundWorker1.RunWorkerAsync(ep)
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button2.Enabled = False
        If File.Exists(strPath) Then
            streamReader = New StreamReader(strPath, True)
            TextBox1.Text = streamReader.ReadToEnd
            streamReader.Close()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Timer1.Stop()
        Button1.Enabled = True
        Button2.Enabled = False
        client.Send(fakesend, fakesend.Length, fake)
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        rcvbytes = client.Receive(e.Argument)
        e.Result = ASCII.GetString(rcvbytes)
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If Button1.Enabled = True Then
            client.Close()
            TextBox1.AppendText(Button2.Text + vbCrLf)
            WriteLog(Button2.Text)
        Else
            TextBox1.AppendText(e.Result.ToString + vbCrLf)
            WriteLog(e.Result.ToString)
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox1.Clear()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Clipboard.Clear()
        TextBox1.SelectAll()
        TextBox1.Copy()
    End Sub

    Private Sub udp_frm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If MessageBox.Show("Are you sure you want to quit?", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then
            e.Cancel = True
        End If
    End Sub

    Private Sub OpenFile()
        If File.Exists(strPath) Then
            fileStream = New FileStream(strPath, FileMode.Append, FileAccess.Write)
        Else
            fileStream = New FileStream(strPath, FileMode.Create, FileAccess.Write)
        End If
        streamWriter = New StreamWriter(fileStream)
    End Sub

    Private Sub WriteLog(ByVal strComments As String)
        OpenFile()
        streamWriter.WriteLine(strComments)
        CloseFile()
    End Sub

    Private Sub CloseFile()
        streamWriter.Close()
        fileStream.Close()
    End Sub
End Class

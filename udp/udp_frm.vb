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

    Dim nolog As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NotifyIcon1.Text = Me.Text
        Button2.Enabled = False 'До вызова Start()
        Dim cmdvalue As String() = Environment.GetCommandLineArgs()
        For Each retvalue As String In cmdvalue
            If retvalue = "-start" Then
                Start()
                Label2.Text = Label2.Text & " -start"
            ElseIf retvalue = "-tray" Then
                FormMinimized()
                Label2.Text = Label2.Text & " -tray"
            ElseIf retvalue = "-nolog" Then
                nolog = True
                Label2.Text = Label2.Text & " -nolog"
            End If
        Next
        If Label2.Text = "Loaded with:" Then Label2.Text = Label2.Text & " none"
        'После получения nolog
        If File.Exists(strPath) And nolog = False Then
            streamReader = New StreamReader(strPath, True)
            TextBox1.Text = streamReader.ReadToEnd
            streamReader.Close()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Start()
    End Sub

    Private Sub Start()
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Timer1.Stop()
        Button1.Enabled = True
        Button2.Enabled = False
        client.Send(fakesend, fakesend.Length, fake)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox1.Clear()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Clipboard.Clear()
        TextBox1.SelectAll()
        TextBox1.Copy()
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
            ErrorToTray(e.Result.ToString)
        End If
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
        If nolog = False Then
            OpenFile()
            streamWriter.WriteLine(strComments)
            CloseFile()
        End If
    End Sub

    Private Sub CloseFile()
        streamWriter.Close()
        fileStream.Close()
    End Sub

    Private Sub FormMinimized()
        'Me.Hide() 'Под XP не работает
        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False
        NotifyIcon1.Visible = True
        NotifyIcon1.BalloonTipText = "I'm here!"
        NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
        NotifyIcon1.BalloonTipTitle = Me.Text
        NotifyIcon1.ShowBalloonTip(5000)
    End Sub

    Private Sub NotifyIcon1_Click(sender As Object, e As EventArgs) Handles NotifyIcon1.Click
        'Me.Show() 'Под XP не работает
        Me.WindowState = FormWindowState.Normal
        Me.ShowInTaskbar = True
        NotifyIcon1.Visible = False
    End Sub

    Private Sub udp_frm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then FormMinimized()
        If Me.WindowState = FormWindowState.Normal Then
            NotifyIcon1.Visible = False 'Убрать значок в трее при повторном запуске приложения
            Button4.Focus()
        End If
    End Sub

    Private Sub ErrorToTray(ByVal message As String)
        If Me.WindowState = FormWindowState.Minimized Then
            NotifyIcon1.BalloonTipText = message
            NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning
            NotifyIcon1.ShowBalloonTip(60000)
        End If
    End Sub
End Class

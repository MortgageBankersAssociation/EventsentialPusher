
Imports System.Timers
Imports System.Net.Mail

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Imports System.IO
Public Class Form1
    Private t As Timer
    Dim constring As String = "data source=imissql;initial catalog=iMIS_MBA;password=deaddog;persist security info=True;user id=sa;packet size=4096;Max Pool Size=20000"


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'If m = "2:00 PM" Or m = "11:00 PM" Then

        Try
            '//### Code to pull JSON from File

            Dim JSONconferences As String = "[{""ConferenceId"":""2171404"", ""EventsentialId"":""10833""}, {""ConferenceId"":""2162018"", ""EventsentialId"":""555""}, ""{""ConferenceId"":""2162056"", ""EventsentialId"":""572""}]"

            '//#### Loop through JSON and call Push Method


            For Each item In JSONconferences

                Try
                    'CallPush(item.ConferenceId, item.EventsentialId)
                Catch ex As Exception
                    ' SendNotification("Error in " & item.ConferenceId & ", " & item.EventsentialId & " " & Environment.NewLine & ex.Message.ToString())
                End Try
            Next
        Catch ex As Exception

        End Try




        Try
            CallPush("2171404", "10833")
        Catch ex As Exception
            SendNotification("Error in 2171404, 10833" & Environment.NewLine & ex.Message.ToString())
        End Try

        Try
            CallPush("2162018", "555")
        Catch ex As Exception
            SendNotification("Error in 2162018, 555" & Environment.NewLine & ex.Message.ToString())
        End Try
        Try
            CallPush("2162056", "572")
        Catch ex As Exception
            SendNotification("Error in 2162056, 572" & Environment.NewLine & ex.Message.ToString())
        End Try
        Try
            CallPush("2162032", "601")
        Catch ex As Exception
            SendNotification("Error in 2162032, 601" & Environment.NewLine & ex.Message.ToString())
        End Try

        Try
            CallPush("2162050", "662")
        Catch ex As Exception
            SendNotification("Error in 2162050, 662" & Environment.NewLine & ex.Message.ToString())
        End Try



        ' ClassLib.WriteInTextFile("time of the day in ToLongTimeString: " & e.SignalTime.ToLongTimeString.ToString() & Environment.NewLine & " Hour: " & a.ToString())
        'ClassLib.WriteInTextFile("time of the day in ToShortTimeString: " & m.ToString() & Environment.NewLine & Environment.NewLine & Environment.NewLine & Environment.NewLine)
        '


    End Sub
    Private Sub CallPush(ByVal CurrentConference As String, ByVal eventsentialsID As String)

        'Dim CurrentConference As String = "2162032"
        ' Dim constring As String = "data source=imissql;initial catalog=iMIS_MBA;password=deaddog;persist security info=True;user id=sa;packet size=4096;Max Pool Size=20000"

        Dim txt As String = "Conference Selected." & vbCrLf

        Dim obj As New WebProxyEventsentials

        obj.ConnectionString = constring ' System.Configuration.ConfigurationSettings.AppSettings("connContent")

        obj.ConferenceCode = CurrentConference

        '   obj.eventsentialsID = "601"
        obj.eventsentialsID = eventsentialsID

        txt = vbCrLf & txt & vbCrLf & "Just ran Post: " & System.DateTime.Now
        txt = Environment.NewLine & txt & Environment.NewLine & " Exhibitor Code: " & CurrentConference & Environment.NewLine & " EventsentialsID: " & obj.eventsentialsID & Environment.NewLine
        'ClassLib.WriteInTextFile(Environment.NewLine & txt & Environment.NewLine & " Exhibitor Code: " & CurrentConference & Environment.NewLine & " EventsentialsID: " & obj.eventsentialsID)
        txt = txt & vbCrLf & "About to run Post" & ":"
        txt = txt & vbCrLf & vbCrLf & obj.PostEExhibitors(CurrentConference)
        ClassLib.WriteInTextFile(txt)
        SendNotification(txt)
    End Sub

    '## function for Sending email nottifications
    Private Sub SendNotification(ByVal Body As String)
        Dim message As String = "Eventsentials API Push"

        '## username for the mail account
        Dim userName As String = "EventsentialsWebAPIPush@mba.org"

        Dim fromText As String = "Testing Eventsentials API Push"

        'Whatever to identify the Sender
        Dim toText As String = "developrs@mba.org"

        'Dim CCText As String = "nhazari@mba.org"

        Dim subjectText As String = "Eventsentials API Push Info"

        Dim bodyText As String = Body & Environment.NewLine & Environment.NewLine
        If Body.IndexOf("ERROR") > 0 Then
            bodyText += Environment.NewLine & "Extract of first error: " & """" & Body.Substring(Body.IndexOf("ERROR"), 54) & """" & Environment.NewLine
        End If

        bodyText = bodyText + Environment.NewLine & Convert.ToString("*** This is an automatically generated email, please do not reply to this message ***")
        Dim smtpClient As New SmtpClient()
        smtpClient.Host = "smtpout1.mbaa.local" ' old server '"10.16.30.6"
        '"SPAMFILTER1";
        smtpClient.Port = 25
        smtpClient.EnableSsl = False
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.UseDefaultCredentials = False
        '  smtpClient.Credentials = new NetworkCredential(userName, passWord);
        Dim mail As New MailMessage(userName, toText, subjectText, bodyText)
        '        mail.CC.Add(CCText)
        mail.BodyEncoding = UTF8Encoding.UTF8
        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
        smtpClient.Send(mail)
    End Sub



    '

    'Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
    '    'Dim MyLog As New EventLog() ' create a new event log
    '    '' Check if the the Event Log Exists
    '    'If Not MyLog.Source.MyLog.SourceExists("MyService") Then
    '    '    MyLog.CreateEventSource("MyService", "Myservice Log")
    '    '    ' Create Log
    '    'End If
    '    'MyLog.Source = "MyService"
    '    '' Write to the Log
    '    'MyLog.WriteEntry("MyService Log", "This is log on " & _
    '    '                  CStr(TimeOfDay), _
    '    '                  EventLogEntryType.Information)
    'End Sub
End Class
Public NotInheritable Class ClassLib
    Private Sub New()
    End Sub
    Public Shared Sub WriteInTextFile(val As String)
        Dim S As StreamWriter = Nothing
        S = New StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "ExhibitorPushServiceLog.txt", True)
        S.WriteLine(Convert.ToString(vbLf) & val)
        S.Flush()
        S.Close()
    End Sub
End Class



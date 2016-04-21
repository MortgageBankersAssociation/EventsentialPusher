Imports System.Web
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Public Class WebProxyEventsentials



#Region "Declarations"

    Private _meetingid As String
    Private _sessionid As Integer
    Private _uri As String
    Private _jsonData As String
    Private _eventsentialsID As String
    Private _localJsonHashTable As New Hashtable

    Private _ConnectionString As String


#End Region

#Region "Properties"
    Public Property ConnectionString() As String
        Get
            Return Me._ConnectionString
        End Get
        Set(ByVal Value As String)
            Me._ConnectionString = Value
        End Set
    End Property

    Public Property ConferenceCode() As String
        Get
            Return Me._meetingid
        End Get
        Set(ByVal Value As String)
            Me._meetingid = Value
        End Set
    End Property

    Public Property EventsentialUri() As String
        Get
            Return Me._uri
        End Get
        Set(ByVal Value As String)
            Me._uri = Value
        End Set
    End Property

    Public Property eventsentialsID() As String
        Get
            Return Me._eventsentialsID
        End Get
        Set(ByVal Value As String)
            Me._eventsentialsID = Value
        End Set
    End Property


#End Region

#Region "Methods / Functions"

    Public Function EExhibitors() As DataTable

        '//Create Object for DataEventsentials
        Dim objData As New DataEventsentials

        '//Connection string assigning
        objData.ConnectionString = Me.ConnectionString

        '//Meeting id assinging to ConferenceCode
        objData.ConferenceCode = Me._meetingid

        'Declare Expo ID
        Dim strExpoID As String = ""

        '// ExpoId 
        strExpoID = Replace(objData.ConferenceCode, "M", "")

        Dim ds As DataSet = New DataSet()

        '//Get dataset from getExhibitorsWithProductsAndServices by passing 
        ds = objData.getExhibitorsWithProductsAndServices(strExpoID)

        Return ds.Tables(0)

    End Function



    Public Function PostEExhibitors(ByVal meetingcode As String) As String

        Dim strResponse As String = "", strPost As String = ""
        Dim sExhibitor As Byte()
        Dim tmpname As String = ""

        Try

            Dim dt As DataTable
            Me._meetingid = meetingcode
            dt = Me.EExhibitors
            'Dim drow2 As DataRow


            For Each drow As DataRow In dt.Rows

                strPost = Me.buildJSONExhibitor(drow)

                sExhibitor = Encoding.UTF8.GetBytes(strPost)
                Me._uri = "https://api.eventsential.org/data/exhibitors"
                'send data string

                strResponse = strResponse & vbCrLf & Me.SendJsonData(sExhibitor, Me._jsonData) & vbCrLf

                '//## Nov-05-2015 
                'If strResponse.IndexOf("ERROR") = 0 Then
                '    strResponse = ""
                'End If
                '///##

                'Exit For

                'End If

                'tmpname = drow.Item("COMPANY_SORT")
                'tmpname = drow.Item("EXHB_ID")

            Next

            strResponse = strResponse & vbCrLf & vbCrLf & "Done with Exhibitors" & vbCrLf & vbCrLf

        Catch ex As Exception
            strResponse = vbCrLf & vbCrLf & "Exhibitors ERROR" & vbCrLf & strResponse & vbCrLf & vbCrLf & ex.Message.ToString() & vbCrLf & vbCrLf
        End Try

        'strResponse = "Successful Speakers Push!" & vbCrLf & vbCrLf

        Return strResponse




    End Function

    Private Function SendJsonData(ByVal jsonDataBytes() As Byte, ByVal testSession As String) As String

        Dim user As String = "api-780a649d65cf298b1c5feee951932aec"
        Dim pwd As String = "GQZYZDEYJUPRWJFORF4RAXYPLLK6YSTD"

        Dim theUri As New Uri(Me._uri)

        Dim req As HttpWebRequest = HttpWebRequest.Create(theUri)
        Dim MyCred As New CredentialCache
        Try


            MyCred.Add(New Uri(Me._uri), "Basic", New NetworkCredential(user, pwd))

            req.ContentType = "application/json; charset=utf-8"
            req.Method = "POST"
            req.Accept = "application/json; charset=utf-8"
            req.Credentials = MyCred
            req.PreAuthenticate = True
            req.ContentLength = jsonDataBytes.Length

            Dim stream As Stream = req.GetRequestStream()
            stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
            stream.Close()

            Dim response1 As HttpWebResponse = req.GetResponse()

            Dim responsestream As Stream = response1.GetResponseStream()

            Dim reader As New StreamReader(responsestream)
            Dim res As String = reader.ReadToEnd()

            Return res


        Catch ex As Exception

            Dim wex As WebException = DirectCast(ex, WebException)

            Dim s = wex.Response.GetResponseStream()
            Dim ss As String = ""
            Dim lastNum As Integer = 0

            Do
                lastNum = s.ReadByte()
                ss += ChrW(lastNum)
            Loop While (lastNum <> -1)

            s.Close()


            Dim stringb As New StringBuilder
            stringb.Append(vbCrLf & vbCrLf & "ERROR #2:" & vbCrLf & ss & "  Response from Events =  ")
            stringb.Append(wex.StackTrace.ToString() & "  Content Length = ")
            stringb.Append(req.ContentLength.ToString() & "  Content Type =  ")
            stringb.Append(req.ContentType.ToString() & "  Headers = ")
            stringb.Append(req.Headers.ToString() & "  Method = ")
            stringb.Append(req.Method.ToString() & "  Request URI = ")
            stringb.Append(req.RequestUri.ToString() & " Content = ")
            stringb.Append("JSON info: " & testSession & vbCrLf & vbCrLf)

            Return stringb.ToString()



        End Try



    End Function



    Public Function ParseExhibitorJson(ByVal jsonDataString As String)

        Dim tmpJsonData As String = jsonDataString
        Dim charlocation As Integer = 0
        Dim startchar As Integer = 0
        Dim endchar As Integer = 0
        Dim parsingJsonStatus As String = ""
        Dim parsedJsonData As String = ""
        Dim substring As Integer = 0


        'For Each c As Char In tmpJsonData
        For value As Integer = 0 To jsonDataString.Length

            Try

                If tmpJsonData.Chars(value) = "{" Then
                    If parsingJsonStatus = "Begun" Or parsingJsonStatus = "BegunAgain" Then
                        parsingJsonStatus = "SubstringFound"
                        substring = substring + 1
                    Else
                        If parsingJsonStatus <> "SubstringFound" Then
                            startchar = charlocation
                            'to ensure we are starting fresh
                            endchar = 0
                            parsingJsonStatus = "Begun"
                        Else

                        End If
                    End If
                End If

                If tmpJsonData.Chars(value) = "}" Then
                    If parsingJsonStatus = "SubstringFound" Then
                        substring = substring - 1
                        parsingJsonStatus = "BegunAgain"
                    Else
                        endchar = charlocation
                        parsingJsonStatus = "End"
                        parsedJsonData = tmpJsonData.Substring(startchar, (endchar - startchar) + 1)
                        CompileParsedJson(parsedJsonData)
                    End If
                End If

                charlocation = charlocation + 1

            Catch ex As Exception
            End Try

        Next


    End Function

    Private Function CompileParsedJson(ByVal ParsedJsonString As String)

        'This function adds records to the HashTable 
        ' with the HashTable Key as the SpeakerExternalKey
        Dim tmpSpeakerExternalKey As String

        tmpSpeakerExternalKey = GetJsonItemValue("externalKey", ParsedJsonString)
        Me._localJsonHashTable.Add(tmpSpeakerExternalKey, ParsedJsonString)

    End Function

    Private Function GetJsonItemValue(ByVal ItemName As String, ByVal JsonString As String) As String

        'This function gets a value from the a Json String
        Dim charlocation As Integer = 0
        Dim startchar As Integer = 0
        Dim endchar As Integer = 0
        Dim parsingJsonStatus As String = ""

        Try
            'remove session references
            While JsonString.IndexOf("[") > 0
                startchar = JsonString.IndexOf("[")
                endchar = JsonString.IndexOf("]")
                JsonString = JsonString.Remove(startchar, (endchar - startchar) + 1)
            End While

        Catch ex As Exception

        End Try

        Try
            charlocation = JsonString.IndexOf(ItemName)

            For value As Integer = charlocation To JsonString.Length

                If parsingJsonStatus <> "Begun" Then
                    If JsonString.Chars(value) = ":" Then
                        startchar = charlocation + 2
                        parsingJsonStatus = "Begun"
                        charlocation = charlocation + 3
                        value = value + 3
                    End If
                Else
                    If JsonString.Chars(value) = """" Then
                        endchar = charlocation - 1
                        'parsingJsonStatus = JsonString.Chars(value).ToString
                        parsingJsonStatus = "End"
                        Exit For
                    End If
                End If
                charlocation = charlocation + 1
            Next

            parsingJsonStatus = JsonString.Substring(startchar, (endchar - startchar) + 1)
            Return parsingJsonStatus

        Catch ex As Exception

        End Try

    End Function

    Public Function buildJSONExhibitor(ByVal dRow As DataRow) As String

        '{
        '"aboutContent": "sample string 1",
        '"booth": "sample string 2",
        '"websiteURL": "sample string 3",
        '"email": "sample string 4",
        '"phone": "sample string 5",
        '"isFeatured": true,
        '"logoURL": "sample string 7",
        '"id": 1,
        '"eventID": 8,
        '"name": "sample string 9",
        '"externalKey": "sample string 10",
        '"externalTimestamp": "2014-09-18T15:07:04-04:00",
        '"externalHash": "sample string 11",
        '"version": "sample string 12",
        '"tags": {"Products And Services": ["a product", "a service", "etc.."]},
        '"createdOn": "2014-09-18T15:07:04-04:00",
        '"updatedOn": "2014-09-18T15:07:04-04:00"
        '}


        'Preprocess the data here and handle any issues prior to submitting
        Dim aboutExhibitor As String = ""
        Dim booth As String = ""
        Dim websiteURL As String = ""
        'Dim email As String = ""
        Dim phone As String = ""
        'Dim isFeatured As String = ""
        'Dim logoURL As String = ""
        Dim eventsentialsID As String = Me._eventsentialsID
        Dim name As String = ""
        Dim externalKey As String = ""
        Dim bizformat As New stringFormatting
        Dim exhibitorTags As String = ""



        Try
            'aboutExhibitor = bizformat.NoNulls(dRow.Item("SpeakerType"))
            Try
                aboutExhibitor = bizformat.NoNulls(dRow.Item("DESCRIPTION"))
                aboutExhibitor = bizformat.chkDoubleQuote(aboutExhibitor)
            Catch ex As Exception
                'aboutExhibitor &= "  error2 (" & ex.Message.ToString() & ") "
                'aboutExhibitor &= "  error2b"
                aboutExhibitor = ""
            End Try
        Catch ex As Exception
            aboutExhibitor = ""
        End Try

        'Validate products and services
        Try
            Try
                exhibitorTags = bizformat.NoNulls(dRow.Item("ProductsAndServices"))
                'exhibitorTags = bizformat.chkDoubleQuote(exhibitorTags)
            Catch ex As Exception

                exhibitorTags = ""
            End Try
        Catch ex As Exception
            exhibitorTags = ""
        End Try


        Try
            booth = bizformat.NoNulls(dRow.Item("BOOTH_NUMBER"))
            booth = bizformat.chkDoubleQuote(booth)
        Catch ex As Exception
            booth = ""
        End Try

        Try
            websiteURL = bizformat.NoNulls(dRow.Item("WEB"))
            websiteURL = bizformat.chkDoubleQuote(websiteURL)
        Catch ex As Exception
            websiteURL = ""
        End Try

        Try
            phone = bizformat.NoNulls(dRow.Item("PHONE"))
            phone = bizformat.chkDoubleQuote(phone)
        Catch ex As Exception
            phone = ""
        End Try

        Try
            name = bizformat.NoNulls(dRow.Item("COMPANY"))
            name = bizformat.chkDoubleQuote(name)
        Catch ex As Exception
            name = ""
        End Try

        Try
            externalKey = bizformat.NoNulls(dRow.Item("EXHB_ID"))
        Catch ex As Exception
            externalKey = ""
        End Try



        'Data has been processed, now build JSON in order to Push

        Me._jsonData = "{"

        Me._jsonData &= """" & "aboutContent" & """" & ":" & """" & aboutExhibitor & """" & ","

        Me._jsonData &= """" & "booth" & """" & ":" & """" & booth & """" & ","

        Me._jsonData &= """" & "websiteURL" & """" & ":" & """" & websiteURL & """" & ","

        Me._jsonData &= """" & "phone" & """" & ":" & """" & phone & """" & ","

        Me._jsonData &= """" & "eventID" & """" & ":" & """" & eventsentialsID & """" & ","

        Me._jsonData &= """" & "name" & """" & ":" & """" & name & """" & ","

        Me._jsonData &= """" & "externalKey" & """" & ":" & """" & externalKey & """" & ","

        Me._jsonData &= """" & "tags" & """" & ":{" & """" & "Products And Services" & """" & ":[" & exhibitorTags & "]}"

        Me._jsonData &= "}"


        Return Me._jsonData

    End Function

#End Region

    Private Function SendJsonDatanew(ByVal jsonDataBytes() As Byte) As String
        'We need a container to store the cookies in.
        Dim cookies As New CookieContainer

        'Request login page to get a session cookie
        GETHtml("https://api.eventsential.org/data/sessions", cookies)

        Dim user As String = "api-780a649d65cf298b1c5feee951932aec"
        Dim pwd As String = "GQZYZDEYJUPRWJFORF4RAXYPLLK6YSTD"

        'Now we can post data
        SendJsonData(user, pwd, cookies, jsonDataBytes)
    End Function

    Public Function SendJsonData(ByVal Username As String, ByVal Password As String, ByVal cookies As CookieContainer, ByVal jsonDataBytes() As Byte) As String
        Dim poststring As String = String.Format("username={0}&password={1}&user_login_button.x=63&user_login_button.y=13&user_login_button=login", Username, Password)
        Dim postdata As Byte() = Encoding.UTF8.GetBytes(poststring)

        Dim webRequest__1 As HttpWebRequest = DirectCast(WebRequest.Create("https://api.eventsential.org/data/sessions"), HttpWebRequest)
        webRequest__1.CookieContainer = cookies
        webRequest__1.Method = "POST"
        webRequest__1.Referer = "https://api.eventsential.org/data/sessions"
        webRequest__1.Headers.Add("origin", "https://api.eventsential.org")
        ' webRequest__1.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.5.21022;"
        webRequest__1.ContentType = "application/json; charset=utf-8"
        webRequest__1.ContentLength = jsonDataBytes.Length







        Dim writer As Stream = webRequest__1.GetRequestStream()
        writer.Write(jsonDataBytes, 0, jsonDataBytes.Length)


        Dim webResponse As HttpWebResponse = DirectCast(webRequest__1.GetResponse(), HttpWebResponse)
        'We need to add any response cookies to our cookie container
        cookies.Add(webResponse.Cookies)

        'Only for debug
        Dim stream = New StreamReader(webResponse.GetResponseStream())
        System.Diagnostics.Debug.WriteLine(stream.ReadToEnd())


        Return (webResponse.StatusCode = HttpStatusCode.OK)

    End Function

    Public Function GETHtml(ByVal url As String, ByVal cookies As CookieContainer) As String
        Dim webRequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        webRequest__1.CookieContainer = cookies
        webRequest__1.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.5.21022;"
        webRequest__1.Referer = "https://api.eventsential.org/data/sessions"

        Try
            Dim webResponse As HttpWebResponse = DirectCast(webRequest__1.GetResponse(), HttpWebResponse)
            'We need to add any response cookies to our cookie container           
            cookies.Add(webResponse.Cookies)

            Dim Stream As New StreamReader(webResponse.GetResponseStream())
            Return Stream.ReadToEnd()

        Catch ex As Exception

        End Try



    End Function

End Class
Public Class stringFormatting

    Public Function NoNulls(ByVal stringIn As String) As String
        Return stringIn
    End Function
    Public Function NoNulls(ByVal stringIn As System.DBNull) As String
        Return ""
    End Function
    Public Function NoNulls(ByVal stringIn As Object) As String
        Dim test As String
        Try

            test = stringIn
        Catch ex As Exception
            test = ""
        End Try
        Return test
    End Function

    Public Function chkQuote(ByVal str As String) As String
        Dim tempString As String = str
        Try
            If InStr(tempString, "'") > 0 Then
                tempString = Replace(tempString, "'", "''")
            Else
                tempString = tempString
            End If

            tempString = Trim(tempString)

            Return tempString
        Catch ex As Exception
            Return str
        End Try

    End Function

    Public Function chkDoubleQuote(ByVal str As String) As String
        Dim tempString As String = str
        Try
            If InStr(tempString, Chr(34)) > 0 Then
                tempString = Replace(tempString, Chr(34), " ")
            End If

            tempString = Trim(tempString)

            Return tempString
        Catch ex As Exception
            Return str
        End Try

    End Function

    Public Function NoNullsBoolean(ByVal stringIn As System.DBNull) As Boolean
        Return False
    End Function
    Public Function NoNullsBoolean(ByVal stringIn As Boolean) As Boolean
        Return stringIn
    End Function
    Public Function NoNullsBoolean(ByVal stringIn As Object) As Boolean
        If stringIn = "" Or stringIn = "0" Or stringIn = "false" Or stringIn = "False" Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function alphaToNumeric(ByVal strIn As String, ByVal maxLength As Integer)

        Dim HTan As New Hashtable
        Dim encodedString As String = ""
        strIn = strIn.Trim()
        Dim loopCount As Integer = 1


        HTan.Add("a", "1")
        HTan.Add("b", "2")
        HTan.Add("c", "3")
        HTan.Add("d", "4")
        HTan.Add("e", "5")
        HTan.Add("f", "6")
        HTan.Add("g", "7")
        HTan.Add("h", "8")
        HTan.Add("i", "9")
        HTan.Add("j", "10")
        HTan.Add("k", "11")
        HTan.Add("l", "12")
        HTan.Add("m", "13")
        HTan.Add("n", "14")
        HTan.Add("o", "15")
        HTan.Add("p", "16")
        HTan.Add("q", "17")
        HTan.Add("r", "18")
        HTan.Add("s", "19")
        HTan.Add("t", "20")
        HTan.Add("u", "21")
        HTan.Add("v", "22")
        HTan.Add("w", "23")
        HTan.Add("x", "24")
        HTan.Add("y", "25")
        HTan.Add("z", "26")
        HTan.Add("'", "27")
        HTan.Add("-", "28")
        HTan.Add(" ", "29")

        For Each c As Char In strIn
            If loopCount = maxLength Then
                Exit For
            End If
            Try
                encodedString = encodedString + HTan(c)
            Catch ex As Exception
                encodedString = encodedString + "30"
            End Try
            loopCount = loopCount + 1
        Next

        Return encodedString

    End Function

End Class

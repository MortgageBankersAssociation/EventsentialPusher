Imports System
Imports System.Data
Imports System.Data.SqlClient
Public Class DataEventsentials



#Region "Declarations"

    Private _meetingid As String
    Protected SQL As String
    Protected _connectionString As String
#End Region

#Region "Properties"

    Public Property ConferenceCode() As String
        Get
            Return Me._meetingid
        End Get
        Set(ByVal Value As String)
            Me._meetingid = Value
        End Set
    End Property



    Public Property ConnectionString() As String
        Get
            Return Me._connectionString
        End Get
        Set(ByVal Value As String)
            Me._connectionString = Value
        End Set
    End Property

#End Region
    Dim conn As SqlConnection = New SqlConnection("data source=imissql;initial catalog=iMIS_MBA;password=deaddog;persist security info=True;user id=sa;packet size=4096;Max Pool Size=20000")
    Dim ds As DataSet = New DataSet()
    Dim da As SqlDataAdapter
#Region "Method / Functions"
    Public Function getExhibitorsWithProductsAndServices(ByVal expoID As String) As DataSet
        Try
            '//Declare Sql Command 
            Dim cmd As SqlCommand = conn.CreateCommand

            '// Define CommandType as StoredProcedure 
            cmd.CommandType = CommandType.StoredProcedure

            '//sp_MBA_GetExhibitorListWithProductsServices as command text
            cmd.CommandText = "sp_MBA_GetExhibitorListWithProductsServices"

            'Declare SqlParameter
            Dim param As SqlParameter = cmd.Parameters.Add("@ExpoID", SqlDbType.VarChar, 15)

            '//SqlParameter with value
            param.Value = expoID

            '//SqlDataAdapter defining with command 
            da = New SqlDataAdapter(cmd)

            '//Load in dataset
            da.Fill(ds)

            '//Return dataset
            Return ds

        Catch ex As Exception
            'ex.Message
        End Try



    End Function

    Public Function getExhibitors(ByVal expoID As String) As DataSet
        'Me.SQL = "sp_Big5ExhibitorList '" & expoID & "','','', ' ORDER By Company'"
        da = New SqlDataAdapter("sp_Big5ExhibitorList ", conn)

        Dim productTitle As String = ""
        Dim CompanyBooth As String = ""
        Dim orderClause As String = " ORDER By Company"

        da.SelectCommand.Parameters.Add(expoID)
        da.SelectCommand.Parameters.Add(productTitle)
        da.SelectCommand.Parameters.Add(CompanyBooth)
        da.SelectCommand.Parameters.Add(orderClause)
        da.Fill(ds)
        Return ds

    End Function


#End Region

End Class

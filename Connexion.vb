Imports MySql.Data.MySqlClient

Public Class Connexion
    Private _user As String
    Private _password As String
    Private _dbname As String
    Private _host As String
    Private _port As Integer
    Private _con As MySqlConnection

    Public Sub New(user As String, password As String, dbname As String, host As String, port As Integer)
        Me._user = user
        Me._password = password
        Me._dbname = dbname
        Me._host = host
        Me._port = port
        Me._con = Nothing
    End Sub

    Public Function Connecter() As Boolean
        Try
            Dim connString As String = $"Server={_host};Port={_port};Database={_dbname};Uid={_user};Pwd={_password};"
            _con = New MySqlConnection(connString)
            _con.Open()
            Console.WriteLine($"Connexion réussie à la base de données : {_dbname}")
            Return True
        Catch ex As MySqlException
            Console.WriteLine($"Erreur lors de la connexion : {ex.Message}")
            Return False
        End Try
    End Function

    Public Function ExecuteQuery(query As String) As List(Of List(Of String))
        Dim result As New List(Of List(Of String))
        Try
            If _con Is Nothing OrElse _con.State = ConnectionState.Closed Then
                Console.WriteLine("Connexion fermée ou non initialisée.")
                Return result
            End If

            Dim cmd As New MySqlCommand(query, _con)
            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                Dim row As New List(Of String)
                For i As Integer = 0 To reader.FieldCount - 1
                    If reader.IsDBNull(i) Then
                        row.Add("NULL")
                    Else
                        row.Add(reader(i).ToString())
                    End If
                Next
                result.Add(row)
            End While

            reader.Close()
            Return result
        Catch ex As MySqlException
            Console.WriteLine($"Erreur lors de l'exécution de la requête : {ex.Message}")
            Return result
        End Try
    End Function


    Public Function ExecuteUpdate(query As String) As Boolean
        Try
            If _con Is Nothing OrElse _con.State = ConnectionState.Closed Then
                Console.WriteLine("Connexion fermée ou non initialisée.")
                Return False
            End If

            Dim cmd As New MySqlCommand(query, _con)
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As MySqlException
            Console.WriteLine($"Erreur lors de l'exécution de l'update : {ex.Message}")
            Return False
        End Try
    End Function

    Public Sub CloseConnection()
        If _con IsNot Nothing AndAlso _con.State <> ConnectionState.Closed Then
            _con.Close()
            _con = Nothing
            Console.WriteLine("Connexion fermée.")
        End If
    End Sub

    Public Function GetConnection() As MySqlConnection
        If _con Is Nothing OrElse _con.State = ConnectionState.Closed Then
            Connecter() ' Ensure the connection is open
        End If
        Return _con
    End Function
End Class
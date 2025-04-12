Imports MySql.Data.MySqlClient

Public Class Acceleration_Log
    Private _id As Integer
    Private _voitureId As Integer
    Private _vitesseInitiale As Double
    Private _acceleration As Double
    Private _heureAcceleration As String

    ' Constructeur
    Public Sub New(id As Integer, voitureId As Integer, vitesseInitiale As Double, acceleration As Double, heureAcceleration As String)
        Me._id = id
        Me._voitureId = voitureId
        Me._vitesseInitiale = vitesseInitiale
        Me._acceleration = acceleration
        Me._heureAcceleration = heureAcceleration
    End Sub

    ' Créer un nouvel enregistrement dans la base de données
    Public Shared Sub Create(conn As Connexion, voitureId As Integer, vitesse As Double, acc As Double, heure As String)
        Try
            ' Vérifier si la connexion est ouverte
            Dim connection As MySqlConnection = conn.GetConnection()
            If connection Is Nothing OrElse connection.State = ConnectionState.Closed Then
                MessageBox.Show("La connexion à la base de données est fermée !")
                Console.WriteLine("Erreur : Connexion à la base de données fermée.")
                Return
            End If

            ' Utiliser des paramètres pour éviter les problèmes de format
            Dim query As String = "INSERT INTO acceleration_log (voiture_id, vitesseInitiale, acceleration, heure_acceleration) VALUES (@voitureId, @vitesse, @acceleration, @heure)"
            Using cmd As New MySqlCommand(query, connection)
                cmd.Parameters.AddWithValue("@voitureId", voitureId)
                cmd.Parameters.AddWithValue("@vitesse", vitesse)
                cmd.Parameters.AddWithValue("@acceleration", acc)
                cmd.Parameters.AddWithValue("@heure", heure)
                cmd.ExecuteNonQuery()
            End Using

            Console.WriteLine("Historique d'accélération créé avec succès !")
        Catch ex As MySqlException
            MessageBox.Show($"Erreur SQL lors de l'insertion : {ex.Message}")
            Console.WriteLine($"Erreur SQL lors de l'insertion : {ex.ToString()}")
        Catch ex As Exception
            MessageBox.Show($"Erreur générale lors de l'insertion : {ex.Message}")
            Console.WriteLine($"Erreur générale lors de l'insertion : {ex.ToString()}")
        End Try
    End Sub

    ' Récupérer toutes les actions pour une voiture
    Public Shared Function GetAllActionForVoiture(conn As Connexion, voiture As Voiture) As List(Of Acceleration_Log)
        Dim all As New List(Of Acceleration_Log)
        Dim query As String = $"SELECT * FROM acceleration_log WHERE voiture_id = {voiture.Id}"
        Dim result As List(Of List(Of String)) = conn.ExecuteQuery(query)

        For Each row In result
            Dim a As New Acceleration_Log(
                Integer.Parse(row(0)),
                Integer.Parse(row(1)),
                Double.Parse(row(2)),
                Double.Parse(row(3)),
                row(4)
            )
            all.Add(a)
        Next

        Return all
    End Function

    ' Calculer la consommation moyenne
    ' Calculer la consommation moyenne


    Public Shared Function CalculerConsoMoyenne(conn As Connexion, voiture As Voiture) As Double
        Dim historique As List(Of Acceleration_Log) = GetAllActionForVoiture(conn, voiture)
        If historique.Count < 2 Then
            Console.WriteLine("Données insuffisantes pour calculer la consommation.")
            Return 0.0
        End If

        Dim consoTotale As Double = 0.0
        Dim distanceTotale As Double = 0.0
        Dim niveauEssenceInitial As Double = voiture.NiveauEssence

        ' Calcul de la consommation et de la distance pour chaque segment
        For i As Integer = 0 To historique.Count - 2
            Dim current As Acceleration_Log = historique(i)
            Dim nextLog As Acceleration_Log = historique(i + 1)

            ' Parse time strings (assuming format HH:mm:ss.fff)
            Dim time1 As TimeSpan
            Dim time2 As TimeSpan
            Try
                time1 = TimeSpan.Parse(current.HeureAcceleration)
                time2 = TimeSpan.Parse(nextLog.HeureAcceleration)
            Catch ex As FormatException
                Console.WriteLine($"Erreur de format d'heure pour {current.HeureAcceleration} ou {nextLog.HeureAcceleration}")
                Continue For
            End Try

            ' Calculate time difference in seconds
            Dim tempsSecondes As Double = (time2 - time1).TotalSeconds

            If tempsSecondes <= 0 Then
                Continue For ' Ignore invalid time differences
            End If

            ' Accélération du log courant
            Dim acceleration As Double = current.Acceleration

            ' Calcul de la distance parcourue pendant cette période
            Dim distance As Double = voiture.CalculDistanceParcouru(tempsSecondes, acceleration) / 1000.0 ' en km
            distanceTotale += distance

            ' Calcul de la consommation d'essence pour ce segment
            Dim pourcent As Double = voiture.GetFiryPourcent(acceleration)
            Dim essenceAvant As Double = voiture.NiveauEssence
            voiture.ConsommerEssence(tempsSecondes, pourcent)
            Dim essenceApres As Double = voiture.NiveauEssence
            Dim consommation As Double = essenceAvant - essenceApres
            consoTotale += consommation

            ' Affichage pour chaque segment
            Console.WriteLine($"Segment {i + 1} : {current.HeureAcceleration} à {nextLog.HeureAcceleration}")
            Console.WriteLine($"  Temps : {tempsSecondes} s")
            Console.WriteLine($"  Accélération : {acceleration} km/h/s")
            Console.WriteLine($"  Distance : {distance} km")
            Console.WriteLine($"  Conso : {consommation} L")
        Next

        ' Remettre le niveau d’essence initial
        voiture.NiveauEssence = niveauEssenceInitial

        ' Si la distance totale est trop faible, retourner 0.0
        If distanceTotale <= 0.0001 Then
            Console.WriteLine("Distance trop faible pour un calcul fiable.")
            Return 0.0
        End If

        ' Calcul de la consommation moyenne
        Dim consoMoyenne As Double = (consoTotale / distanceTotale) * 100.0

        ' Affichage final
        Console.WriteLine($"Consommation totale : {consoTotale} L")
        Console.WriteLine($"Distance totale : {distanceTotale} km")
        Console.WriteLine($"Consommation moyenne : {consoMoyenne} L/100 km")

        Return consoMoyenne
    End Function

    Public Shared Function calculConso100km(v As Voiture) As Double
        If v Is Nothing Then
            Throw New ArgumentNullException("La voiture ne peut pas être nulle.")
        End If

        Dim essenceConsommee As Double = v.CapaciteReservoir - v.NiveauEssence
        Dim distance As Double = v.DistanceParcourue / 1000 ' Convertir mètres en kilomètres

        If distance <= 0 Then
            Return 0 ' Éviter la division par zéro
        End If

        Dim consoMoyenne As Double = (essenceConsommee / distance) * 100 ' Consommation en L/100km
        Return consoMoyenne
    End Function

    ' Propriétés
    Public Property Id As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property

    Public Property VoitureId As Integer
        Get
            Return _voitureId
        End Get
        Set(value As Integer)
            _voitureId = value
        End Set
    End Property

    Public Property VitesseInitiale As Double
        Get
            Return _vitesseInitiale
        End Get
        Set(value As Double)
            _vitesseInitiale = value
        End Set
    End Property

    Public Property Acceleration As Double
        Get
            Return _acceleration
        End Get
        Set(value As Double)
            _acceleration = value
        End Set
    End Property

    Public Property HeureAcceleration As String
        Get
            Return _heureAcceleration
        End Get
        Set(value As String)
            _heureAcceleration = value
        End Set
    End Property
End Class
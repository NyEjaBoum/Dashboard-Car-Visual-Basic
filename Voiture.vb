Imports MySql.Data.MySqlClient

Public Class Voiture
    Private _id As Integer
    Private _nom As String
    Private _accel As Double
    Private _dec As Double
    Private _vitesse As Double
    Private _capaciteReservoir As Double
    Private _niveauEssence As Double
    Private _consoMax As Double
    Private _distanceParcourue As Double

    ' Constructeur
    Public Sub New(id As Integer, nom As String, accel As Double, dec As Double, vitesse As Double, capaciteReservoir As Double, consoMax As Double)
        Me._id = id
        Me._nom = nom
        Me._accel = accel
        Me._dec = dec
        Me._vitesse = vitesse
        Me._capaciteReservoir = capaciteReservoir
        Me._niveauEssence = capaciteReservoir ' Réservoir plein au départ
        Me._consoMax = consoMax
        Me._distanceParcourue = 0.0
    End Sub

    ' Afficher les détails
    Public Sub AfficherDetails()
        Console.WriteLine($"Voiture ID : {Me._id}")
        Console.WriteLine($"Nom : {Me._nom}")
        Console.WriteLine($"Accélération : {Me._accel} m/s²")
        Console.WriteLine($"Décélération : {Me._dec} m/s²")
    End Sub

    ' Calcul de la distance parcourue
    Public Function CalculDistanceParcouru(temps As Double, accel As Double) As Double
        Dim accelMS2 As Double = accel * 0.27778 ' Conversion km/h/s en m/s²
        Dim vitesseMS As Double = Me._vitesse * 0.27778 ' Conversion km/h en m/s
        Dim distance As Double = (0.5 * accelMS2 * temps * temps) + (vitesseMS * temps)
        Me._distanceParcourue += distance
        'Me._vitesse += accel * temps ' Mise à jour de la vitesse en km/h
        Return DistanceParcourue
    End Function

    ' Consommation en pourcentage
    Public Function GetConsoPourcentage(pourcent As Double) As Double
        Return pourcent * Me._consoMax
    End Function

    ' Calcul du pourcentage d'accélération utilisé
    Public Function GetFiryPourcent(accel As Double) As Double
        Return (accel * 100) / Me._accel / 100
    End Function

    ' Consommer de l'essence
    Public Function ConsommerEssence(temps As Double, pourcent As Double) As Double
        Dim pourcentageConso As Double = GetConsoPourcentage(pourcent)
        Me._niveauEssence -= (pourcentageConso * temps)
        If Me._niveauEssence < 0 Then Me._niveauEssence = 0
        Return Me._niveauEssence
    End Function

    ' Accélérer
    Public Function Acceleration(temps As Double, pourcent As Double) As Double
        Console.WriteLine($"Son temps est {temps} s")
        Console.WriteLine($"Son pourcent est {pourcent} s")
        Console.WriteLine($"Son accel est {Me._accel} s")
        Dim vitesseInitiale As Double = Me._vitesse
        Console.WriteLine($"Son vitesse initial est {vitesseInitiale} s")
        Console.WriteLine($"fois est {Me._accel * pourcent} s")

        vitesseInitiale += (Me._accel * pourcent * temps)
        Me._vitesse = vitesseInitiale
        Console.WriteLine($"Son vitesse est {Me._vitesse} s")

        Console.WriteLine($"Son vitesse initial vaovao est {vitesseInitiale} s")

        Console.WriteLine($"Sa vitesse maintenant est {vitesseInitiale} km/h")
        Return vitesseInitiale
    End Function

    ' Décélérer
    Public Function Decceleration(temps As Double, pourcent As Double) As Double
        Dim vitesseInitiale As Double = Me._vitesse
        vitesseInitiale += (Me._dec * -1 * temps * pourcent)
        If vitesseInitiale < 0 Then vitesseInitiale = 0
        Me._vitesse = vitesseInitiale
        Console.WriteLine($"Sa vitesse maintenant est {vitesseInitiale} km/h")
        Return vitesseInitiale
    End Function

    ' Récupérer toutes les voitures depuis la base de données
    Public Shared Function GetAll(conn As Connexion) As List(Of Voiture)
        Dim voitures As New List(Of Voiture)
        Dim query As String = "SELECT * FROM voiture"
        Dim result As List(Of List(Of String)) = conn.ExecuteQuery(query)

        For Each row In result
            Dim v As New Voiture(
                Integer.Parse(row(0)),
                row(1),
                Double.Parse(row(2)),
                Double.Parse(row(3)),
                Double.Parse(row(4)),
                Double.Parse(row(5)),
                Double.Parse(row(6))
            )
            voitures.Add(v)
        Next

        Return voitures
    End Function

    ' Récupérer une voiture par ID
    Public Shared Function GetById(conn As Connexion, idVoiture As Integer) As Voiture
        Dim query As String = $"SELECT * FROM voiture WHERE id = {idVoiture}"
        Dim result As List(Of List(Of String)) = conn.ExecuteQuery(query)

        If result.Count = 0 Then Return Nothing

        Dim row = result(0)
        Return New Voiture(
            Integer.Parse(row(0)),
            row(1),
            Double.Parse(row(2)),
            Double.Parse(row(3)),
            Double.Parse(row(4)),
            Double.Parse(row(5)),
            Double.Parse(row(6))
        )
    End Function

    ' Propriétés (getters et setters)
    Public Property Id As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property

    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            _nom = value
        End Set
    End Property

    Public Property Accel As Double
        Get
            Return _accel
        End Get
        Set(value As Double)
            _accel = value
        End Set
    End Property

    Public Property Dec As Double
        Get
            Return _dec
        End Get
        Set(value As Double)
            _dec = value
        End Set
    End Property

    Public Property Vitesse As Double
        Get
            Return _vitesse
        End Get
        Set(value As Double)
            _vitesse = value
        End Set
    End Property

    Public Property CapaciteReservoir As Double
        Get
            Return _capaciteReservoir
        End Get
        Set(value As Double)
            _capaciteReservoir = value
        End Set
    End Property

    Public Property NiveauEssence As Double
        Get
            Return _niveauEssence
        End Get
        Set(value As Double)
            _niveauEssence = value
        End Set
    End Property

    Public Property ConsoMax As Double
        Get
            Return _consoMax
        End Get
        Set(value As Double)
            _consoMax = value
        End Set
    End Property

    Public Property DistanceParcourue As Double
        Get
            Return _distanceParcourue
        End Get
        Set(value As Double)
            _distanceParcourue = value
        End Set
    End Property
End Class
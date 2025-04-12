'Imports System.Windows.Forms

'Module program
'    Sub main()
'        ' connexion à la base de données
'        Dim conn As New Connexion("root", "", "voiture", "localhost", 3306)
'        If Not conn.Connecter() Then
'            Console.WriteLine("échec de la connexion à la base de données. fin du programme.")
'            Return
'        End If

'        ' récupération d'une voiture depuis la base de données (id = 6)
'        Dim voiture As Voiture = Voiture.GetById(conn, 1)
'        If voiture Is Nothing Then
'            Console.WriteLine("aucune voiture trouvée avec l'id 6.")
'            conn.CloseConnection()
'            Return
'        End If

'        ' tests des fonctionnalités de la classe voiture
'        voiture.Acceleration(10, 0.6)
'        Console.WriteLine($"consommation (10s, 50%) : {voiture.ConsommerEssence(10, 0.5)} l restant")

'        voiture.Acceleration(5, 0)
'        voiture.Decceleration(5, 0.2)

'        Console.WriteLine(voiture.Vitesse)
'        Dim test As Double = Acceleration_Log.calculConso100km(voiture)
'        Console.WriteLine("conso 100km " & test)



'        ' calcul de la consommation moyenne
'        'dim consomoyenne as double = acceleration_log.calculerconsomoyenne(conn, voiture)
'        'console.writeline($"la consommation moyenne de la voiture {voiture.id} est de {consomoyenne} l/100 km")

'        ' test de calcul de distance parcourue
'        'console.writeline($"distance parcourue (10s, 25 km/h/s) : {voiture.calculdistanceparcouru(10, 25)} m")
'        'console.writeline($"distance parcourue (5s, 0 km/h/s) : {voiture.calculdistanceparcouru(5, 0)} m")
'        ' console.writeline($"distance parcourue (5s, -4 km/h/s) : {voiture.calculdistanceparcouru(5, -4)} m")
'        ' console.writeline($"distance totale parcourue : {voiture.distanceparcourue} syou'distance total parcourue m")

'        ' test de consommation d'essence
'        Console.WriteLine($"niveau d'essence avant : {voiture.NiveauEssence} l")
'        Console.WriteLine($"niveau d'essence après : {voiture.NiveauEssence} l")
'        Dim essenceconsommer = (voiture.CapaciteReservoir) - voiture.NiveauEssence
'        Console.WriteLine($"essence consommer : {essenceconsommer} l")


'        'dim conso100km = (essenceconsommer / (voiture.distanceparcourue / 1000)) * 100
'        'console.writeline($"la consommation moyenne de la voiture {voiture.id} est de {conso100km} l/100 km")



'        ' commentaire : désactivation de l'interface graphique pour n'afficher que la console
'        ' application.run(new fenetre(800, 600, "vitesse voiture", conn))

'        ' fermeture de la connexion
'        conn.CloseConnection()

'        ' attendre une entrée utilisateur pour garder la console ouverte
'        Console.WriteLine("appuyez sur entrée pour quitter...")
'        Console.ReadLine()
'    End Sub
'End Module
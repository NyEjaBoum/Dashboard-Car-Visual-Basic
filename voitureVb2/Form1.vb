Imports MySql.Data.MySqlClient
Imports System.Threading
Imports System.Diagnostics
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar

Public Class Form1
    Private conn As Connexion
    Private voitures As List(Of Voiture)
    Public voituresSelectionne As Voiture

    ' Variables existantes
    Private spacePressed As Boolean = False
    Private controlPressed As Boolean = False
    Private isPressed(10) As Boolean
    Private isSelecting As Boolean = True
    Private selectedIndex As Integer = 0
    Private accelPourcent As Double = 0
    Private toucheNumeriquePressee As Boolean = False
    Private numericKeyWasPressed As Boolean = False
    Private currentNumericKey As String = "Aucune"
    Private lastKeyState As String = "Aucune touche"
    Private isPaused As Boolean = False

    ' Variables pour la gestion du temps (inspirées de MainPanel)
    Private elapsedTimer As Stopwatch ' Pour mesurer le temps total écoulé
    Private frameStopwatch As Stopwatch ' Pour mesurer le temps entre les frames
    Private lastUpdateTime As Long ' Dernier temps de mise à jour
    Private lastFpsUpdateTime As Long ' Dernier temps de calcul des FPS
    Private frameCount As Integer ' Compteur de frames
    Private currentFps As Integer ' FPS actuel 
    Private TARGET_FPS As Integer = 60 ' Cible FPS
    Private TARGET_FRAME_TIME_MS As Integer = 1000 \ TARGET_FPS ' Temps cible par frame

    ' Contrôles UI
    Private WithEvents vitesseLabel As Label
    Private WithEvents chronoLabel As Label
    Private WithEvents essenceLabel As Label
    Private WithEvents distanceLabel As Label
    Private WithEvents keyTimerLabel As Label
    Private WithEvents fpsLabel As Label ' Nouveau label pour afficher les FPS

    Private WithEvents renderTimer As System.Windows.Forms.Timer ' Timer de haute précision

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        Me.AcceptButton = Nothing
        Console.WriteLine("AcceptButton désactivé")
        conn = New Connexion("root", "", "voiture", "localhost", 3306)
        If Not conn.Connecter() Then
            MessageBox.Show("Échec de la connexion à la base de données.")
            Return
        End If

        Panel1.Visible = True
        Panel2.Visible = False

        InitializeSimulationControls()
        chargerVoituresDansComboBox()

        ' Initialisation des chronomètres de haute précision (de MainPanel)
        Me.elapsedTimer = New Stopwatch()
        Me.frameStopwatch = New Stopwatch()
        Me.elapsedTimer.Start()
        Me.frameStopwatch.Start()
        Me.lastUpdateTime = 0
        Me.lastFpsUpdateTime = 0
        Me.frameCount = 0
        Me.currentFps = 0

        ' Initialisation du timer de rendu haute performance (de MainPanel)
        renderTimer = New System.Windows.Forms.Timer() With {
            .Interval = TARGET_FRAME_TIME_MS ' Intervalle optimisé pour 60 FPS
        }
        AddHandler renderTimer.Tick, AddressOf OnRenderTick
    End Sub

    Private Sub InitializeSimulationControls()
        vitesseLabel = New Label()
        vitesseLabel.AutoSize = True
        vitesseLabel.Font = New Font("Arial", 12, FontStyle.Bold)
        vitesseLabel.Location = New Point(300, 30)
        Me.Panel2.Controls.Add(vitesseLabel)

        chronoLabel = New Label()
        chronoLabel.AutoSize = True
        chronoLabel.Font = New Font("Arial", 12, FontStyle.Regular)
        chronoLabel.Location = New Point(300, 60)
        Me.Panel2.Controls.Add(chronoLabel)

        essenceLabel = New Label()
        essenceLabel.AutoSize = True
        essenceLabel.Font = New Font("Arial", 12, FontStyle.Regular)
        essenceLabel.Location = New Point(300, 90)
        Me.Panel2.Controls.Add(essenceLabel)

        distanceLabel = New Label()
        distanceLabel.AutoSize = True
        distanceLabel.Font = New Font("Arial", 12, FontStyle.Regular)
        distanceLabel.Location = New Point(300, 120)
        Me.Panel2.Controls.Add(distanceLabel)

        keyTimerLabel = New Label()
        keyTimerLabel.AutoSize = True
        keyTimerLabel.Font = New Font("Arial", 12, FontStyle.Regular)
        keyTimerLabel.Location = New Point(300, 150)
        keyTimerLabel.Text = "Touche : Aucune - 0.00s"
        Me.Panel2.Controls.Add(keyTimerLabel)

        ' Ajout d'un label pour afficher les FPS
        fpsLabel = New Label()
        fpsLabel.AutoSize = True
        fpsLabel.Font = New Font("Arial", 10, FontStyle.Regular)
        fpsLabel.Location = New Point(300, 180)
        fpsLabel.Text = "FPS: 0"
        Me.Panel2.Controls.Add(fpsLabel)
    End Sub



    Private Sub chargerVoituresDansComboBox()
        voitures = Voiture.GetAll(conn)
        ComboBox1.Items.Clear()
        ComboBox1.DataSource = voitures
        ComboBox1.DisplayMember = "Nom"
        ComboBox1.ValueMember = "Id"
        If ComboBox1.Items.Count > 0 Then ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If renderTimer IsNot Nothing Then
            renderTimer.Stop()
            renderTimer.Dispose()
        End If
        If conn IsNot Nothing Then conn.CloseConnection()
    End Sub

    Private Sub choixVoiture(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedItem IsNot Nothing Then
            Dim selectedId As Integer = ComboBox1.SelectedValue
            voituresSelectionne = Voiture.GetById(conn, selectedId)

            If voituresSelectionne IsNot Nothing Then
                Panel2.Visible = True
                isSelecting = False

                ' Réinitialisation des chronomètres
                Me.elapsedTimer.Restart()
                Me.frameStopwatch.Restart()
                Me.lastUpdateTime = 0
                Me.lastFpsUpdateTime = 0
                Me.frameCount = 0

                renderTimer.Start() ' Démarrer le timer de rendu haute précision
                UpdateUI()

                MessageBox.Show($"Voiture sélectionnée : {voituresSelectionne.Nom}" & vbCrLf &
                              $"ID : {voituresSelectionne.Id}" & vbCrLf &
                              $"Vitesse : {voituresSelectionne.Vitesse} km/h" & vbCrLf &
                              $"Essence : {voituresSelectionne.NiveauEssence:F2}/{voituresSelectionne.CapaciteReservoir} L")
                Button1.Enabled = False
            Else
                MessageBox.Show("Aucune voiture trouvée avec cet ID.")
            End If
        Else
            MessageBox.Show("Veuillez sélectionner une voiture dans la liste.")
        End If
    End Sub

    Private Sub pictureBoxSpeed_Paint(sender As Object, e As PaintEventArgs) Handles pictureBoxSpeed.Paint
        Dim g = e.Graphics
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Dim centerX As Integer = pictureBoxSpeed.Width / 2
        Dim centerY As Integer = pictureBoxSpeed.Height / 2
        Dim radius = Math.Min(centerX, centerY) - 20

        Using brush As New Drawing2D.LinearGradientBrush(pictureBoxSpeed.ClientRectangle, Color.LightGray, Color.White, 45)
            g.FillEllipse(brush, centerX - radius, centerY - radius, radius * 2, radius * 2)
        End Using
        g.DrawEllipse(New Pen(Color.DarkGray, 2), centerX - radius, centerY - radius, radius * 2, radius * 2)

        For i = 0 To 240 Step 20
            Dim angle = i / 240 * 180 - 90
            Dim rad = angle * Math.PI / 180
            Dim x1 As Single = centerX + radius * 0.85 * Math.Cos(rad)
            Dim y1 As Single = centerY + radius * 0.85 * Math.Sin(rad)
            Dim x2 As Single = centerX + radius * Math.Cos(rad)
            Dim y2 As Single = centerY + radius * Math.Sin(rad)
            g.DrawLine(Pens.DarkGray, x1, y1, x2, y2)

            If i Mod 40 = 0 Then
                Dim textX As Single = centerX + radius * 1.1 * Math.Cos(rad) - 10
                Dim textY As Single = centerY + radius * 1.1 * Math.Sin(rad) - 5
                g.DrawString(i.ToString, New Font("Arial", 10, FontStyle.Bold), Brushes.Black, textX, textY)
            End If
        Next

        If voituresSelectionne IsNot Nothing Then
            Dim speed = voituresSelectionne.Vitesse
            If speed > 240 Then speed = 240
            If speed < 0 Then speed = 0

            Dim angle = speed / 240 * 180 - 90
            Dim rad = angle * Math.PI / 180
            Dim x As Single = centerX + radius * 0.8 * Math.Cos(rad)
            Dim y As Single = centerY + radius * 0.8 * Math.Sin(rad)
            g.DrawLine(New Pen(Color.Red, 3), centerX, centerY, x, y)
        End If
    End Sub

    Private Sub pictureBoxFuel_Paint(sender As Object, e As PaintEventArgs) Handles pictureBoxFuel.Paint
        Dim g = e.Graphics
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Dim width = pictureBoxFuel.Width - 40
        Dim height = pictureBoxFuel.Height - 20
        Dim x = 30
        Dim y = 10

        g.FillRectangle(Brushes.LightGray, x, y, width, height)
        g.DrawRectangle(New Pen(Color.DarkGray, 2), x, y, width, height)

        For i = 0 To 100 Step 25
            Dim yGrad As Single = y + height * (1 - i / 100)
            g.DrawLine(Pens.Black, x - 5, yGrad, x, yGrad)
            g.DrawString($"{i}%", New Font("Arial", 8), Brushes.Black, x - 25, yGrad - 5)
        Next

        g.DrawString("E", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, x - 25, y + height - 5)
        g.DrawString("F", New Font("Arial", 10, FontStyle.Bold), Brushes.Black, x - 25, y - 5)

        If voituresSelectionne IsNot Nothing Then
            Dim fuelPercent = voituresSelectionne.NiveauEssence / voituresSelectionne.CapaciteReservoir * 100
            If fuelPercent > 100 Then fuelPercent = 100
            If fuelPercent < 0 Then fuelPercent = 0

            Dim fillColor As Brush
            If fuelPercent > 50 Then
                fillColor = Brushes.Green
            ElseIf fuelPercent > 20 Then
                fillColor = Brushes.Yellow
            Else
                fillColor = Brushes.Red
            End If

            Dim fillHeight As Single = height * (fuelPercent / 100)
            Dim fillY = y + height - fillHeight
            g.FillRectangle(fillColor, x + 2, fillY, width - 4, fillHeight)
        End If
    End Sub

    ' Nouvelle méthode de rendu à haute précision basée sur MainPanel
    Private Sub OnRenderTick(sender As Object, e As EventArgs)
        ' Temps écoulé depuis la dernière frame en millisecondes
        Dim currentTime As Long = Me.elapsedTimer.ElapsedMilliseconds
        Dim elapsedTimeMs As Single = currentTime - lastUpdateTime
        lastUpdateTime = currentTime

        ' Calcul des FPS
        frameCount += 1
        If currentTime - lastFpsUpdateTime >= 1000 Then ' Mise à jour des FPS chaque seconde
            currentFps = frameCount
            frameCount = 0
            lastFpsUpdateTime = currentTime
        End If

        ' Convertir en secondes pour les calculs de physique
        Dim elapsedTimeSec As Single = elapsedTimeMs / 1000.0F



        If Not isPaused Then
            ' Mise à jour de la simulation avec le temps précis
            UpdateSimulation(elapsedTimeSec)
        End If

        ' Mettre à jour l'interface utilisateur
        UpdateUI()
    End Sub

    ' Nouvelle méthode de simulation avec temps précis (inspirée de MainPanel.UpdateSimulation)
    Private Sub UpdateSimulation(elapsedTimeSec As Single)
        If voituresSelectionne Is Nothing Then Return

        ' Conversion du temps en heures pour les calculs de distance
        Dim elapsedTimeHour As Single = elapsedTimeSec / 3600.0F
        Dim accelle As Double = 0

        ' Application des contrôles et calculs physiques

        If spacePressed AndAlso numericKeyWasPressed Then
            If voituresSelectionne.NiveauEssence > 0 Then
                accelle = voituresSelectionne.Accel * accelPourcent

                ' Utiliser directement le temps écoulé réel pour des calculs plus précis
                voituresSelectionne.Acceleration(elapsedTimeSec, accelPourcent)
                voituresSelectionne.ConsommerEssence(elapsedTimeSec, accelPourcent)
            End If
        End If

        If controlPressed AndAlso numericKeyWasPressed Then
            If voituresSelectionne.Vitesse > 0 AndAlso voituresSelectionne.NiveauEssence > 0 Then
                voituresSelectionne.Decceleration(elapsedTimeSec, accelPourcent)
            End If
        End If

        ' Calcul de la distance avec le temps précis
        voituresSelectionne.CalculDistanceParcouru(elapsedTimeSec, accelle)
    End Sub

    Private Sub UpdateUI()
        If voituresSelectionne IsNot Nothing Then
            vitesseLabel.Text = $"Vitesse : {voituresSelectionne.Vitesse:F2} km/h"

            Dim pressedDuration As Single = CSng(elapsedTimer.Elapsed.TotalSeconds)
            Dim minutes As Integer = CInt(pressedDuration) \ 60
            Dim seconds As Integer = CInt(pressedDuration) Mod 60
            chronoLabel.Text = $"Temps: {minutes}m {seconds}s"

            Dim niveau As Double = voituresSelectionne.NiveauEssence
            Dim capaciteMax As Double = voituresSelectionne.CapaciteReservoir
            essenceLabel.Text = $"Essence: {niveau:F2}L / {capaciteMax:F2}L"

            distanceLabel.Text = $"Distance: {voituresSelectionne.DistanceParcourue:F2} m"

            Dim keyTimeDuration As String = "0.00"
            If isKeyTimerRunning Then
                keyTimeDuration = keyTimer.Elapsed.TotalSeconds.ToString("F2")
            End If
            keyTimerLabel.Text = $"Touche : {lastKeyState} - {keyTimeDuration}s"

            ' Affichage des FPS
            fpsLabel.Text = $"FPS: {currentFps}"

            ' Mise à jour des composants graphiques
            pictureBoxSpeed.Invalidate()
            pictureBoxFuel.Invalidate()
        End If
    End Sub

    ' Variables pour le timer des touches (conservées du code original)
    Private keyTimer As New Stopwatch()
    Private isKeyTimerRunning As Boolean = False

    ' Gestion des événements clavier (identique à l'original)
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If isSelecting OrElse voituresSelectionne Is Nothing Then Return

        Dim now As DateTime = DateTime.Now
        Dim heure As String = $"{now:HH:mm:ss.fff}"

        ' Touche P pour mettre en pause (comme dans MainPanel)
        If e.KeyCode = Keys.P Then
            ButtonPause_Click(ButtonPause, New EventArgs())
            Return
        End If

        Dim touches() As Keys = {Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.A}
        Dim pourcentages() As Double = {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.0}

        If e.KeyCode = Keys.Space Then
            spacePressed = True
            If Not isKeyTimerRunning Then
                keyTimer.Restart()
                isKeyTimerRunning = True
                lastKeyState = "Space"
                Console.WriteLine($"Touche Space enfoncée à {heure}")
            End If
        ElseIf e.KeyCode = Keys.ControlKey OrElse e.KeyCode = Keys.LControlKey OrElse e.KeyCode = Keys.RControlKey Then
            controlPressed = True
            If Not isKeyTimerRunning Then
                keyTimer.Restart()
                isKeyTimerRunning = True
                lastKeyState = "Ctrl"
                Console.WriteLine($"Touche Ctrl enfoncée à {heure}")
            End If
        Else
            For i As Integer = 0 To 10
                If e.KeyCode = touches(i) Then
                    Dim keyName As String = If(i = 10, "A", (i + 1).ToString())
                    If keyName <> currentNumericKey Then
                        toucheNumeriquePressee = True
                        numericKeyWasPressed = True
                        accelPourcent = pourcentages(i)
                        isPressed(i) = True
                        currentNumericKey = keyName
                        Console.WriteLine($"Touche {keyName} enfoncée à {heure}")

                        If spacePressed Then
                            Acceleration_Log.Create(conn, voituresSelectionne.Id, voituresSelectionne.Vitesse, voituresSelectionne.Accel * accelPourcent, heure)
                            Console.WriteLine($"Insertion effectuée dans la base à {heure} avec accélération {voituresSelectionne.Accel * accelPourcent} (touche {keyName})")
                        End If

                        If controlPressed Then
                            Acceleration_Log.Create(conn, voituresSelectionne.Id, voituresSelectionne.Vitesse, -voituresSelectionne.Dec * accelPourcent, heure)
                        End If
                    End If
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If isSelecting OrElse voituresSelectionne Is Nothing Then Return

        Dim now As DateTime = DateTime.Now
        Dim heure As String = $"{now:HH:mm:ss.fff}"
        Dim touches() As Keys = {Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.A}

        If e.KeyCode = Keys.Space Then
            If spacePressed AndAlso numericKeyWasPressed Then
                Acceleration_Log.Create(conn, voituresSelectionne.Id, voituresSelectionne.Vitesse, 0, heure)
                Console.WriteLine($"Insertion effectuée dans la base à {heure} avec accélération 0 (relâchement)")
            End If

            spacePressed = False
            toucheNumeriquePressee = False
            numericKeyWasPressed = False
            currentNumericKey = "Aucune"

            If isKeyTimerRunning Then
                keyTimer.Stop()
                isKeyTimerRunning = False
                Dim duration As Single = CSng(keyTimer.Elapsed.TotalSeconds)
                Console.WriteLine($"Touche Space relâchée à {heure} - Durée: {duration:F2}s")
                lastKeyState = "Aucune"
            End If
        End If

        If e.KeyCode = Keys.ControlKey OrElse e.KeyCode = Keys.LControlKey OrElse e.KeyCode = Keys.RControlKey Then
            If controlPressed AndAlso numericKeyWasPressed Then
                Acceleration_Log.Create(conn, voituresSelectionne.Id, voituresSelectionne.Vitesse, 0, heure)
                Console.WriteLine($"Insertion effectuée dans la base à {heure} avec accélération 0 (relâchement)")
            End If

            controlPressed = False
            toucheNumeriquePressee = False
            numericKeyWasPressed = False
            currentNumericKey = "Aucune"

            If isKeyTimerRunning Then
                keyTimer.Stop()
                isKeyTimerRunning = False
                Dim duration As Single = CSng(keyTimer.Elapsed.TotalSeconds)
                Console.WriteLine($"Touche Ctrl relâchée à {heure} - Durée: {duration:F2}s")
                lastKeyState = "Aucune"
            End If
        End If

        For i As Integer = 0 To 10
            If e.KeyCode = touches(i) Then
                isPressed(i) = False
                Dim keyName As String = If(i = 10, "A", (i + 1).ToString())
                If keyName = currentNumericKey Then
                    toucheNumeriquePressee = False
                    currentNumericKey = "Aucune"
                    Console.WriteLine($"Touche {keyName} relâchée à {heure}")
                End If
                Exit For
            End If
        Next
    End Sub

    ' Ajout d'une méthode de redimensionnement des contrôles (comme dans MainPanel)
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ' Mise à jour des positions des contrôles si nécessaire
    End Sub

    Private Sub Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Panel2.Paint
        ' Code de dessin personnalisé si nécessaire
    End Sub


    Private Sub ButtonPause_Click(sender As Object, e As EventArgs) Handles ButtonPause.Click
        isPaused = Not isPaused
        ButtonPause.Text = If(isPaused, "Reprendre", "Pause")

        If isPaused Then
            ' Arrêter tous les timers
            If renderTimer IsNot Nothing Then renderTimer.Stop()
            If elapsedTimer IsNot Nothing Then elapsedTimer.Stop()
            If frameStopwatch IsNot Nothing Then frameStopwatch.Stop()
            If keyTimer IsNot Nothing Then keyTimer.Stop()

            ' Afficher la consommation moyenne
            If voituresSelectionne IsNot Nothing Then
                Try
                    Dim consoMoyenne As Double = Acceleration_Log.calculConso100km(voituresSelectionne)
                    MessageBox.Show($"Consommation moyenne : {consoMoyenne:F2} L/100km", "Consommation", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Erreur lors du calcul de la consommation : " & ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        Else
            ' Redémarrer les timers
            If renderTimer IsNot Nothing Then renderTimer.Start()
            If elapsedTimer IsNot Nothing Then elapsedTimer.Start()
            If frameStopwatch IsNot Nothing Then frameStopwatch.Start()
            ' Ne pas redémarrer keyTimer ici, car il est géré par les événements clavier
        End If
    End Sub
End Class
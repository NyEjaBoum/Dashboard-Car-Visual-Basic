<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Panel1 = New Panel()
        Panel2 = New Panel()
        ButtonPause = New Button()
        pictureBoxFuel = New PictureBox()
        pictureBoxSpeed = New PictureBox()
        Button1 = New Button()
        ComboBox1 = New ComboBox()
        Label1 = New Label()
        Panel1.SuspendLayout()
        Panel2.SuspendLayout()
        CType(pictureBoxFuel, ComponentModel.ISupportInitialize).BeginInit()
        CType(pictureBoxSpeed, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(Panel2)
        Panel1.Controls.Add(Button1)
        Panel1.Controls.Add(ComboBox1)
        Panel1.Controls.Add(Label1)
        Panel1.Dock = DockStyle.Fill
        Panel1.Location = New Point(0, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(860, 550)
        Panel1.TabIndex = 0
        ' 
        ' Panel2
        ' 
        Panel2.Controls.Add(ButtonPause)
        Panel2.Controls.Add(pictureBoxFuel)
        Panel2.Controls.Add(pictureBoxSpeed)
        Panel2.Dock = DockStyle.Fill
        Panel2.Location = New Point(0, 0)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(860, 550)
        Panel2.TabIndex = 4
        ' 
        ' ButtonPause
        ' 
        ButtonPause.Location = New Point(723, 48)
        ButtonPause.Name = "ButtonPause"
        ButtonPause.Size = New Size(75, 23)
        ButtonPause.TabIndex = 2
        ButtonPause.Text = "Button2"
        ButtonPause.UseVisualStyleBackColor = True
        ' 
        ' pictureBoxFuel
        ' 
        pictureBoxFuel.Location = New Point(547, 120)
        pictureBoxFuel.Name = "pictureBoxFuel"
        pictureBoxFuel.Size = New Size(100, 150)
        pictureBoxFuel.TabIndex = 1
        pictureBoxFuel.TabStop = False
        ' 
        ' pictureBoxSpeed
        ' 
        pictureBoxSpeed.Location = New Point(12, 120)
        pictureBoxSpeed.Name = "pictureBoxSpeed"
        pictureBoxSpeed.Size = New Size(233, 172)
        pictureBoxSpeed.TabIndex = 0
        pictureBoxSpeed.TabStop = False
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(343, 183)
        Button1.Name = "Button1"
        Button1.Size = New Size(75, 23)
        Button1.TabIndex = 3
        Button1.Text = "Button1"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' ComboBox1
        ' 
        ComboBox1.FormattingEnabled = True
        ComboBox1.Location = New Point(326, 120)
        ComboBox1.Name = "ComboBox1"
        ComboBox1.Size = New Size(121, 23)
        ComboBox1.TabIndex = 1
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(316, 52)
        Label1.Name = "Label1"
        Label1.Size = New Size(131, 15)
        Label1.TabIndex = 0
        Label1.Text = "Choisissez votre voiture"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(860, 550)
        Controls.Add(Panel1)
        Name = "Form1"
        Text = "Form1"
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        Panel2.ResumeLayout(False)
        CType(pictureBoxFuel, ComponentModel.ISupportInitialize).EndInit()
        CType(pictureBoxSpeed, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents pictureBoxFuel As PictureBox
    Friend WithEvents pictureBoxSpeed As PictureBox
    Friend WithEvents ButtonPause As Button
    Friend WithEvents Replay As Button

End Class

Imports System.Net.Http
Imports System.Security
Imports System.Text
Imports System.Text.Json
Imports System.Threading

Module Program

    Private HamsterBearers As New List(Of HamsterBearer)
    Private HamsterMiniGames As New List(Of String)

    Sub Main()
        HamsterBearers.Add(New HamsterBearer With {.Index = 0, .Name = "Acc1", .Auth = "Bearer1", .Cipher = False, .Task = True, .Combo = False, .Upgrade = True, .MiniGame = True})
        HamsterBearers.Add(New HamsterBearer With {.Index = 1, .Name = "Acc2", .Auth = "Bearer2", .Cipher = False, .Task = True, .Combo = False, .Upgrade = True, .MiniGame = True})
        HamsterBearers.Add(New HamsterBearer With {.Index = 2, .Name = "Acc3", .Auth = "Bearer3", .Cipher = False, .Task = True, .Combo = False, .Upgrade = True, .MiniGame = True})
        HamsterBearers.Add(New HamsterBearer With {.Index = 3, .Name = "Acc4", .Auth = "Bearer4", .Cipher = False, .Task = True, .Combo = False, .Upgrade = True, .MiniGame = True})

        HamsterMiniGames.Add("Candles")
        HamsterMiniGames.Add("Tiles")

        Console.WriteLine("--------------- Hamster Kombat Bot Starting ---------------")
        For Each Bearer In HamsterBearers
            Dim MessageThread As New Thread(Sub() BearerThread(Bearer))
            MessageThread.Start()

            Thread.Sleep(60000)
        Next
        Console.ReadLine()
    End Sub

    Public Async Sub BearerThread(Bearer As HamsterBearer)
        While True
            Dim RND As New Random()

            If Bearer.Task Then
                Dim taskList = Await GetTaskListAsync(Bearer)
                If taskList IsNot Nothing Then
                    Dim nonCompletedTasks = taskList.Tasks.Where(Function(x) x.IsCompleted = False And x.Id.Contains("invite") = False).ToList()
                    If nonCompletedTasks.Count <> 0 Then
                        For Each task In nonCompletedTasks
                            Dim completedTask = Await CheckTaskAsync(Bearer, task.Id)
                            If completedTask Then
                                Log.Show($"[{Bearer.Name}] task '{task.Id}' completed", ConsoleColor.Green)
                            Else
                                Log.Show($"[{Bearer.Name}] task '{task.Id}' failed", ConsoleColor.Red)
                            End If
                            Dim eachtaskRND As Integer = RND.Next(7, 20)
                            Thread.Sleep(eachtaskRND * 1000)
                        Next
                    End If
                End If
            End If

            If Bearer.MiniGame Then
                For Each minigame In HamsterMiniGames
                    Dim startMinigame = Await StartKeysMinigameAsync(Bearer, minigame)
                    If startMinigame IsNot Nothing Then
                        If Not startMinigame.DailyKeysMiniGames.IsClaimed Then
                            Thread.Sleep(25000)
                            Dim cipher As String = GetMiniGameCipher(startMinigame.InterludeUser.Id, startMinigame.DailyKeysMiniGames.StartDate, minigame, startMinigame.DailyKeysMiniGames.RemainPoints)
                            Dim claimMinigame = Await ClaimDailyKeysMiniGame(Bearer, cipher, minigame)
                            If claimMinigame Then
                                Log.Show($"[{Bearer.Name}] {minigame} minigame claimed", ConsoleColor.Green)
                            Else
                                Log.Show($"[{Bearer.Name}] {minigame} minigame failed", ConsoleColor.Red)
                            End If
                        End If
                    End If

                    Thread.Sleep(5000)
                Next
            End If

            Dim Sync = Await SyncAsync(Bearer)
            If Sync IsNot Nothing Then
                Log.Show($"[{Bearer.Name}] synced successfully", ConsoleColor.Green)
            Else
                Log.Show($"[{Bearer.Name}] synced failed", ConsoleColor.Red)
            End If

            If Bearer.Upgrade Then
                Dim upgradeList = Await GetUpgradeListAsync(Bearer)
                If upgradeList IsNot Nothing Then
                    Dim needUpgrade = upgradeList.UpgradesForBuy.Where(Function(x) x.IsAvailable = True And x.IsExpired = False And (x.ProfitPerHourDelta * IIf(Sync.InterludeUser.BalanceDiamonds > 500, 5000, IIf(Sync.InterludeUser.BalanceDiamonds > 100, 1000, 500))) >= x.Price And x.CoolDownSeconds = 0).ToList()
                    For Each upgrade In needUpgrade
                        Dim completedUpgrade = Await UpgradeAsync(Bearer, upgrade.Id)
                        If completedUpgrade Then
                            Log.Show($"[{Bearer.Name}] card '{upgrade.Id}' upgraded", ConsoleColor.Green)
                        Else
                            Log.Show($"[{Bearer.Name}] card '{upgrade.Id}' upgrade failed", ConsoleColor.Red)
                        End If
                        Dim eachupgradeRND As Integer = RND.Next(3, 8)
                        Thread.Sleep(eachupgradeRND * 1000)
                    Next
                End If
            End If

            Dim syncRND As Integer = RND.Next(6000, 9000)
            Log.Show($"[{Bearer.Name}] sync sleep '{Int(syncRND / 60)}m {syncRND Mod 60}s'", ConsoleColor.Yellow)
            Thread.Sleep(syncRND * 1000)
        End While
    End Sub

    Public Async Function SyncAsync(Bearer As HamsterBearer) As Task(Of HamsterSyncResponse)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/sync", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of HamsterSyncResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function ConfigAsync(Bearer As HamsterBearer) As Task(Of HamsterConfigResponse)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/config", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of HamsterConfigResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Function Decode(ByVal encoded As String) As String
        Dim mixed = $"{encoded.Substring(0, 3)}{encoded.Substring(4)}"
        Dim base64Bytes = Convert.FromBase64String(mixed)
        Dim decoded = Encoding.UTF8.GetString(base64Bytes)
        Return decoded
    End Function

    Public Async Function ClaimDailyCipher(Bearer As HamsterBearer, request As HamsterCipherRequest) As Task(Of Boolean)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/claim-daily-cipher", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of HamsterConfigResponse)(responseStream)
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function ClaimDailyCipher(Bearer As HamsterBearer, cipher As String) As Task(Of Boolean)
        Return ClaimDailyCipher(Bearer, New HamsterCipherRequest With {.Cipher = cipher})
    End Function

    Public Async Function GetTaskListAsync(Bearer As HamsterBearer) As Task(Of HamsterTaskListResponse)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/list-tasks", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of HamsterTaskListResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function CheckTaskAsync(Bearer As HamsterBearer, request As HamsterCheckTaskRequest) As Task(Of Boolean)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/check-task", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function CheckTaskAsync(Bearer As HamsterBearer, taskId As String) As Task(Of Boolean)
        Return CheckTaskAsync(Bearer, New HamsterCheckTaskRequest With {.TaskId = taskId})
    End Function

    Public Async Function GetUpgradeListAsync(Bearer As HamsterBearer) As Task(Of HamsterUpgradeListResponse)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/upgrades-for-buy", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of HamsterUpgradeListResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function UpgradeAsync(Bearer As HamsterBearer, request As HamsterUpgradeRequest) As Task(Of Boolean)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/buy-upgrade", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function UpgradeAsync(Bearer As HamsterBearer, upgradeId As String) As Task(Of Boolean)
        Return UpgradeAsync(Bearer, New HamsterUpgradeRequest With {.TimeStamp = CLng(Date.Now.ToString("yyyyMMddHHmmssffff")), .UpgradeId = upgradeId})
    End Function

    Public Async Function StartKeysMinigameAsync(Bearer As HamsterBearer, request As HamsterStartKeyMinigameRequest) As Task(Of HamsterStartKeyMinigame)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/start-keys-minigame", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of HamsterStartKeyMinigame)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Function StartKeysMinigameAsync(Bearer As HamsterBearer, miniGameId As String) As Task(Of HamsterStartKeyMinigame)
        Return StartKeysMinigameAsync(Bearer, New HamsterStartKeyMinigameRequest With {.MiniGameId = miniGameId})
    End Function

    Public Async Function ClaimDailyKeysMiniGame(Bearer As HamsterBearer, request As HamsterClaimDailyKeysMiniGameRequest) As Task(Of Boolean)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/claim-daily-keys-minigame", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function ClaimDailyKeysMiniGame(Bearer As HamsterBearer, cipher As String, miniGameId As String) As Task(Of Boolean)
        Return ClaimDailyKeysMiniGame(Bearer, New HamsterClaimDailyKeysMiniGameRequest With {.Cipher = cipher, .MiniGameId = miniGameId})
    End Function

    Public Function GetGameCipher(startNumber As Long) As String
        Dim magicIndex As Integer = CInt((startNumber Mod (startNumber.ToString().Length - 2)))
        Dim random = New Random()
        Dim res = New StringBuilder()

        For i = 0 To startNumber.ToString().Length - 1
            res.Append(If(i = magicIndex, "0"c, random.Next(10).ToString()))
        Next

        Return res.ToString()
    End Function

    Public Function GetMiniGameCipher(userId As Long, startDate As String, miniGameId As String, score As Long) As String
        Dim secret1 = "R1cHard_AnA1"
        Dim secret2 = "G1ve_Me_y0u7_Pa55w0rD"

        Dim startDt As Date = Date.Parse(startDate).ToUniversalTime()
        Dim startNumber As Long = CType(startDt, DateTimeOffset).ToUnixTimeSeconds()
        Dim cipherScore = (startNumber + score) * 2

        Dim combinedString = $"{secret1}{cipherScore}{secret2}"

        Using sha256 = Cryptography.SHA256.Create()
            Dim hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString))
            Dim sig = Convert.ToBase64String(hashBytes)

            Dim gameCipher = GetGameCipher(startNumber)

            Dim data = $"{gameCipher}|{userId}|{miniGameId}|{cipherScore}|{sig}"

            Return Convert.ToBase64String(Encoding.UTF8.GetBytes(data))
        End Using
    End Function

    Public Async Function GetComboListAsync() As Task(Of DataVibeGetCombo)
        Dim client As New HttpClient With {
            .Timeout = New TimeSpan(0, 0, 30)
        }
        Dim httpResponse = Await client.GetAsync("https://api21.datavibe.top/api/GetCombo")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of DataVibeGetCombo)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function ClaimDailyComboAsync(Bearer As HamsterBearer) As Task(Of Boolean)
        Dim HAPI As New HamsterApi(Bearer.Auth, Bearer.Index)
        Dim httpResponse = Await HAPI.HAPIPost("https://api.hamsterkombatgame.io/interlude/claim-daily-combo", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function UnixToDateTime(ByVal strUnixTime As Double) As DateTime
        Dim nDateTime As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0)
        nDateTime = nDateTime.AddSeconds(strUnixTime)
        Return nDateTime.ToLocalTime
    End Function
End Module

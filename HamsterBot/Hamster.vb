Imports System.Text.Json.Serialization

Public Class HamsterBearer
    Public Property Index As Integer
    Public Property Name As String
    Public Property Auth As String
    Public Property Cipher As Boolean
    Public Property Task As Boolean
    Public Property Combo As Boolean
    Public Property Upgrade As Boolean
    Public Property MiniGame As Boolean
End Class

Public Class HamsterSyncResponse
    <JsonPropertyName("interludeUser")>
    Public Property InterludeUser As HamsterInterludeUser
End Class

Public Class HamsterInterludeUser
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("totalDiamonds")>
    Public Property TotalDiamonds As Double
    <JsonPropertyName("balanceDiamonds")>
    Public Property BalanceDiamonds As Double
    <JsonPropertyName("earnPassivePerSec")>
    Public Property EarnPassivePerSec As Double
    <JsonPropertyName("earnPassivePerHour")>
    Public Property EarnPassivePerHour As Double
    <JsonPropertyName("lastPassiveEarn")>
    Public Property LastPassiveEarn As Double
    <JsonPropertyName("lastSyncUpdate")>
    Public Property LastSyncUpdate As Long
    <JsonPropertyName("exchangeId")>
    Public Property ExchangeId As String
    <JsonPropertyName("referralsCount")>
    Public Property ReferralsCount As Integer
End Class

Public Class HamsterConfigResponse
    <JsonPropertyName("dailyCipher")>
    Public Property DailyCipher As HamsterDailyCipherConfig
End Class

Public Class HamsterDailyCipherConfig
    <JsonPropertyName("cipher")>
    Public Property Cipher As String
    <JsonPropertyName("bonusCoins")>
    Public Property BonusCoins As Integer
    <JsonPropertyName("isClaimed")>
    Public Property IsClaimed As Boolean
    <JsonPropertyName("remainSeconds")>
    Public Property RemainSeconds As Integer
End Class

Public Class HamsterCipherRequest
    <JsonPropertyName("cipher")>
    Public Property Cipher As String
End Class

Public Class HamsterTaskListResponse
    <JsonPropertyName("tasks")>
    Public Property Tasks As List(Of HamsterTask)
End Class

Public Class HamsterTask
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("rewardCoins")>
    Public Property RewardCoins As Integer
    <JsonPropertyName("periodicity")>
    Public Property Periodicity As String
    <JsonPropertyName("link")>
    Public Property Link As String
    <JsonPropertyName("isCompleted")>
    Public Property IsCompleted As Boolean
    <JsonPropertyName("completedAt")>
    Public Property CompletedAt As DateTime
    <JsonPropertyName("channelId")>
    Public Property ChannelId As Long
    <JsonPropertyName("days")>
    Public Property Days As Integer
    <JsonPropertyName("remainSeconds")>
    Public Property RemainSeconds As Integer
End Class

Public Class HamsterCheckTaskRequest
    <JsonPropertyName("taskId")>
    Public Property TaskId As String
End Class

Public Class HamsterUpgradeListResponse
    <JsonPropertyName("upgradesForBuy")>
    Public Property UpgradesForBuy As List(Of HamsterUpgradeForBuy)
End Class

Public Class HamsterUpgradeForBuy
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("name")>
    Public Property Name As String
    <JsonPropertyName("cooldownSeconds")>
    Public Property CoolDownSeconds As Integer
    <JsonPropertyName("price")>
    Public Property Price As Double
    <JsonPropertyName("profitPerHourDelta")>
    Public Property ProfitPerHourDelta As Double
    <JsonPropertyName("isAvailable")>
    Public Property IsAvailable As Boolean
    <JsonPropertyName("isExpired")>
    Public Property IsExpired As Boolean
End Class

Public Class HamsterDailyCombo
    <JsonPropertyName("upgradeIds")>
    Public Property UpgradeIds As List(Of String)
    <JsonPropertyName("isClaimed")>
    Public Property IsClaimed As Boolean
End Class

Public Class HamsterUpgradeRequest
    <JsonPropertyName("timestamp")>
    Public Property TimeStamp As Long
    <JsonPropertyName("upgradeId")>
    Public Property UpgradeId As String
End Class

Public Class HamsterStartKeyMinigameRequest
    <JsonPropertyName("miniGameId")>
    Public Property MiniGameId As String
End Class

Public Class HamsterStartKeyMinigame
    <JsonPropertyName("interludeUser")>
    Public Property InterludeUser As HamsterInterludeUser
    <JsonPropertyName("dailyKeysMiniGames")>
    Public Property DailyKeysMiniGames As HamsterDailyKeysMiniGames
End Class

Public Class HamsterDailyKeysMiniGames
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("startDate")>
    Public Property StartDate As String
    <JsonPropertyName("isClaimed")>
    Public Property IsClaimed As Boolean
    <JsonPropertyName("maxPoints")>
    Public Property MaxPoints As Integer
    <JsonPropertyName("remainPoints")>
    Public Property RemainPoints As Integer
End Class

Public Class HamsterClaimDailyKeysMiniGameRequest
    <JsonPropertyName("cipher")>
    Public Property Cipher As String
    <JsonPropertyName("miniGameId")>
    Public Property MiniGameId As String
End Class

Public Class DataVibeGetCombo
    <JsonPropertyName("combo")>
    Public Property Combo As List(Of String)
    <JsonPropertyName("expires")>
    Public Property Expires As Double
End Class
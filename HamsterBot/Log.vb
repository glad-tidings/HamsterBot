Public Class Log
    Public Shared Sub Show(ByVal Message As String, ByVal Color As ConsoleColor)
        Console.ForegroundColor = ConsoleColor.White
        Console.Write($"[{Date.Now.ToString("yyyy-MM-dd HH:mm:ss")}] ")
        Console.ResetColor()
        Console.ForegroundColor = Color
        Console.WriteLine(Message)
        Console.ResetColor()
    End Sub
End Class

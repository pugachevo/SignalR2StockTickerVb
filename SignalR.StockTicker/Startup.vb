Imports System
Imports System.Threading.Tasks
Imports Microsoft.Owin
Imports Owin

<Assembly: OwinStartup(GetType(SignalR.StockTicker.Startup))>
Namespace SignalR.StockTicker
    Public Class Startup
        Public Sub Configuration(ByVal app As IAppBuilder)
            app.MapSignalR()
        End Sub
    End Class
End Namespace
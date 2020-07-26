Imports System.Collections.Generic
Imports Microsoft.AspNet.SignalR
Imports Microsoft.AspNet.SignalR.Hubs


<HubName("stockTickerMini")>
    Public Class StockTickerHub
        Inherits Hub

        Private ReadOnly _stockTicker As StockTicker

        Public Sub New()
            Me.New(StockTicker.Instance)
        End Sub

        Public Sub New(ByVal stockTicker As StockTicker)
            _stockTicker = stockTicker
        End Sub

        Public Function GetAllStocks() As IEnumerable(Of Stock)
            Return _stockTicker.GetAllStocks()
        End Function
    End Class



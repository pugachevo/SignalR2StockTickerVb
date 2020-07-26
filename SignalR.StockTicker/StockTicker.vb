Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports Microsoft.AspNet.SignalR
Imports Microsoft.AspNet.SignalR.Hubs
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI.WebControls
Imports System.Net


Public Class StockTicker

        ' Singleton instance
        Private Shared ReadOnly _instance As Lazy(Of StockTicker) =
            New Lazy(Of StockTicker)(Function() New StockTicker(GlobalHost.ConnectionManager.GetHubContext(Of StockTickerHub)().Clients))

        Private _stocks As ConcurrentDictionary(Of String, Stock) = New ConcurrentDictionary(Of String, Stock)

        Private _updateStockPricesLock As Object = New Object

        'stock can go up or down by a percentage of this factor on each change
        Private _rangePercent As Double

        Private _updateInterval As TimeSpan = TimeSpan.FromMilliseconds(250)

        Private _updateOrNotRandom As Random = New Random

        Private _timer As Timer

        Private _updatingStockPrices As Boolean = False

        Public skl2008connectionString As String = ConfigurationManager.ConnectionStrings("skl2008ConnectionString").ConnectionString
        Private Property _Clients As IHubConnectionContext(Of Object)

        Private Sub New(ByVal clients As IHubConnectionContext(Of Object))
            _Clients = clients
            _stocks.Clear()
            Dim stocks = New List(Of Stock) From {
                New Stock With {
                    .Symbol = "MSFT",
                    .Price = 30.31D
                },
                New Stock With {
                    .Symbol = "APPL",
                    .Price = 578.18D
                },
                New Stock With {
                    .Symbol = "GOOG",
                    .Price = 570.3D
                }
            }
            stocks.ForEach(Function(stock) _stocks.TryAdd(stock.Symbol, stock))
            _timer = New Timer(AddressOf UpdateStockPrices, Nothing, _updateInterval, _updateInterval)
        End Sub

        Public Shared ReadOnly Property Instance As StockTicker
            Get
                Return _instance.Value
            End Get
        End Property

        Public Function GetAllStocks() As IEnumerable(Of Stock)
            Return _stocks.Values
        End Function

        Private Sub UpdateStockPrices(ByVal state As Object)
            ' _updateStockPricesLock
            'TODO: lock is not supported at this time
            ' If Not _updatingStockPrices Then
            _updatingStockPrices = True
                For Each stock In _stocks.Values
                    If TryUpdateStockPrice(stock) Then
                        BroadcastStockPrice(stock)
                    End If

                Next
                _updatingStockPrices = False
            ' End If

        End Sub

        Private Function TryUpdateStockPrice(ByVal stock As Stock) As Boolean
            Dim r = _updateOrNotRandom.NextDouble()

            If r > 0.1 Then
                Return False
            End If

            Dim random = New Random(CInt(Math.Floor(stock.Price)))
            Dim percentChange As Decimal = Rnd() / 100
            Dim pos = random.NextDouble() > 0.51
            Dim change = Math.Round(stock.Price * CDec(percentChange), 2)
            change = If(pos, change, -change)
            stock.Price += change
            Return True
        End Function

        Private Sub BroadcastStockPrice(ByVal stock As Stock)
            _Clients.All.updateStockPrice(stock)
        End Sub

        Function GetDataTable(ByVal queryString As String) As DataTable

            Dim dt As New DataTable()

            Try
                ' Connect to the database and run the query.
                Dim connection As New SqlConnection(skl2008connectionString)
                Dim adapter As New SqlDataAdapter(queryString, connection)
                adapter.SelectCommand.CommandTimeout = 180
                ' Fill the DataTable.
                adapter.Fill(dt)

            Catch ex As Exception

                ' The connection failed. Display an error message.
                ' Text = "Unable to connect to the database."

            End Try

            Return dt

        End Function

    End Class

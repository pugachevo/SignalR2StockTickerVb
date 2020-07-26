Imports System

Public Class Stock
        Private _price As Decimal
        Public Property Symbol As String

        Public Property Price As Decimal
            Get
                Return _price
            End Get
            Set(ByVal value As Decimal)

                If _price = value Then
                    Return
                End If

                _price = value

                If DayOpen = 0 Then
                    DayOpen = _price
                End If
            End Set
        End Property

        Public Property DayOpen As Decimal

        Public ReadOnly Property Change As Decimal
            Get
                Return Price - DayOpen
            End Get
        End Property

        Public ReadOnly Property PercentChange As Double
            Get
                Return CDbl(Math.Round(Change / Price, 4))
            End Get
        End Property
    End Class


Imports Microsoft.VisualBasic
Imports System.Data

Public Class ExecuteStatement

    Public Shared Function CountValue(ByVal strSql) As String
        Dim myConn1 As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)
        Dim myCommand As New System.Data.OleDb.OleDbCommand(strSql, myConn1)
        Dim obj As Object
        Dim ctobj As Int32
        myConn1.Open()
        obj = myCommand.ExecuteScalar
        ctobj = Convert.ToInt32(obj)
        myCommand.Connection.Close()
        myCommand.Dispose()
        myConn1.Close()
        myConn1.Dispose()
        Return ctobj
    End Function
    Public Shared Function CountValueDouble(ByVal strSql) As String
        Dim myConn1 As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)
        Dim myCommand As New System.Data.OleDb.OleDbCommand(strSql, myConn1)
        Dim obj As Object
        Dim ctobj As Double
        myConn1.Open()
        obj = myCommand.ExecuteScalar
        ctobj = Convert.ToDouble(obj)
        myCommand.Connection.Close()
        myCommand.Dispose()
        myConn1.Close()
        myConn1.Dispose()
        Return ctobj
    End Function

    Public Shared Function SetData(ByVal strSql) As Boolean
        Dim myConn1 As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)
        Dim myCommand As New System.Data.OleDb.OleDbCommand(strSql, myConn1)
        Dim tStatus As Boolean
        Try
            myCommand.Connection.Open()
            myCommand.ExecuteNonQuery()
        Catch ex As Exception
        Finally
            myCommand.Connection.Close()
            myConn1.Close()
        End Try
        myCommand.Connection.Close()
        myCommand.Dispose()
        myConn1.Close()
        myConn1.Dispose()
        Return tStatus
    End Function

    Public Shared Function SelectString(ByVal strSql) As String
        Dim myConn1 As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)
        Dim myCommand As New System.Data.OleDb.OleDbCommand(strSql, myConn1)
        Dim myReader As System.Data.OleDb.OleDbDataReader
        Dim tStatus As String
        Dim TextValue As String
        myConn1.Open()
        myReader = myCommand.ExecuteReader()
        Try
            While myReader.Read()
                TextValue = myReader.GetString(0).ToString
                tStatus = TextValue
            End While
        Finally
            myReader.Close()
            myConn1.Close()
            myConn1.Dispose()
        End Try
        myCommand.Connection.Close()
        myConn1.Close()
        myConn1.Dispose()
        myCommand.Dispose()
        Return tStatus
    End Function

    Public Shared Function CountValueHO(ByVal strSql) As String
        Dim myConn1 As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)
        Dim myCommand As New System.Data.OleDb.OleDbCommand(strSql, myConn1)
        Dim obj As Object
        Dim ctobj As Int32
        myConn1.Open()
        obj = myCommand.ExecuteScalar
        ctobj = Convert.ToInt32(obj)
        myCommand.Connection.Close()
        myCommand.Dispose()
        myConn1.Close()
        myConn1.Dispose()
        Return ctobj
    End Function

    Public Shared Function SetDataHO(ByVal strSql) As Boolean
        Dim myConn1 As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)
        Dim myCommand As New System.Data.OleDb.OleDbCommand(strSql, myConn1)
        Dim tStatus As Boolean
        Try
            myCommand.Connection.Open()
            myCommand.ExecuteNonQuery()
        Catch ex As Exception
        Finally
            myCommand.Connection.Close()
            myConn1.Close()
        End Try
        myCommand.Connection.Close()
        myCommand.Dispose()
        myConn1.Close()
        myConn1.Dispose()
        Return tStatus
    End Function

    Public Shared Function SelectStringHO(ByVal strSql) As String

        Dim myConn1 As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)
        Dim myCommand As New System.Data.OleDb.OleDbCommand(strSql, myConn1)
        Dim myReader As System.Data.OleDb.OleDbDataReader
        Dim tStatus As String
        Dim TextValue As String
        myConn1.Open()
        myReader = myCommand.ExecuteReader()
        Try
            While myReader.Read()
                TextValue = myReader.GetString(0).ToString
                tStatus = TextValue
            End While
        Finally
            myReader.Close()
            myConn1.Close()
            myConn1.Dispose()
        End Try
        myCommand.Connection.Close()
        myConn1.Close()
        myConn1.Dispose()
        myCommand.Dispose()
        Return tStatus
    End Function
    Public Shared Function createDataSet(strQuery As String, strTableName As String) As DataSet
        Dim ds As New DataSet
        Dim objConnection As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)

         Dim objAdapter As New System.Data.OleDb.OleDbDataAdapter
        Dim objCommandBuilder As New System.Data.OleDb.OleDbCommandBuilder
        objConnection.Open()

        Try
            'If open_con Then
            objAdapter = New System.Data.OleDb.OleDbDataAdapter(strQuery, objConnection)
            objCommandBuilder = New System.Data.OleDb.OleDbCommandBuilder(objAdapter)
            objAdapter.Fill(ds, strTableName)

            'err_flag = False
            'Else
            'ErrorStr = "Error : Unable to connect to database."
            'err_flag = True
            'End If
        Catch objError As Exception
            'ErrorStr = objError.Message
            'err_flag = True

            objCommandBuilder.Dispose()
            objConnection.Close()
            objConnection.Dispose()
        Finally
            objCommandBuilder.Dispose()
            objConnection.Close()
            objConnection.Dispose()
        End Try
        Return ds
    End Function


    Public Shared Function saveDataTable(strQuery As String, ds As DataSet) As String
        Dim ErrorStr As String
        Dim objConnection As New System.Data.OleDb.OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings("raConnection1").ConnectionString)
        Dim objAdapter As New System.Data.OleDb.OleDbDataAdapter
        Dim objCommandBuilder As New System.Data.OleDb.OleDbCommandBuilder
        objConnection.Open()

        Try
            'If open_con Then
            objAdapter.SelectCommand = New System.Data.OleDb.OleDbCommand(strQuery, objConnection)
            objCommandBuilder = New System.Data.OleDb.OleDbCommandBuilder(objAdapter)

            Dim i As Int32 = 0
            For i = 0 To ds.Tables.Count - 1
                Dim strTableName As String = ""
                strTableName = ds.Tables(i).TableName

                objAdapter.Update(ds, strTableName)
            Next

            'err_flag = False
            'Else
            ErrorStr = "Y"
            'err_flag = True
            'End If
        Catch objError As Exception
            ErrorStr = objError.Message
            'err_flag = True

            objCommandBuilder.Dispose()
            objConnection.Close()
            objConnection.Dispose()
        Finally
            objCommandBuilder.Dispose()
            objConnection.Close()
            objConnection.Dispose()
        End Try
        Return ErrorStr
    End Function
End Class

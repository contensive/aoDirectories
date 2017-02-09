
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses
Imports Newtonsoft.Json

Namespace Contensive.Addons.Directories
    '
    ' Sample Vb addon
    '
    Public Class groupDirectoryClass
        Inherits AddonBaseClass
        '====================================================================================================
        ''' <summary>
        ''' create a directory from a group
        ''' </summary>
        ''' <param name="CP"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Execute(ByVal CP As BaseClasses.CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim groupID As Integer = CP.Utils.EncodeInteger(CP.Doc.Var("Group"))
                Dim cs As BaseClasses.CPCSBaseClass = CP.CSNew()
                'Dim groupName As String = ""
                Dim dataString As String = ""
                Dim js As String = ""   '   script string
                Dim iSS As String = ""   '   inner script string
                'Dim company As String = ""
                'Dim image As String = ""
                'Dim bio As String = ""
                Dim sql As String = ""
                Dim rightNow As Date = Now()
                Dim sqlRightNow As String = CP.Db.EncodeSQLDate(rightNow)
                Dim directory As New directoryClass
                Dim criteria As String
                directory.listData = New List(Of directoryDataClass)
                '
                'CP.Utils.AppendLog("groupDir.log", "Entry")
                '
                If groupID <> 0 Then
                    directory.groupName = CP.Content.GetRecordName("Groups", groupID)
                    criteria = "(r.groupID=" & groupID & ")"
                Else
                    directory.groupName = "Directory"
                    criteria = "(r.groupID>0)"
                End If
                '
                sql = "SELECT distinct m.firstName, m.lastName, m.imageFileName, m.organizationID, m.company, m.email, m.resumeFileName "
                sql += "FROM ccMembers M, ccMemberRules R "
                sql += "WHERE (m.id=r.memberID) and" & criteria
                sql += "and (m.active<>0) and (r.active<>0) "
                sql += "and ((r.dateExpires>=" & sqlRightNow & ") or (r.dateExpires is null)) "
                sql += "ORDER BY m.lastName"
                '
                cs.OpenSQL(sql)
                Do While cs.OK()
                    Dim dirEntry As New directoryDataClass
                    dirEntry.name = cs.GetText("firstName") & " " & cs.GetText("lastName")
                    dirEntry.image = cs.GetText("imageFileName")
                    dirEntry.bio = CP.File.ReadVirtual(cs.GetText("resumeFileName"))
                    dirEntry.company = CP.Content.GetRecordName("Organizations", cs.GetInteger("organizationID"))
                    '
                    If dirEntry.image <> "" Then
                        If dirEntry.image.IndexOf("://") >= 0 Then
                            '
                            ' field contains the entire url, use as is
                            '
                        Else
                            '
                            ' field contains the path to content files
                            '
                            dirEntry.image = CP.Site.FilePath & dirEntry.image
                        End If
                    Else
                        dirEntry.image = "/images/anon.png"
                    End If
                    '
                    If dirEntry.company = "" Then
                        dirEntry.company = cs.GetText("company")
                    End If
                    directory.listData.Add(dirEntry)
                    cs.GoNext()
                Loop
                cs.Close()
                '
                js = "var listData = " & JsonConvert.SerializeObject(directory) & ";"
                '
                'CP.Doc.AddHeadJavascript(js)
                Dim layout As Contensive.BaseClasses.CPBlockBaseClass = CP.BlockNew()
                layout.OpenLayout("Directories, Group Directory")
                returnHtml = layout.GetHtml() & vbCrLf & vbTab & "<script>" & vbCrLf & vbTab & js & vbCrLf & vbTab & "</script>"
                '
                'CP.Utils.AppendLog("groupDir.log", "Exit [" & CP.Utils.EncodeHTML(returnHtml) & "]")
                '
            Catch ex As Exception
                '
                'CP.Utils.AppendLog("groupDir.log", "error")
                '
                CP.Site.ErrorReport(ex, "error in Contensive.Addons.aoMembershipManager.directoryGroupClass.execute")
            End Try
            Return returnHtml
        End Function
    End Class
    '

    Public Class directoryClass
        Public groupName As String
        Public listData As List(Of directoryDataClass)
    End Class
    '
    Public Class directoryDataClass
        Public name As String
        Public image As String
        Public company As String
        Public email As String
        Public bio As String
    End Class
End Namespace

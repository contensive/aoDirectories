Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses

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
                Dim groupName As String = ""
                Dim dataString As String = ""
                Dim sS As String = ""   '   script string
                Dim iSS As String = ""   '   inner script string
                Dim company As String = ""
                Dim image As String = ""
                Dim bio As String = ""
                Dim sql As String = ""
                Dim rightNow As Date = Now()
                Dim sqlRightNow As String = CP.Db.EncodeSQLDate(rightNow)
                '
                If groupID <> 0 Then
                    groupName = CP.Content.GetRecordName("Groups", groupID)
                End If
                '
                sql = "SELECT m.firstName, m.lastName, m.imageFileName, m.organizationID, m.company, m.email, m.resumeFileName "
                sql += "FROM ccMembers M, ccMemberRules R "
                sql += "WHERE (m.id=r.memberID) and (r.groupID=" & groupID & ") "
                sql += "and (m.active<>0) and (r.active<>0) "
                sql += "and ((r.dateExpires>=" & sqlRightNow & ") or (r.dateExpires is null)) "
                sql += "ORDER BY m.lastName"
                '
                cs.OpenSQL(sql)
                Do While cs.OK()
                    image = cs.GetText("imageFileName")
                    bio = CP.File.ReadVirtual(cs.GetText("resumeFileName"))
                    company = CP.Content.GetRecordName("Organizations", cs.GetInteger("organizationID"))
                    '
                    If image <> "" Then
                        image = CP.Site.FilePath & image
                    Else
                        image = "/images/anon.png"
                    End If
                    '
                    If company = "" Then
                        company = cs.GetText("company")
                    End If
                    '
                    If iSS <> "" Then
                        iSS += ","
                    End If
                    iSS += "{""name"": """ & cs.GetText("firstName") & " " & cs.GetText("lastName") & """, ""image"": """ & image & """, ""company"": """ & company & """, ""email"": """ & cs.GetText("email") & """, ""bio"": """ & bio & """}"
                    cs.GoNext()
                Loop
                cs.Close()
                '
                sS = "var listData = {""groupName"": """ & CP.Utils.EncodeJavascript(groupName) & """, ""listData"": [" & CP.Utils.EncodeJavascript(iSS) & "]};"
                '
                CP.Doc.AddHeadJavascript(sS)
                Dim layout As Contensive.BaseClasses.CPBlockBaseClass = CP.BlockNew()
                layout.OpenLayout("Directories, Group Directory")
                returnHtml = layout.GetHtml()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "error in Contensive.Addons.aoMembershipManager.directoryGroupClass.execute")
            End Try
            Return returnHtml
        End Function
    End Class
End Namespace

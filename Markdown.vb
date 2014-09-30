﻿
'
' * MarkdownSharp
' * -------------
' * a C# Markdown processor
' * 
' * Markdown is a text-to-HTML conversion tool for web writers
' * Copyright (c) 2004 John Gruber
' * http://daringfireball.net/projects/markdown/
' * 
' * Markdown.NET
' * Copyright (c) 2004-2009 Milan Negovan
' * http://www.aspnetresources.com
' * http://aspnetresources.com/blog/markdown_announced.aspx
' * 
' * MarkdownSharp
' * Copyright (c) 2009-2011 Jeff Atwood
' * http://stackoverflow.com
' * http://www.codinghorror.com/blog/
' * http://code.google.com/p/markdownsharp/
' * 
' * History: Milan ported the Markdown processor to C#. He granted license to me so I can open source it
' * and let the community contribute to and improve MarkdownSharp.
' * 
' 


#Region "Copyright and license"

'
'
'Copyright (c) 2009 - 2010 Jeff Atwood
'
'http://www.opensource.org/licenses/mit-license.php
'  
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in
'all copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
'THE SOFTWARE.
'
'Copyright (c) 2003-2004 John Gruber
'<http://daringfireball.net/>   
'All rights reserved.
'
'Redistribution and use in source and binary forms, with or without
'modification, are permitted provided that the following conditions are
'met:
'
'* Redistributions of source code must retain the above copyright notice,
'  this list of conditions and the following disclaimer.
'
'* Redistributions in binary form must reproduce the above copyright
'  notice, this list of conditions and the following disclaimer in the
'  documentation and/or other materials provided with the distribution.
'
'* Neither the name "Markdown" nor the names of its contributors may
'  be used to endorse or promote products derived from this software
'  without specific prior written permission.
'
'This software is provided by the copyright holders and contributors "as
'is" and any express or implied warranties, including, but not limited
'to, the implied warranties of merchantability and fitness for a
'particular purpose are disclaimed. In no event shall the copyright owner
'or contributors be liable for any direct, indirect, incidental, special,
'exemplary, or consequential damages (including, but not limited to,
'procurement of substitute goods or services; loss of use, data, or
'profits; or business interruption) however caused and on any theory of
'liability, whether in contract, strict liability, or tort (including
'negligence or otherwise) arising in any way out of the use of this
'software, even if advised of the possibility of such damage.
'


#End Region

Imports System.Collections.Generic
Imports System.Configuration
Imports System.Text
Imports System.Text.RegularExpressions

Namespace MarkdownSharp

    Public Class MarkdownOptions
        ''' <summary>
        ''' when true, (most) bare plain URLs are auto-hyperlinked  
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property AutoHyperlink() As Boolean
            Get
                Return m_AutoHyperlink
            End Get
            Set(value As Boolean)
                m_AutoHyperlink = Value
            End Set
        End Property
        Private m_AutoHyperlink As Boolean
        ''' <summary>
        ''' when true, RETURN becomes a literal newline  
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property AutoNewlines() As Boolean
            Get
                Return m_AutoNewlines
            End Get
            Set(value As Boolean)
                m_AutoNewlines = Value
            End Set
        End Property
        Private m_AutoNewlines As Boolean
        ''' <summary>
        ''' use ">" for HTML output, or " />" for XHTML output
        ''' </summary>
        Public Property EmptyElementSuffix() As String
            Get
                Return m_EmptyElementSuffix
            End Get
            Set(value As String)
                m_EmptyElementSuffix = Value
            End Set
        End Property
        Private m_EmptyElementSuffix As String
        ''' <summary>
        ''' when true, problematic URL characters like [, ], (, and so forth will be encoded 
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property EncodeProblemUrlCharacters() As Boolean
            Get
                Return m_EncodeProblemUrlCharacters
            End Get
            Set(value As Boolean)
                m_EncodeProblemUrlCharacters = Value
            End Set
        End Property
        Private m_EncodeProblemUrlCharacters As Boolean
        ''' <summary>
        ''' when false, email addresses will never be auto-linked  
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property LinkEmails() As Boolean
            Get
                Return m_LinkEmails
            End Get
            Set(value As Boolean)
                m_LinkEmails = Value
            End Set
        End Property
        Private m_LinkEmails As Boolean
        ''' <summary>
        ''' when true, bold and italic require non-word characters on either side  
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property StrictBoldItalic() As Boolean
            Get
                Return m_StrictBoldItalic
            End Get
            Set(value As Boolean)
                m_StrictBoldItalic = Value
            End Set
        End Property
        Private m_StrictBoldItalic As Boolean
    End Class


    ''' <summary>
    ''' Markdown is a text-to-HTML conversion tool for web writers. 
    ''' Markdown allows you to write using an easy-to-read, easy-to-write plain text format, 
    ''' then convert it to structurally valid XHTML (or HTML).
    ''' </summary>
    Public Class Markdown
        Private Const _version As String = "1.13"

#Region "Constructors and Options"

        ''' <summary>
        ''' Create a new Markdown instance using default options
        ''' </summary>
        Public Sub New()
            'Me.New(False)
        End Sub

        ''' <summary>
        ''' Create a new Markdown instance and optionally load options from a configuration
        ''' file. There they should be stored in the appSettings section, available options are:
        ''' 
        '''     Markdown.StrictBoldItalic (true/false)
        '''     Markdown.EmptyElementSuffix (">" or " />" without the quotes)
        '''     Markdown.LinkEmails (true/false)
        '''     Markdown.AutoNewLines (true/false)
        '''     Markdown.AutoHyperlink (true/false)
        '''     Markdown.EncodeProblemUrlCharacters (true/false) 
        '''     
        ''' </summary>
        Public Sub New(loadOptionsFromConfigFile As Boolean)
            If Not loadOptionsFromConfigFile Then
                Return
            End If

            'Dim settings = System.Configuration.ConfigurationManager.AppSettings
            'For Each key As String In settings.Keys
            '    Select Case key
            '        Case "Markdown.AutoHyperlink"
            '            _autoHyperlink = Convert.ToBoolean(settings(key))
            '            Exit Select
            '        Case "Markdown.AutoNewlines"
            '            _autoNewlines = Convert.ToBoolean(settings(key))
            '            Exit Select
            '        Case "Markdown.EmptyElementSuffix"
            '            _emptyElementSuffix = settings(key)
            '            Exit Select
            '        Case "Markdown.EncodeProblemUrlCharacters"
            '            _encodeProblemUrlCharacters = Convert.ToBoolean(settings(key))
            '            Exit Select
            '        Case "Markdown.LinkEmails"
            '            _linkEmails = Convert.ToBoolean(settings(key))
            '            Exit Select
            '        Case "Markdown.StrictBoldItalic"
            '            _strictBoldItalic = Convert.ToBoolean(settings(key))
            '            Exit Select
            '    End Select
            'Next
        End Sub

        ''' <summary>
        ''' Create a new Markdown instance and set the options from the MarkdownOptions object.
        ''' </summary>
        Public Sub New(options As MarkdownOptions)
            _autoHyperlink = options.AutoHyperlink
            _autoNewlines = options.AutoNewlines
            _emptyElementSuffix = options.EmptyElementSuffix
            _encodeProblemUrlCharacters = options.EncodeProblemUrlCharacters
            _linkEmails = options.LinkEmails
            _strictBoldItalic = options.StrictBoldItalic
        End Sub


        ''' <summary>
        ''' use ">" for HTML output, or " />" for XHTML output
        ''' </summary>
        Public Property EmptyElementSuffix() As String
            Get
                Return _emptyElementSuffix
            End Get
            Set(value As String)
                _emptyElementSuffix = value
            End Set
        End Property
        Private _emptyElementSuffix As String = " />"

        ''' <summary>
        ''' when false, email addresses will never be auto-linked  
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property LinkEmails() As Boolean
            Get
                Return _linkEmails
            End Get
            Set(value As Boolean)
                _linkEmails = value
            End Set
        End Property
        Private _linkEmails As Boolean = True

        ''' <summary>
        ''' when true, bold and italic require non-word characters on either side  
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property StrictBoldItalic() As Boolean
            Get
                Return _strictBoldItalic
            End Get
            Set(value As Boolean)
                _strictBoldItalic = value
            End Set
        End Property
        Private _strictBoldItalic As Boolean = False

        ''' <summary>
        ''' when true, RETURN becomes a literal newline  
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property AutoNewLines() As Boolean
            Get
                Return _autoNewlines
            End Get
            Set(value As Boolean)
                _autoNewlines = value
            End Set
        End Property
        Private _autoNewlines As Boolean = False

        ''' <summary>
        ''' when true, (most) bare plain URLs are auto-hyperlinked  
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property AutoHyperlink() As Boolean
            Get
                Return _autoHyperlink
            End Get
            Set(value As Boolean)
                _autoHyperlink = value
            End Set
        End Property
        Private _autoHyperlink As Boolean = False

        ''' <summary>
        ''' when true, problematic URL characters like [, ], (, and so forth will be encoded 
        ''' WARNING: this is a significant deviation from the markdown spec
        ''' </summary>
        Public Property EncodeProblemUrlCharacters() As Boolean
            Get
                Return _encodeProblemUrlCharacters
            End Get
            Set(value As Boolean)
                _encodeProblemUrlCharacters = value
            End Set
        End Property
        Private _encodeProblemUrlCharacters As Boolean = False

#End Region

        Private Enum TokenType
            Text
            Tag
        End Enum

        Private Structure Token
            Public Sub New(type As TokenType, value As String)
                Me.Type = type
                Me.Value = value
            End Sub
            Public Type As TokenType
            Public Value As String
        End Structure

        ''' <summary>
        ''' maximum nested depth of [] and () supported by the transform; implementation detail
        ''' </summary>
        Private Const _nestDepth As Integer = 6

        ''' <summary>
        ''' Tabs are automatically converted to spaces as part of the transform  
        ''' this constant determines how "wide" those tabs become in spaces  
        ''' </summary>
        Private Const _tabWidth As Integer = 4

        Private Const _markerUL As String = "[*+-]"
        Private Const _markerOL As String = "\d+[.]"

        Private Shared ReadOnly _escapeTable As Dictionary(Of String, String)
        Private Shared ReadOnly _invertedEscapeTable As Dictionary(Of String, String)
        Private Shared ReadOnly _backslashEscapeTable As Dictionary(Of String, String)

        Private ReadOnly _urls As New Dictionary(Of String, String)()
        Private ReadOnly _titles As New Dictionary(Of String, String)()
        Private ReadOnly _htmlBlocks As New Dictionary(Of String, String)()

        Private _listLevel As Integer
        Private Shared AutoLinkPreventionMarker As String = ChrW(26) & "P"
        ' temporarily replaces "://" where auto-linking shouldn't happen;
        ''' <summary>
        ''' In the static constuctor we'll initialize what stays the same across all transforms.
        ''' </summary>
        Shared Sub New()
            ' Table of hash values for escaped characters:
            _escapeTable = New Dictionary(Of String, String)()
            _invertedEscapeTable = New Dictionary(Of String, String)()
            ' Table of hash value for backslash escaped characters:
            _backslashEscapeTable = New Dictionary(Of String, String)()

            Dim backslashPattern As String = ""

            For Each c As Char In "\`*_{}[]()>#+-.!/"
                Dim key As String = c.ToString()
                Dim hash As String = GetHashKey(key, isHtmlBlock:=False)
                _escapeTable.Add(key, hash)
                _invertedEscapeTable.Add(hash, key)
                _backslashEscapeTable.Add(Convert.ToString("\") & key, hash)
                backslashPattern += Regex.Escape(Convert.ToString("\") & key) + "|"
            Next

            _backslashEscapes = New Regex(backslashPattern.Substring(0, backslashPattern.Length - 1), RegexOptions.Compiled)
        End Sub

        ''' <summary>
        ''' current version of MarkdownSharp;  
        ''' see http://code.google.com/p/markdownsharp/ for the latest code or to contribute
        ''' </summary>
        Public ReadOnly Property Version() As String
            Get
                Return _version
            End Get
        End Property

        ''' <summary>
        ''' Transforms the provided Markdown-formatted text to HTML;  
        ''' see http://en.wikipedia.org/wiki/Markdown
        ''' </summary>
        ''' <remarks>
        ''' The order in which other subs are called here is
        ''' essential. Link and image substitutions need to happen before
        ''' EscapeSpecialChars(), so that any *'s or _'s in the a
        ''' and img tags get encoded.
        ''' </remarks>
        Public Function Transform(text As String) As String
            If [String].IsNullOrEmpty(text) Then
                Return ""
            End If

            Setup()

            text = Normalize(text)

            text = HashHTMLBlocks(text)
            text = StripLinkDefinitions(text)
            text = RunBlockGamut(text)
            text = Unescape(text)

            Cleanup()

            Return text & Convert.ToString(vbLf)
        End Function


        ''' <summary>
        ''' Perform transformations that form block-level tags like paragraphs, headers, and list items.
        ''' </summary>
        Private Function RunBlockGamut(text As String, Optional unhash As Boolean = True) As String
            text = DoHeaders(text)
            text = DoHorizontalRules(text)
            text = DoLists(text)
            text = DoCodeBlocks(text)
            text = DoBlockQuotes(text)

            ' We already ran HashHTMLBlocks() before, in Markdown(), but that
            ' was to escape raw HTML in the original Markdown source. This time,
            ' we're escaping the markup we've just created, so that we don't wrap
            ' <p> tags around block-level tags.
            text = HashHTMLBlocks(text)

            text = FormParagraphs(text, unhash:=unhash)

            Return text
        End Function


        ''' <summary>
        ''' Perform transformations that occur *within* block-level tags like paragraphs, headers, and list items.
        ''' </summary>
        Private Function RunSpanGamut(text As String) As String
            text = DoCodeSpans(text)
            text = EscapeSpecialCharsWithinTagAttributes(text)
            text = EscapeBackslashes(text)

            ' Images must come first, because ![foo][f] looks like an anchor.
            text = DoImages(text)
            text = DoAnchors(text)

            ' Must come after DoAnchors(), because you can use < and >
            ' delimiters in inline links like [this](<url>).
            text = DoAutoLinks(text)

            text = text.Replace(AutoLinkPreventionMarker, "://")

            text = EncodeAmpsAndAngles(text)
            text = DoItalicsAndBold(text)
            text = DoHardBreaks(text)

            Return text
        End Function

        Private Shared _newlinesLeadingTrailing As New Regex("^\n+|\n+\z", RegexOptions.Compiled)
        Private Shared _newlinesMultiple As New Regex("\n{2,}", RegexOptions.Compiled)
        Private Shared _leadingWhitespace As New Regex("^[ ]*", RegexOptions.Compiled)

        Private Shared _htmlBlockHash As New Regex(ChrW(26) & "H\d+H", RegexOptions.Compiled)

        ''' <summary>
        ''' splits on two or more newlines, to form "paragraphs";    
        ''' each paragraph is then unhashed (if it is a hash and unhashing isn't turned off) or wrapped in HTML p tag
        ''' </summary>
        Private Function FormParagraphs(text As String, Optional unhash As Boolean = True) As String
            ' split on two or more newlines
            Dim grafs As String() = _newlinesMultiple.Split(_newlinesLeadingTrailing.Replace(text, ""))

            For i As Integer = 0 To grafs.Length - 1
                If grafs(i).StartsWith(ChrW(26) & "H") Then
                    ' unhashify HTML blocks
                    If unhash Then
                        Dim sanityCheck As Integer = 50
                        ' just for safety, guard against an infinite loop
                        Dim keepGoing As Boolean = True
                        ' as long as replacements where made, keep going
                        While keepGoing AndAlso sanityCheck > 0
                            keepGoing = False
                            grafs(i) = _htmlBlockHash.Replace(grafs(i), Function(match)
                                                                            keepGoing = True
                                                                            Return _htmlBlocks(match.Value)

                                                                        End Function)
                            sanityCheck -= 1
                            ' if (keepGoing)
                            '                        {
                            '                            // Logging of an infinite loop goes here.
                            '                            // If such a thing should happen, please open a new issue on http://code.google.com/p/markdownsharp/
                            '                            // with the input that caused it.
                            '                        }

                        End While
                    End If
                Else
                    ' do span level processing inside the block, then wrap result in <p> tags
                    grafs(i) = _leadingWhitespace.Replace(RunSpanGamut(grafs(i)), "<p>") + "</p>"
                End If
            Next

            Return String.Join(vbLf & vbLf, grafs)
        End Function


        Private Sub Setup()
            ' Clear the global hashes. If we don't clear these, you get conflicts
            ' from other articles when generating a page which contains more than
            ' one article (e.g. an index page that shows the N most recent
            ' articles):
            _urls.Clear()
            _titles.Clear()
            _htmlBlocks.Clear()
            _listLevel = 0
        End Sub

        Private Sub Cleanup()
            Setup()
        End Sub

        Private Shared _nestedBracketsPattern As String

        ''' <summary>
        ''' Reusable pattern to match balanced [brackets]. See Friedl's 
        ''' "Mastering Regular Expressions", 2nd Ed., pp. 328-331.
        ''' </summary>
        Private Shared Function GetNestedBracketsPattern() As String
            ' in other words [this] and [this[also]] and [this[also[too]]]
            ' up to _nestDepth
            If _nestedBracketsPattern Is Nothing Then
                _nestedBracketsPattern = RepeatString(vbCr & vbLf & "                    (?>              # Atomic matching" & vbCr & vbLf & "                       [^\[\]]+      # Anything other than brackets" & vbCr & vbLf & "                     |" & vbCr & vbLf & "                       \[" & vbCr & vbLf & "                           ", _nestDepth) & RepeatString(" \]" & vbCr & vbLf & "                    )*", _nestDepth)
            End If
            Return _nestedBracketsPattern
        End Function

        Private Shared _nestedParensPattern As String

        ''' <summary>
        ''' Reusable pattern to match balanced (parens). See Friedl's 
        ''' "Mastering Regular Expressions", 2nd Ed., pp. 328-331.
        ''' </summary>
        Private Shared Function GetNestedParensPattern() As String
            ' in other words (this) and (this(also)) and (this(also(too)))
            ' up to _nestDepth
            If _nestedParensPattern Is Nothing Then
                _nestedParensPattern = RepeatString(vbCr & vbLf & "                    (?>              # Atomic matching" & vbCr & vbLf & "                       [^()\s]+      # Anything other than parens or whitespace" & vbCr & vbLf & "                     |" & vbCr & vbLf & "                       \(" & vbCr & vbLf & "                           ", _nestDepth) & RepeatString(" \)" & vbCr & vbLf & "                    )*", _nestDepth)
            End If
            Return _nestedParensPattern
        End Function

        Private Shared _linkDef As New Regex(String.Format(vbCr & vbLf & "                        ^[ ]{{0,{0}}}\[([^\[\]]+)\]:  # id = $1" & vbCr & vbLf & "                          [ ]*" & vbCr & vbLf & "                          \n?                   # maybe *one* newline" & vbCr & vbLf & "                          [ ]*" & vbCr & vbLf & "                        <?(\S+?)>?              # url = $2" & vbCr & vbLf & "                          [ ]*" & vbCr & vbLf & "                          \n?                   # maybe one newline" & vbCr & vbLf & "                          [ ]*" & vbCr & vbLf & "                        (?:" & vbCr & vbLf & "                            (?<=\s)             # lookbehind for whitespace" & vbCr & vbLf & "                            [""(]" & vbCr & vbLf & "                            (.+?)               # title = $3" & vbCr & vbLf & "                            ["")]" & vbCr & vbLf & "                            [ ]*" & vbCr & vbLf & "                        )?                      # title is optional" & vbCr & vbLf & "                        (?:\n+|\Z)", _tabWidth - 1), RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        ''' <summary>
        ''' Strips link definitions from text, stores the URLs and titles in hash references.
        ''' </summary>
        ''' <remarks>
        ''' ^[id]: url "optional title"
        ''' </remarks>
        Private Function StripLinkDefinitions(text As String) As String
            Return _linkDef.Replace(text, New MatchEvaluator(AddressOf LinkEvaluator))
        End Function

        Private Function LinkEvaluator(match As Match) As String
            Dim linkID As String = match.Groups(1).Value.ToLowerInvariant()
            _urls(linkID) = EncodeAmpsAndAngles(match.Groups(2).Value)

            If match.Groups(3) IsNot Nothing AndAlso match.Groups(3).Length > 0 Then
                _titles(linkID) = match.Groups(3).Value.Replace("""", "&quot;")
            End If

            Return ""
        End Function

        ' compiling this monster regex results in worse performance. trust me.
        Private Shared _blocksHtml As New Regex(GetBlockPattern(), RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace)


        ''' <summary>
        ''' derived pretty much verbatim from PHP Markdown
        ''' </summary>
        Private Shared Function GetBlockPattern() As String

            ' Hashify HTML blocks:
            ' We only want to do this for block-level HTML tags, such as headers,
            ' lists, and tables. That's because we still want to wrap <p>s around
            ' "paragraphs" that are wrapped in non-block-level tags, such as anchors,
            ' phrase emphasis, and spans. The list of tags we're looking for is
            ' hard-coded:
            '
            ' *  List "a" is made of tags which can be both inline or block-level.
            '    These will be treated block-level when the start tag is alone on 
            '    its line, otherwise they're not matched here and will be taken as 
            '    inline later.
            ' *  List "b" is made of tags which are always block-level;
            '
            Dim blockTagsA As String = "ins|del"
            Dim blockTagsB As String = "p|div|h[1-6]|blockquote|pre|table|dl|ol|ul|address|script|noscript|form|fieldset|iframe|math"

            ' Regular expression for the content of a block tag.
            Dim attr As String = vbCr & vbLf & "            (?>﻿  ﻿  ﻿  ﻿              # optional tag attributes" & vbCr & vbLf & "              \s﻿  ﻿  ﻿              # starts with whitespace" & vbCr & vbLf & "              (?>" & vbCr & vbLf & "                [^>""/]+﻿              # text outside quotes" & vbCr & vbLf & "              |" & vbCr & vbLf & "                /+(?!>)﻿  ﻿              # slash not followed by >" & vbCr & vbLf & "              |" & vbCr & vbLf & "                ""[^""]*""﻿  ﻿          # text inside double quotes (tolerate >)" & vbCr & vbLf & "              |" & vbCr & vbLf & "                '[^']*'﻿                  # text inside single quotes (tolerate >)" & vbCr & vbLf & "              )*" & vbCr & vbLf & "            )?﻿  " & vbCr & vbLf & "            "

            ' end of opening tag
            ' last level nested tag content
            Dim content As String = Convert.ToString(RepeatString((Convert.ToString(vbCr & vbLf & "                (?>" & vbCr & vbLf & "                  [^<]+﻿  ﻿  ﻿          # content without tag" & vbCr & vbLf & "                |" & vbCr & vbLf & "                  <\2﻿  ﻿  ﻿          # nested opening tag" & vbCr & vbLf & "                    ") & attr) + "       # attributes" & vbCr & vbLf & "                  (?>" & vbCr & vbLf & "                      />" & vbCr & vbLf & "                  |" & vbCr & vbLf & "                      >", _nestDepth) & Convert.ToString(".*?")) & RepeatString(vbCr & vbLf & "                      </\2\s*>﻿          # closing nested tag" & vbCr & vbLf & "                  )" & vbCr & vbLf & "                  |﻿  ﻿  ﻿  ﻿  " & vbCr & vbLf & "                  <(?!/\2\s*>           # other tags with a different name" & vbCr & vbLf & "                  )" & vbCr & vbLf & "                )*", _nestDepth)

            Dim content2 As String = content.Replace("\2", "\3")

            ' First, look for nested blocks, e.g.:
            ' ﻿  <div>
            ' ﻿  ﻿  <div>
            ' ﻿  ﻿  tags for inner block must be indented.
            ' ﻿  ﻿  </div>
            ' ﻿  </div>
            '
            ' The outermost tags must start at the left margin for this to match, and
            ' the inner nested divs must be indented.
            ' We need to do this before the next, more liberal match, because the next
            ' match will start at the first `<div>` and stop at the first `</div>`.
            Dim pattern As String = vbCr & vbLf & "            (?>" & vbCr & vbLf & "                  (?>" & vbCr & vbLf & "                    (?<=\n)     # Starting at the beginning of a line" & vbCr & vbLf & "                    |           # or" & vbCr & vbLf & "                    \A\n?       # the beginning of the doc" & vbCr & vbLf & "                  )" & vbCr & vbLf & "                  (             # save in $1" & vbCr & vbLf & vbCr & vbLf & "                    # Match from `\n<tag>` to `</tag>\n`, handling nested tags " & vbCr & vbLf & "                    # in between." & vbCr & vbLf & "                      " & vbCr & vbLf & "                        <($block_tags_b_re)   # start tag = $2" & vbCr & vbLf & "                        $attr>                # attributes followed by > and \n" & vbCr & vbLf & "                        $content              # content, support nesting" & vbCr & vbLf & "                        </\2>                 # the matching end tag" & vbCr & vbLf & "                        [ ]*                  # trailing spaces" & vbCr & vbLf & "                        (?=\n+|\Z)            # followed by a newline or end of document" & vbCr & vbLf & vbCr & vbLf & "                  | # Special version for tags of group a." & vbCr & vbLf & vbCr & vbLf & "                        <($block_tags_a_re)   # start tag = $3" & vbCr & vbLf & "                        $attr>[ ]*\n          # attributes followed by >" & vbCr & vbLf & "                        $content2             # content, support nesting" & vbCr & vbLf & "                        </\3>                 # the matching end tag" & vbCr & vbLf & "                        [ ]*                  # trailing spaces" & vbCr & vbLf & "                        (?=\n+|\Z)            # followed by a newline or end of document" & vbCr & vbLf & "                      " & vbCr & vbLf & "                  | # Special case just for <hr />. It was easier to make a special " & vbCr & vbLf & "                    # case than to make the other regex more complicated." & vbCr & vbLf & "                  " & vbCr & vbLf & "                        [ ]{0,$less_than_tab}" & vbCr & vbLf & "                        <hr" & vbCr & vbLf & "                        $attr                 # attributes" & vbCr & vbLf & "                        /?>                   # the matching end tag" & vbCr & vbLf & "                        [ ]*" & vbCr & vbLf & "                        (?=\n{2,}|\Z)         # followed by a blank line or end of document" & vbCr & vbLf & "                  " & vbCr & vbLf & "                  | # Special case for standalone HTML comments:" & vbCr & vbLf & "                  " & vbCr & vbLf & "                      (?<=\n\n|\A)            # preceded by a blank line or start of document" & vbCr & vbLf & "                      [ ]{0,$less_than_tab}" & vbCr & vbLf & "                      (?s:" & vbCr & vbLf & "                        <!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->" & vbCr & vbLf & "                      )" & vbCr & vbLf & "                      [ ]*" & vbCr & vbLf & "                      (?=\n{2,}|\Z)            # followed by a blank line or end of document" & vbCr & vbLf & "                  " & vbCr & vbLf & "                  | # PHP and ASP-style processor instructions (<? and <%)" & vbCr & vbLf & "                  " & vbCr & vbLf & "                      [ ]{0,$less_than_tab}" & vbCr & vbLf & "                      (?s:" & vbCr & vbLf & "                        <([?%])                # $4" & vbCr & vbLf & "                        .*?" & vbCr & vbLf & "                        \4>" & vbCr & vbLf & "                      )" & vbCr & vbLf & "                      [ ]*" & vbCr & vbLf & "                      (?=\n{2,}|\Z)            # followed by a blank line or end of document" & vbCr & vbLf & "                      " & vbCr & vbLf & "                  )" & vbCr & vbLf & "            )"

            pattern = pattern.Replace("$less_than_tab", (_tabWidth - 1).ToString())
            pattern = pattern.Replace("$block_tags_b_re", blockTagsB)
            pattern = pattern.Replace("$block_tags_a_re", blockTagsA)
            pattern = pattern.Replace("$attr", attr)
            pattern = pattern.Replace("$content2", content2)
            pattern = pattern.Replace("$content", content)

            Return pattern
        End Function

        ''' <summary>
        ''' replaces any block-level HTML blocks with hash entries
        ''' </summary>
        Private Function HashHTMLBlocks(text As String) As String
            Return _blocksHtml.Replace(text, New MatchEvaluator(AddressOf HtmlEvaluator))
        End Function

        Private Function HtmlEvaluator(match As Match) As String
            Dim text As String = match.Groups(1).Value
            Dim key As String = GetHashKey(text, isHtmlBlock:=True)
            _htmlBlocks(key) = text

            Return String.Concat(vbLf & vbLf, key, vbLf & vbLf)
        End Function

        Private Shared Function GetHashKey(s As String, isHtmlBlock As Boolean) As String
            Dim delim = If(isHtmlBlock, "H"c, "E"c)
            Return ChrW(26) + delim + Math.Abs(s.GetHashCode()).ToString() + delim
        End Function

        Private Shared _htmlTokens As New Regex((Convert.ToString(Convert.ToString(vbCr & vbLf & "            (<!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->)|        # match <!-- foo -->" & vbCr & vbLf & "            (<\?.*?\?>)|                 # match <?foo?> ") & RepeatString(" " & vbCr & vbLf & "            (<[A-Za-z\/!$](?:[^<>]|", _nestDepth)) & RepeatString(")*>)", _nestDepth)) + " # match <tag> and </tag>", RegexOptions.Multiline Or RegexOptions.Singleline Or RegexOptions.ExplicitCapture Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        ''' <summary>
        ''' returns an array of HTML tokens comprising the input string. Each token is 
        ''' either a tag (possibly with nested, tags contained therein, such 
        ''' as &lt;a href="&lt;MTFoo&gt;"&gt;, or a run of text between tags. Each element of the 
        ''' array is a two-element array; the first is either 'tag' or 'text'; the second is 
        ''' the actual value.
        ''' </summary>
        Private Function TokenizeHTML(text As String) As List(Of Token)
            Dim pos As Integer = 0
            Dim tagStart As Integer = 0
            Dim tokens = New List(Of Token)()

            ' this regex is derived from the _tokenize() subroutine in Brad Choate's MTRegex plugin.
            ' http://www.bradchoate.com/past/mtregex.php
            For Each m As Match In _htmlTokens.Matches(text)
                tagStart = m.Index

                If pos < tagStart Then
                    tokens.Add(New Token(TokenType.Text, text.Substring(pos, tagStart - pos)))
                End If

                tokens.Add(New Token(TokenType.Tag, m.Value))
                pos = tagStart + m.Length
            Next

            If pos < text.Length Then
                tokens.Add(New Token(TokenType.Text, text.Substring(pos, text.Length - pos)))
            End If

            Return tokens
        End Function


        Private Shared _anchorRef As New Regex(String.Format(vbCr & vbLf & "            (                               # wrap whole match in $1" & vbCr & vbLf & "                \[" & vbCr & vbLf & "                    ({0})                   # link text = $2" & vbCr & vbLf & "                \]" & vbCr & vbLf & vbCr & vbLf & "                [ ]?                        # one optional space" & vbCr & vbLf & "                (?:\n[ ]*)?                 # one optional newline followed by spaces" & vbCr & vbLf & vbCr & vbLf & "                \[" & vbCr & vbLf & "                    (.*?)                   # id = $3" & vbCr & vbLf & "                \]" & vbCr & vbLf & "            )", GetNestedBracketsPattern()), RegexOptions.Singleline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        Private Shared _anchorInline As New Regex(String.Format(vbCr & vbLf & "                (                           # wrap whole match in $1" & vbCr & vbLf & "                    \[" & vbCr & vbLf & "                        ({0})               # link text = $2" & vbCr & vbLf & "                    \]" & vbCr & vbLf & "                    \(                      # literal paren" & vbCr & vbLf & "                        [ ]*" & vbCr & vbLf & "                        ({1})               # href = $3" & vbCr & vbLf & "                        [ ]*" & vbCr & vbLf & "                        (                   # $4" & vbCr & vbLf & "                        (['""])           # quote char = $5" & vbCr & vbLf & "                        (.*?)               # title = $6" & vbCr & vbLf & "                        \5                  # matching quote" & vbCr & vbLf & "                        [ ]*                # ignore any spaces between closing quote and )" & vbCr & vbLf & "                        )?                  # title is optional" & vbCr & vbLf & "                    \)" & vbCr & vbLf & "                )", GetNestedBracketsPattern(), GetNestedParensPattern()), RegexOptions.Singleline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        Private Shared _anchorRefShortcut As New Regex(vbCr & vbLf & "            (                               # wrap whole match in $1" & vbCr & vbLf & "              \[" & vbCr & vbLf & "                 ([^\[\]]+)                 # link text = $2; can't contain [ or ]" & vbCr & vbLf & "              \]" & vbCr & vbLf & "            )", RegexOptions.Singleline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        ''' <summary>
        ''' Turn Markdown link shortcuts into HTML anchor tags
        ''' </summary>
        ''' <remarks>
        ''' [link text](url "title") 
        ''' [link text][id] 
        ''' [id] 
        ''' </remarks>
        Private Function DoAnchors(text As String) As String
            ' First, handle reference-style links: [link text] [id]
            text = _anchorRef.Replace(text, New MatchEvaluator(AddressOf AnchorRefEvaluator))

            ' Next, inline-style links: [link text](url "optional title") or [link text](url "optional title")
            text = _anchorInline.Replace(text, New MatchEvaluator(AddressOf AnchorInlineEvaluator))

            '  Last, handle reference-style shortcuts: [link text]
            '  These must come last in case you've also got [link test][1]
            '  or [link test](/foo)
            text = _anchorRefShortcut.Replace(text, New MatchEvaluator(AddressOf AnchorRefShortcutEvaluator))
            Return text
        End Function

        Private Function SaveFromAutoLinking(s As String) As String
            Return s.Replace("://", AutoLinkPreventionMarker)
        End Function

        Private Function AnchorRefEvaluator(match As Match) As String
            Dim wholeMatch As String = match.Groups(1).Value
            Dim linkText As String = SaveFromAutoLinking(match.Groups(2).Value)
            Dim linkID As String = match.Groups(3).Value.ToLowerInvariant()

            Dim result As String

            ' for shortcut links like [this][].
            If linkID = "" Then
                linkID = linkText.ToLowerInvariant()
            End If

            If _urls.ContainsKey(linkID) Then
                Dim url As String = _urls(linkID)

                url = EncodeProblemUrlChars(url)
                url = EscapeBoldItalic(url)
                result = (Convert.ToString("<a href=""") & url) + """"

                If _titles.ContainsKey(linkID) Then
                    Dim title As String = AttributeEncode(_titles(linkID))
                    title = AttributeEncode(EscapeBoldItalic(title))
                    result += (Convert.ToString(" title=""") & title) + """"
                End If

                result += (Convert.ToString(">") & linkText) + "</a>"
            Else
                result = wholeMatch
            End If

            Return result
        End Function

        Private Function AnchorRefShortcutEvaluator(match As Match) As String
            Dim wholeMatch As String = match.Groups(1).Value
            Dim linkText As String = SaveFromAutoLinking(match.Groups(2).Value)
            Dim linkID As String = Regex.Replace(linkText.ToLowerInvariant(), "[ ]*\n[ ]*", " ")
            ' lower case and remove newlines / extra spaces
            Dim result As String

            If _urls.ContainsKey(linkID) Then
                Dim url As String = _urls(linkID)

                url = EncodeProblemUrlChars(url)
                url = EscapeBoldItalic(url)
                result = (Convert.ToString("<a href=""") & url) + """"

                If _titles.ContainsKey(linkID) Then
                    Dim title As String = AttributeEncode(_titles(linkID))
                    title = EscapeBoldItalic(title)
                    result += (Convert.ToString(" title=""") & title) + """"
                End If

                result += (Convert.ToString(">") & linkText) + "</a>"
            Else
                result = wholeMatch
            End If

            Return result
        End Function


        Private Function AnchorInlineEvaluator(match As Match) As String
            Dim linkText As String = SaveFromAutoLinking(match.Groups(2).Value)
            Dim url As String = match.Groups(3).Value
            Dim title As String = match.Groups(6).Value
            Dim result As String

            url = EncodeProblemUrlChars(url)
            url = EscapeBoldItalic(url)
            If url.StartsWith("<") AndAlso url.EndsWith(">") Then
                url = url.Substring(1, url.Length - 2)
            End If
            ' remove <>'s surrounding URL, if present            
            result = String.Format("<a href=""{0}""", url)

            If Not [String].IsNullOrEmpty(title) Then
                title = AttributeEncode(title)
                title = EscapeBoldItalic(title)
                result += String.Format(" title=""{0}""", title)
            End If

            result += String.Format(">{0}</a>", linkText)
            Return result
        End Function

        Private Shared _imagesRef As New Regex(vbCr & vbLf & "                    (               # wrap whole match in $1" & vbCr & vbLf & "                    !\[" & vbCr & vbLf & "                        (.*?)       # alt text = $2" & vbCr & vbLf & "                    \]" & vbCr & vbLf & vbCr & vbLf & "                    [ ]?            # one optional space" & vbCr & vbLf & "                    (?:\n[ ]*)?     # one optional newline followed by spaces" & vbCr & vbLf & vbCr & vbLf & "                    \[" & vbCr & vbLf & "                        (.*?)       # id = $3" & vbCr & vbLf & "                    \]" & vbCr & vbLf & vbCr & vbLf & "                    )", RegexOptions.IgnorePatternWhitespace Or RegexOptions.Singleline Or RegexOptions.Compiled)

        Private Shared _imagesInline As New Regex([String].Format(vbCr & vbLf & "              (                     # wrap whole match in $1" & vbCr & vbLf & "                !\[" & vbCr & vbLf & "                    (.*?)           # alt text = $2" & vbCr & vbLf & "                \]" & vbCr & vbLf & "                \s?                 # one optional whitespace character" & vbCr & vbLf & "                \(                  # literal paren" & vbCr & vbLf & "                    [ ]*" & vbCr & vbLf & "                    ({0})           # href = $3" & vbCr & vbLf & "                    [ ]*" & vbCr & vbLf & "                    (               # $4" & vbCr & vbLf & "                    (['""])       # quote char = $5" & vbCr & vbLf & "                    (.*?)           # title = $6" & vbCr & vbLf & "                    \5              # matching quote" & vbCr & vbLf & "                    [ ]*" & vbCr & vbLf & "                    )?              # title is optional" & vbCr & vbLf & "                \)" & vbCr & vbLf & "              )", GetNestedParensPattern()), RegexOptions.IgnorePatternWhitespace Or RegexOptions.Singleline Or RegexOptions.Compiled)

        ''' <summary>
        ''' Turn Markdown image shortcuts into HTML img tags. 
        ''' </summary>
        ''' <remarks>
        ''' ![alt text][id]
        ''' ![alt text](url "optional title")
        ''' </remarks>
        Private Function DoImages(text As String) As String
            ' First, handle reference-style labeled images: ![alt text][id]
            text = _imagesRef.Replace(text, New MatchEvaluator(AddressOf ImageReferenceEvaluator))

            ' Next, handle inline images:  ![alt text](url "optional title")
            ' Don't forget: encode * and _
            text = _imagesInline.Replace(text, New MatchEvaluator(AddressOf ImageInlineEvaluator))

            Return text
        End Function

        ' This prevents the creation of horribly broken HTML when some syntax ambiguities
        ' collide. It likely still doesn't do what the user meant, but at least we're not
        ' outputting garbage.
        Private Function EscapeImageAltText(s As String) As String
            s = EscapeBoldItalic(s)
            s = Regex.Replace(s, "[\[\]()]", Function(m) _escapeTable(m.ToString()))
            Return s
        End Function

        Private Function ImageReferenceEvaluator(match As Match) As String
            Dim wholeMatch As String = match.Groups(1).Value
            Dim altText As String = match.Groups(2).Value
            Dim linkID As String = match.Groups(3).Value.ToLowerInvariant()

            ' for shortcut links like ![this][].
            If linkID = "" Then
                linkID = altText.ToLowerInvariant()
            End If

            If _urls.ContainsKey(linkID) Then
                Dim url As String = _urls(linkID)
                Dim title As String = Nothing

                If _titles.ContainsKey(linkID) Then
                    title = _titles(linkID)
                End If

                Return ImageTag(url, altText, title)
            Else
                ' If there's no such link ID, leave intact:
                Return wholeMatch
            End If
        End Function

        Private Function ImageInlineEvaluator(match As Match) As String
            Dim alt As String = match.Groups(2).Value
            Dim url As String = match.Groups(3).Value
            Dim title As String = match.Groups(6).Value

            If url.StartsWith("<") AndAlso url.EndsWith(">") Then
                url = url.Substring(1, url.Length - 2)
            End If
            ' Remove <>'s surrounding URL, if present
            Return ImageTag(url, alt, title)
        End Function

        Private Function ImageTag(url As String, altText As String, title As String) As String
            altText = EscapeImageAltText(AttributeEncode(altText))
            url = EncodeProblemUrlChars(url)
            url = EscapeBoldItalic(url)
            Dim result = String.Format("<img src=""{0}"" alt=""{1}""", url, altText)
            If Not [String].IsNullOrEmpty(title) Then
                title = AttributeEncode(EscapeBoldItalic(title))
                result += String.Format(" title=""{0}""", title)
            End If
            result += _emptyElementSuffix
            Return result
        End Function

        Private Shared _headerSetext As New Regex(vbCr & vbLf & "                ^(.+?)" & vbCr & vbLf & "                [ ]*" & vbCr & vbLf & "                \n" & vbCr & vbLf & "                (=+|-+)     # $1 = string of ='s or -'s" & vbCr & vbLf & "                [ ]*" & vbCr & vbLf & "                \n+", RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        Private Shared _headerAtx As New Regex(vbCr & vbLf & "                ^(\#{1,6})  # $1 = string of #'s" & vbCr & vbLf & "                [ ]*" & vbCr & vbLf & "                (.+?)       # $2 = Header text" & vbCr & vbLf & "                [ ]*" & vbCr & vbLf & "                \#*         # optional closing #'s (not counted)" & vbCr & vbLf & "                \n+", RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        ''' <summary>
        ''' Turn Markdown headers into HTML header tags
        ''' </summary>
        ''' <remarks>
        ''' Header 1  
        ''' ========  
        ''' 
        ''' Header 2  
        ''' --------  
        ''' 
        ''' # Header 1  
        ''' ## Header 2  
        ''' ## Header 2 with closing hashes ##  
        ''' ...  
        ''' ###### Header 6  
        ''' </remarks>
        Private Function DoHeaders(text As String) As String
            text = _headerSetext.Replace(text, New MatchEvaluator(AddressOf SetextHeaderEvaluator))
            text = _headerAtx.Replace(text, New MatchEvaluator(AddressOf AtxHeaderEvaluator))
            Return text
        End Function

        Private Function SetextHeaderEvaluator(match As Match) As String
            Dim header As String = match.Groups(1).Value
            Dim level As Integer = If(match.Groups(2).Value.StartsWith("="), 1, 2)
            Return String.Format("<h{1}>{0}</h{1}>" & vbLf & vbLf, RunSpanGamut(header), level)
        End Function

        Private Function AtxHeaderEvaluator(match As Match) As String
            Dim header As String = match.Groups(2).Value
            Dim level As Integer = match.Groups(1).Value.Length
            Return String.Format("<h{1}>{0}</h{1}>" & vbLf & vbLf, RunSpanGamut(header), level)
        End Function


        Private Shared _horizontalRules As New Regex(vbCr & vbLf & "            ^[ ]{0,3}         # Leading space" & vbCr & vbLf & "                ([-*_])       # $1: First marker" & vbCr & vbLf & "                (?>           # Repeated marker group" & vbCr & vbLf & "                    [ ]{0,2}  # Zero, one, or two spaces." & vbCr & vbLf & "                    \1        # Marker character" & vbCr & vbLf & "                ){2,}         # Group repeated at least twice" & vbCr & vbLf & "                [ ]*          # Trailing spaces" & vbCr & vbLf & "                $             # End of line." & vbCr & vbLf & "            ", RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        ''' <summary>
        ''' Turn Markdown horizontal rules into HTML hr tags
        ''' </summary>
        ''' <remarks>
        ''' ***  
        ''' * * *  
        ''' ---
        ''' - - -
        ''' </remarks>
        Private Function DoHorizontalRules(text As String) As String
            Return _horizontalRules.Replace(text, (Convert.ToString("<hr") & _emptyElementSuffix) + vbLf)
        End Function

        Private Shared _wholeList As String = String.Format(vbCr & vbLf & "            (                               # $1 = whole list" & vbCr & vbLf & "              (                             # $2" & vbCr & vbLf & "                [ ]{{0,{1}}}" & vbCr & vbLf & "                ({0})                       # $3 = first list item marker" & vbCr & vbLf & "                [ ]+" & vbCr & vbLf & "              )" & vbCr & vbLf & "              (?s:.+?)" & vbCr & vbLf & "              (                             # $4" & vbCr & vbLf & "                  \z" & vbCr & vbLf & "                |" & vbCr & vbLf & "                  \n{{2,}}" & vbCr & vbLf & "                  (?=\S)" & vbCr & vbLf & "                  (?!                       # Negative lookahead for another list item marker" & vbCr & vbLf & "                    [ ]*" & vbCr & vbLf & "                    {0}[ ]+" & vbCr & vbLf & "                  )" & vbCr & vbLf & "              )" & vbCr & vbLf & "            )", String.Format("(?:{0}|{1})", _markerUL, _markerOL), _tabWidth - 1)

        Private Shared _listNested As New Regex(Convert.ToString("^") & _wholeList, RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        Private Shared _listTopLevel As New Regex(Convert.ToString("(?:(?<=\n\n)|\A\n?)") & _wholeList, RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        ''' <summary>
        ''' Turn Markdown lists into HTML ul and ol and li tags
        ''' </summary>
        Private Function DoLists(text As String, Optional isInsideParagraphlessListItem As Boolean = False) As String
            ' We use a different prefix before nested lists than top-level lists.
            ' See extended comment in _ProcessListItems().
            If _listLevel > 0 Then
                text = _listNested.Replace(text, GetListEvaluator(isInsideParagraphlessListItem))
            Else
                text = _listTopLevel.Replace(text, GetListEvaluator(False))
            End If

            Return text
        End Function

        Private Function GetListEvaluator(Optional isInsideParagraphlessListItem As Boolean = False) As MatchEvaluator
            Return New MatchEvaluator(Function(match)
                                          Dim list As String = match.Groups(1).Value
                                          Dim listType As String = If(Regex.IsMatch(match.Groups(3).Value, _markerUL), "ul", "ol")
                                          Dim result As String

                                          result = ProcessListItems(list, If(listType = "ul", _markerUL, _markerOL), isInsideParagraphlessListItem)

                                          result = String.Format("<{0}>" & vbLf & "{1}</{0}>" & vbLf, listType, result)
                                          Return result

                                      End Function)
        End Function

        ''' <summary>
        ''' Process the contents of a single ordered or unordered list, splitting it
        ''' into individual list items.
        ''' </summary>
        Private Function ProcessListItems(list As String, marker As String, Optional isInsideParagraphlessListItem As Boolean = False) As String
            ' The listLevel global keeps track of when we're inside a list.
            ' Each time we enter a list, we increment it; when we leave a list,
            ' we decrement. If it's zero, we're not in a list anymore.

            ' We do this because when we're not inside a list, we want to treat
            ' something like this:

            '    I recommend upgrading to version
            '    8. Oops, now this line is treated
            '    as a sub-list.

            ' As a single paragraph, despite the fact that the second line starts
            ' with a digit-period-space sequence.

            ' Whereas when we're inside a list (or sub-list), that line will be
            ' treated as the start of a sub-list. What a kludge, huh? This is
            ' an aspect of Markdown's syntax that's hard to parse perfectly
            ' without resorting to mind-reading. Perhaps the solution is to
            ' change the syntax rules such that sub-lists must start with a
            ' starting cardinal number; e.g. "1." or "a.".

            _listLevel += 1

            ' Trim trailing blank lines:
            list = Regex.Replace(list, "\n{2,}\z", vbLf)

            Dim pattern As String = String.Format("(^[ ]*)                    # leading whitespace = $1" & vbCr & vbLf & "                ({0}) [ ]+                 # list marker = $2" & vbCr & vbLf & "                ((?s:.+?)                  # list item text = $3" & vbCr & vbLf & "                (\n+))      " & vbCr & vbLf & "                (?= (\z | \1 ({0}) [ ]+))", marker)

            Dim lastItemHadADoubleNewline As Boolean = False

            ' has to be a closure, so subsequent invocations can share the bool
            Dim ListItemEvaluator As MatchEvaluator = Function(match As Match)
                                                          Dim item As String = match.Groups(3).Value

                                                          Dim endsWithDoubleNewline As Boolean = item.EndsWith(vbLf & vbLf)
                                                          Dim containsDoubleNewline As Boolean = endsWithDoubleNewline OrElse item.Contains(vbLf & vbLf)

                                                          If containsDoubleNewline OrElse lastItemHadADoubleNewline Then
                                                              ' we could correct any bad indentation here..
                                                              item = RunBlockGamut(Outdent(item) & Convert.ToString(vbLf), unhash:=False)
                                                          Else
                                                              ' recursion for sub-lists
                                                              item = DoLists(Outdent(item), isInsideParagraphlessListItem:=True)
                                                              item = item.TrimEnd(ControlChars.Lf)
                                                              If Not isInsideParagraphlessListItem Then
                                                                  ' only the outer-most item should run this, otherwise it's run multiple times for the inner ones
                                                                  item = RunSpanGamut(item)
                                                              End If
                                                          End If
                                                          lastItemHadADoubleNewline = endsWithDoubleNewline
                                                          Return String.Format("<li>{0}</li>" & vbLf, item)

                                                      End Function

            list = Regex.Replace(list, pattern, ListItemEvaluator, RegexOptions.IgnorePatternWhitespace Or RegexOptions.Multiline)
            _listLevel -= 1
            Return list
        End Function

        Private Shared _codeBlock As New Regex(String.Format(vbCr & vbLf & "                    (?:\n\n|\A\n?)" & vbCr & vbLf & "                    (                        # $1 = the code block -- one or more lines, starting with a space" & vbCr & vbLf & "                    (?:" & vbCr & vbLf & "                        (?:[ ]{{{0}}})       # Lines must start with a tab-width of spaces" & vbCr & vbLf & "                        .*\n+" & vbCr & vbLf & "                    )+" & vbCr & vbLf & "                    )" & vbCr & vbLf & "                    ((?=^[ ]{{0,{0}}}[^ \t\n])|\Z) # Lookahead for non-space at line-start, or end of doc", _tabWidth), RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

        ''' <summary>
        ''' /// Turn Markdown 4-space indented code into HTML pre code blocks
        ''' </summary>
        Private Function DoCodeBlocks(text As String) As String
            text = _codeBlock.Replace(text, New MatchEvaluator(AddressOf CodeBlockEvaluator))
            Return text
        End Function

        Private Function CodeBlockEvaluator(match As Match) As String
            Dim codeBlock As String = match.Groups(1).Value

            codeBlock = EncodeCode(Outdent(codeBlock))
            codeBlock = _newlinesLeadingTrailing.Replace(codeBlock, "")

            Return String.Concat(vbLf & vbLf & "<pre><code>", codeBlock, vbLf & "</code></pre>" & vbLf & vbLf)
        End Function

        Private Shared _codeSpan As New Regex(vbCr & vbLf & "                    (?<![\\`])   # Character before opening ` can't be a backslash or backtick" & vbCr & vbLf & "                    (`+)      # $1 = Opening run of `" & vbCr & vbLf & "                    (?!`)     # and no more backticks -- match the full run" & vbCr & vbLf & "                    (.+?)     # $2 = The code block" & vbCr & vbLf & "                    (?<!`)" & vbCr & vbLf & "                    \1" & vbCr & vbLf & "                    (?!`)", RegexOptions.IgnorePatternWhitespace Or RegexOptions.Singleline Or RegexOptions.Compiled)

        ''' <summary>
        ''' Turn Markdown `code spans` into HTML code tags
        ''' </summary>
        Private Function DoCodeSpans(text As String) As String
            '    * You can use multiple backticks as the delimiters if you want to
            '        include literal backticks in the code span. So, this input:
            '
            '        Just type ``foo `bar` baz`` at the prompt.
            '
            '        Will translate to:
            '
            '          <p>Just type <code>foo `bar` baz</code> at the prompt.</p>
            '
            '        There's no arbitrary limit to the number of backticks you
            '        can use as delimters. If you need three consecutive backticks
            '        in your code, use four for delimiters, etc.
            '
            '    * You can use spaces to get literal backticks at the edges:
            '
            '          ... type `` `bar` `` ...
            '
            '        Turns to:
            '
            '          ... type <code>`bar`</code> ...         
            '

            Return _codeSpan.Replace(text, New MatchEvaluator(AddressOf CodeSpanEvaluator))
        End Function

        Private Function CodeSpanEvaluator(match As Match) As String
            Dim span As String = match.Groups(2).Value
            span = Regex.Replace(span, "^[ ]*", "")
            ' leading whitespace
            span = Regex.Replace(span, "[ ]*$", "")
            ' trailing whitespace
            span = EncodeCode(span)
            span = SaveFromAutoLinking(span)
            ' to prevent auto-linking. Not necessary in code *blocks*, but in code spans.
            Return String.Concat("<code>", span, "</code>")
        End Function


        Private Shared _bold As New Regex("(\*\*|__) (?=\S) (.+?[*_]*) (?<=\S) \1", RegexOptions.IgnorePatternWhitespace Or RegexOptions.Singleline Or RegexOptions.Compiled)
        Private Shared _strictBold As New Regex("(^|[\W_])(?:(?!\1)|(?=^))(\*|_)\2(?=\S)(.*?\S)\2\2(?!\2)(?=[\W_]|$)", RegexOptions.Singleline Or RegexOptions.Compiled)

        Private Shared _italic As New Regex("(\*|_) (?=\S) (.+?) (?<=\S) \1", RegexOptions.IgnorePatternWhitespace Or RegexOptions.Singleline Or RegexOptions.Compiled)
        Private Shared _strictItalic As New Regex("(^|[\W_])(?:(?!\1)|(?=^))(\*|_)(?=\S)((?:(?!\2).)*?\S)\2(?!\2)(?=[\W_]|$)", RegexOptions.Singleline Or RegexOptions.Compiled)

        ''' <summary>
        ''' Turn Markdown *italics* and **bold** into HTML strong and em tags
        ''' </summary>
        Private Function DoItalicsAndBold(text As String) As String

            ' <strong> must go first, then <em>
            If _strictBoldItalic Then
                text = _strictBold.Replace(text, "$1<strong>$3</strong>")
                text = _strictItalic.Replace(text, "$1<em>$3</em>")
            Else
                text = _bold.Replace(text, "<strong>$2</strong>")
                text = _italic.Replace(text, "<em>$2</em>")
            End If
            Return text
        End Function

        ''' <summary>
        ''' Turn markdown line breaks (two space at end of line) into HTML break tags
        ''' </summary>
        Private Function DoHardBreaks(text As String) As String
            If _autoNewlines Then
                text = Regex.Replace(text, "\n", String.Format("<br{0}" & vbLf, _emptyElementSuffix))
            Else
                text = Regex.Replace(text, " {2,}\n", String.Format("<br{0}" & vbLf, _emptyElementSuffix))
            End If
            Return text
        End Function

        Private Shared _blockquote As New Regex(vbCr & vbLf & "            (                           # Wrap whole match in $1" & vbCr & vbLf & "                (" & vbCr & vbLf & "                ^[ ]*>[ ]?              # '>' at the start of a line" & vbCr & vbLf & "                    .+\n                # rest of the first line" & vbCr & vbLf & "                (.+\n)*                 # subsequent consecutive lines" & vbCr & vbLf & "                \n*                     # blanks" & vbCr & vbLf & "                )+" & vbCr & vbLf & "            )", RegexOptions.IgnorePatternWhitespace Or RegexOptions.Multiline Or RegexOptions.Compiled)

        ''' <summary>
        ''' Turn Markdown > quoted blocks into HTML blockquote blocks
        ''' </summary>
        Private Function DoBlockQuotes(text As String) As String
            Return _blockquote.Replace(text, New MatchEvaluator(AddressOf BlockQuoteEvaluator))
        End Function

        Private Function BlockQuoteEvaluator(match As Match) As String
            Dim bq As String = match.Groups(1).Value

            bq = Regex.Replace(bq, "^[ ]*>[ ]?", "", RegexOptions.Multiline)
            ' trim one level of quoting
            bq = Regex.Replace(bq, "^[ ]+$", "", RegexOptions.Multiline)
            ' trim whitespace-only lines
            bq = RunBlockGamut(bq)
            ' recurse
            bq = Regex.Replace(bq, "^", "  ", RegexOptions.Multiline)

            ' These leading spaces screw with <pre> content, so we need to fix that:
            bq = Regex.Replace(bq, "(\s*<pre>.+?</pre>)", New MatchEvaluator(AddressOf BlockQuoteEvaluator2), RegexOptions.IgnorePatternWhitespace Or RegexOptions.Singleline)

            bq = String.Format("<blockquote>" & vbLf & "{0}" & vbLf & "</blockquote>", bq)
            Dim key As String = GetHashKey(bq, isHtmlBlock:=True)
            _htmlBlocks(key) = bq

            Return (Convert.ToString(vbLf & vbLf) & key) + vbLf & vbLf
        End Function

        Private Function BlockQuoteEvaluator2(match As Match) As String
            Return Regex.Replace(match.Groups(1).Value, "^  ", "", RegexOptions.Multiline)
        End Function

        Private Const _charInsideUrl As String = "[-A-Z0-9+&@#/%?=~_|\[\]\(\)!:,\.;" + ChrW(26) & "]"
        Private Const _charEndingUrl As String = "[-A-Z0-9+&@#/%=~_|\[\])]"

        Private Shared _autolinkBare As New Regex((Convert.ToString((Convert.ToString("(<|="")?\b(https?|ftp)(://") & _charInsideUrl) + "*") & _charEndingUrl) + ")(?=$|\W)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        Private Shared _endCharRegex As New Regex(_charEndingUrl, RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        Private Shared Function handleTrailingParens(match As Match) As String
            ' The first group is essentially a negative lookbehind -- if there's a < or a =", we don't touch this.
            ' We're not using a *real* lookbehind, because of links with in links, like <a href="http://web.archive.org/web/20121130000728/http://www.google.com/">
            ' With a real lookbehind, the full link would never be matched, and thus the http://www.google.com *would* be matched.
            ' With the simulated lookbehind, the full link *is* matched (just not handled, because of this early return), causing
            ' the google link to not be matched again.
            If match.Groups(1).Success Then
                Return match.Value
            End If

            Dim protocol = match.Groups(2).Value
            Dim link = match.Groups(3).Value
            If Not link.EndsWith(")") Then
                Return "<" + protocol + link + ">"
            End If
            Dim level = 0
            For Each c As Match In Regex.Matches(link, "[()]")
                If c.Value = "(" Then
                    If level <= 0 Then
                        level = 1
                    Else
                        level += 1
                    End If
                Else
                    level -= 1
                End If
            Next
            Dim tail = ""
            If level < 0 Then
                link = Regex.Replace(link, "\){1," + (-level) + "}$", Function(m)
                                                                          tail = m.Value
                                                                          Return ""

                                                                      End Function)
            End If
            If tail.Length > 0 Then
                Dim lastChar = link(link.Length - 1)
                If Not _endCharRegex.IsMatch(lastChar.ToString()) Then
                    tail = lastChar + tail
                    link = link.Substring(0, link.Length - 1)
                End If
            End If
            Return "<" + protocol + link + ">" + tail
        End Function

        ''' <summary>
        ''' Turn angle-delimited URLs into HTML anchor tags
        ''' </summary>
        ''' <remarks>
        ''' &lt;http://www.example.com&gt;
        ''' </remarks>
        Private Function DoAutoLinks(text As String) As String

            If _autoHyperlink Then
                ' fixup arbitrary URLs by adding Markdown < > so they get linked as well
                ' note that at this point, all other URL in the text are already hyperlinked as <a href=""></a>
                ' *except* for the <http://www.foo.com> case
                text = _autolinkBare.Replace(text, AddressOf handleTrailingParens)
            End If

            ' Hyperlinks: <http://foo.com>
            text = Regex.Replace(text, "<((https?|ftp):[^'"">\s]+)>", New MatchEvaluator(AddressOf HyperlinkEvaluator))

            If _linkEmails Then
                ' Email addresses: <address@domain.foo>
                Dim pattern As String = "<" & vbCr & vbLf & "                      (?:mailto:)?" & vbCr & vbLf & "                      (" & vbCr & vbLf & "                        [-.\w]+" & vbCr & vbLf & "                        \@" & vbCr & vbLf & "                        [-a-z0-9]+(\.[-a-z0-9]+)*\.[a-z]+" & vbCr & vbLf & "                      )" & vbCr & vbLf & "                      >"
                text = Regex.Replace(text, pattern, New MatchEvaluator(AddressOf EmailEvaluator), RegexOptions.IgnoreCase Or RegexOptions.IgnorePatternWhitespace)
            End If

            Return text
        End Function

        Private Function HyperlinkEvaluator(match As Match) As String
            Dim link As String = match.Groups(1).Value
            Return String.Format("<a href=""{0}"">{1}</a>", EscapeBoldItalic(EncodeProblemUrlChars(link)), link)
        End Function

        Private Function EmailEvaluator(match As Match) As String
            Dim email As String = Unescape(match.Groups(1).Value)

            '
            '    Input: an email address, e.g. "foo@example.com"
            '
            '    Output: the email address as a mailto link, with each character
            '            of the address encoded as either a decimal or hex entity, in
            '            the hopes of foiling most address harvesting spam bots. E.g.:
            '
            '      <a href="&#x6D;&#97;&#105;&#108;&#x74;&#111;:&#102;&#111;&#111;&#64;&#101;
            '        x&#x61;&#109;&#x70;&#108;&#x65;&#x2E;&#99;&#111;&#109;">&#102;&#111;&#111;
            '        &#64;&#101;x&#x61;&#109;&#x70;&#108;&#x65;&#x2E;&#99;&#111;&#109;</a>
            '
            '    Based by a filter by Matthew Wickline, posted to the BBEdit-Talk
            '    mailing list: <http://tinyurl.com/yu7ue>
            '
            email = Convert.ToString("mailto:") & email

            ' leave ':' alone (to spot mailto: later) 
            email = EncodeEmailAddress(email)

            email = String.Format("<a href=""{0}"">{0}</a>", email)

            ' strip the mailto: from the visible part
            email = Regex.Replace(email, """>.+?:", """>")
            Return email
        End Function


        Private Shared _outDent As New Regex("^[ ]{1," + _tabWidth.ToString + "}", RegexOptions.Multiline Or RegexOptions.Compiled)

        ''' <summary>
        ''' Remove one level of line-leading spaces
        ''' </summary>
        Private Function Outdent(block As String) As String
            Return _outDent.Replace(block, "")
        End Function


#Region "Encoding and Normalization"


        ''' <summary>
        ''' encodes email address randomly  
        ''' roughly 10% raw, 45% hex, 45% dec 
        ''' note that @ is always encoded and : never is
        ''' </summary>
        Private Function EncodeEmailAddress(addr As String) As String
            Dim sb = New StringBuilder(addr.Length * 5)
            Dim rand = New Random()
            Dim r As Integer
            For Each c As Char In addr
                r = rand.[Next](1, 100)
                If (r > 90 OrElse c = ":"c) AndAlso c <> "@"c Then
                    sb.Append(c)
                    ' m
                ElseIf r < 45 Then
                    sb.AppendFormat("&#x{0:x};", AscW(c))
                Else
                    ' &#x6D
                    sb.AppendFormat("&#{0};", AscW(c))
                    ' &#109
                End If
            Next
            Return sb.ToString()
        End Function

        Private Shared _codeEncoder As New Regex("&|<|>|\\|\*|_|\{|\}|\[|\]", RegexOptions.Compiled)

        ''' <summary>
        ''' Encode/escape certain Markdown characters inside code blocks and spans where they are literals
        ''' </summary>
        Private Function EncodeCode(code As String) As String
            Return _codeEncoder.Replace(code, AddressOf EncodeCodeEvaluator)
        End Function
        Private Function EncodeCodeEvaluator(match As Match) As String
            Select Case match.Value
                ' Encode all ampersands; HTML entities are not
                ' entities within a Markdown code span.
                Case "&"
                    Return "&amp;"
                    ' Do the angle bracket song and dance
                Case "<"
                    Return "&lt;"
                Case ">"
                    Return "&gt;"
                Case Else
                    ' escape characters that are magic in Markdown
                    Return _escapeTable(match.Value)
            End Select
        End Function


        Private Shared _amps As New Regex("&(?!((#[0-9]+)|(#[xX][a-fA-F0-9]+)|([a-zA-Z][a-zA-Z0-9]*));)", RegexOptions.ExplicitCapture Or RegexOptions.Compiled)
        Private Shared _angles As New Regex("<(?![A-Za-z/?\$!])", RegexOptions.ExplicitCapture Or RegexOptions.Compiled)

        ''' <summary>
        ''' Encode any ampersands (that aren't part of an HTML entity) and left or right angle brackets
        ''' </summary>
        Private Function EncodeAmpsAndAngles(s As String) As String
            s = _amps.Replace(s, "&amp;")
            s = _angles.Replace(s, "&lt;")
            Return s
        End Function

        Private Shared _backslashEscapes As Regex

        ''' <summary>
        ''' Encodes any escaped characters such as \`, \*, \[ etc
        ''' </summary>
        Private Function EscapeBackslashes(s As String) As String
            Return _backslashEscapes.Replace(s, New MatchEvaluator(AddressOf EscapeBackslashesEvaluator))
        End Function
        Private Function EscapeBackslashesEvaluator(match As Match) As String
            Return _backslashEscapeTable(match.Value)
        End Function

        Private Shared _unescapes As New Regex(ChrW(26) + "E\d+E", RegexOptions.Compiled)

        ''' <summary>
        ''' swap back in all the special characters we've hidden
        ''' </summary>
        Private Function Unescape(s As String) As String
            Return _unescapes.Replace(s, New MatchEvaluator(AddressOf UnescapeEvaluator))
        End Function
        Private Function UnescapeEvaluator(match As Match) As String
            Return _invertedEscapeTable(match.Value)
        End Function


        ''' <summary>
        ''' escapes Bold [ * ] and Italic [ _ ] characters
        ''' </summary>
        Private Function EscapeBoldItalic(s As String) As String
            s = s.Replace("*", _escapeTable("*"))
            s = s.Replace("_", _escapeTable("_"))
            Return s
        End Function

        Private Shared Function AttributeEncode(s As String) As String
            Return s.Replace(">", "&gt;").Replace("<", "&lt;").Replace("""", "&quot;")
        End Function

        Private Shared ReadOnly _problemUrlChars As Char() = """'*()[]$:".ToCharArray()

        ''' <summary>
        ''' hex-encodes some unusual "problem" chars in URLs to avoid URL detection problems 
        ''' </summary>
        Private Function EncodeProblemUrlChars(url As String) As String
            If Not _encodeProblemUrlCharacters Then
                Return url
            End If

            Dim sb = New StringBuilder(url.Length)
            Dim encode As Boolean
            Dim c As Char

            For i As Integer = 0 To url.Length - 1
                c = url(i)
                encode = Array.IndexOf(_problemUrlChars, c) <> -1
                If encode AndAlso c = ":"c AndAlso i < url.Length - 1 Then
                    encode = Not (url(i + 1) = "/"c) AndAlso Not (url(i + 1) >= "0"c AndAlso url(i + 1) <= "9"c)
                End If

                If encode Then
                    sb.Append("%" + [String].Format("{0:x}", AscW(c)))
                Else
                    sb.Append(c)
                End If
            Next

            Return sb.ToString()
        End Function


        ''' <summary>
        ''' Within tags -- meaning between &lt; and &gt; -- encode [\ ` * _] so they 
        ''' don't conflict with their use in Markdown for code, italics and strong. 
        ''' We're replacing each such character with its corresponding hash 
        ''' value; this is likely overkill, but it should prevent us from colliding 
        ''' with the escape values by accident.
        ''' </summary>
        Private Function EscapeSpecialCharsWithinTagAttributes(text As String) As String
            Dim tokens = TokenizeHTML(text)

            ' now, rebuild text from the tokens
            Dim sb = New StringBuilder(text.Length)

            For Each token In tokens
                Dim value As String = token.Value

                If token.Type = TokenType.Tag Then
                    value = value.Replace("\", _escapeTable("\"))

                    If _autoHyperlink AndAlso value.StartsWith("<!") Then
                        ' escape slashes in comments to prevent autolinking there -- http://meta.stackoverflow.com/questions/95987/html-comment-containing-url-breaks-if-followed-by-another-html-comment
                        value = value.Replace("/", _escapeTable("/"))
                    End If

                    value = Regex.Replace(value, "(?<=.)</?code>(?=.)", _escapeTable("`"))
                    value = EscapeBoldItalic(value)
                End If

                sb.Append(value)
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' convert all tabs to _tabWidth spaces; 
        ''' standardizes line endings from DOS (CR LF) or Mac (CR) to UNIX (LF); 
        ''' makes sure text ends with a couple of newlines; 
        ''' removes any blank lines (only spaces) in the text
        ''' </summary>
        Private Function Normalize(text As String) As String
            Dim output = New StringBuilder(text.Length)
            Dim line = New StringBuilder()
            Dim valid As Boolean = False

            For i As Integer = 0 To text.Length - 1
                Select Case text(i)
                    Case ControlChars.Lf
                        If valid Then
                            output.Append(line)
                        End If
                        output.Append(ControlChars.Lf)
                        line.Length = 0
                        valid = False
                        Exit Select
                    Case ControlChars.Cr
                        If (i < text.Length - 1) AndAlso (text(i + 1) <> ControlChars.Lf) Then
                            If valid Then
                                output.Append(line)
                            End If
                            output.Append(ControlChars.Lf)
                            line.Length = 0
                            valid = False
                        End If
                        Exit Select
                    Case ControlChars.Tab
                        Dim width As Integer = (_tabWidth - line.Length Mod _tabWidth)
                        For k As Integer = 0 To width - 1
                            line.Append(" "c)
                        Next
                        Exit Select
                    Case ChrW(26)
                        Exit Select
                    Case Else
                        If Not valid AndAlso text(i) <> " "c Then
                            valid = True
                        End If
                        line.Append(text(i))
                        Exit Select
                End Select
            Next

            If valid Then
                output.Append(line)
            End If
            output.Append(ControlChars.Lf)

            ' add two newlines to the end before return
            Return output.Append(vbLf & vbLf).ToString()
        End Function

#End Region

        ''' <summary>
        ''' this is to emulate what's evailable in PHP
        ''' </summary>
        Private Shared Function RepeatString(text As String, count As Integer) As String
            Dim sb = New StringBuilder(text.Length * count)
            For i As Integer = 0 To count - 1
                sb.Append(text)
            Next
            Return sb.ToString()
        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================

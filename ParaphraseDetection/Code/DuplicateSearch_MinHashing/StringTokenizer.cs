using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringParser
{
 public class StringTokenizerPos
 {
  public int char_pos, line_no;
  public StringTokenizerPos( int c, int l )
  {
   char_pos = c;
   line_no = l;
  }

  /// <summary>
  /// Возвращает номер текущей строки
  /// </summary>
  public int GetLineNumber() { return line_no; }
 }

 public class StringTokenizer
 {
  private int line_no = 1, prev_line_no = 1;
  private string str;
  private int str_len;
  private int pos = 0, prev_pos = -1;
  System.Text.StringBuilder buffer = new StringBuilder();

  // многосимвольные разделители
  private string[] delim_strings;

  public StringTokenizer( string _str )
  {
   str = _str;
   str_len = str.Length;
   delim_strings = new string[] { "..." };
   return;
  }

  public int GetCurrentLineNumber() { return line_no; }

  private bool IsSpace( char c ) { return char.IsWhiteSpace( c ); }

  private const string delimiters = "~?!@#$%^&*()-+=[{]};:|<,>.";
  private bool IsDelimiter( char c )
  {
   return delimiters.IndexOf( c ) != -1;
  }

  public void SkipWhite()
  {
   while( pos < str_len )
    if( IsSpace( str[pos] ) )
    {
     if( str[pos] == '\r' )
      line_no++;
     else if( str[pos] == '\n' )
     {
      if( pos == 0 || str[pos - 1] != '\r' )
       line_no++;
     }

     pos++;
    }
    else
    {
     if( str[pos] == '/' && pos < str_len - 1 )
     {
      if( str[pos + 1] == '/' )
      {
       SkipLine();
       continue;
      }
      else if( str[pos + 1] == '*' )
      {
       pos += 2;
       SkipCommentBlock();
       continue;
      }
      else
       break;
     }

     break;
    }

   return;
  }


  // пропускаем символы до конца строки, обычно это комментарий // ...
  private void SkipLine()
  {
   while( pos < str_len - 1 )
   {
    if( str[pos] == '\r' )
    {
     if( pos < str_len - 1 && str[pos + 1] == '\n' )
      pos++;

     pos++;

     line_no++;
     break;
    }
    else if( str[pos] == '\n' )
    {
     pos++;
     line_no++;
     break;
    }
    else
     pos++;
   }

   return;
  }

  // пропускаем комментарий в /* .... */
  private void SkipCommentBlock()
  {
   while( pos < str_len - 1 )
   {
    if( str[pos] == '*' && pos < str_len - 2 && str[pos + 1] == '/' )
    {
     pos += 2;
     return;
    }
    else
    {
     if( str[pos] == '\n' )
      line_no++;

     pos++;
    }
   }

   throw new ApplicationException( "незакрытый комментарий /* ... */" );
  }

  public bool eof() { return pos >= str_len; }

  private string ReadQuotedToken( char quotation_char )
  {
   prev_pos = pos;
   prev_line_no = line_no;

   buffer.Length = 0;

   buffer.Append( quotation_char );
   pos++;

   while( pos < str_len )
   {
    char c = str[pos];
    pos++;

    if( c == quotation_char )
    {
     buffer.Append( c );
     break;
    }

    buffer.Append( c );
   }

   return buffer.ToString();
  }

  private string TryReadN( int nchar )
  {
   string buf = string.Empty;
   for( int i = 0; i < nchar && !eof(); ++i )
   {
    char c = str[pos++];
    buf = buf + c;
   }

   if( buf.Length == nchar )
    return buf;
   else
    return string.Empty;
  }


  public string read()
  {
   prev_pos = pos;
   prev_line_no = line_no;

   buffer.Length = 0;

   SkipWhite();

   if( pos < str_len && str[pos] == '\'' )
    return ReadQuotedToken( str[pos] );

   while( pos < str_len )
   {
    char c = str[pos];
    if( IsSpace( c ) )
    {
     if( c == '\n' )
      line_no++;
     else if( c == '\r' )
     {
      line_no++;
      pos++; // пропускаем \n
     }

     pos++;
     break;
    }
    else if( IsDelimiter( c ) )
    {
     if( c == '\n' )
      line_no++;
     else if( c == '\r' )
     {
      line_no++;
      pos++; // пропускаем \n
     }

     int pos0 = pos;
     foreach( string d in delim_strings )
      if( c == d[0] )
      {
       // многосимвольный разделитель?
       string s = string.Format( "{1}", c, TryReadN( d.Length ) );
       if( d == s && buffer.Length == 0 )
       {
        return d;
       }
      }

     pos = pos0;
     if( buffer.Length == 0 )
     {
      buffer.Append( c );
      pos++;
     }

     break;
    }
    else
    {
     buffer.Append( c );
     pos++;
    }
   }

   return buffer.ToString();
  }

  public void back()
  {
   if( prev_pos == -1 )
    throw new ApplicationException( "Parsing error: can not move the pointer backward" );

   pos = prev_pos;
   prev_pos = -1;

   line_no = prev_line_no;
   prev_line_no = -1;

   return;
  }

  public void read_it( string required )
  {
   string t = read();
   if( t != required )
    throw new ApplicationException( string.Format( "Parsing error: {0} is required, {1} is extracted from the string", required, t ) );

   return;
  }



  public bool probe( string required )
  {
   string t = read();
   if( t != required )
   {
    back();
    return false;
   }
   else
   {
    return true;
   }
  }

  public StringTokenizerPos tellp()
  {
   return new StringTokenizerPos( pos, line_no );
  }

  public void seekp( StringTokenizerPos where )
  {
   prev_pos = pos;
   prev_line_no = line_no;
   line_no = where.line_no;
   pos = where.char_pos;
   return;
  }

  public string ReadStringUntill( string terminator )
  {
   int beg = pos;
   while( !eof() )
   {
    StringTokenizerPos b = tellp();
    string t = read();
    if( t == terminator )
    {
     seekp( b );
     return str.Substring( beg, b.char_pos - beg );
    }
   }

   throw new ApplicationException( string.Format( "Не могу найти токен {0}", terminator ) );
  }

  public static string Strip( string str, char c )
  {
   if( str.Length > 2 && str[0] == c && str[str.Length - 1] == c )
    return str.Substring( 1, str.Length - 2 );
   else
    return str;
  }

  public static string UnQuote( string s )
  {
   if( s.Length > 0 )
   {
    if( s[0] == '"' )
     return Strip( s, '"' );
    else if( s[0] == '\'' )
     return Strip( s, '\'' );
   }

   return s;
  }

 }

}

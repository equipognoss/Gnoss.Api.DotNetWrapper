using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Helpers
{

    /// <summary>
    /// Enumeration with the posible transformation of an image
    /// </summary>
    public enum ImageTransformationType
    {
        /// <summary>
        /// Resize keeping the aspect ratio to height
        /// </summary>
        ResizeToHeight = 0,

        /// <summary>
        /// Resize keeping the aspect ratio to width
        /// </summary>
        ResizeToWidth = 1,

        /// <summary>
        /// Resize to the indicated size, crop the image and take the top of the image if it is vertical, or the central part if its horizontal
        /// </summary>
        Crop = 2,

        /// <summary>
        /// Resize the image keeping the aspect ratio without exceeding the width or height indicated
        /// </summary>
        ResizeToHeightAndWidth = 3
    }

    /// <summary>
    /// Indicates the administration page type
    /// </summary>
    public enum AdministrationPageType
    {
        /// <summary>
        /// 
        /// </summary>
        Design = 0,
        /// <summary>
        /// 
        /// </summary>
        Page = 1,
        /// <summary>
        /// 
        /// </summary>
        Semantic = 2,
        /// <summary>
        /// 
        /// </summary>
        Thesaurus = 3,
        /// <summary>
        /// 
        /// </summary>
        Text = 4
    }

    /// <summary>
    /// Datatypes
    /// </summary>
    public class DataTypes

    {
        /// <summary>
        /// ListString
        /// </summary>
        public static Type ListString = Type.GetType("System.Collections.Generic.List`1[System.String]");

        /// <summary>
        /// String
        /// </summary>
        public static Type String = Type.GetType("System.String");

        /// <summary>
        /// Bool
        /// </summary>
        public static Type Bool = Type.GetType("System.Boolean");

        /// <summary>
        /// OntologyPropertyImage
        /// </summary>
        public static Type OntologyPropertyImage = Type.GetType("Gnoss.ApiWrapper.Model.ImageOntologyProperty");

        /// <summary>
        /// OntologyPropertyListString
        /// </summary>
        public static Type OntologyPropertyListString = Type.GetType("Gnoss.ApiWrapper.Model.ListStringOntologyProperty");

        /// <summary>
        /// OntologyPropertyString
        /// </summary>
        public static Type OntologyPropertyString = Type.GetType("Gnoss.ApiWrapper.Model.StringOntologyProperty");

        /// <summary>
        /// OntologyPropertyDate
        /// </summary>
        public static Type OntologyPropertyDate = Type.GetType("Gnoss.ApiWrapper.Model.DateOntologyProperty");

        /// <summary>
        /// OntologyPropertyBoolean
        /// </summary>
        public static Type OntologyPropertyBoolean = Type.GetType("Gnoss.ApiWrapper.Model.BoolOntologyProperty");
    }

    /// <summary>
    /// Error texts from the API
    /// </summary>
    public class ErrorText
    {
        /// <summary>
        /// Mapping error
        /// </summary>
        public const string Mapping = "Imposible to map, missing parameters";

        /// <summary>
        /// Required parameter for the constructor
        /// </summary>
        public const string RequiredParameterConstructor = "Every constructor parameter must have value";
    }

    /// <summary>
    /// Accurancy of a date
    /// </summary>
    public class DateAccurancy
    {
        /// <summary>
        /// Exact accurancy
        /// </summary>
        public const string Exact = "";

        /// <summary>
        /// Approximate accurancy
        /// </summary>
        public const string Approximate = "ca";

        /// <summary>
        /// Doubtful accurancy. When you aren't sure about the date of an event
        /// </summary>
        public const string Doubtful = "?";
    }

    /// <summary>
    /// Datatypes for dates
    /// </summary>
    public class DateTypes
    {
        /// <summary>
        /// After Christ
        /// </summary>
        public const string AC = "A.C.";

        /// <summary>
        /// After Christ, spanish version
        /// </summary>
        public const string DC = "";

        /// <summary>
        /// Before Christ
        /// </summary>
        public const string BP = "B.P.";
    }

    /// <summary>
    /// Languages
    /// </summary>
    public class Languages
    {
        /// <summary>
        /// Spanish
        /// </summary>
        public const string Spanish = "es";

        /// <summary>
        /// English
        /// </summary>
        public const string English = "en";

        /// <summary>
        /// French
        /// </summary>
        public const string French = "fr";

        /// <summary>
        /// Euskera
        /// </summary>
        public const string Euskera = "eu";

        /// <summary>
        /// Portuguese
        /// </summary>
        public const string Portuguese = "pt";

        /// <summary>
        /// Catalán
        /// </summary>
        public const string Catalan = "ca";

        /// <summary>
        /// Gallego
        /// </summary>
        public const string Gallego = "gl";

        /// <summary>
        /// German
        /// </summary>
        public const string German = "de";

        /// <summary>
        /// Italian
        /// </summary>
        public const string Italian = "it";
    }

    /// <summary>
    /// Types of communities
    /// </summary>
    public class CommunityType
    {
        /// <summary>
        /// Organization community
        /// </summary>
        public const short OrganizationCommunity = 0;

        /// <summary>
        /// Online community
        /// </summary>
        public const short OnlineCommunity = 1;

        /// <summary>
        /// Metacommunity
        /// </summary>
        public const short Metacommunity = 2;

        /// <summary>
        /// University community
        /// </summary>
        public const short University = 3;

        /// <summary>
        /// Expanded education
        /// </summary>
        public const short ExpandedEducation = 4;

        /// <summary>
        /// Catalog community
        /// </summary>
        public const short Catalog = 5;

        /// <summary>
        /// Primary education
        /// </summary>
        public const short PrimaryEducation = 6;

        /// <summary>
        /// Catalog community without members with one resource type
        /// </summary>
        public const short NoSocialCatalogWithAResourceType = 7;

        /// <summary>
        /// Catalog community without members
        /// </summary>
        public const short NoSocialCatalog = 8;
    }

    /// <summary>
    /// Access type of a community
    /// </summary>
    public class AccessType
    {
        /// <summary>
        /// Public community
        /// </summary>
        public const short Public = 0;

        /// <summary>
        /// Private community
        /// </summary>
        public const short Private = 1;

        /// <summary>
        /// Restricted community
        /// </summary>
        public const short Restricted = 2;

        /// <summary>
        /// Reserved community
        /// </summary>
        public const short Reserved = 3;

    }

    /// <summary>
    /// Identity types
    /// </summary>
    public class IdentityTypes
    {
        /// <summary>
        /// Personal
        /// </summary>
        public const short Personal = 0;

        /// <summary>
        /// Personal professional
        /// </summary>
        public const short PersonalProfessional = 1;

        /// <summary>
        /// Corporate professional
        /// </summary>
        public const short CorporateProfessional = 2;

        /// <summary>
        /// Organization
        /// </summary>
        public const short Organization = 3;
    }

    /// <summary>
    /// Identity types for organizations
    /// </summary>
    public class IdentityOrganizationTypes
    {
        /// <summary>
        /// Personal professional
        /// </summary>
        public const short PersonalProfessional = 1;

        /// <summary>
        /// Corporate professional
        /// </summary>
        public const short CorporateProfessional = 2;
    }

    #region Type delimiters

    /// <summary>
    /// Delimiters as char[]
    /// </summary>
    public static class CharArrayDelimiters
    {
        /// <summary>
        /// Space
        /// </summary>
        public static char[] Space = { ' ' };

        /// <summary>
        /// Slash (/)
        /// </summary>
        public static char[] Slash = { '/' };

        /// <summary>
        /// Backslash (\)
        /// </summary>
        public static char[] Backslash = { '\\' };

        /// <summary>
        /// Pipe (|)
        /// </summary>
        public static char[] Pipe = { '|' };

        /// <summary>
        /// Hypen (-)
        /// </summary>
        public static char[] Hypen = { '-' };

        /// <summary>
        /// Underscore (_)
        /// </summary>
        public static char[] Underscore = { '_' };

        /// <summary>
        /// Comma (,)
        /// </summary>
        public static char[] Comma = { ',' };

        /// <summary>
        /// Colon (:)
        /// </summary>
        public static char[] Colon = { ':' };

        /// <summary>
        /// Equal (=)
        /// </summary>
        public static char[] Equal = { '=' };

        /// <summary>
        /// Point (.)
        /// </summary>
        public static char[] Point = { '.' };

        /// <summary>
        /// Semicolon (;)
        /// </summary>
        public static char[] Semicolon = { ';' };
       
    }

    /// <summary>
    /// Delimiters as char
    /// </summary>
    public static class CharDelimiters
    {
        /// <summary>
        /// Space
        /// </summary>
        public static char Space = ' ';

        /// <summary>
        /// Slash (/)
        /// </summary>
        public static char Slash = '/';

        /// <summary>
        /// Backslash (\)
        /// </summary>
        public static char Backslash = '\\';

        /// <summary>
        /// Pipe (|)
        /// </summary>
        public static char Pipe = '|';

        /// <summary>
        /// Guión (-) 
        /// </summary>
        public static char Hypen = '-';

        /// <summary>
        /// Underscore (_)
        /// </summary>
        public static char Underscore = '_';

        /// <summary>
        /// Comma (,)
        /// </summary>
        public static char Comma = ',';

        /// <summary>
        /// Colon (:)
        /// </summary>
        public static char Colon = ':';

        /// <summary>
        /// Equal (=)
        /// </summary>
        public static char Equal = '=';

        /// <summary>
        /// Point (.)
        /// </summary>
        public static char Point = '.' ;

        /// <summary>
        /// Semicolon (;)
        /// </summary>
        public static char Semicolon = ';';
    }

    /// <summary>
    /// Delimiters as string[]
    /// </summary>
    public static class StringArrayDelimiters
    {
        /// <summary>
        /// Espacio
        /// </summary>
        public static string[] Space = { " " };

        /// <summary>
        /// Slash (/)
        /// </summary>
        public static string[] Slash = { "/" };

        /// <summary>
        /// Backslash (\)
        /// </summary>
        public static string[] Backslash = { "\\" };

        /// <summary>
        /// Pipe (|)
        /// </summary>
        public static string[] Pipe = { "|" };

        /// <summary>
        /// Hypen (-)
        /// </summary>
        public static string[] Hypen = { "-" };

        /// <summary>
        /// Underscore (_)
        /// </summary>
        public static string[] Underscore = { "_" };

        /// <summary>
        /// Comma (,)
        /// </summary>
        public static string[] Comma = { "," };

        /// <summary>
        /// Three Pipes (|||)
        /// </summary>
        public static string[] ThreePipes = { "|||" };

        /// <summary>
        /// Colon (:)
        /// </summary>
        public static string[] Colon = { ":" };

        /// <summary>
        /// Equal (=)
        /// </summary>
        public static string[] Equal = { "=" };

        /// <summary>
        /// Point (.)
        /// </summary>
        public static string[] Point = { "." };

        /// <summary>
        /// Semicolon (;)
        /// </summary>
        public static string[] Semicolon = { ";" };
    }

    /// <summary>
    /// Delimiters as string
    /// </summary>
    public static class StringDelimiters
    {
        /// <summary>
        /// Space
        /// </summary>
        public static string Space = " ";

        /// <summary>
        /// Slash (/)
        /// </summary>
        public static string Slash = "/";

        /// <summary>
        /// Backslash (\)
        /// </summary>
        public static string Backslash = "\\";

        /// <summary>
        /// Pipe (|)
        /// </summary>
        public static string Pipe = "|";

        /// <summary>
        /// Hypen (-)
        /// </summary>
        public static string Hypen = "-";

        /// <summary>
        /// Underscore (_)
        /// </summary>
        public static string Underscore = "_";

        /// <summary>
        /// Comma (,)
        /// </summary>
        public static string Comma = ",";

        /// <summary>
        /// Three Pipes (|||)
        /// </summary>
        public static string ThreePipes = "|||";

        /// <summary>
        /// Colon (:)
        /// </summary>
        public static string Colon = ":";

        /// <summary>
        /// Semicolon (:)
        /// </summary>
        public static string Semicolon = ";";

        /// <summary>
        /// Equal (=)
        /// </summary>
        public static string Equal = "=";

        /// <summary>
        /// Point (.)
        /// </summary>
        public static string Point = ".";
    }


    #endregion
}

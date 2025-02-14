using System.Text;

namespace LibraryManagementSystem.Utilities;
public static class TextUtils
{


    public static string[] CutTextIntoSegments(string text, int maxCharLength, int? maxWordsPerLine = null)
    {
        //Handle edge cases
        if (string.IsNullOrEmpty(text))
            return [];

        if (maxCharLength <= 0)
            throw new ArgumentException("maxCharLength must be greater than 0.", nameof(maxCharLength));
        if (maxWordsPerLine <= 0 && maxWordsPerLine != null)
            throw new ArgumentException("maxWordsPerLine must be greater than 0.", nameof(maxWordsPerLine));

        List<string> textList = [];

        string[] textToWords = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        StringBuilder formingSection = new();

        int wordsPerLine = 0;
        foreach (string word in textToWords)
        {
            if (formingSection.Length + word.Length + 1 > maxCharLength)
            {
                textList.Add(formingSection.ToString().Trim());
                formingSection.Clear();
                wordsPerLine = 0;


            }
            else
            {
                if (wordsPerLine == maxWordsPerLine && maxWordsPerLine != null)
                {
                    formingSection.Append('\n');
                    wordsPerLine = 0;
                }
                formingSection.Append(word).Append(' ');
                wordsPerLine++;
            }

        }

        if (formingSection.Length > 0)
        {
            textList.Add(formingSection.ToString().Trim());
        }

        return textList.ToArray();

    }

    public static string[] SegmentNewline(string text)
    {
        return text.Split(@"\n");
    }

    public static string CombineSegments(string[] segments)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < segments.Length; i++)
        {
            stringBuilder.Append(segments[i]);

            stringBuilder.Append('\n');

        }
        return stringBuilder.ToString();
    }


    public static string[] MjdPages()
    {
        List<string> pages =
        [@"
____$$$$$$$$$____ 
____$$$$$$$$$____ 
______$$$$$______ 
______$$$$$______ 
______$$$$$______ 
______$$$$$______ 
____$$$$$$$$$____ 
____$$$$$$$$$____ ",
@"
_$$$$$$___$$$$$$_ 
$$$$$$$$_$$$$$$$$ 
$$$$$$$$$$$$$$$$$ 
_$$$$$$$$$$$$$$$_ 
___$$$$$$$$$$$___ 
_____$$$$$$$_____ 
_______$$$_______ 
________$________ 
",
@"
__$$$$$___$$$$$__ 
__$$$$$$_$$$$$$__ 
___$$$$$$$$$$$___ 
_____$$$$$$$_____ 
______$$$$$______ 
______$$$$$______ 
______$$$$$______ 
_________________ 
_____$$$$$$$_____ 
___$$$$$$$$$$$___ 
__$$$$$___$$$$$__ 
__$$$$$___$$$$$__ 
__$$$$$___$$$$$__ 
___$$$$$$$$$$$___ 
_____$$$$$$$_____ 
_________________ 
__$$$$$___$$$$$__ 
__$$$$$___$$$$$__ 
__$$$$$___$$$$$__ 
__$$$$$___$$$$$__ 
__$$$$$$_$$$$$$__ 
___$$$$$$$$$$$___ 
____$$$$$$$$$____
",
            @"

_______$__________$\n
______$$_____$___$$\n
_______$$_$___$_$$\n
______$$$__$____$$$\n
___$___$$_$$$__$$$$__$\n
__$$___$$$$$__$$$$___$$\n
___$$_$$$$$___$$$$$_$$\n
___$$$$$$$$$_$$$$$$$$$\n
__$$$*****$$$$$*****$$$$$\n
__$*_______*_*_______*$\n
__*____________________*\n
__*_____________    ____*\n
__*_____I love u bbg___*\n
___*__________________*\n
____*_______________*\n
______*___________*\n
________*_______*\n
__________*___*\n
____________*\n
        ",

        @"
        You when im done with you

_______$$$$$
____$$$$$$$$$$
___$$$$$$$$$$$$
__$$$$$$$$$$$$$
_$$$$$$$$$$$$$$
_$$$$$$$$$$$$$$
$$$_$$$$$$$$$$
$$$_$$$$$$$
$$__$$$$$
_$$$$$$$$$$
$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$
$$$$$_$$$$$$$$$$
$$$$$_$$$$$$$$$$$
$$$$___$$$$$$$$$$$
$$$$___$$$$$$$$$$$$
$$$$___$$$$$$$$$$$$$
$$$$___$$$$$$$$$$$$$$
$$$___$$$$$$$$$$$$$$$
$$$$__$$$$$$$$$$$$$$$
_$$$_$$$$$$$$$$$$$$$$
_$$$_$$$$$$$$$$$$$$$
_$$$$$$$$$$$$$$$$$$
_$$$$$$$$$$$$$$$$
_$$$$$$$$$$$$$$$$$
__$$_$$$$$$$$$$$$$$
______$$$$$$$$$$$$$$
______$$$$$$_$$$$$$$
______$$$$$$__$$$$$$$
_____$$$$$$$____$$$$$$
_____$$$$$$______$$$$$
_____$$$$$_______$$$$$
_____$$$$$_______$$$$
_____$$$$$______$$$$$
_____$$$$$______$$$$$
_____$$$$$______$$$$
______$$$$_____$$$$$
______$$$$_____$$$$
______$$$$$____$$$$
______$$$$$$___$$$$
_____$$JK$$$$_$$$$$
_____$$$$$$$$$$$$$$$
______$$__$$$$$$$$$$$
_______$____$$$$$$$$$$$
_______________$__$$$$$$$

        
        ",
        ];

        return pages.ToArray();

    }


    public static string CropString(string text, int maxCharLength)
    {
        return (text.Length > maxCharLength ? text.Substring(0, maxCharLength) : text) + "...";
    }
}
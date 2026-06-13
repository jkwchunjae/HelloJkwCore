using System.Text.RegularExpressions;

namespace ProjectWorldCup.Pages.Wc2026;

public static class Wc2026ScenarioRound32Simulator
{
    private const string ThirdPlaceAssignmentTargetGroups = "ABDEGIKL";

    private static readonly string[] ThirdPlaceCombinationRows =
    {
        "EFGHIJKL:EJIFHGLK",
        "DFGHIJKL:HGIDJFLK",
        "DEGHIJKL:EJIDHGLK",
        "DEFHIJKL:EJIDHFLK",
        "DEFGIJKL:EGIDJFLK",
        "DEFGHJKL:EGJDHFLK",
        "DEFGHIKL:EGIDHFLK",
        "DEFGHIJL:EGJDHFLI",
        "DEFGHIJK:EGJDHFIK",
        "CFGHIJKL:HGICJFLK",
        "CEGHIJKL:EJICHGLK",
        "CEFHIJKL:EJICHFLK",
        "CEFGIJKL:EGICJFLK",
        "CEFGHJKL:EGJCHFLK",
        "CEFGHIKL:EGICHFLK",
        "CEFGHIJL:EGJCHFLI",
        "CEFGHIJK:EGJCHFIK",
        "CDGHIJKL:HGICJDLK",
        "CDFHIJKL:CJIDHFLK",
        "CDFGIJKL:CGIDJFLK",
        "CDFGHJKL:CGJDHFLK",
        "CDFGHIKL:CGIDHFLK",
        "CDFGHIJL:CGJDHFLI",
        "CDFGHIJK:CGJDHFIK",
        "CDEHIJKL:EJICHDLK",
        "CDEGIJKL:EGICJDLK",
        "CDEGHJKL:EGJCHDLK",
        "CDEGHIKL:EGICHDLK",
        "CDEGHIJL:EGJCHDLI",
        "CDEGHIJK:EGJCHDIK",
        "CDEFIJKL:CJEDIFLK",
        "CDEFHJKL:CJEDHFLK",
        "CDEFHIKL:CEIDHFLK",
        "CDEFHIJL:CJEDHFLI",
        "CDEFHIJK:CJEDHFIK",
        "CDEFGJKL:CGEDJFLK",
        "CDEFGIKL:CGEDIFLK",
        "CDEFGIJL:CGEDJFLI",
        "CDEFGIJK:CGEDJFIK",
        "CDEFGHKL:CGEDHFLK",
        "CDEFGHJL:CGJDHFLE",
        "CDEFGHJK:CGJDHFEK",
        "CDEFGHIL:CGEDHFLI",
        "CDEFGHIK:CGEDHFIK",
        "CDEFGHIJ:CGJDHFEI",
        "BFGHIJKL:HJBFIGLK",
        "BEGHIJKL:EJIBHGLK",
        "BEFHIJKL:EJBFIHLK",
        "BEFGIJKL:EJBFIGLK",
        "BEFGHJKL:EJBFHGLK",
        "BEFGHIKL:EGBFIHLK",
        "BEFGHIJL:EJBFHGLI",
        "BEFGHIJK:EJBFHGIK",
        "BDGHIJKL:HJBDIGLK",
        "BDFHIJKL:HJBDIFLK",
        "BDFGIJKL:IGBDJFLK",
        "BDFGHJKL:HGBDJFLK",
        "BDFGHIKL:HGBDIFLK",
        "BDFGHIJL:HGBDJFLI",
        "BDFGHIJK:HGBDJFIK",
        "BDEHIJKL:EJBDIHLK",
        "BDEGIJKL:EJBDIGLK",
        "BDEGHJKL:EJBDHGLK",
        "BDEGHIKL:EGBDIHLK",
        "BDEGHIJL:EJBDHGLI",
        "BDEGHIJK:EJBDHGIK",
        "BDEFIJKL:EJBDIFLK",
        "BDEFHJKL:EJBDHFLK",
        "BDEFHIKL:EIBDHFLK",
        "BDEFHIJL:EJBDHFLI",
        "BDEFHIJK:EJBDHFIK",
        "BDEFGJKL:EGBDJFLK",
        "BDEFGIKL:EGBDIFLK",
        "BDEFGIJL:EGBDJFLI",
        "BDEFGIJK:EGBDJFIK",
        "BDEFGHKL:EGBDHFLK",
        "BDEFGHJL:HGBDJFLE",
        "BDEFGHJK:HGBDJFEK",
        "BDEFGHIL:EGBDHFLI",
        "BDEFGHIK:EGBDHFIK",
        "BDEFGHIJ:HGBDJFEI",
        "BCGHIJKL:HJBCIGLK",
        "BCFHIJKL:HJBCIFLK",
        "BCFGIJKL:IGBCJFLK",
        "BCFGHJKL:HGBCJFLK",
        "BCFGHIKL:HGBCIFLK",
        "BCFGHIJL:HGBCJFLI",
        "BCFGHIJK:HGBCJFIK",
        "BCEHIJKL:EJBCIHLK",
        "BCEGIJKL:EJBCIGLK",
        "BCEGHJKL:EJBCHGLK",
        "BCEGHIKL:EGBCIHLK",
        "BCEGHIJL:EJBCHGLI",
        "BCEGHIJK:EJBCHGIK",
        "BCEFIJKL:EJBCIFLK",
        "BCEFHJKL:EJBCHFLK",
        "BCEFHIKL:EIBCHFLK",
        "BCEFHIJL:EJBCHFLI",
        "BCEFHIJK:EJBCHFIK",
        "BCEFGJKL:EGBCJFLK",
        "BCEFGIKL:EGBCIFLK",
        "BCEFGIJL:EGBCJFLI",
        "BCEFGIJK:EGBCJFIK",
        "BCEFGHKL:EGBCHFLK",
        "BCEFGHJL:HGBCJFLE",
        "BCEFGHJK:HGBCJFEK",
        "BCEFGHIL:EGBCHFLI",
        "BCEFGHIK:EGBCHFIK",
        "BCEFGHIJ:HGBCJFEI",
        "BCDHIJKL:HJBCIDLK",
        "BCDGIJKL:IGBCJDLK",
        "BCDGHJKL:HGBCJDLK",
        "BCDGHIKL:HGBCIDLK",
        "BCDGHIJL:HGBCJDLI",
        "BCDGHIJK:HGBCJDIK",
        "BCDFIJKL:CJBDIFLK",
        "BCDFHJKL:CJBDHFLK",
        "BCDFHIKL:CIBDHFLK",
        "BCDFHIJL:CJBDHFLI",
        "BCDFHIJK:CJBDHFIK",
        "BCDFGJKL:CGBDJFLK",
        "BCDFGIKL:CGBDIFLK",
        "BCDFGIJL:CGBDJFLI",
        "BCDFGIJK:CGBDJFIK",
        "BCDFGHKL:CGBDHFLK",
        "BCDFGHJL:CGBDHFLJ",
        "BCDFGHJK:HGBCJFDK",
        "BCDFGHIL:CGBDHFLI",
        "BCDFGHIK:CGBDHFIK",
        "BCDFGHIJ:HGBCJFDI",
        "BCDEIJKL:EJBCIDLK",
        "BCDEHJKL:EJBCHDLK",
        "BCDEHIKL:EIBCHDLK",
        "BCDEHIJL:EJBCHDLI",
        "BCDEHIJK:EJBCHDIK",
        "BCDEGJKL:EGBCJDLK",
        "BCDEGIKL:EGBCIDLK",
        "BCDEGIJL:EGBCJDLI",
        "BCDEGIJK:EGBCJDIK",
        "BCDEGHKL:EGBCHDLK",
        "BCDEGHJL:HGBCJDLE",
        "BCDEGHJK:HGBCJDEK",
        "BCDEGHIL:EGBCHDLI",
        "BCDEGHIK:EGBCHDIK",
        "BCDEGHIJ:HGBCJDEI",
        "BCDEFJKL:CJBDEFLK",
        "BCDEFIKL:CEBDIFLK",
        "BCDEFIJL:CJBDEFLI",
        "BCDEFIJK:CJBDEFIK",
        "BCDEFHKL:CEBDHFLK",
        "BCDEFHJL:CJBDHFLE",
        "BCDEFHJK:CJBDHFEK",
        "BCDEFHIL:CEBDHFLI",
        "BCDEFHIK:CEBDHFIK",
        "BCDEFHIJ:CJBDHFEI",
        "BCDEFGKL:CGBDEFLK",
        "BCDEFGJL:CGBDJFLE",
        "BCDEFGJK:CGBDJFEK",
        "BCDEFGIL:CGBDEFLI",
        "BCDEFGIK:CGBDEFIK",
        "BCDEFGIJ:CGBDJFEI",
        "BCDEFGHL:CGBDHFLE",
        "BCDEFGHK:CGBDHFEK",
        "BCDEFGHJ:HGBCJFDE",
        "BCDEFGHI:CGBDHFEI",
        "AFGHIJKL:HJIFAGLK",
        "AEGHIJKL:EJIAHGLK",
        "AEFHIJKL:EJIFAHLK",
        "AEFGIJKL:EJIFAGLK",
        "AEFGHJKL:EGJFAHLK",
        "AEFGHIKL:EGIFAHLK",
        "AEFGHIJL:EGJFAHLI",
        "AEFGHIJK:EGJFAHIK",
        "ADGHIJKL:HJIDAGLK",
        "ADFHIJKL:HJIDAFLK",
        "ADFGIJKL:IGJDAFLK",
        "ADFGHJKL:HGJDAFLK",
        "ADFGHIKL:HGIDAFLK",
        "ADFGHIJL:HGJDAFLI",
        "ADFGHIJK:HGJDAFIK",
        "ADEHIJKL:EJIDAHLK",
        "ADEGIJKL:EJIDAGLK",
        "ADEGHJKL:EGJDAHLK",
        "ADEGHIKL:EGIDAHLK",
        "ADEGHIJL:EGJDAHLI",
        "ADEGHIJK:EGJDAHIK",
        "ADEFIJKL:EJIDAFLK",
        "ADEFHJKL:HJEDAFLK",
        "ADEFHIKL:HEIDAFLK",
        "ADEFHIJL:HJEDAFLI",
        "ADEFHIJK:HJEDAFIK",
        "ADEFGJKL:EGJDAFLK",
        "ADEFGIKL:EGIDAFLK",
        "ADEFGIJL:EGJDAFLI",
        "ADEFGIJK:EGJDAFIK",
        "ADEFGHKL:HGEDAFLK",
        "ADEFGHJL:HGJDAFLE",
        "ADEFGHJK:HGJDAFEK",
        "ADEFGHIL:HGEDAFLI",
        "ADEFGHIK:HGEDAFIK",
        "ADEFGHIJ:HGJDAFEI",
        "ACGHIJKL:HJICAGLK",
        "ACFHIJKL:HJICAFLK",
        "ACFGIJKL:IGJCAFLK",
        "ACFGHJKL:HGJCAFLK",
        "ACFGHIKL:HGICAFLK",
        "ACFGHIJL:HGJCAFLI",
        "ACFGHIJK:HGJCAFIK",
        "ACEHIJKL:EJICAHLK",
        "ACEGIJKL:EJICAGLK",
        "ACEGHJKL:EGJCAHLK",
        "ACEGHIKL:EGICAHLK",
        "ACEGHIJL:EGJCAHLI",
        "ACEGHIJK:EGJCAHIK",
        "ACEFIJKL:EJICAFLK",
        "ACEFHJKL:HJECAFLK",
        "ACEFHIKL:HEICAFLK",
        "ACEFHIJL:HJECAFLI",
        "ACEFHIJK:HJECAFIK",
        "ACEFGJKL:EGJCAFLK",
        "ACEFGIKL:EGICAFLK",
        "ACEFGIJL:EGJCAFLI",
        "ACEFGIJK:EGJCAFIK",
        "ACEFGHKL:HGECAFLK",
        "ACEFGHJL:HGJCAFLE",
        "ACEFGHJK:HGJCAFEK",
        "ACEFGHIL:HGECAFLI",
        "ACEFGHIK:HGECAFIK",
        "ACEFGHIJ:HGJCAFEI",
        "ACDHIJKL:HJICADLK",
        "ACDGIJKL:IGJCADLK",
        "ACDGHJKL:HGJCADLK",
        "ACDGHIKL:HGICADLK",
        "ACDGHIJL:HGJCADLI",
        "ACDGHIJK:HGJCADIK",
        "ACDFIJKL:CJIDAFLK",
        "ACDFHJKL:HJFCADLK",
        "ACDFHIKL:HFICADLK",
        "ACDFHIJL:HJFCADLI",
        "ACDFHIJK:HJFCADIK",
        "ACDFGJKL:CGJDAFLK",
        "ACDFGIKL:CGIDAFLK",
        "ACDFGIJL:CGJDAFLI",
        "ACDFGIJK:CGJDAFIK",
        "ACDFGHKL:HGFCADLK",
        "ACDFGHJL:CGJDAFLH",
        "ACDFGHJK:HGJCAFDK",
        "ACDFGHIL:HGFCADLI",
        "ACDFGHIK:HGFCADIK",
        "ACDFGHIJ:HGJCAFDI",
        "ACDEIJKL:EJICADLK",
        "ACDEHJKL:HJECADLK",
        "ACDEHIKL:HEICADLK",
        "ACDEHIJL:HJECADLI",
        "ACDEHIJK:HJECADIK",
        "ACDEGJKL:EGJCADLK",
        "ACDEGIKL:EGICADLK",
        "ACDEGIJL:EGJCADLI",
        "ACDEGIJK:EGJCADIK",
        "ACDEGHKL:HGECADLK",
        "ACDEGHJL:HGJCADLE",
        "ACDEGHJK:HGJCADEK",
        "ACDEGHIL:HGECADLI",
        "ACDEGHIK:HGECADIK",
        "ACDEGHIJ:HGJCADEI",
        "ACDEFJKL:CJEDAFLK",
        "ACDEFIKL:CEIDAFLK",
        "ACDEFIJL:CJEDAFLI",
        "ACDEFIJK:CJEDAFIK",
        "ACDEFHKL:HEFCADLK",
        "ACDEFHJL:HJFCADLE",
        "ACDEFHJK:HJECAFDK",
        "ACDEFHIL:HEFCADLI",
        "ACDEFHIK:HEFCADIK",
        "ACDEFHIJ:HJECAFDI",
        "ACDEFGKL:CGEDAFLK",
        "ACDEFGJL:CGJDAFLE",
        "ACDEFGJK:CGJDAFEK",
        "ACDEFGIL:CGEDAFLI",
        "ACDEFGIK:CGEDAFIK",
        "ACDEFGIJ:CGJDAFEI",
        "ACDEFGHL:HGFCADLE",
        "ACDEFGHK:HGECAFDK",
        "ACDEFGHJ:HGJCAFDE",
        "ACDEFGHI:HGECAFDI",
        "ABGHIJKL:HJBAIGLK",
        "ABFHIJKL:HJBAIFLK",
        "ABFGIJKL:IJBFAGLK",
        "ABFGHJKL:HJBFAGLK",
        "ABFGHIKL:HGBAIFLK",
        "ABFGHIJL:HJBFAGLI",
        "ABFGHIJK:HJBFAGIK",
        "ABEHIJKL:EJBAIHLK",
        "ABEGIJKL:EJBAIGLK",
        "ABEGHJKL:EJBAHGLK",
        "ABEGHIKL:EGBAIHLK",
        "ABEGHIJL:EJBAHGLI",
        "ABEGHIJK:EJBAHGIK",
        "ABEFIJKL:EJBAIFLK",
        "ABEFHJKL:EJBFAHLK",
        "ABEFHIKL:EIBFAHLK",
        "ABEFHIJL:EJBFAHLI",
        "ABEFHIJK:EJBFAHIK",
        "ABEFGJKL:EJBFAGLK",
        "ABEFGIKL:EGBAIFLK",
        "ABEFGIJL:EJBFAGLI",
        "ABEFGIJK:EJBFAGIK",
        "ABEFGHKL:EGBFAHLK",
        "ABEFGHJL:HJBFAGLE",
        "ABEFGHJK:HJBFAGEK",
        "ABEFGHIL:EGBFAHLI",
        "ABEFGHIK:EGBFAHIK",
        "ABEFGHIJ:HJBFAGEI",
        "ABDHIJKL:IJBDAHLK",
        "ABDGIJKL:IJBDAGLK",
        "ABDGHJKL:HJBDAGLK",
        "ABDGHIKL:IGBDAHLK",
        "ABDGHIJL:HJBDAGLI",
        "ABDGHIJK:HJBDAGIK",
        "ABDFIJKL:IJBDAFLK",
        "ABDFHJKL:HJBDAFLK",
        "ABDFHIKL:HIBDAFLK",
        "ABDFHIJL:HJBDAFLI",
        "ABDFHIJK:HJBDAFIK",
        "ABDFGJKL:FJBDAGLK",
        "ABDFGIKL:IGBDAFLK",
        "ABDFGIJL:FJBDAGLI",
        "ABDFGIJK:FJBDAGIK",
        "ABDFGHKL:HGBDAFLK",
        "ABDFGHJL:HGBDAFLJ",
        "ABDFGHJK:HGBDAFJK",
        "ABDFGHIL:HGBDAFLI",
        "ABDFGHIK:HGBDAFIK",
        "ABDFGHIJ:HGBDAFIJ",
        "ABDEIJKL:EJBAIDLK",
        "ABDEHJKL:EJBDAHLK",
        "ABDEHIKL:EIBDAHLK",
        "ABDEHIJL:EJBDAHLI",
        "ABDEHIJK:EJBDAHIK",
        "ABDEGJKL:EJBDAGLK",
        "ABDEGIKL:EGBAIDLK",
        "ABDEGIJL:EJBDAGLI",
        "ABDEGIJK:EJBDAGIK",
        "ABDEGHKL:EGBDAHLK",
        "ABDEGHJL:HJBDAGLE",
        "ABDEGHJK:HJBDAGEK",
        "ABDEGHIL:EGBDAHLI",
        "ABDEGHIK:EGBDAHIK",
        "ABDEGHIJ:HJBDAGEI",
        "ABDEFJKL:EJBDAFLK",
        "ABDEFIKL:EIBDAFLK",
        "ABDEFIJL:EJBDAFLI",
        "ABDEFIJK:EJBDAFIK",
        "ABDEFHKL:HEBDAFLK",
        "ABDEFHJL:HJBDAFLE",
        "ABDEFHJK:HJBDAFEK",
        "ABDEFHIL:HEBDAFLI",
        "ABDEFHIK:HEBDAFIK",
        "ABDEFHIJ:HJBDAFEI",
        "ABDEFGKL:EGBDAFLK",
        "ABDEFGJL:EGBDAFLJ",
        "ABDEFGJK:EGBDAFJK",
        "ABDEFGIL:EGBDAFLI",
        "ABDEFGIK:EGBDAFIK",
        "ABDEFGIJ:EGBDAFIJ",
        "ABDEFGHL:HGBDAFLE",
        "ABDEFGHK:HGBDAFEK",
        "ABDEFGHJ:HGBDAFEJ",
        "ABDEFGHI:HGBDAFEI",
        "ABCHIJKL:IJBCAHLK",
        "ABCGIJKL:IJBCAGLK",
        "ABCGHJKL:HJBCAGLK",
        "ABCGHIKL:IGBCAHLK",
        "ABCGHIJL:HJBCAGLI",
        "ABCGHIJK:HJBCAGIK",
        "ABCFIJKL:IJBCAFLK",
        "ABCFHJKL:HJBCAFLK",
        "ABCFHIKL:HIBCAFLK",
        "ABCFHIJL:HJBCAFLI",
        "ABCFHIJK:HJBCAFIK",
        "ABCFGJKL:CJBFAGLK",
        "ABCFGIKL:IGBCAFLK",
        "ABCFGIJL:CJBFAGLI",
        "ABCFGIJK:CJBFAGIK",
        "ABCFGHKL:HGBCAFLK",
        "ABCFGHJL:HGBCAFLJ",
        "ABCFGHJK:HGBCAFJK",
        "ABCFGHIL:HGBCAFLI",
        "ABCFGHIK:HGBCAFIK",
        "ABCFGHIJ:HGBCAFIJ",
        "ABCEIJKL:EJBAICLK",
        "ABCEHJKL:EJBCAHLK",
        "ABCEHIKL:EIBCAHLK",
        "ABCEHIJL:EJBCAHLI",
        "ABCEHIJK:EJBCAHIK",
        "ABCEGJKL:EJBCAGLK",
        "ABCEGIKL:EGBAICLK",
        "ABCEGIJL:EJBCAGLI",
        "ABCEGIJK:EJBCAGIK",
        "ABCEGHKL:EGBCAHLK",
        "ABCEGHJL:HJBCAGLE",
        "ABCEGHJK:HJBCAGEK",
        "ABCEGHIL:EGBCAHLI",
        "ABCEGHIK:EGBCAHIK",
        "ABCEGHIJ:HJBCAGEI",
        "ABCEFJKL:EJBCAFLK",
        "ABCEFIKL:EIBCAFLK",
        "ABCEFIJL:EJBCAFLI",
        "ABCEFIJK:EJBCAFIK",
        "ABCEFHKL:HEBCAFLK",
        "ABCEFHJL:HJBCAFLE",
        "ABCEFHJK:HJBCAFEK",
        "ABCEFHIL:HEBCAFLI",
        "ABCEFHIK:HEBCAFIK",
        "ABCEFHIJ:HJBCAFEI",
        "ABCEFGKL:EGBCAFLK",
        "ABCEFGJL:EGBCAFLJ",
        "ABCEFGJK:EGBCAFJK",
        "ABCEFGIL:EGBCAFLI",
        "ABCEFGIK:EGBCAFIK",
        "ABCEFGIJ:EGBCAFIJ",
        "ABCEFGHL:HGBCAFLE",
        "ABCEFGHK:HGBCAFEK",
        "ABCEFGHJ:HGBCAFEJ",
        "ABCEFGHI:HGBCAFEI",
        "ABCDIJKL:IJBCADLK",
        "ABCDHJKL:HJBCADLK",
        "ABCDHIKL:HIBCADLK",
        "ABCDHIJL:HJBCADLI",
        "ABCDHIJK:HJBCADIK",
        "ABCDGJKL:CJBDAGLK",
        "ABCDGIKL:IGBCADLK",
        "ABCDGIJL:CJBDAGLI",
        "ABCDGIJK:CJBDAGIK",
        "ABCDGHKL:HGBCADLK",
        "ABCDGHJL:HGBCADLJ",
        "ABCDGHJK:HGBCADJK",
        "ABCDGHIL:HGBCADLI",
        "ABCDGHIK:HGBCADIK",
        "ABCDGHIJ:HGBCADIJ",
        "ABCDFJKL:CJBDAFLK",
        "ABCDFIKL:CIBDAFLK",
        "ABCDFIJL:CJBDAFLI",
        "ABCDFIJK:CJBDAFIK",
        "ABCDFHKL:HFBCADLK",
        "ABCDFHJL:CJBDAFLH",
        "ABCDFHJK:HJBCAFDK",
        "ABCDFHIL:HFBCADLI",
        "ABCDFHIK:HFBCADIK",
        "ABCDFHIJ:HJBCAFDI",
        "ABCDFGKL:CGBDAFLK",
        "ABCDFGJL:CGBDAFLJ",
        "ABCDFGJK:CGBDAFJK",
        "ABCDFGIL:CGBDAFLI",
        "ABCDFGIK:CGBDAFIK",
        "ABCDFGIJ:CGBDAFIJ",
        "ABCDFGHL:CGBDAFLH",
        "ABCDFGHK:HGBCAFDK",
        "ABCDFGHJ:HGBCAFDJ",
        "ABCDFGHI:HGBCAFDI",
        "ABCDEJKL:EJBCADLK",
        "ABCDEIKL:EIBCADLK",
        "ABCDEIJL:EJBCADLI",
        "ABCDEIJK:EJBCADIK",
        "ABCDEHKL:HEBCADLK",
        "ABCDEHJL:HJBCADLE",
        "ABCDEHJK:HJBCADEK",
        "ABCDEHIL:HEBCADLI",
        "ABCDEHIK:HEBCADIK",
        "ABCDEHIJ:HJBCADEI",
        "ABCDEGKL:EGBCADLK",
        "ABCDEGJL:EGBCADLJ",
        "ABCDEGJK:EGBCADJK",
        "ABCDEGIL:EGBCADLI",
        "ABCDEGIK:EGBCADIK",
        "ABCDEGIJ:EGBCADIJ",
        "ABCDEGHL:HGBCADLE",
        "ABCDEGHK:HGBCADEK",
        "ABCDEGHJ:HGBCADEJ",
        "ABCDEGHI:HGBCADEI",
        "ABCDEFKL:CEBDAFLK",
        "ABCDEFJL:CJBDAFLE",
        "ABCDEFJK:CJBDAFEK",
        "ABCDEFIL:CEBDAFLI",
        "ABCDEFIK:CEBDAFIK",
        "ABCDEFIJ:CJBDAFEI",
        "ABCDEFHL:HFBCADLE",
        "ABCDEFHK:HEBCAFDK",
        "ABCDEFHJ:HJBCAFDE",
        "ABCDEFHI:HEBCAFDI",
        "ABCDEFGL:CGBDAFLE",
        "ABCDEFGK:CGBDAFEK",
        "ABCDEFGJ:CGBDAFEJ",
        "ABCDEFGI:CGBDAFEI",
        "ABCDEFGH:HGBCAFDE",
    };

    private static readonly IReadOnlyDictionary<string, Wc2026ThirdPlaceCombination> ThirdPlaceCombinations =
        ThirdPlaceCombinationRows
            .Select((row, index) => Wc2026ThirdPlaceCombination.Create(index + 1, row))
            .ToDictionary(combination => combination.AdvancingGroups);

    private static readonly Wc2026Round32MatchTemplate[] MatchTemplates =
    {
        new(73, 'A', 2, 'B', 2),
        new(74, 'E', 1, thirdPlaceAssignmentTargetGroup: 'E'),
        new(75, 'F', 1, 'C', 2),
        new(76, 'C', 1, 'F', 2),
        new(77, 'I', 1, thirdPlaceAssignmentTargetGroup: 'I'),
        new(78, 'E', 2, 'I', 2),
        new(79, 'A', 1, thirdPlaceAssignmentTargetGroup: 'A'),
        new(80, 'L', 1, thirdPlaceAssignmentTargetGroup: 'L'),
        new(81, 'D', 1, thirdPlaceAssignmentTargetGroup: 'D'),
        new(82, 'G', 1, thirdPlaceAssignmentTargetGroup: 'G'),
        new(83, 'K', 2, 'L', 2),
        new(84, 'H', 1, 'J', 2),
        new(85, 'B', 1, thirdPlaceAssignmentTargetGroup: 'B'),
        new(86, 'J', 1, 'H', 2),
        new(87, 'K', 1, thirdPlaceAssignmentTargetGroup: 'K'),
        new(88, 'D', 2, 'G', 2),
    };

    public static int ThirdPlaceCombinationCount => ThirdPlaceCombinationRows.Length;

    public static Wc2026Round32SimulationResult CreateSimulation(IEnumerable<Wc2026ScenarioGroup> groups)
    {
        var groupSnapshots = (groups ?? Enumerable.Empty<Wc2026ScenarioGroup>())
            .Where(group => group != null)
            .Select(Wc2026Round32GroupSnapshot.Create)
            .Where(group => group.IsValid)
            .OrderBy(group => group.SortOrder)
            .ThenBy(group => group.Name)
            .ToList();

        if (groupSnapshots.Count < 12)
        {
            return Wc2026Round32SimulationResult.Blocked("12개 조 데이터가 모두 있어야 32강을 시뮬레이션할 수 있습니다.");
        }

        var blankGroups = groupSnapshots
            .Where(group => group.IsBlankDrawGroup)
            .Select(group => group.Name)
            .ToList();

        if (blankGroups.Any())
        {
            return Wc2026Round32SimulationResult.Blocked(
                $"아직 입력하지 않은 조가 있습니다: {string.Join(", ", blankGroups)}");
        }

        var thirdPlaceQualifiers = groupSnapshots
            .Select(group => Wc2026Round32ThirdPlaceStanding.Create(group))
            .OrderByDescending(standing => standing.Points)
            .ThenByDescending(standing => standing.GoalDifference)
            .ThenByDescending(standing => standing.GoalsFor)
            .ThenBy(standing => standing.GroupSortOrder)
            .Take(8)
            .ToList();

        if (thirdPlaceQualifiers.Count < 8)
        {
            return Wc2026Round32SimulationResult.Blocked("3위 진출 팀 8팀을 계산할 수 없습니다.");
        }

        var advancingThirdPlaceGroups = string.Concat(thirdPlaceQualifiers
            .Select(standing => standing.GroupCode)
            .OrderBy(group => group));

        if (!ThirdPlaceCombinations.TryGetValue(advancingThirdPlaceGroups, out var thirdPlaceCombination))
        {
            return Wc2026Round32SimulationResult.Blocked("현재 3위 진출 조합을 32강 배정표에서 찾지 못했습니다.");
        }

        var groupMap = groupSnapshots.ToDictionary(group => group.Code);
        var thirdPlaceAssignments = thirdPlaceCombination.GetAssignmentsByWinnerGroup();
        var matches = MatchTemplates
            .Select(template => template.CreateMatch(groupMap, thirdPlaceAssignments))
            .ToList();

        return new Wc2026Round32SimulationResult
        {
            CombinationNumber = thirdPlaceCombination.Number,
            ThirdPlaceGroupKey = advancingThirdPlaceGroups,
            ThirdPlaceGroups = thirdPlaceQualifiers
                .Select(standing => standing.GroupName)
                .ToList(),
            Matches = matches,
        };
    }

    private sealed class Wc2026Round32GroupSnapshot
    {
        public char Code { get; init; }
        public string Name { get; init; }
        public int SortOrder { get; init; }
        public IReadOnlyList<Wc2026ScenarioStanding> Standings { get; init; }
        public bool IsValid => Code != '\0' && Standings.Count >= 4;
        public bool IsBlankDrawGroup => Standings.Count == 4
            && Standings.All(standing =>
                standing.Won == 0
                && standing.Drawn == 3
                && standing.Lost == 0);

        public GroupTeam GetTeam(int placement)
        {
            var standing = Standings[placement - 1];
            var team = standing.Team;

            return new GroupTeam
            {
                Id = string.IsNullOrWhiteSpace(team.BettingTeamId) ? team.Id : team.BettingTeamId,
                FifaTeamId = team.Id,
                GroupName = Name,
                Placement = placement.ToString(),
                Name = team.Name,
                Flag = team.Flag,
            };
        }

        public static Wc2026Round32GroupSnapshot Create(Wc2026ScenarioGroup group)
        {
            var code = GetGroupCode(group);
            return new Wc2026Round32GroupSnapshot
            {
                Code = code,
                Name = code == '\0' ? group.Name : $"{code}조",
                SortOrder = group.SortOrder,
                Standings = group.Standings.ToList(),
            };
        }

        private static char GetGroupCode(Wc2026ScenarioGroup group)
        {
            var match = Regex.Match(
                group.Name ?? "",
                @"([A-L])\s*조|Group\s*([A-L])|\b([A-L])\b",
                RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return '\0';
            }

            var value = match.Groups
                .Cast<Group>()
                .Skip(1)
                .FirstOrDefault(group => group.Success)
                ?.Value;

            return string.IsNullOrWhiteSpace(value)
                ? '\0'
                : char.ToUpperInvariant(value[0]);
        }
    }

    private sealed class Wc2026Round32ThirdPlaceStanding
    {
        public char GroupCode { get; init; }
        public string GroupName { get; init; }
        public int GroupSortOrder { get; init; }
        public int Points { get; init; }
        public int GoalDifference { get; init; }
        public int GoalsFor { get; init; }

        public static Wc2026Round32ThirdPlaceStanding Create(Wc2026Round32GroupSnapshot group)
        {
            var standing = group.Standings[2];
            return new Wc2026Round32ThirdPlaceStanding
            {
                GroupCode = group.Code,
                GroupName = group.Name,
                GroupSortOrder = group.SortOrder,
                Points = standing.Points,
                GoalDifference = standing.GoalDifference,
                GoalsFor = standing.GoalsFor,
            };
        }
    }

    private sealed class Wc2026ThirdPlaceCombination
    {
        public int Number { get; init; }
        public string AdvancingGroups { get; init; }
        public string Assignments { get; init; }

        public Dictionary<char, char> GetAssignmentsByWinnerGroup()
        {
            return ThirdPlaceAssignmentTargetGroups
                .Select((targetGroup, index) => (targetGroup, assignedThirdPlaceGroup: Assignments[index]))
                .ToDictionary(item => item.targetGroup, item => item.assignedThirdPlaceGroup);
        }

        public static Wc2026ThirdPlaceCombination Create(int number, string row)
        {
            var values = row.Split(':');
            if (values.Length != 2 || values[0].Length != 8 || values[1].Length != 8)
            {
                throw new InvalidOperationException($"Invalid round of 32 third-place row: {row}");
            }

            return new Wc2026ThirdPlaceCombination
            {
                Number = number,
                AdvancingGroups = values[0],
                Assignments = values[1],
            };
        }
    }

    private sealed class Wc2026Round32MatchTemplate
    {
        private readonly int _matchNumber;
        private readonly char _homeGroup;
        private readonly int _homePlacement;
        private readonly char _awayGroup;
        private readonly int _awayPlacement;
        private readonly char? _thirdPlaceAssignmentTargetGroup;

        public Wc2026Round32MatchTemplate(
            int matchNumber,
            char homeGroup,
            int homePlacement,
            char awayGroup = '\0',
            int awayPlacement = 0,
            char? thirdPlaceAssignmentTargetGroup = null)
        {
            _matchNumber = matchNumber;
            _homeGroup = homeGroup;
            _homePlacement = homePlacement;
            _awayGroup = awayGroup;
            _awayPlacement = awayPlacement;
            _thirdPlaceAssignmentTargetGroup = thirdPlaceAssignmentTargetGroup;
        }

        public Wc2026Round32SimulationMatch CreateMatch(
            IReadOnlyDictionary<char, Wc2026Round32GroupSnapshot> groups,
            IReadOnlyDictionary<char, char> thirdPlaceAssignments)
        {
            var homeTeam = groups[_homeGroup].GetTeam(_homePlacement);
            var awayGroup = _thirdPlaceAssignmentTargetGroup == null
                ? _awayGroup
                : thirdPlaceAssignments[_thirdPlaceAssignmentTargetGroup.Value];
            var awayPlacement = _thirdPlaceAssignmentTargetGroup == null ? _awayPlacement : 3;
            var awayTeam = groups[awayGroup].GetTeam(awayPlacement);

            return new Wc2026Round32SimulationMatch
            {
                MatchNumber = _matchNumber,
                HomeSlot = $"{_homePlacement}{_homeGroup}",
                AwaySlot = $"{awayPlacement}{awayGroup}",
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
            };
        }
    }
}

public class Wc2026Round32SimulationResult
{
    public int CombinationNumber { get; init; }
    public string ThirdPlaceGroupKey { get; init; } = "";
    public IReadOnlyList<string> ThirdPlaceGroups { get; init; } = Array.Empty<string>();
    public IReadOnlyList<Wc2026Round32SimulationMatch> Matches { get; init; } = Array.Empty<Wc2026Round32SimulationMatch>();
    public string BlockReason { get; init; }
    public bool CanSimulate => string.IsNullOrWhiteSpace(BlockReason) && Matches.Any();

    public static Wc2026Round32SimulationResult Blocked(string reason)
    {
        return new Wc2026Round32SimulationResult
        {
            BlockReason = reason,
        };
    }
}

public class Wc2026Round32SimulationMatch
{
    public int MatchNumber { get; init; }
    public string HomeSlot { get; init; } = "";
    public string AwaySlot { get; init; } = "";
    public GroupTeam HomeTeam { get; init; }
    public GroupTeam AwayTeam { get; init; }
}

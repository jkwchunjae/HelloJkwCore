namespace HelloJkwCore.Hangul3;

internal interface IHangul3InputConverter
{
    Hangul3Type ConverterType { get; }
    Input3 Input2ToInput3(Input2 input2);
}

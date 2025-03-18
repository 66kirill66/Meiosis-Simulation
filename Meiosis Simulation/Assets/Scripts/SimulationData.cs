using System;
public enum AlignOnType
{
    ToTheRightOfCenter,
    Poles,
    ToTheLeftOfCenter,
    TheRightPole,
    Center,
    TheLeftPole,
    Random,
    None
}

public enum ViewType
{
    ChromosomesView,
    FertilizationView,
    FamilyView,
    MitosisMacroView,
    MeiosisView,
    DoubleMeiosisView,
    MitosisView
}

public enum MoveTowardType
{
    LeftPole,
    NearestPole,
    RightPole
}

public enum SimulationEventsTypes
{
    SelectLongChromosome,
    SelectShortChromosome,
    ClickBloodIcon,
    ClickCorianderIcon,
    ClickFrecklesIcon,
    ClickEarwaxIcon,
    ClickStart,
    ClickFertilizationStart,

    ChromosomesReplicated,
    SisterChromatidsFormed,
    ChromosomesAlignedOnCenter,
    SisterChromatidsAlignedOnCenter,
    HomologousChromosomesMovedToNearestPole,
    SisterChromatidsMovedToNearestPole,
    CellUnderwentFirstDivision,
    CellUnderwentSecondDivision,
    HomologousChromosomesUndergoCrossingOver
}

public static class TypeHelper
{
    public static TEnum? Parse<TEnum>(string value) where TEnum : struct, Enum
    {
        if (Enum.TryParse(value, true, out TEnum result))
        {
            return result;
        }
        return null; 
    }
}

public class Data
{
    public int id;
    public string entityType;
    public string locationType;
    public string viewType;

}

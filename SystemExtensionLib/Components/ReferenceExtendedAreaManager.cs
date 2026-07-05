#pragma warning disable CS1591

using MBMScripts;

namespace SystemExtensionLib.Components;

public class ReferenceExtendedAreaManager : Reference
{
    public override void Initialize()
    {
        ReferenceType = EReferenceType.Unit;
    }

    public override Unit? GetUnit()
    {
        TargetUnit targetUnit = base.Updater.TargetUnit;
        if ((targetUnit?.Unit) is not Character)
        {
            return null;
        }
        return targetUnit.Unit;
    }
}
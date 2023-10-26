using Content.Shared.Polymorph;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Resources.MAZOR - mods.systems;


namespace Resources\MAZOR - mods\components;

[RegisterComponent]
public sealed partial class ChangeEntetyPrototypeComponent : Component
{
    [DataField("polymorph prototype", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<PolymorphPrototype>))]
    public string Polymorph-prototype = default!;

    

    
}
